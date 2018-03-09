using System;
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
	public class CobrancaDa
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

        public CobrancaDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

        public Cobranca Salvar(Cobranca Cobranca, BancoDeDados banco = null)
        {
            if (Cobranca == null)
            {
                throw new Exception("Cobranca é nulo.");
            }

            if (Cobranca.Id <= 0)
            {
                Cobranca = Criar(Cobranca, banco);
            }
            else
            {
                Cobranca = Editar(Cobranca, banco);
            }

            return Cobranca;
        }

        public Cobranca Criar(Cobranca cobranca, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"
                                    insert into {0}tab_fisc_cobranca (id,
																	  fiscalizacao,
																	  autuado,
																	  codigoreceita,
																	  serie,
																	  iuf_numero,
																	  iuf_data,
																	  protoc_num,
																	  autos,
																	  not_iuf_data,
																	  not_jiapi_data,
																	  not_core_data,
                                                                   tid)
                                    values ({0}seq_fisc_cobranca.nextval,
											:fiscalizacao,
											:autuado,
											:codigoreceita,
											:serie,
											:iuf_numero,
											:iuf_data,
											:protoc_num,
											:autos,
											:not_iuf_data,
											:not_jiapi_data,
											:not_core_data,
                                            :tid)
                                    returning id into :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("fiscalizacao", cobranca.NumeroFiscalizacao, DbType.Int32);
                comando.AdicionarParametroEntrada("autuado", cobranca.AutuadoPessoaId, DbType.Int32);
                comando.AdicionarParametroEntrada("codigoreceita", cobranca.CodigoReceitaId, DbType.Int32);
                comando.AdicionarParametroEntrada("serie", cobranca.SerieId, DbType.Int32);
                comando.AdicionarParametroEntrada("iuf_numero", cobranca.NumeroIUF, DbType.Int32);
                comando.AdicionarParametroEntrada("iuf_data", cobranca.DataLavratura.Data, DbType.DateTime);
                comando.AdicionarParametroEntrada("protoc_num", cobranca.ProcessoNumero, DbType.Int32);
                comando.AdicionarParametroEntrada("autos", cobranca.NumeroAutos, DbType.Int32);
                comando.AdicionarParametroEntrada("not_iuf_data", cobranca.DataIUF.Data, DbType.DateTime);
                comando.AdicionarParametroEntrada("not_jiapi_data", cobranca.DataJIAPI.Data, DbType.DateTime);
                comando.AdicionarParametroEntrada("not_core_data", cobranca.DataCORE.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroSaida("id", DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                cobranca.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				Historico.Gerar(cobranca.Id, eHistoricoArtefato.cobranca, eHistoricoAcao.criar, bancoDeDados);

                //Consulta.Gerar(cobranca.Id, eHistoricoArtefato.cobranca, bancoDeDados);

                bancoDeDados.Commit();
            }
            return cobranca;
        }

        public Cobranca Editar(Cobranca cobranca, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"
                                    update {0}tab_fisc_cobranca t
                                    set 
										t.fiscalizacao = :fiscalizacao,
										t.autuado = :autuado,
										t.codigoreceita = :codigoreceita,
										t.serie = :serie,
										t.iuf_numero = :iuf_numero,
										t.iuf_data = :iuf_data,
										t.protoc_num = :protoc_num,
										t.autos = :autos,
										t.not_iuf_data = :not_iuf_data,
										t.not_jiapi_data = :not_jiapi_data,
										t.not_core_data = :not_core_data,
                                        t.tid = :tid
                                    where t.id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", cobranca.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("fiscalizacao", cobranca.NumeroFiscalizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("autuado", cobranca.AutuadoPessoaId, DbType.Int32);
				comando.AdicionarParametroEntrada("codigoreceita", cobranca.CodigoReceitaId, DbType.Int32);
				comando.AdicionarParametroEntrada("serie", cobranca.SerieId, DbType.Int32);
				comando.AdicionarParametroEntrada("iuf_numero", cobranca.NumeroIUF, DbType.Int32);
				comando.AdicionarParametroEntrada("iuf_data", cobranca.DataLavratura.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("protoc_num", cobranca.ProcessoNumero, DbType.Int32);
				comando.AdicionarParametroEntrada("autos", cobranca.NumeroAutos, DbType.Int32);
				comando.AdicionarParametroEntrada("not_iuf_data", cobranca.DataIUF.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("not_jiapi_data", cobranca.DataJIAPI.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("not_core_data", cobranca.DataCORE.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(cobranca.Id, eHistoricoArtefato.cobranca, eHistoricoAcao.atualizar, bancoDeDados);

                //Consulta.Gerar(cobranca.Id, eHistoricoArtefato.cobranca, bancoDeDados);

                bancoDeDados.Commit();
            }
            return cobranca;
        }

		public bool Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando("update {0}tab_fisc_cobranca t set t.tid = :tid where t.id = :id");
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				#region Histórico

				Historico.Gerar(id, eHistoricoArtefato.cobranca, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				comando = bancoDeDados.CriarComando(
					"begin " +
						"delete {0}tab_fisc_cobranca t where t.id = :id; " +
					"end;", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#region Consulta

				//Consulta.Deletar(id, eHistoricoArtefato.cobranca, bancoDeDados);

				#endregion

				bancoDeDados.Commit();

				return true;
			}
		}

		#endregion

		#region Obter / Filtrar

		public Cobranca Obter(int fiscalizacao, BancoDeDados banco = null)
        {
            var cobranca = new Cobranca();
			var cobrancaParcelamentoDa = new CobrancaParcelamentoDa();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"
                                    select c.id,
											c.fiscalizacao,
											c.autuado,
											c.codigoreceita,
											(select lfc.texto
												from lov_fisc_infracao_codigo_rece lfc
												where lfc.id = c.codigoreceita) as codigoreceita_texto,
											c.serie,
											(select lfs.texto
												from lov_fiscalizacao_serie lfs
												where lfs.id = c.serie) as serie_texto,
											c.iuf_numero,
											c.iuf_data,
											c.protoc_num,
											c.autos,
											c.not_iuf_data,
											c.not_jiapi_data,
											c.not_core_data
										from tab_fisc_cobranca c
										where c.fiscalizacao = :fiscalizacao", EsquemaBanco);

                comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacao, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        cobranca = new Cobranca
                        {
                            Id = reader.GetValue<int>("id"),
							NumeroFiscalizacao = reader.GetValue<int>("fiscalizacao"),
							ProcessoNumero = reader.GetValue<string>("protoc_num"),
							NumeroAutos = reader.GetValue<int>("autos"),
							NumeroIUF = reader.GetValue<string>("iuf_numero"),
							SerieId = reader.GetValue<int>("serie"),
							SerieTexto = reader.GetValue<string>("serie_texto"),
							AutuadoPessoaId = reader.GetValue<int>("autuado"),
							CodigoReceitaId = reader.GetValue<int>("codigoreceita"),
							CodigoReceitaTexto = reader.GetValue<string>("codigoreceita_texto")
						};

						cobranca.DataLavratura.Data = reader.GetValue<DateTime>("iuf_data");
						cobranca.DataIUF.Data = reader.GetValue<DateTime>("not_iuf_data");
						cobranca.DataJIAPI.Data = reader.GetValue<DateTime>("not_jiapi_data");
						cobranca.DataCORE.Data = reader.GetValue<DateTime>("not_core_data");
						if (cobranca.DataLavratura.Data.HasValue && cobranca.DataLavratura.Data.Value.Year == 1)
							cobranca.DataLavratura = new DateTecno();
						if (cobranca.DataIUF.Data.HasValue && cobranca.DataIUF.Data.Value.Year == 1)
							cobranca.DataIUF = new DateTecno();
						if (cobranca.DataJIAPI.Data.HasValue && cobranca.DataJIAPI.Data.Value.Year == 1)
							cobranca.DataJIAPI = new DateTecno();
						if (cobranca.DataCORE.Data.HasValue && cobranca.DataCORE.Data.Value.Year == 1)
							cobranca.DataCORE = new DateTecno();
						cobranca.AutuadoPessoa = cobranca.AutuadoPessoaId > 0 ? new PessoaBus().Obter(cobranca.AutuadoPessoaId) : new Pessoa();
						cobranca.Notificacao = cobranca.NumeroFiscalizacao > 0 ? new NotificacaoBus().Obter(cobranca.NumeroFiscalizacao) : new Notificacao();
						cobranca.Parcelamentos = cobrancaParcelamentoDa.Obter(cobranca.Id);
					}
                    else
                    {
                        cobranca = null;
                    }
                    reader.Close();
                }
			}

			return cobranca;
        }

		#endregion
	}
}