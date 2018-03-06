using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data
{
	public class CobrancaParcelamentoDa
	{
		#region Propriedade e Atributos

		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();
		ConfigFiscalizacaoDa _configuracaoDa = new ConfigFiscalizacaoDa();
		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }
		private string EsquemaBanco { get; set; }
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		public String EsquemaBancoGeo { get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); } }

		#endregion

        public CobrancaParcelamentoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

        public CobrancaParcelamento Salvar(CobrancaParcelamento cobrancaParcelamento, BancoDeDados banco = null)
        {
            if (cobrancaParcelamento == null)
            {
                throw new Exception("Cobranca é nulo.");
            }

            if (cobrancaParcelamento.Id <= 0)
            {
                cobrancaParcelamento = Criar(cobrancaParcelamento, banco);
            }
            else
            {
                cobrancaParcelamento = Editar(cobrancaParcelamento, banco);
            }

            return cobrancaParcelamento;
        }

        public CobrancaParcelamento Criar(CobrancaParcelamento cobrancaParcelamento, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"
                                    insert into {0}tab_fisc_cob_parcelamento (id,
																   cobranca,
                                                                   tid)
                                    values ({0}seq_fisc_cob_parc.nextval,
											:cobranca,
                                            :tid)
                                    returning id into :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("cobranca", cobrancaParcelamento.CobrancaId, DbType.Int32);
                comando.AdicionarParametroSaida("id", DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                cobrancaParcelamento.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				Historico.Gerar(cobrancaParcelamento.Id, eHistoricoArtefato.cobranca, eHistoricoAcao.atualizar, bancoDeDados);

                Consulta.Gerar(cobrancaParcelamento.Id, eHistoricoArtefato.cobranca, bancoDeDados);

                bancoDeDados.Commit();
            }
            return cobrancaParcelamento;
        }

        public CobrancaParcelamento Editar(CobrancaParcelamento cobrancaParcelamento, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"
                                    update {0}tab_fisc_cob_parcelamento t
                                    set 
										t.cobranca = :cobranca,
                                        t.tid = :tid
                                    where t.id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", cobrancaParcelamento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("cobranca", cobrancaParcelamento.CobrancaId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(cobrancaParcelamento.Id, eHistoricoArtefato.cobranca, eHistoricoAcao.atualizar, bancoDeDados);

                Consulta.Gerar(cobrancaParcelamento.Id, eHistoricoArtefato.cobranca, bancoDeDados);

                bancoDeDados.Commit();
            }
            return cobrancaParcelamento;
        }

		public bool Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando("update {0}tab_fisc_cob_parcelamento t set t.tid = :tid where t.id = :id");
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				#region Histórico

				Historico.Gerar(id, eHistoricoArtefato.cobranca, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				comando = bancoDeDados.CriarComando(
					"begin " +
						"delete {0}tab_fisc_cob_parcelamento t where t.id = :id; " +
					"end;", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#region Consulta

				Consulta.Deletar(id, eHistoricoArtefato.cobranca, bancoDeDados);

				#endregion

				bancoDeDados.Commit();

				return true;
			}
		}

		#endregion

		#region Obter / Filtrar

		public List<CobrancaParcelamento> Obter(int cobrancaId)
		{
			var lista = new List<CobrancaParcelamento>();
			var cobrancaDUADa = new CobrancaDUADa();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select  p.id,
											p.cobranca,
											p.tid
										from {0}tab_fisc_cob_parcelamento p
										where p.cobranca = :cobranca", EsquemaBanco);

				comando.AdicionarParametroEntrada("cobranca", cobrancaId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						var cobrancaParcelamento = new CobrancaParcelamento
						{
							Id = reader.GetValue<int>("id"),
							CobrancaId = reader.GetValue<int>("cobranca"),
						};
						cobrancaParcelamento.DUAS = cobrancaDUADa.Obter(cobrancaParcelamento.CobrancaId);

						lista.Add(cobrancaParcelamento);
					}

					reader.Close();
				}
			}

			return lista;
		}

		#endregion
	}
}