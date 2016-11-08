using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.EtramiteX.WindowsService.SVCCARSolicitacao.Entities;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.WindowsService.SVCCARSolicitacao.Data
{
	class CARSolicitacaoDa
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

		public CARSolicitacaoDa(string esquema = null)
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

		internal List<CARSolicitacao> ObterSolicitacoesValidasPorNumDiasPassados(int numDias, BancoDeDados banco = null)
		{

			List<CARSolicitacao> lista = new List<CARSolicitacao>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{

				Comando comando = bancoDeDados.CriarComando(@"select t.id solicitacao_id, t.requerimento requerimento_id from tab_car_solicitacao t 
															where t.situacao = 2 /*Valido*/ and t.situacao_data <= (sysdate - :dias) and t.requerimento not 
															in (select p.requerimento from tab_projeto_digital p where p.situacao = 5/*Aguardando Análise*/)");


				comando.AdicionarParametroEntrada("dias", numDias, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						CARSolicitacao car = new CARSolicitacao();
						car.Id = reader.GetValue<int>("solicitacao_id");
						car.Requerimento.Id = reader.GetValue<int>("requerimento_id");

						lista.Add(car);
					}

					reader.Close();
				}
			}

			return lista;
		}

		internal List<CARSolicitacao> ObterSolicitacoesPorRequerimentoNaoProtocolado(List<CARSolicitacao> solicitacoes, BancoDeDados banco = null)
		{
			List<CARSolicitacao> lista = new List<CARSolicitacao>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioInterno))
			{
				for (int i = 0; i < solicitacoes.Count; i++)
				{
					CARSolicitacao solicitacao = solicitacoes[i];

					Comando comando = bancoDeDados.CriarComando(@"select count(1) from tab_requerimento r where r.id = :requerimento and r.situacao = 3 /*Protocolado*/");

					comando.AdicionarParametroEntrada("requerimento", solicitacao.Requerimento.Id, DbType.Int32);

					bool isProtocolado = Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));

					if (!isProtocolado)
					{
						lista.Add(solicitacao);
					}
				}
			}

			return lista;
		}

		internal void AlterarSituacao(CARSolicitacao entidade, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
				update tab_car_solicitacao t
				set t.situacao               = :situacao,
					t.motivo				 = :motivo,
					t.situacao_data          = sysdate,
					t.situacao_anterior      = t.situacao,
					t.situacao_anterior_data = t.situacao_data,
					t.tid                    = :tid
				where t.id = :id");

				comando.AdicionarParametroEntrada("id", entidade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", entidade.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("motivo", entidade.Motivo, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(entidade.Id, eHistoricoArtefato.carsolicitacao, eHistoricoAcao.alterarsituacao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}
	}
}