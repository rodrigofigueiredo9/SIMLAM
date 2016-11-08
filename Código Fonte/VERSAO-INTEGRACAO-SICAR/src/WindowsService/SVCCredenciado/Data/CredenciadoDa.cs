using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.WindowsService.SVCCredenciado.Entities;

namespace Tecnomapas.EtramiteX.WindowsService.SVCCredenciado.Data
{
	class CredenciadoDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		public Historico Historico { get { return _historico; } }

		public String EsquemaBanco { get; set; }

		public CredenciadoDa(string esquema = null)
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
					comando = bancoDeDados.CriarComando(@"update cnf_servicos_credenciado r set r.data_inicio_execucao = :data_inicio_execucao, 
					r.em_execucao = :em_execucao, r.tid = :tid where r.id = :id");

					configuracao.DataInicioExecucao = configuracao.DataInicioExecucao.Value.AddHours(configuracao.Intervalo.TotalHours);
					comando.AdicionarParametroEntrada("data_inicio_execucao", configuracao.DataInicioExecucao, DbType.DateTime);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"update cnf_servicos_credenciado r set r.data_ultima_execucao = sysdate, 
					r.em_execucao = :em_execucao, r.tid = :tid where r.id = :id");
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
				nvl(data_ultima_execucao, data_inicio) data_ultima_execucao, em_execucao, tid from cnf_servicos_credenciado t");

				if (servico != eServico.Todos)
				{
					comando.DbCommand.CommandText += " where t.id = :servico";
					comando.AdicionarParametroEntrada("servico", (int)servico, DbType.Int32);
				}

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ConfiguracaoServico conf;

					while (reader.Read())
					{
						conf = new ConfiguracaoServico();
						conf.Id = reader.GetValue<Int32>("id");
						conf.Servico = reader.GetValue<String>("servico");
						conf.Intervalo = new TimeSpan(reader.GetValue<Int32>("intervalo"), 0, 0);
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

		internal void CredenciadoInutilizadoExcluir(BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComandoPlSql(
				@"declare 
					v_aux number;
				begin 
					/*Atualizar o TID de conjuge de pessoa*/
					update tab_pessoa c set c.tid = :tid where c.id in (select r.conjuge from tab_pessoa_conjuge r 
					where r.pessoa in (select c.pessoa from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate));

					/*Atualizar o TID de conjuge de representantes legais*/
					update tab_pessoa c set c.tid = :tid where c.id in (select pc.conjuge from tab_pessoa_conjuge pc 
					where pc.pessoa in (select r.representante from tab_pessoa_representante r where r.pessoa in 
					(select c.pessoa from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate)));

					/*Atualizar o TID de representantes legais*/
					update tab_pessoa c set c.tid = :tid where c.id in (select r.representante from tab_pessoa_representante r 
					where r.pessoa in (select c.pessoa from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate));

					/*Atualizar o TID da pessoa credenciada*/
					update tab_pessoa c set c.tid = :tid where c.id in (select c.pessoa from tab_credenciado c 
					where c.situacao = 1 and (c.data_cadastro + 5) < sysdate);

					/*Atualizar o TID do credenciado*/
					update tab_credenciado c set c.tid = :tid where c.situacao = 1 and (c.data_cadastro + 5) < sysdate;

					/*Historico de representantes legais*/
					for i in (select p.id, p.estado_civil from tab_pessoa p where p.id in (select r.representante from tab_pessoa_representante r 
						where r.pessoa in (select c.pessoa from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate)))
					loop 
						if(i.estado_civil = 2 or i.estado_civil = 5) then 
							select nvl((select c.conjuge from tab_pessoa_conjuge c where c.pessoa = i.id), 0) into v_aux from dual;
							historico.pessoa(v_aux, :acao, :executor_id, :executor_nome, :executor_login, :executor_tipo_id, :executor_tid);
						end if;
						historico.pessoa(i.id, :acao, :executor_id, :executor_nome, :executor_login, :executor_tipo_id, :executor_tid);
					end loop;

					/*Historico da pessoa credenciada*/
					for i in (select p.id, p.estado_civil from tab_pessoa p where p.id in (select c.pessoa from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate))
					loop 
						if(i.estado_civil = 2 or i.estado_civil = 5) then 
							select nvl((select c.conjuge from tab_pessoa_conjuge c where c.pessoa = i.id), 0) into v_aux from dual;
							historico.pessoa(v_aux, :acao, :executor_id, :executor_nome, :executor_login, :executor_tipo_id, :executor_tid);
						end if;
						historico.pessoa(i.id, :acao, :executor_id, :executor_nome, :executor_login, :executor_tipo_id, :executor_tid);
					end loop;
					
					/*Historico credenciado*/
					for i in (select c.id from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate)
					loop 
						historico.credenciado(i.id, :acao, :executor_id, :executor_nome, :executor_login, :executor_tipo_id, :executor_tid);
					end loop;

					/*Excluir conjuge da pessoa*/
					delete tab_pessoa_conjuge a where a.pessoa in (select c.pessoa from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate); 
					delete tab_pessoa_representante a where a.pessoa in (select r.conjuge from tab_pessoa_conjuge r where r.pessoa in (select c.pessoa from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate)); 
					delete tab_pessoa_profissao a where a.pessoa in (select r.conjuge from tab_pessoa_conjuge r where r.pessoa in (select c.pessoa from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate)); 
					delete tab_pessoa_endereco a where a.pessoa in (select r.conjuge from tab_pessoa_conjuge r where r.pessoa in (select c.pessoa from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate)); 
					delete tab_pessoa_meio_contato a where a.pessoa in (select r.conjuge from tab_pessoa_conjuge r where r.pessoa in (select c.pessoa from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate)); 
					delete tab_pessoa a where a.id in (select r.conjuge from tab_pessoa_conjuge r where r.pessoa in (select c.pessoa from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate)); 

					/*Excluir conjuge dos representantes legais*/
					delete tab_pessoa_conjuge a where a.pessoa in (select p.id from tab_pessoa p where p.credenciado in (select c.id from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate)); 
					delete tab_pessoa_representante a where a.pessoa in (select pc.conjuge from tab_pessoa_conjuge pc where pc.pessoa in (select r.representante from tab_pessoa_representante r where r.pessoa in (select c.pessoa from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate))); 
					delete tab_pessoa_profissao a where a.pessoa in (select pc.conjuge from tab_pessoa_conjuge pc where pc.pessoa in (select r.representante from tab_pessoa_representante r where r.pessoa in (select c.pessoa from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate))); 
					delete tab_pessoa_endereco a where a.pessoa in (select pc.conjuge from tab_pessoa_conjuge pc where pc.pessoa in (select r.representante from tab_pessoa_representante r where r.pessoa in (select c.pessoa from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate))); 
					delete tab_pessoa_meio_contato a where a.pessoa in (select pc.conjuge from tab_pessoa_conjuge pc where pc.pessoa in (select r.representante from tab_pessoa_representante r where r.pessoa in (select c.pessoa from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate))); 
					delete tab_pessoa a where a.id in (select pc.conjuge from tab_pessoa_conjuge pc where pc.pessoa in (select r.representante from tab_pessoa_representante r where r.pessoa in (select c.pessoa from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate))); 

					/*Excluir pessoa credenciada*/
					delete tab_pessoa_representante a where a.pessoa in (select c.pessoa from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate); 
					delete tab_pessoa_profissao a where a.pessoa in (select c.pessoa from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate); 
					delete tab_pessoa_endereco a where a.pessoa in (select c.pessoa from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate); 
					delete tab_pessoa_meio_contato a where a.pessoa in (select c.pessoa from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate); 
					delete tab_pessoa a where a.id in (select c.pessoa from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate); 

					/*Excluir representantes legais*/
					delete tab_pessoa_representante a where a.pessoa in (select p.id from tab_pessoa p where p.credenciado in (select c.id from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate)); 
					delete tab_pessoa_profissao a where a.pessoa in (select p.id from tab_pessoa p where p.credenciado in (select c.id from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate)); 
					delete tab_pessoa_endereco a where a.pessoa in (select p.id from tab_pessoa p where p.credenciado in (select c.id from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate)); 
					delete tab_pessoa_meio_contato a where a.pessoa in (select p.id from tab_pessoa p where p.credenciado in (select c.id from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate)); 
					delete tab_pessoa a where a.id in (select p.id from tab_pessoa p where p.credenciado in (select c.id from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate)); 

					/*Deletar credenciado*/
					for i in (select c.id from tab_credenciado c where c.situacao = 1 and (c.data_cadastro + 5) < sysdate)
					loop 
						delete tab_credenciado a where a.id = i.id; 
					end loop; 
				end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("acao", Convert.ToInt32(eHistoricoAcao.excluir), DbType.Int32);
				comando.AdicionarParametroEntrada("executor_id", Executor.Current.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("executor_tid", Executor.Current.Tid, DbType.String);
				comando.AdicionarParametroEntrada("executor_nome", Executor.Current.Nome, DbType.String);
				comando.AdicionarParametroEntrada("executor_login", Executor.Current.Login, DbType.String);
				comando.AdicionarParametroEntrada("executor_tipo_id", Executor.Current.Tipo, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}
	}
}