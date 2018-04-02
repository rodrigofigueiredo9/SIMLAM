﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
				EsquemaBanco = strBancoDeDados;
		}

		#region Ações de DML

		public Cobranca Salvar(Cobranca Cobranca, BancoDeDados banco = null)
		{
			if (Cobranca == null)
				throw new Exception("Cobranca é nulo.");

			if (Cobranca.Id <= 0)
				Cobranca = Criar(Cobranca, banco);
			else
				Cobranca = Editar(Cobranca, banco);

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
																	  numero_autuacao,
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
											:numero_autuacao,
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
				comando.AdicionarParametroEntrada("iuf_data", cobranca.DataEmissaoIUF.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("protoc_num", cobranca.ProcessoNumero, DbType.String);
				comando.AdicionarParametroEntrada("numero_autuacao", cobranca.NumeroAutuacao, DbType.String);
				comando.AdicionarParametroEntrada("not_iuf_data", cobranca.DataIUF.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("not_jiapi_data", cobranca.DataJIAPI.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("not_core_data", cobranca.DataCORE.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				cobranca.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				Historico.Gerar(cobranca.Id, eHistoricoArtefato.cobranca, eHistoricoAcao.criar, bancoDeDados);

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
										t.numero_autuacao = :numero_autuacao,
										t.not_iuf_data = :not_iuf_data,
										t.not_jiapi_data = :not_jiapi_data,
										t.not_core_data = :not_core_data,
                                        t.tid = :tid
                                    where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", cobranca.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("fiscalizacao", cobranca.NumeroFiscalizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("autuado", cobranca.AutuadoPessoaId, DbType.Int32);
				comando.AdicionarParametroEntrada("codigoreceita", cobranca.CodigoReceitaId, DbType.Int32);
				comando.AdicionarParametroEntrada("serie", cobranca.SerieId > 0 ? cobranca.SerieId : null, DbType.Int32);
				comando.AdicionarParametroEntrada("iuf_numero", cobranca.NumeroIUF, DbType.Int32);
				comando.AdicionarParametroEntrada("iuf_data", cobranca.DataEmissaoIUF.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("protoc_num", cobranca.ProcessoNumero, DbType.String);
				comando.AdicionarParametroEntrada("numero_autuacao", cobranca.NumeroAutuacao, DbType.String);
				comando.AdicionarParametroEntrada("not_iuf_data", cobranca.DataIUF.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("not_jiapi_data", cobranca.DataJIAPI.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("not_core_data", cobranca.DataCORE.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(cobranca.Id, eHistoricoArtefato.cobranca, eHistoricoAcao.atualizar, bancoDeDados);

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
											coalesce(i.pessoa, i.responsavel, c.autuado) autuado,
											c.codigoreceita,
											(select lfc.texto
												from lov_fisc_infracao_codigo_rece lfc
												where lfc.id = c.codigoreceita) as codigoreceita_texto,
											c.serie,
											(select lfs.texto
												from lov_fiscalizacao_serie lfs
												where lfs.id = c.serie) serie_texto,
											coalesce(cast(m.iuf_numero as varchar2(10)), tfi.numero_auto_infracao_bloco, cast(f.autos as varchar2(10)), cast(c.iuf_numero as varchar2(10))) iuf_numero,
											coalesce(m.iuf_data, tfi.data_lavratura_auto, c.iuf_data) iuf_data,
											case when p.id > 0
											  then concat(concat(cast(p.numero as VARCHAR2(30)), '/'), cast(p.ano as VARCHAR2(30)))
											  else cast(c.protoc_num as VARCHAR2(30)) end protoc_num,
											coalesce(p.numero_autuacao, c.numero_autuacao) numero_autuacao,
											coalesce(n.forma_iuf_data, c.not_iuf_data) not_iuf_data,
											coalesce(n.forma_jiapi_data, c.not_jiapi_data) not_jiapi_data,
											coalesce(n.forma_core_data, c.not_core_data) not_core_data
										from tab_fisc_cobranca c
										left join tab_fiscalizacao f
											on (f.id = c.fiscalizacao)
										left join tab_protocolo p
											on (p.fiscalizacao = c.fiscalizacao)
										left join tab_fisc_notificacao n
											on (n.fiscalizacao = c.fiscalizacao)
										left join tab_fisc_multa m
											on (m.fiscalizacao = c.fiscalizacao)
										left join tab_fisc_local_infracao i
											on (i.fiscalizacao = f.id)
										left join tab_fisc_infracao tfi
											on (tfi.fiscalizacao = f.id)
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
							NumeroAutuacao = reader.GetValue<string>("numero_autuacao"),
							NumeroIUF = reader.GetValue<string>("iuf_numero"),
							SerieId = reader.GetValue<int>("serie"),
							SerieTexto = reader.GetValue<string>("serie_texto"),
							AutuadoPessoaId = reader.GetValue<int>("autuado"),
							CodigoReceitaId = reader.GetValue<int>("codigoreceita"),
							CodigoReceitaTexto = reader.GetValue<string>("codigoreceita_texto")
						};

						cobranca.DataEmissaoIUF.Data = reader.GetValue<DateTime>("iuf_data");
						cobranca.DataIUF.Data = reader.GetValue<DateTime>("not_iuf_data");
						cobranca.DataJIAPI.Data = reader.GetValue<DateTime>("not_jiapi_data");
						cobranca.DataCORE.Data = reader.GetValue<DateTime>("not_core_data");
						if (cobranca.DataEmissaoIUF.Data.HasValue && cobranca.DataEmissaoIUF.Data.Value.Year == 1)
							cobranca.DataEmissaoIUF = new DateTecno();
						if (cobranca.DataIUF.Data.HasValue && cobranca.DataIUF.Data.Value.Year == 1)
							cobranca.DataIUF = new DateTecno();
						if (cobranca.DataJIAPI.Data.HasValue && cobranca.DataJIAPI.Data.Value.Year == 1)
							cobranca.DataJIAPI = new DateTecno();
						if (cobranca.DataCORE.Data.HasValue && cobranca.DataCORE.Data.Value.Year == 1)
							cobranca.DataCORE = new DateTecno();
						cobranca.AutuadoPessoa = cobranca.AutuadoPessoaId > 0 ? new PessoaBus().Obter(cobranca.AutuadoPessoaId) : new Pessoa();
						cobranca.Notificacao = cobranca.NumeroFiscalizacao > 0 ? new NotificacaoBus().Obter(cobranca.NumeroFiscalizacao) : new Notificacao();
						cobranca.Parcelamentos = cobrancaParcelamentoDa.Obter(cobranca.Id);

						if (cobranca.DataEmissaoIUF.Data.HasValue && cobranca.DataEmissaoIUF.Data.Value.Year == 1)
						{
							var _fiscDA = new FiscalizacaoDa();
							cobranca.DataEmissaoIUF = _fiscDA.ObterDataConclusao(cobranca.NumeroFiscalizacao);
						}
					}
					else
						cobranca = null;

					reader.Close();
				}
			}

			return cobranca;
		}

		public Resultados<CobrancasResultado> CobrancaFiltrar(Filtro<CobrancaListarFiltro> filtros, BancoDeDados banco = null)
		{
			var lista = new Resultados<CobrancasResultado>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				string comandtxt = string.Empty;
				string whereSituacao = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				if (filtros.Dados.NumeroRegistroProcesso != null)
					comandtxt += comando.FiltroAnd("c.protoc_num", "protoc_num", filtros.Dados.NumeroRegistroProcesso);

				if (filtros.Dados.NumeroDUA != null)
					comandtxt += comando.FiltroAnd("d.numero_dua", "numero_dua", filtros.Dados.NumeroDUA);

				if (filtros.Dados.NumeroFiscalizacao != null)
					comandtxt += comando.FiltroAnd("c.fiscalizacao", "fiscalizacao", filtros.Dados.NumeroFiscalizacao);

				if (filtros.Dados.NumeroAIIUF != null)
					comandtxt += comando.FiltroAnd("c.iuf_numero", "iuf_numero", filtros.Dados.NumeroAIIUF);

				if (filtros.Dados.NumeroAutuacao != null)
					comandtxt += comando.FiltroAnd("c.numero_autuacao", "numero_autuacao", filtros.Dados.NumeroAutuacao);

				if (Convert.ToInt32(filtros.Dados.SituacaoFiscalizacao) != 0)
					comandtxt += comando.FiltroAnd("f.situacao", "situacaofiscalizacao", filtros.Dados.SituacaoFiscalizacao);

				if (filtros.Dados.NomeRazaoSocial != null)
					comandtxt += comando.FiltroAndLike("a.nome", "nomeautuado", filtros.Dados.NomeRazaoSocial, true);

				if (filtros.Dados.CPFCNPJ != null)
					comandtxt += comando.FiltroAnd("a.cpf", "cpfautuado", filtros.Dados.CPFCNPJ);

				if (Convert.ToBoolean(filtros.Dados.DataVencimentoDe?.IsValido))
				{
					comandtxt += " and d.vencimento_data >= :vencimentode ";
					comando.AdicionarParametroEntrada("vencimentode", filtros.Dados.DataVencimentoDe.Data, DbType.Date);
				}

				if (Convert.ToBoolean(filtros.Dados.DataVencimentoAte?.IsValido))
				{
					comandtxt += " and d.vencimento_data <= :vencimentoate ";
					comando.AdicionarParametroEntrada("vencimentoate", filtros.Dados.DataVencimentoAte.Data, DbType.Date);
				}

				if (Convert.ToBoolean(filtros.Dados.DataPagamentoDe?.IsValido))
				{
					comandtxt += " and d.pagamento_data >= :pagamentode ";
					comando.AdicionarParametroEntrada("pagamentode", filtros.Dados.DataPagamentoDe.Data, DbType.Date);
				}

				if (Convert.ToBoolean(filtros.Dados.DataPagamentoAte?.IsValido))
				{
					comandtxt += " and d.pagamento_data <= :pagamentoate ";
					comando.AdicionarParametroEntrada("pagamentoate", filtros.Dados.DataPagamentoAte.Data, DbType.Date);
				}

				if (Convert.ToInt32(filtros.Dados.SituacaoDUA) != 0)
					whereSituacao += comando.FiltroAnd("situacao", "situacao", GetEnumSituacaoDuo(Convert.ToInt32(filtros.Dados.SituacaoDUA)));

				#endregion

				#region Ordenação
				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "parcela", "numero_dua", "protoc_num", "iuf_numero", "dataemissao", "valor_dua", "valor_pago", "vrte", "pagamento_data", "situacao" };

				if (filtros.OdenarPor > 0)
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				else
					ordenar.Add("iuf_numero");
				#endregion Ordenação

				comando.DbCommand.CommandText = String.Format(@"select count(*) from (select * from (select d.id,
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
                                                                from tab_fisc_cob_dua d
															    left join tab_fisc_cob_parcelamento p
															    on (d.cob_parc = p.id)
															    left join tab_fisc_cobranca c
																on (p.cobranca = c.id)
																left join tab_fiscalizacao f
																on (c.fiscalizacao = f.id)
																left join tab_pessoa a
																on (c.autuado = a.id)
																where 1=1" + comandtxt + @") consulta 
																where 1=1" + whereSituacao + ")", (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				lista.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				#region QUERY

				comandtxt = String.Format(@"select * from (select d.id,
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
                                            c.fiscalizacao,
                                            c.protoc_num,
                                            c.iuf_numero,
                                            c.numero_autuacao,
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
                                            left join tab_fisc_cob_parcelamento p
											on (d.cob_parc = p.id)
                                            left join tab_fisc_cobranca c
											on (p.cobranca = c.id)
											left join tab_fiscalizacao f
											on (c.fiscalizacao = f.id)
											left join tab_pessoa a
											on (c.autuado = a.id) where 1=1 " + comandtxt + @") cobranca
                                            where 1=1 " + whereSituacao + DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						var cobrancaDUA = new CobrancasResultado
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
							Fiscalizacao = reader.GetValue<string>("fiscalizacao"),
							ProcNumero = reader.GetValue<string>("protoc_num"),
							Situacao = reader.GetValue<string>("situacao"),
							iufNumero = reader.GetValue<string>("iuf_numero")
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

						lista.Itens.Add(cobrancaDUA);
					}

					reader.Close();
				}
			}

			return lista;
		}

		private string GetEnumSituacaoDuo(int v)
		{
			switch (v)
			{
				case (int)eSituacaoCobranca.EmAberto:
					return "Em Aberto";
				case (int)eSituacaoCobranca.Cancelado:
					return "Cancelado";
				case (int)eSituacaoCobranca.Pago:
					return "Pago";
				case (int)eSituacaoCobranca.PagoParcial:
					return "Pago Parcial";
				case (int)eSituacaoCobranca.Atrasado:
					return "Atrasado";
			}

			return "";
		}

		#endregion
	}
}