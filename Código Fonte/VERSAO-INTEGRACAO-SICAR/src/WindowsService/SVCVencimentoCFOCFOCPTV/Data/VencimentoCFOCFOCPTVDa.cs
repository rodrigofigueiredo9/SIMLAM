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
using Tecnomapas.EtramiteX.WindowsService.SVCVencimentoCFOCFOCPTV.Entities;

namespace Tecnomapas.EtramiteX.WindowsService.SVCVencimentoCFOCFOCPTV.Data
{
	class VencimentoCFOCFOCPTVDa
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

		public VencimentoCFOCFOCPTVDa(string esquema = null)
		{
			EsquemaBanco = esquema;
		}

		public string TID { get; set; }

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
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, this.TID);

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

		internal List<Dictionary<string, object>> ObterVencimentosCFOCFOCPTV(BancoDeDados banco)
		{
			List<Dictionary<string, object>> lista = new List<Dictionary<string, object>>();

			var sql = @"
                select t.id,
                        t.data_ativacao,
                        t.data_ativacao + t.validade_certificado prazo,
                        'CFO' tipo
                    from cre_cfo t
                    where t.situacao = 2 /*Válido*/
                    and t.data_ativacao + t.validade_certificado < sysdate
                union all
                select t.id,
                        t.data_ativacao,
                        t.data_ativacao + t.validade_certificado prazo,
                        'CFOC' tipo
                    from cre_cfoc t
                    where t.situacao = 2 /*Válido*/
                    and t.data_ativacao + t.validade_certificado < sysdate
                union all
                select t.id, 
                        t.data_ativacao, 
                        t.valido_ate prazo, 
                        'PTV' tipo
                    from tab_ptv t
                    where t.situacao = 2 /*Válido*/
                    and t.valido_ate < sysdate
                union all    
                select t.id,
                       t.data_ativacao,
                       t.valido_ate prazo,
                       'PTV_UF' tipo
                  from tab_ptv_outrouf t
                 where t.situacao = 2 /*Válido*/
                   and t.valido_ate < sysdate";

			using (var comando = banco.CriarComando(sql))
			{
				lista = banco.ExecutarDictionaryList(comando);
			}

			return lista;
		}

		internal void AlterarSituacaoCFO(int cfoId, BancoDeDados banco)
		{
			var sql = @"
                update tab_cfo p
                   set p.situacao      = 3/*Inválido*/,
                       p.tid           = :tid
                 where p.id = :cfoId";

			using (var comando = banco.CriarComando(sql))
			{
				comando.AdicionarParametroEntrada("cfoId", cfoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, this.TID);
				banco.ExecutarNonQuery(comando);
				Historico.Gerar(cfoId, eHistoricoArtefato.emissaocfo, eHistoricoAcao.invalidar, banco);
			}
		}

		internal void AlterarSituacaoCFOC(int cfocId, BancoDeDados banco)
		{
			var sql = @"
                update tab_cfoc p
                   set p.situacao      = 3/*Inválido*/,
                       p.tid           = :tid
                 where p.id = :cfocId";

			using (var comando = banco.CriarComando(sql))
			{

				comando.AdicionarParametroEntrada("cfocId", cfocId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, this.TID);
				banco.ExecutarNonQuery(comando);
				Historico.Gerar(cfocId, eHistoricoArtefato.emissaocfoc, eHistoricoAcao.invalidar, banco);
			}
		}

		internal void AlterarSituacaoPTV(int ptvId, BancoDeDados banco)
		{
			var sql = @"
                update tab_ptv p
                   set p.situacao      = 4/*Inválido*/,
                       p.tid           = :tid
                 where p.id = :ptvId";

			using (var comando = banco.CriarComando(sql))
			{

				comando.AdicionarParametroEntrada("ptvId", ptvId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, this.TID);
				banco.ExecutarNonQuery(comando);
				Historico.Gerar(ptvId, eHistoricoArtefato.emitirptv, eHistoricoAcao.invalidar, banco);
			}
		}

		internal void AlterarSituacaoPTVOutroUF(int ptvId, BancoDeDados banco)
		{
			var sql = @"
                update tab_ptv_outrouf p
                   set p.situacao      = 4/*Inválido*/,
                       p.tid           = :tid
                 where p.id = :ptvId";

			using (var comando = banco.CriarComando(sql))
			{
				comando.AdicionarParametroEntrada("ptvId", ptvId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, this.TID);
				banco.ExecutarNonQuery(comando);
				Historico.Gerar(ptvId, eHistoricoArtefato.emitirptvoutro, eHistoricoAcao.invalidar, banco);
			}
		}
	}
}