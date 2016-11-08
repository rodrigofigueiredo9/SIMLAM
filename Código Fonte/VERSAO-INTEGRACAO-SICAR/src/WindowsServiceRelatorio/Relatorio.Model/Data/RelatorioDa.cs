using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.WindowsService.Relatorio.Model.Entities;

namespace Tecnomapas.EtramiteX.WindowsService.Relatorio.Model.Data
{
	public class RelatorioDa
	{
		ConfiguracaoSistema _configSys;

		public RelatorioDa()
		{
			_configSys = new ConfiguracaoSistema();
		}

		public ConfiguracaoRelatorio Configuracao(eFato fato)
		{
			return Configuracoes(fato).FirstOrDefault() ?? new ConfiguracaoRelatorio();
		}

		public List<ConfiguracaoRelatorio> Configuracoes(eFato fato)
		{
			List<ConfiguracaoRelatorio> retorno = new List<ConfiguracaoRelatorio>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(_configSys.UsuarioRelatorio))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.fato, t.processo, t.intervalo, t.dados_ate, 
				sysdate execucao_inicio, t.execucao_fim, t.em_execucao, t.erro, t.tid from cnf_fato_etl t");

				if (fato != eFato.Todos)
				{
					comando.DbCommand.CommandText += " where t.id = :fato";
					comando.AdicionarParametroEntrada("fato", (int)fato, DbType.Int32);
				}

				comando.DbCommand.CommandText += " order by t.id";

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ConfiguracaoRelatorio conf;
					while (reader.Read())
					{
						conf = new ConfiguracaoRelatorio();
						conf.Id = reader.GetValue<Int32>("id");
						conf.Fato = reader.GetValue<String>("fato");
						conf.Processo = reader.GetValue<String>("processo");
						conf.Intervalo = new TimeSpan(reader.GetValue<Int32>("intervalo"), 0, 0);
						conf.DadosAte = reader.GetValue<DateTime>("dados_ate");
						conf.ExecucaoInicio = reader.GetValue<DateTime>("execucao_inicio");
						conf.ExecucaoFim = reader.GetValue<DateTime>("execucao_fim");
						conf.EmExecucao = reader.GetValue<Boolean>("em_execucao");
						conf.Erro = reader.GetValue<Boolean>("erro");
						conf.Tid = reader.GetValue<String>("tid");

						retorno.Add(conf);
					}

					reader.Close();
				}

				return retorno;
			}
		}

		public void AtualizarConfiguracao(ConfiguracaoRelatorio configuracao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, _configSys.UsuarioRelatorio))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update cnf_fato_etl c set c.dados_ate = :dados_ate,  c.execucao_inicio = :execucao_inicio, 
				c.execucao_fim = sysdate, c.em_execucao = :em_execucao, c.erro = :erro, c.tid = :tid where c.id = :id");

				comando.AdicionarParametroEntrada("dados_ate", configuracao.DadosAte, DbType.Date);
				comando.AdicionarParametroEntrada("execucao_inicio", configuracao.ExecucaoInicio, DbType.Date);
				comando.AdicionarParametroEntrada("em_execucao", (configuracao.EmExecucao ? 1 : 0), DbType.Int32);
				comando.AdicionarParametroEntrada("erro", (configuracao.Erro ? 1 : 0), DbType.Int32);
				comando.AdicionarParametroEntrada("tid", configuracao.Tid, DbType.String);
				comando.AdicionarParametroEntrada("id", configuracao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();
			}
		}

		public void SalvarCampo(RelatorioCampo campo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, _configSys.UsuarioRelatorio))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_campo c where c.fato = (select t.id from TAB_FATO t where upper(t.tabela) = upper(:tabelafato)) 
					and c.dimensao = (select t.id from TAB_DIMENSAO t where upper(t.tabela) = upper(:tabeladimensao) )
					and upper(c.campo) = upper(:campoNome) ");

				comando.AdicionarParametroEntrada("tabelafato", DbType.String, 40, campo.TabelaFato);
				comando.AdicionarParametroEntrada("tabeladimensao", DbType.String, 40, campo.TabelaDimensao);
				comando.AdicionarParametroEntrada("campoNome", DbType.String, 30, campo.Nome);

				if (bancoDeDados.ExecutarScalar<Decimal>(comando) == 0)
				{
					CriarCampo(campo, bancoDeDados);
				}
				else
				{
					EditarCampo(campo, bancoDeDados);
				}

				bancoDeDados.Commit();
			}
		}

		public void CriarCampo(RelatorioCampo campo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, _configSys.UsuarioRelatorio))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into tab_campo(id, codigo, fato, dimensao, alias, campo, tipo_dado, campo_exibicao, campo_filtro, consulta, campo_consulta, sistema_consulta, campo_ordenacao) 
				values (seq_campo.nextval, null, (select t.id from TAB_FATO t where upper(t.tabela) = upper(:tabelafato)), 
				(select t.id from TAB_DIMENSAO t where upper(t.tabela) = upper(:tabeladimensao)), 
				:alias, :campoNome, :tipo_dado, :campo_exibicao, :campo_filtro, :consulta, :campo_consulta, :sistema_consulta, :campo_ordenacao)");

				comando.AdicionarParametroEntrada("tabelafato", DbType.String, 40, campo.TabelaFato);
				comando.AdicionarParametroEntrada("tabeladimensao", DbType.String, 40, campo.TabelaDimensao);
				comando.AdicionarParametroEntrada("alias", DbType.String, 4000, campo.Alias);
				comando.AdicionarParametroEntrada("campoNome", DbType.String, 30, campo.Nome);
				comando.AdicionarParametroEntrada("tipo_dado", (int)campo.TipoDado, DbType.Int32);
				comando.AdicionarParametroEntrada("campo_exibicao", campo.Exibicao ? 1 : 0, DbType.Int32);
				comando.AdicionarParametroEntrada("campo_filtro", campo.Filtro ? 1 : 0, DbType.Int32);
				comando.AdicionarParametroEntrada("consulta", DbType.String, 4000, campo.ConsultaSql);
				comando.AdicionarParametroEntrada("campo_consulta", DbType.String, 40, campo.ConsultaCampo);
				comando.AdicionarParametroEntrada("sistema_consulta", (int)campo.ConsultaSistema, DbType.Int32);
				comando.AdicionarParametroEntrada("campo_ordenacao", campo.Ordenacao ? 1 : 0, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		public void EditarCampo(RelatorioCampo campo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, _configSys.UsuarioRelatorio))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update tab_campo c set c.alias = :alias 
					where c.fato = (select t.id from TAB_FATO t where upper(t.tabela) = upper(:tabelafato)) 
					and c.dimensao = (select t.id from TAB_DIMENSAO t where upper(t.tabela) = upper(:tabeladimensao) )
					and upper(c.campo) = upper(:campoNome) ");

				comando.AdicionarParametroEntrada("tabelafato", DbType.String, 40, campo.TabelaFato);
				comando.AdicionarParametroEntrada("tabeladimensao", DbType.String, 40, campo.TabelaDimensao);
				comando.AdicionarParametroEntrada("campoNome", DbType.String, 30, campo.Nome);

				comando.AdicionarParametroEntrada("alias", DbType.String, 4000, campo.Alias);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}
	}
}