using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.WindowsService.SVCEmissaoCFOCFOC.Entities;

namespace Tecnomapas.EtramiteX.WindowsService.SVCCARSolicitacao.Data
{
	class EmissaoCFOCFOCDa
	{
		#region Propriedades

		ConfiguracaoSistema _configSis = new ConfiguracaoSistema();
		Historico _historico = new Historico();
		public Historico Historico { get { return _historico; } }

		public String EsquemaBanco { get; set; }

		public string UsuarioCredenciado
		{
			get { return _configSis.UsuarioCredenciado; }
		}
		public string UsuarioInterno
		{
			get { return _configSis.UsuarioInterno; }
		}

		public EmissaoCFOCFOCDa(string esquema = null)
		{
			EsquemaBanco = esquema;
		}

		#endregion Propriedades

		#region Configurações

		internal void EditarConfiguracao(ConfiguracaoServico configuracao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando;

				if (configuracao.EmExecucao)
				{
					comando = bancoDeDados.CriarComando(@"update cnf_servicos r set r.data_inicio_execucao = :data_inicio_execucao, r.em_execucao = :em_execucao, r.tid = :tid where r.id = :id");

					configuracao.DataInicioExecucao = configuracao.DataInicioExecucao.Value.AddHours(configuracao.Intervalo.TotalHours);
					comando.AdicionarParametroEntrada("data_inicio_execucao", configuracao.DataInicioExecucao, DbType.DateTime);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"update cnf_servicos r set r.data_ultima_execucao = sysdate, r.em_execucao = :em_execucao, r.tid = :tid where r.id = :id");
				}

				comando.AdicionarParametroEntrada("id", configuracao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("em_execucao", configuracao.EmExecucao ? 1 : 0, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		internal ConfiguracaoServico Configuracao(eServico servico)
		{
			return Configuracoes(servico).FirstOrDefault() ?? new ConfiguracaoServico();
		}

		internal List<ConfiguracaoServico> Configuracoes(eServico servico)
		{
			List<ConfiguracaoServico> retorno = new List<ConfiguracaoServico>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id, servico, intervalo, data_inicio, data_inicio_execucao, 
				nvl(data_ultima_execucao, data_inicio) data_ultima_execucao, em_execucao, tid from cnf_servicos t");

				comando.DbCommand.CommandText += " where t.id = :servico";
				comando.AdicionarParametroEntrada("servico", (int)servico, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ConfiguracaoServico conf;

					while (reader.Read())
					{
						conf = new ConfiguracaoServico();
						conf.Id = reader.GetValue<Int32>("id");
						conf.Servico = reader.GetValue<String>("servico");
						conf.Intervalo = new TimeSpan(reader.GetValue<Int32>("intervalo"), 0, 0); //Em Horas
						conf.DataInicio = reader.GetValue<DateTime>("data_inicio");
						conf.DataInicioExecucao = reader.GetValue<DateTime>("data_inicio_execucao");
						conf.DataUltimaExecucao = reader.GetValue<DateTime>("data_ultima_execucao");
						conf.EmExecucao = reader.GetValue<Boolean>("em_execucao");
						retorno.Add(conf);
					}

					reader.Close();
				}

				return retorno;
			}
		}

		#endregion Configurações

		internal List<HabilitarEmissaoCFOCFOC> ObterEmissoesComValidadeVencida(BancoDeDados banco = null)
		{
			List<HabilitarEmissaoCFOCFOC> lista = new List<HabilitarEmissaoCFOCFOC>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.id, 3 motivo from tab_hab_emi_cfo_cfoc c where c.validade_registro < sysdate
				union select a.hab id, 4 motivo from (select count(p.id) qtd, p.habilitar_emi_id hab from tab_hab_emi_cfo_cfoc_praga p group by p.habilitar_emi_id) a,
				(select count(p.id) qtd_venc, p.habilitar_emi_id hab_venc from tab_hab_emi_cfo_cfoc_praga p where p.data_habilitacao_final < sysdate 
				group by p.habilitar_emi_id) b where a.hab = b.hab_venc and a.qtd = b.qtd_venc order by 2 desc");

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						HabilitarEmissaoCFOCFOC entidade = new HabilitarEmissaoCFOCFOC();
						entidade.Id = reader.GetValue<Int32>("id");
						entidade.Motivo = reader.GetValue<Int32>("motivo");
						lista.Add(entidade);
					}

					reader.Close();
				}
			}

			return lista;
		}

		internal void AlterarSituacao(HabilitarEmissaoCFOCFOC entidade, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
				update tab_hab_emi_cfo_cfoc p set p.motivo = :motivo, p.observacao = :observacao, p.situacao_data = sysdate, p.situacao = :situacao, 
				p.tid = :tid where p.id = :id and p.situacao != 3", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", entidade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("motivo", (entidade.Motivo.HasValue && entidade.Motivo > 0) ? entidade.Motivo : null, DbType.Int32);
				comando.AdicionarParametroEntrada("observacao", DbType.String, 250, entidade.Observacao);
				comando.AdicionarParametroEntrada("situacao", entidade.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(entidade.Id, eHistoricoArtefato.habilitaremissaocfocfoc, eHistoricoAcao.alterarsituacao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}
	}
}