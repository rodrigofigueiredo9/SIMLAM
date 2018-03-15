using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data
{
	public class CobrancaDUADa
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

        public CobrancaDUADa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

        public CobrancaDUA Salvar(CobrancaDUA cobrancaDUA, BancoDeDados banco = null)
        {
            if (cobrancaDUA == null)
                throw new Exception("Cobranca é nulo.");

			if (cobrancaDUA.ParcelaPaiId == 0)
				cobrancaDUA.ParcelaPaiId = null;

			if (cobrancaDUA.Id <= 0)
                cobrancaDUA = Criar(cobrancaDUA, banco);
            else
                cobrancaDUA = Editar(cobrancaDUA, banco);

            return cobrancaDUA;
        }

        public CobrancaDUA Criar(CobrancaDUA cobrancaDUA, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"
                                    insert into {0}tab_fisc_cob_dua (id,
																vencimento_data,
																dataemissao,
																parcela,				  
																numero_dua,
																valor_dua, 
																valor_pago,
																vrte,
																pagamento_data, 
																complemento,
																pai_dua, 
																cob_parc,
																cancelamento_data,
                                                                tid)
                                    values ({0}seq_fisc_cob_dua.nextval,
											:vencimento_data,
											:dataemissao,
											:parcela,				  
											:numero_dua,
											:valor_dua, 
											:valor_pago,
											:vrte,
											:pagamento_data, 
											:complemento,
											:pai_dua, 
											:cob_parc,
											:cancelamento_data,
                                            :tid)
                                    returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("vencimento_data", cobrancaDUA.DataVencimento.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("dataemissao", cobrancaDUA.DataEmissao.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("parcela", cobrancaDUA.Parcela, DbType.String);
				comando.AdicionarParametroEntrada("numero_dua", cobrancaDUA.NumeroDUA, DbType.Int32);
				comando.AdicionarParametroEntrada("valor_dua", cobrancaDUA.ValorDUA, DbType.Decimal);
				comando.AdicionarParametroEntrada("valor_pago", cobrancaDUA.ValorPago, DbType.Decimal);
				comando.AdicionarParametroEntrada("vrte", cobrancaDUA.VRTE, DbType.Decimal);
				comando.AdicionarParametroEntrada("pagamento_data", cobrancaDUA.DataPagamento.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("complemento", cobrancaDUA.InformacoesComplementares, DbType.String);
				comando.AdicionarParametroEntrada("pai_dua", cobrancaDUA.ParcelaPaiId, DbType.Int32);
				comando.AdicionarParametroEntrada("cob_parc", cobrancaDUA.ParcelamentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("cancelamento_data", cobrancaDUA.DataCancelamento.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroSaida("id", DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                cobrancaDUA.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				Historico.Gerar(cobrancaDUA.Id, eHistoricoArtefato.cobranca, eHistoricoAcao.atualizar, bancoDeDados);

                //Consulta.Gerar(cobrancaDUA.Id, eHistoricoArtefato.cobranca, bancoDeDados);

                bancoDeDados.Commit();
            }
            return cobrancaDUA;
        }

        public CobrancaDUA Editar(CobrancaDUA cobrancaDUA, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"
                                    update {0}tab_fisc_cob_dua t
                                    set 
										t.vencimento_data = :vencimento_data,
										t.dataemissao = :dataemissao,
										t.parcela = :parcela,
										t.numero_dua = :numero_dua,
										t.valor_dua = :valor_dua,
										t.valor_pago = :valor_pago,
										t.vrte = :vrte,
										t.pagamento_data = :pagamento_data,
										t.complemento = :complemento,
										t.pai_dua = :pai_dua,
										t.cob_parc = :cob_parc,
										t.cancelamento_data = :cancelamento_data,
                                        t.tid = :tid
                                    where t.id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", cobrancaDUA.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("vencimento_data", cobrancaDUA.DataVencimento.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("dataemissao", cobrancaDUA.DataEmissao.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("parcela", cobrancaDUA.Parcela, DbType.String);
				comando.AdicionarParametroEntrada("numero_dua", cobrancaDUA.NumeroDUA, DbType.Int32);
				comando.AdicionarParametroEntrada("valor_dua", cobrancaDUA.ValorDUA, DbType.Decimal);
				comando.AdicionarParametroEntrada("valor_pago", cobrancaDUA.ValorPago, DbType.Decimal);
				comando.AdicionarParametroEntrada("vrte", cobrancaDUA.VRTE, DbType.Decimal);
				comando.AdicionarParametroEntrada("pagamento_data", cobrancaDUA.DataPagamento.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("complemento", cobrancaDUA.InformacoesComplementares, DbType.String);
				comando.AdicionarParametroEntrada("pai_dua", cobrancaDUA.ParcelaPaiId, DbType.Int32);
				comando.AdicionarParametroEntrada("cob_parc", cobrancaDUA.ParcelamentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("cancelamento_data", cobrancaDUA.DataCancelamento.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(cobrancaDUA.Id, eHistoricoArtefato.cobranca, eHistoricoAcao.atualizar, bancoDeDados);

                //Consulta.Gerar(cobrancaDUA.Id, eHistoricoArtefato.cobranca, bancoDeDados);

                bancoDeDados.Commit();
            }
            return cobrancaDUA;
        }

		public bool Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando("update {0}tab_fisc_cob_dua t set t.tid = :tid where t.id = :id");
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				#region Histórico

				Historico.Gerar(id, eHistoricoArtefato.cobranca, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				comando = bancoDeDados.CriarComando(
					"begin " +
						"delete {0}tab_fisc_cob_dua t where t.id = :id; " +
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

		public CobrancaDUA ObterDUA(int cobrancaDUAId, BancoDeDados banco = null)
        {
            var cobrancaDUA = new CobrancaDUA();
            
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"
                                    select d.id,
											d.vencimento_data,
											d.dataemissao,
											d.parcela,
											d.numero_dua,
											d.valor_dua,
											d.valor_pago,
											d.vrte,
											d.pagamento_data,
											d.complemento,
											d.pai_dua,
											d.cob_parc,
											d.cancelamento_data,
											d.tid
										from tab_fisc_cob_dua d
										where d.id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", cobrancaDUAId, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        cobrancaDUA = new CobrancaDUA
						{
							Id = reader.GetValue<int>("id"),
							Parcela = reader.GetValue<string>("parcela"),
							NumeroDUA = reader.GetValue<int>("numero_dua"),
							ValorDUA = reader.GetValue<decimal>("valor_dua"),
							ValorPago = reader.GetValue<decimal>("valor_pago"),
							VRTE = reader.GetValue<decimal>("vrte"),
							InformacoesComplementares = reader.GetValue<string>("complemento"),
							ParcelaPaiId = reader.GetValue<int>("pai_dua"),
							ParcelamentoId = reader.GetValue<int>("cob_parc"),
							Tid = reader.GetValue<string>("tid")
						};

						cobrancaDUA.DataVencimento.Data = reader.GetValue<DateTime>("vencimento_data");
						cobrancaDUA.DataEmissao.Data = reader.GetValue<DateTime>("dataemissao");
						cobrancaDUA.DataPagamento.Data = reader.GetValue<DateTime>("pagamento_data");
						cobrancaDUA.DataCancelamento.Data = reader.GetValue<DateTime>("cancelamento_data");
						if (cobrancaDUA.DataVencimento.Data.HasValue && cobrancaDUA.DataVencimento.Data.Value.Year == 1)
							cobrancaDUA.DataVencimento = new DateTecno();
						if (cobrancaDUA.DataEmissao.Data.HasValue && cobrancaDUA.DataEmissao.Data.Value.Year == 1)
							cobrancaDUA.DataEmissao = new DateTecno();
						if (cobrancaDUA.DataPagamento.Data.HasValue && cobrancaDUA.DataPagamento.Data.Value.Year == 1)
							cobrancaDUA.DataPagamento = new DateTecno();
						if (cobrancaDUA.DataCancelamento.Data.HasValue && cobrancaDUA.DataCancelamento.Data.Value.Year == 1)
							cobrancaDUA.DataCancelamento = new DateTecno();
					}
                    else
                    {
                        cobrancaDUA = null;
                    }
                    reader.Close();
                }
			}

			return cobrancaDUA;
        }

		public List<CobrancaDUA> Obter(int parcelamentoId, BancoDeDados banco = null)
		{
			var lista = new List<CobrancaDUA>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select d.id,
											d.vencimento_data,
											d.dataemissao,
											d.parcela,
											d.numero_dua,
											d.valor_dua,
											d.valor_pago,
											d.vrte,
											d.pagamento_data,
											d.complemento,
											d.pai_dua,
											d.cob_parc,
											d.cancelamento_data,
											d.tid,
											case
											when d.cancelamento_data is not null
												then 'Cancelado'
											when d.pagamento_data is null 
												and d.vencimento_data >= sysdate
												or d.valor_dua is null
												or d.valor_dua = 0
												then 'Em Aberto'
											when d.pagamento_data is not null
												and d.valor_pago >= d.valor_dua
												or exists (select 1 from tab_fisc_cob_dua dc where dc.pai_dua = d.id)
												then 'Pago'
											when d.pagamento_data is not null
												and d.valor_pago < d.valor_dua
												then 'Pago Parcial'
											when d.pagamento_data is null and d.vencimento_data < sysdate
												then 'Atrasado'
											end as situacao
										from {0}tab_fisc_cob_dua d
										where d.cob_parc = :parcelamentoId
										order by d.parcela", EsquemaBanco);

                comando.AdicionarParametroEntrada("parcelamentoId", parcelamentoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						var cobrancaDUA = new CobrancaDUA
						{
							Id = reader.GetValue<int>("id"),
							Parcela = reader.GetValue<string>("parcela"),
							NumeroDUA = reader.GetValue<int>("numero_dua"),
							ValorDUA = reader.GetValue<decimal>("valor_dua"),
							ValorPago = reader.GetValue<decimal>("valor_pago"),
							VRTE = reader.GetValue<decimal>("vrte"),
							InformacoesComplementares = reader.GetValue<string>("complemento"),
							ParcelaPaiId = reader.GetValue<int>("pai_dua"),
							ParcelamentoId = reader.GetValue<int>("cob_parc"),
							Tid = reader.GetValue<string>("tid"),
							Situacao = reader.GetValue<string>("situacao")
						};

						cobrancaDUA.DataVencimento.Data = reader.GetValue<DateTime>("vencimento_data");
						cobrancaDUA.DataEmissao.Data = reader.GetValue<DateTime>("dataemissao");
						cobrancaDUA.DataPagamento.Data = reader.GetValue<DateTime>("pagamento_data");
						cobrancaDUA.DataCancelamento.Data = reader.GetValue<DateTime>("cancelamento_data");
						if (cobrancaDUA.DataVencimento.Data.HasValue && cobrancaDUA.DataVencimento.Data.Value.Year == 1)
							cobrancaDUA.DataVencimento = new DateTecno();
						if (cobrancaDUA.DataEmissao.Data.HasValue && cobrancaDUA.DataEmissao.Data.Value.Year == 1)
							cobrancaDUA.DataEmissao = new DateTecno();
						if (cobrancaDUA.DataPagamento.Data.HasValue && cobrancaDUA.DataPagamento.Data.Value.Year == 1)
							cobrancaDUA.DataPagamento = new DateTecno();
						if (cobrancaDUA.DataCancelamento.Data.HasValue && cobrancaDUA.DataCancelamento.Data.Value.Year == 1)
							cobrancaDUA.DataCancelamento = new DateTecno();
						if (cobrancaDUA.ParcelaPaiId > 0)
							cobrancaDUA.ParcelaPai = this.ObterDUA(cobrancaDUA.ParcelaPaiId.Value);

						lista.Add(cobrancaDUA);
					}

					reader.Close();
				}
			}

			return lista;
		}
		#endregion
	}
}