﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data
{
	public class FiscalizacaoDa
	{
		#region Propriedade e Atributos

		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();
		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }
		private string EsquemaBanco { get; set; }
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		public String EsquemaBancoGeo { get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); } }

		#endregion

		public FiscalizacaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		public int Salvar(Fiscalizacao fiscalizacao, BancoDeDados banco = null)
		{
			if (fiscalizacao == null)
			{
				throw new Exception("Fiscalização é nulo.");
			}

			if (fiscalizacao.Id <= 0)
			{
				Criar(fiscalizacao, banco);
			}
			else
			{
				Editar(fiscalizacao, banco);
			}

			return fiscalizacao.Id;
		}

		public int Criar(Fiscalizacao fiscalizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_fiscalizacao (id, situacao, situacao_data, autuante, tid)
															values ({0}seq_tab_fiscalizacao.nextval, :situacao, :situacao_data, :autuante,
															:tid) returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("situacao", (int)eFiscalizacaoSituacao.EmAndamento, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao_data", DateTime.Now, DbType.Date);
				comando.AdicionarParametroEntrada("autuante", fiscalizacao.Autuante.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				fiscalizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				bancoDeDados.Commit();
			}

			return fiscalizacao.Id;
		}

		public void Editar(Fiscalizacao fiscalizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_fiscalizacao t set t.tid = :tid where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", fiscalizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();
			}
		}

		public void ConcluirCadastro(Fiscalizacao fiscalizacao, bool gerarAutosTermo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_fiscalizacao t set " + (gerarAutosTermo ? @"t.autos = {0}seq_fiscalizacao_autos.nextval," : @"") +
															@"t.vencimento	= :vencimento, t.tid = :tid where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("vencimento", fiscalizacao.Vencimento.DataTexto.ToString() == "01/01/0001" ? null : fiscalizacao.Vencimento.DataTexto, DbType.DateTime);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", fiscalizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#region Alterar Situacao

				fiscalizacao.SituacaoNovaTipo = (int)eFiscalizacaoSituacao.CadastroConcluido;
				fiscalizacao.SituacaoNovaData.Data = DateTime.Now;

				AlterarSituacao(fiscalizacao, banco);

				#endregion

				#region Numero IUF

				comando = bancoDeDados.CriarComando(@"select seq_fisc_iuf_numero.nextval from dual", EsquemaBanco);
				int prox_numero = bancoDeDados.ExecutarScalar<int>(comando);

				comando = bancoDeDados.CriarComando(@"
                            update {0}tab_fisc_apreensao
                            set iuf_numero = :iuf, iuf_data = sysdate
                            where fiscalizacao = :id
                                  and iuf_digital = 1", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", fiscalizacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("iuf", prox_numero, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"
                            update {0}tab_fisc_multa
                            set iuf_numero = :iuf, iuf_data = sysdate
                            where fiscalizacao = :id
                                  and iuf_digital = 1", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", fiscalizacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("iuf", prox_numero, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"
                            update {0}tab_fisc_obj_infracao
                            set iuf_numero = :iuf, iuf_data = sysdate
                            where fiscalizacao = :id
                                  and iuf_digital = 1", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", fiscalizacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("iuf", prox_numero, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"
                            update {0}tab_fisc_outras_penalidades
                            set iuf_numero = :iuf, iuf_data = sysdate
                            where fiscalizacao = :id
                                  and iuf_digital = 1", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", fiscalizacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("iuf", prox_numero, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				bancoDeDados.Commit();
			}
		}

		public void AlterarSituacao(Fiscalizacao fiscalizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = null;

				if (fiscalizacao.SituacaoNovaTipo == (int)eFiscalizacaoSituacao.EmAndamento)
				{
					comando = bancoDeDados.CriarComando(@"update tab_fiscalizacao f set f.situacao = :situacao_nova, f.situacao_data = 
														:situacao_nova_data, f.situacao_anterior = f.situacao, f.situacao_data_anterior = 
														f.situacao_data, f.motivo = :motivo, f.vencimento = :vencimento, f.tid = :tid 
														where f.id = :fiscalizacao", EsquemaBanco);

					comando.AdicionarParametroEntrada("vencimento", (object)DBNull.Value, DbType.Date);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"update tab_fiscalizacao f set f.situacao = :situacao_nova,
														f.situacao_data = :situacao_nova_data, f.situacao_anterior = f.situacao,
														f.situacao_data_anterior = f.situacao_data, f.motivo = :motivo,
														f.tid = :tid where f.id = :fiscalizacao", EsquemaBanco);
				}

				comando.AdicionarParametroEntrada("situacao_nova", fiscalizacao.SituacaoNovaTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao_nova_data", fiscalizacao.SituacaoNovaData.Data, DbType.Date);
				comando.AdicionarParametroEntrada("motivo", String.IsNullOrWhiteSpace(fiscalizacao.SituacaoNovaMotivoTexto) ? (object)DBNull.Value : fiscalizacao.SituacaoNovaMotivoTexto, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		public void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"delete {0}tab_fiscalizacao t where t.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		public void GerarTidExcluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_fiscalizacao c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		public void GerarHistorico(int id, eHistoricoAcao acao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Historico.Gerar(id, eHistoricoArtefato.fiscalizacao, acao, bancoDeDados);
			}
		}

		public void GerarConsulta(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Consulta.Gerar(id, eHistoricoArtefato.fiscalizacao, bancoDeDados);
			}
		}

		public void DeletarConsulta(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Consulta.Deletar(id, eHistoricoArtefato.fiscalizacao, bancoDeDados);
			}
		}

		internal void SalvarDocumentosGerados(Fiscalizacao fiscalizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_fiscalizacao t set t.pdf_auto_termo = :pdf_auto_termo, 
					t.pdf_laudo = :pdf_laudo, t.pdf_croqui = :pdf_croqui, t.pdf_iuf = :pdf_iuf, t.tid = :tid where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("pdf_auto_termo", fiscalizacao.PdfAutoTermo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("pdf_laudo", fiscalizacao.PdfLaudo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("pdf_croqui", fiscalizacao.PdfCroqui.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("pdf_iuf", (fiscalizacao.PdfIUF.Id == 0 ? null : fiscalizacao.PdfIUF.Id), DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", fiscalizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		public Fiscalizacao Obter(int id, BancoDeDados banco = null)
		{
			Fiscalizacao fiscalizacao = new Fiscalizacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id Id, t.situacao SituacaoId, t.situacao_data, ls.texto SituacaoTexto, t.tid Tid, tf.nome funcionarioNome, 
				tf.id funcionarioId, tu.login funcionarioLogin, t.autos NumeroAutos, t.vencimento vencimentoFisc, p.id ProtocoloId, t.pdf_auto_termo, t.pdf_laudo, t.pdf_iuf 
				from {0}tab_fiscalizacao t, {0}tab_protocolo p, {0}lov_fiscalizacao_situacao ls, {0}tab_funcionario tf, {0}tab_usuario tu where t.situacao = ls.id(+) 
				and t.autuante = tf.id and tf.usuario = tu.id and p.fiscalizacao(+) = t.id and t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				fiscalizacao = bancoDeDados.ObterEntity<Fiscalizacao>(comando, (IDataReader reader, Fiscalizacao fiscalizacaoItem) =>
				{
					fiscalizacaoItem.Autuante.Id = reader.GetValue<int>("funcionarioId");
					fiscalizacaoItem.Autuante.Nome = reader.GetValue<string>("funcionarioNome");
					fiscalizacaoItem.Autuante.Usuario.Login = reader.GetValue<string>("funcionarioLogin");
					fiscalizacaoItem.Vencimento.Data = reader.GetValue<DateTime>("vencimentoFisc");
					fiscalizacaoItem.SituacaoAtualData.Data = reader.GetValue<DateTime>("situacao_data");
					fiscalizacaoItem.PdfAutoTermo.Id = reader.GetValue<int>("pdf_auto_termo");
					fiscalizacaoItem.PdfLaudo.Id = reader.GetValue<int>("pdf_laudo");
					fiscalizacaoItem.PdfIUF.Id = reader.GetValue<int>("pdf_iuf");
				});

				fiscalizacao.DataConclusao = ObterDataConclusao(fiscalizacao.Id);
			}
			return fiscalizacao;
		}

		public Fiscalizacao ObterHistorico(int id, BancoDeDados banco = null)
		{
			Fiscalizacao fiscalizacao = new Fiscalizacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.fiscalizacao_id Id, t.situacao_id SituacaoId, t.situacao_data, t.situacao_texto SituacaoTexto, 
					t.tid Tid, t.autos NumeroAutos, t.vencimento vencimentoFisc, t.pdf_auto_termo, t.pdf_laudo, t.pdf_croqui, t.pdf_iuf
					from {0}hst_fiscalizacao t where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				fiscalizacao = bancoDeDados.ObterEntity<Fiscalizacao>(comando, (IDataReader reader, Fiscalizacao fiscalizacaoItem) =>
				{
					fiscalizacaoItem.Vencimento.Data = reader.GetValue<DateTime>("vencimentoFisc");
					fiscalizacaoItem.SituacaoAtualData.Data = reader.GetValue<DateTime>("situacao_data");

					fiscalizacaoItem.PdfAutoTermo.Id = reader.GetValue<Int32>("pdf_auto_termo");
					fiscalizacaoItem.PdfLaudo.Id = reader.GetValue<Int32>("pdf_laudo");
					fiscalizacaoItem.PdfCroqui.Id = reader.GetValue<Int32>("pdf_croqui");
					fiscalizacaoItem.PdfIUF.Id = reader.GetValue<Int32>("pdf_iuf");
				});
			}

			return fiscalizacao;
		}

		internal List<Lista> ObterTipoInfracao()
		{
			List<Lista> lista = new List<Lista>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.texto, t.ativo from {0}cnf_fisc_infracao_tipo t order by t.texto", EsquemaBanco);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lista.Add(new Lista
						{
							Id = reader.GetValue<string>("id"),
							Texto = reader.GetValue<string>("texto"),
							IsAtivo = true
						});
					}

					reader.Close();
				}
			}

			return lista;
		}

		internal List<Lista> ObterItemInfracao()
		{
			List<Lista> lista = new List<Lista>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.texto, t.ativo from {0}cnf_fisc_infracao_item t order by t.texto", EsquemaBanco);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lista.Add(new Lista
						{
							Id = reader.GetValue<string>("id"),
							Texto = reader.GetValue<string>("texto"),
							IsAtivo = true
						});
					}

					reader.Close();
				}
			}

			return lista;
		}

		internal Resultados<Fiscalizacao> Filtrar(Filtro<FiscalizacaoListarFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<Fiscalizacao> retorno = new Resultados<Fiscalizacao>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("e.numero_fiscalizacao", "numero_fiscalizacao", filtros.Dados.NumeroFiscalizacao);

				if (!string.IsNullOrWhiteSpace(filtros.Dados.NumeroAIIUFBloco))
				{
					comandtxt += $" and ((select iuf_numero from tab_fisc_multa m where m.fiscalizacao = e.fiscalizacao_id and rownum = 1) = '{filtros.Dados.NumeroAIIUFBloco}'" +
						$" or (select numero_auto_infracao_bloco from tab_fisc_infracao i where i.fiscalizacao = e.fiscalizacao_id and rownum = 1) = '{filtros.Dados.NumeroAIIUFBloco}'" +
						$" or (select iuf_numero from tab_fisc_obj_infracao obj where obj.fiscalizacao = e.fiscalizacao_id and rownum = 1) = '{filtros.Dados.NumeroAIIUFBloco}'" +
						$" or (select iuf_numero from tab_fisc_apreensao a where a.fiscalizacao = e.fiscalizacao_id and rownum = 1) = '{filtros.Dados.NumeroAIIUFBloco}'" +
						$" or (select tad_numero from tab_fisc_material_apreendido a where a.fiscalizacao = e.fiscalizacao_id and rownum = 1) = '{filtros.Dados.NumeroAIIUFBloco}'" +
						$" or (select iuf_numero from tab_fisc_outras_penalidades p where p.fiscalizacao = e.fiscalizacao_id and rownum = 1) = '{filtros.Dados.NumeroAIIUFBloco}'" +
						$" or e.numero_ai = {filtros.Dados.NumeroAIIUFBloco})";
				}

				comandtxt += comando.FiltroAndLike("e.autuado_nome_razao", "autuado_nome_razao", filtros.Dados.AutuadoNomeRazao, true);

				comandtxt += comando.FiltroAndLike("e.autuado_cpf_cnpj", "autuado_cpf_cnpj", filtros.Dados.AutuadoCpfCnpj);

				comandtxt += comando.FiltroAndLike("e.empreendimento_denominador", "empreendimento_denominador", filtros.Dados.EmpreendimentoDenominador, true);

				comandtxt += comando.FiltroAndLike("e.empreendimento_cnpj", "empreendimento_cnpj", filtros.Dados.EmpreendimentoCnpj);

				comandtxt += comando.FiltroAnd("e.infracao_tipo", "infracao_tipo", filtros.Dados.InfracaoTipo);

				comandtxt += comando.FiltroAnd("e.item_tipo", "item_tipo", filtros.Dados.ItemTipo);

				comandtxt += comando.FiltroAndLike("e.agente_fiscal_texto", "agente_fiscal_texto", filtros.Dados.AgenteFiscal, true, true);

				comandtxt += comando.FiltroAnd("e.situacao", "situacao", filtros.Dados.SituacaoTipo);

				comandtxt += comando.FiltroAnd("e.setor", "setor", filtros.Dados.SetorTipo);

				comandtxt += comando.FiltroAnd("e.protoc_num", "numero", filtros.Dados.Protocolo.Numero);

				comandtxt += comando.FiltroAnd("e.protoc_ano", "ano", filtros.Dados.Protocolo.Ano);

				var filtroArea = "(select area_fiscalizacao from tab_fisc_local_infracao i where i.fiscalizacao = e.fiscalizacao_id)";
				if (filtros.Dados.AreaFiscalizacao == 3)
					comandtxt += $" and {filtroArea} = 0 ";
				else
					comandtxt += comando.FiltroAnd(filtroArea, "area", filtros.Dados.AreaFiscalizacao);

				comandtxt += comando.FiltroAnd("(select classificacao from tab_fisc_infracao i where i.fiscalizacao = e.fiscalizacao_id)", "classificacao", filtros.Dados.Classificacao);

				if (!string.IsNullOrWhiteSpace(filtros.Dados.NumeroTEITADBloco))
					comandtxt += $" and (e.numero_tei= {filtros.Dados.NumeroTEITADBloco} or e.numero_tad = {filtros.Dados.NumeroTEITADBloco})";

				if (filtros.Dados.Serie > 0)
				{
					comandtxt += $" and ((select serie from tab_fisc_multa m where m.fiscalizacao = e.fiscalizacao_id and rownum = 1) = {filtros.Dados.Serie}" +
						$" or (select serie from tab_fisc_infracao i where i.fiscalizacao = e.fiscalizacao_id and rownum = 1) = {filtros.Dados.Serie}" +
						$" or (select coalesce(tei_gerado_pelo_sist_serie, serie) from tab_fisc_obj_infracao obj where obj.fiscalizacao = e.fiscalizacao_id and rownum = 1) = {filtros.Dados.Serie}" +
						$" or (select serie from tab_fisc_apreensao a where a.fiscalizacao = e.fiscalizacao_id and rownum = 1) = {filtros.Dados.Serie}" +
						$" or (select serie from tab_fisc_material_apreendido a where a.fiscalizacao = e.fiscalizacao_id and rownum = 1) = {filtros.Dados.Serie}" +
						$" or (select serie from tab_fisc_outras_penalidades p where p.fiscalizacao = e.fiscalizacao_id and rownum = 1) = {filtros.Dados.Serie})";
				}

				if (!filtros.Dados.DataFiscalizacao.IsEmpty && filtros.Dados.DataFiscalizacao.IsValido)
					comandtxt += $" and (select TO_DATE(coalesce(data_constatacao, data_lavratura_auto)) from tab_fisc_infracao i where i.fiscalizacao = e.fiscalizacao_id  and rownum = 1) = '{filtros.Dados.DataFiscalizacao.DataTexto}'";

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "numero_fiscalizacao", "autuado_nome_razao", "(e.protoc_num||e.protoc_ano)", "data_fiscalizacao", "situacao_texto" };

				if (filtros.OdenarPor > 0)
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				else
					ordenar.Add("numero_fiscalizacao");

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format("select count(*) from {0}lst_fiscalizacao e where 1=1 " + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select e.numero_fiscalizacao, nvl(e.autuado_nome_razao, e.empreendimento_denominador) autuado_nome_razao, 
										    (select TO_DATE(coalesce(data_constatacao, data_lavratura_auto)) from tab_fisc_infracao i where i.fiscalizacao = e.fiscalizacao_id  and rownum = 1) data_fiscalizacao, e.situacao, e.situacao_texto, e.protoc_num, e.protoc_ano,
											coalesce(cast(e.numero_ai as VARCHAR2(10 BYTE)), (select cast(iuf_numero as VARCHAR2(10 BYTE))from tab_fisc_multa m where m.fiscalizacao = e.fiscalizacao_id and rownum = 1),
											(select numero_auto_infracao_bloco from tab_fisc_infracao i where i.fiscalizacao = e.fiscalizacao_id and rownum = 1),
											(select cast(iuf_numero as VARCHAR2(10 BYTE)) from tab_fisc_obj_infracao obj where obj.fiscalizacao = e.fiscalizacao_id and rownum = 1),
											(select cast(iuf_numero as VARCHAR2(10 BYTE)) from tab_fisc_apreensao a where a.fiscalizacao = e.fiscalizacao_id and rownum = 1),
											(select tad_numero from tab_fisc_material_apreendido a where a.fiscalizacao = e.fiscalizacao_id and rownum = 1),
											(select cast(iuf_numero as VARCHAR2(10 BYTE))from tab_fisc_outras_penalidades p where p.fiscalizacao = e.fiscalizacao_id and rownum = 1)
											) numero_ai
											from {0}lst_fiscalizacao e where 1=1 "
				+ comandtxt + DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Fiscalizacao fisc;

					while (reader.Read())
					{
						fisc = new Fiscalizacao();
						fisc.Id = Convert.ToInt32(reader["numero_fiscalizacao"].ToString());
						fisc.NumeroFiscalizacao = fisc.Id.ToString();
						fisc.NomeRazaoSocialAtuado = reader["autuado_nome_razao"].ToString();
						fisc.SituacaoId = Convert.ToInt32(reader["situacao"]);
						fisc.SituacaoTexto = reader["situacao_texto"].ToString();
						fisc.NumeroIUFResultado = reader["numero_ai"].ToString();

						if (reader["protoc_num"] != null && !Convert.IsDBNull(reader["protoc_num"]))
						{
							fisc.ProcessoNumero = reader["protoc_num"].ToString();
							fisc.ProcessoAno = reader["protoc_ano"].ToString();
						}

						if (reader["data_fiscalizacao"] != null && !Convert.IsDBNull(reader["data_fiscalizacao"]))
						{
							fisc.DataFiscalizacao = Convert.ToDateTime(reader["data_fiscalizacao"]).ToShortDateString();
						}

						retorno.Itens.Add(fisc);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		internal List<ListaValor> ObterAssinanteFuncionarios(int setorId, int cargoId, BancoDeDados banco = null)
		{
			List<ListaValor> lst = new List<ListaValor>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select f.id, f.nome 
					from {0}tab_funcionario f 
					where f.id in (
							 select tfc.funcionario
							  from {0}tab_funcionario_setor        tse,
								   {0}tab_funcionario_cargo        tfc
							 where tse.setor = :setorId
							 and tfc.cargo = :cargoId
							 and tse.funcionario = tfc.funcionario)
					order by 2", EsquemaBanco);

				comando.AdicionarParametroEntrada("setorId", setorId, DbType.Int32);
				comando.AdicionarParametroEntrada("cargoId", cargoId, DbType.Int32);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(new ListaValor()
					{
						Id = Convert.ToInt32(item["id"]),
						Texto = item["nome"].ToString(),
						IsAtivo = true
					});
				}
			}

			return lst;
		}

		internal List<ListaValor> ObterAssinanteCargos(int setorId, BancoDeDados banco = null)
		{
			List<ListaValor> lst = new List<ListaValor>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.nome 
					from {0}tab_cargo c 
					where c.id in (
							 select tfc.cargo
							  from {0}tab_funcionario_setor        tse,
								   {0}tab_funcionario_cargo        tfc
							 where tse.setor = :setorId
							 and tse.funcionario = tfc.funcionario)
					order by 2", EsquemaBanco);

				comando.AdicionarParametroEntrada("setorId", setorId, DbType.Int32);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(new ListaValor()
					{
						Id = Convert.ToInt32(item["id"]),
						Texto = item["nome"].ToString(),
						IsAtivo = true
					});
				}
			}

			return lst;
		}

        internal List<FiscalizacaoDocumento> ObterHistoricoDocumentosCancelados(int fiscalizacaoId, BancoDeDados banco = null)
        {
            List<FiscalizacaoDocumento> lst = new List<FiscalizacaoDocumento>();
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"
                                    select f.pdf_iuf,
                                           f.id hst_id,
                                           f.pdf_auto_termo,
                                           f.pdf_laudo,
                                           f.situacao_data,
                                           f.pdf_croqui croqui,
                                           fi.arquivo_id arq_auto_infracao,
                                           fob.arquivo arq_termo_emb_int,
                                           fm.arquivo_id arq_termo_apree_dep,
                                           fcf.arquivo_termo_id arq_termo_comp
                                    from hst_fiscalizacao f,
                                         hst_fisc_infracao fi,
                                         hst_fisc_obj_infracao fob,
                                         hst_fisc_material_apreendido fm,
                                         hst_fisc_consid_final fcf
                                    where f.fiscalizacao_id = :fiscalizacao
                                          and f.situacao_anterior_id = 2
                                          and f.situacao_id = 1
                                          and f.acao_executada = 301
                                          and f.id = fi.fiscalizacao_id_hst
                                          and f.id = fob.fiscalizacao_id_hst(+)
                                          and f.id = fm.fiscalizacao_id_hst(+)
                                          and f.id = fcf.id_hst
                                    order by f.situacao_data desc", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

                int i = 0;

                foreach (var item in daReader)
                {
                    var documento = new FiscalizacaoDocumento();

					documento.FiscalizacaoId = fiscalizacaoId;
					documento.HistoricoId = item.GetValue<int>("hst_id");
					documento.PdfGeradoAutoTermo.Id = item.GetValue<int>("pdf_auto_termo");
					documento.PdfGeradoLaudo.Id = item.GetValue<int>("pdf_laudo");
					documento.SituacaoData.Data = item.GetValue<DateTime>("situacao_data");
					documento.Croqui.Id = item.GetValue<int>("croqui");
					documento.PdfAutoInfracao.Id = item.GetValue<int>("arq_auto_infracao");
					documento.PdfTermoEmbargoInter.Id = item.GetValue<int>("arq_termo_emb_int");
					documento.PdfTermoApreensaoDep.Id = item.GetValue<int>("arq_termo_apree_dep");
					documento.PdfTermoCompromisso.Id = item.GetValue<int>("arq_termo_comp");
					documento.PdfGeradoIUF.Id = item.GetValue<int>("pdf_iuf");

                    if (i > 0 && lst[i - 1].PdfGeradoIUF.Id == documento.PdfGeradoIUF.Id)
                    {
                        documento.PdfGeradoIUF.Id = 0;
                    }

                    lst.Add(documento);

                    i++;
                }

				comando = bancoDeDados.CriarComando(@"
                                    select fcfi.Arquivo_Id pdf_iuf_bloco,
										   fcfi.descricao,
                                           f.id hst_id,
                                           f.situacao_data
									from hst_fiscalizacao f,
									     hst_fisc_consid_final fcf,
									     Hst_Fisc_Consid_Final_Iuf fcfi
									where f.fiscalizacao_id = :fiscalizacao
									      and f.situacao_anterior_id = 2
									      and f.situacao_id = 1
									      and f.acao_executada = 301
									      and fcf.Id = fcfi.Id_Hst(+)
									      and f.id = fcf.id_hst
									order by f.situacao_data desc", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				IEnumerable<IDataReader> daReader2 = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader2)
				{
					var documento = new FiscalizacaoDocumento();

					documento.FiscalizacaoId = fiscalizacaoId;
					documento.HistoricoId = item.GetValue<int>("hst_id");
					documento.SituacaoData.Data = item.GetValue<DateTime>("situacao_data");
					documento.PdfGeradoIUF.Id = item.GetValue<int>("pdf_iuf_bloco");
					documento.NomeArquivo = item.GetValue<string>("descricao");

					if (i > 0 && lst[i - 1].PdfGeradoIUF.Id == documento.PdfGeradoIUF.Id)
					{
						documento.PdfGeradoIUF.Id = 0;
					}

					lst.Add(documento);

					i++;
				}
			}

			lst = lst.OrderByDescending(a => a.HistoricoId).OrderByDescending(a => a.SituacaoData.Data).ToList();

			return lst;
		}

		public Funcionario ObterAutuanteHistorico(int fiscalizacaoId, BancoDeDados banco = null)
		{
			Funcionario funcionario = new Funcionario();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select func.id, func.funcionario_id autuante_id, func.nome autuante_nome, 
															func.cpf autuante_cpf, func.tid autuante_tid from {0}hst_funcionario func,
															(select f.autuante_id, f.autuante_tid from {0}hst_fiscalizacao f where f.fiscalizacao_id = 
															:fiscalizacao and f.situacao_id = 2/*Cadastro Concluido*/ and f.data_execucao =
															(select max(hf.data_execucao) from {0}hst_fiscalizacao hf where hf.situacao_id = 2/*Cadastro Concluido*/
															and hf.fiscalizacao_id = :fiscalizacao)) fisc where func.funcionario_id = fisc.autuante_id
															and func.tid = fisc.autuante_tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				funcionario = bancoDeDados.ObterEntity<Funcionario>(comando, (IDataReader reader, Funcionario item) =>
				{
					item.Id = reader.GetValue<Int32>("autuante_id");
					item.Nome = reader.GetValue<String>("autuante_nome");
					item.Cpf = reader.GetValue<String>("autuante_cpf");
					item.Tid = reader.GetValue<String>("autuante_tid");
				});
			}

			return funcionario;
		}

		#endregion

		#region Validações

		public List<string> TemCadastroVazio(int fiscalizacaoId, BancoDeDados banco = null)
		{
			List<string> lstCadastroVazio = new List<string>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select tab.cadastro from (
						   select  (select count(1) from {0}TAB_FISC_LOCAL_INFRACAO t where t.fiscalizacao = :fiscalizacaoId) qtd, 'Local de Infração' cadastro from dual union all
						   select  (select count(1) from {0}TMP_PROJETO_GEO t where t.fiscalizacao = :fiscalizacaoId) qtd, 'Projeto Geografico' cadastro from dual union all
						   select  (select count(1) from {0}TAB_FISC_INFRACAO t where t.fiscalizacao = :fiscalizacaoId) qtd, 'Infração' cadastro from dual union all
						   select  (select count(1) from {0}TAB_FISC_CONSID_FINAL t where t.fiscalizacao = :fiscalizacaoId) qtd, 'Considerações finais' cadastro from dual union all
						   select (select case when
							 ((select count(1) from tab_fisc_outras_penalidad_infr p where exists (select 1 from tab_fisc_infracao i where i.id = p.infracao and i.fiscalizacao = :fiscalizacaoId)) +
							  (select count(1) from tab_fisc_penalidades_infr p, lov_fisc_penalidades_fixas lfpf where lfpf.texto like '%Advert%' and p.penalidade = lfpf.id and exists (select 1 from tab_fisc_infracao i where i.id = p.infracao and i.fiscalizacao = :fiscalizacaoId))) > 0
							 then (select count(1) from TAB_FISC_OUTRAS_PENALIDADES t where t.fiscalizacao = :fiscalizacaoId)
							 else 1 end qtd from dual) qtd,  'Outras Penalidades' cadastro from dual union all                 
						   select (select case when
							 (select count(1) from tab_fisc_penalidades_infr p, lov_fisc_penalidades_fixas lfpf where lfpf.texto like '%Apreens%' and p.penalidade = lfpf.id and exists (select 1 from tab_fisc_infracao i where i.id = p.infracao and i.fiscalizacao = :fiscalizacaoId)) > 0
							 then (select count(1) from tab_fisc_apreensao t where t.fiscalizacao = :fiscalizacaoId)
							 else 1 end qtd from dual) qtd,  'Apreensao' cadastro from dual union all
						   select (select case when
							 (select count(1) from tab_fisc_penalidades_infr p, lov_fisc_penalidades_fixas lfpf where lfpf.texto like '%Interdi%' and p.penalidade = lfpf.id and exists (select 1 from tab_fisc_infracao i where i.id = p.infracao and i.fiscalizacao = :fiscalizacaoId)) > 0
							 then (select count(1) from tab_fisc_obj_infracao t where t.fiscalizacao = :fiscalizacaoId)
							 else 1 end qtd from dual) qtd,  'Interdicao/Embargo' cadastro from dual) 
					tab where tab.qtd = 0", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacaoId", fiscalizacaoId, DbType.Int32);

				lstCadastroVazio = bancoDeDados.ObterEntityList<string>(comando);
			}
			return lstCadastroVazio;
		}

		public bool PossuiProjetoGeo(int fiscalizacaoId, BancoDeDados banco = null)
		{
			bool possui = false;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select tf.possui_projeto_geo
					from {0}tab_fiscalizacao tf
					where tf.id = :fiscalizacaoId", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacaoId", fiscalizacaoId, DbType.Int32);

				possui = bancoDeDados.ExecutarScalar<int>(comando) > 0 ? true : false;
			}

			return possui;
		}

		public bool ExisteTituloCertidaoDebido(int fiscalizacaoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from esp_certidao_deb_fisc t where t.fiscalizacao_id = :fiscalizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public bool GeradoNumeroIUFDigital(int fiscalizacaoId, BancoDeDados banco = null)
		{
			bool gerado = false;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Apreensão
				Comando comando = bancoDeDados.CriarComando(@"
                                    select count(*)
                                    from {0}hst_fisc_apreensao fa
                                    where fa.fiscalizacao_id = :fiscalizacao
                                          and fa.iuf_digital = 1
                                          and fa.iuf_numero is not null", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				gerado = Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));

				if (gerado == true)
				{
					return gerado;
				}
				#endregion Apreensão

				#region Multa
				comando = bancoDeDados.CriarComando(@"
                                    select count(*)
                                    from {0}hst_fisc_multa fm
                                    where fm.fiscalizacao_id = :fiscalizacao
                                          and fm.iuf_digital = 1
                                          and fm.iuf_numero is not null", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				gerado = Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));

				if (gerado == true)
				{
					return gerado;
				}
				#endregion Multa

				#region Interdição/Embargo
				comando = bancoDeDados.CriarComando(@"
                                    select count(*)
                                    from {0}hst_fisc_obj_infracao foi
                                    where foi.fiscalizacao_id = :fiscalizacao
                                          and foi.iuf_digital = 1
                                          and foi.iuf_numero is not null", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				gerado = Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));

				if (gerado == true)
				{
					return gerado;
				}
				#endregion Interdição/Embargo

				#region Outras Penalidades
				comando = bancoDeDados.CriarComando(@"
                                    select count(*)
                                    from {0}hst_fisc_outras_penalidades fop
                                    where fop.fiscalizacao_id = :fiscalizacao
                                          and fop.iuf_digital = 1
                                          and fop.iuf_numero is not null", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				gerado = Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));

				if (gerado == true)
				{
					return gerado;
				}
				#endregion Outras Penalidades
			}

			return gerado;
		}

		#endregion

		#region Auxiliares

		internal int ObterSituacao(int id, BancoDeDados banco = null)
		{
			int situacaoAtual = 0;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select f.situacao from tab_fiscalizacao f where f.id = :fiscalizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						situacaoAtual = Convert.ToInt32(reader["situacao"]);
					}

					reader.Close();
				}
			}

			return situacaoAtual;
		}

		internal DateTecno ObterDataConclusao(int fiscalizacaoId, BancoDeDados banco = null)
		{
			DateTecno dataConclusao = new DateTecno();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select f.situacao_data data_conclusao from {0}hst_fiscalizacao f 
															where f.situacao_id = 2/*Cadastro Concluido*/ and f.fiscalizacao_id = :fiscalizacao
															and f.data_execucao = (select max(data_execucao) from {0}hst_fiscalizacao where 
															situacao_id = 2/*Cadastro Concluido*/ and fiscalizacao_id = :fiscalizacao)", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						if (reader["data_conclusao"] != null && !Convert.IsDBNull(reader["data_conclusao"]))
						{
							dataConclusao.Data = reader.GetValue<DateTime>("data_conclusao");
						}
					}

					reader.Close();
				}
			}

			return dataConclusao;
		}

		internal bool PossuiAI_TED_TAD(int fiscalizacaoId, BancoDeDados banco = null)
		{
			bool possui = false;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Cabeçalho

				Comando comando = bancoDeDados.CriarComando(@" select 
				  (select count(*) from {0}tab_fisc_infracao tfi where tfi.fiscalizacao = tf.id and tfi.gerado_sistema = '1') is_auto_infracao, 
				  (select count(*) from {0}tab_fisc_material_apreendido tfma where tfma.fiscalizacao = tf.id and tfma.tad_gerado = '1') is_termo_apreensao_deposito, 
				  (select count(*) from {0}tab_fisc_obj_infracao tfoi where  tfoi.fiscalizacao = tf.id and tfoi.tei_gerado_pelo_sist = '1') is_termo_embargo_interdicao
				  from tab_fiscalizacao tf 
				where tf.id = :id ", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", fiscalizacaoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						possui =
						reader.GetValue<Int32>("is_auto_infracao") > 0 ||
						reader.GetValue<Int32>("is_termo_apreensao_deposito") > 0 ||
						reader.GetValue<Int32>("is_termo_embargo_interdicao") > 0;
					}

					reader.Close();
				}

				#endregion
			}

			return possui;
		}

		public bool PossuiDigital(int fiscalizacaoId, BancoDeDados banco = null)
		{
			bool possui = false;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Cabeçalho

				Comando comando = bancoDeDados.CriarComando(@"
                                    select count(tf.id) possui
                                    from tab_fiscalizacao tf
                                    where tf.id = :id
                                          and ( ( select count(m.id) from tab_fisc_multa m where m.fiscalizacao = :id and m.iuf_digital = 1 ) > 0
                                                or ( select count(a.id) from tab_fisc_apreensao a where a.fiscalizacao = :id and a.iuf_digital = 1 ) > 0
                                                or ( select count(oi.id) from tab_fisc_obj_infracao oi where oi.fiscalizacao = :id and oi.iuf_digital = 1 ) > 0
                                                or ( select count(op.id) from tab_fisc_outras_penalidades op where op.fiscalizacao = :id and op.iuf_digital = 1 ) > 0 )", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", fiscalizacaoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						possui = reader.GetValue<Int32>("possui") > 0;
					}

					reader.Close();
				}

				#endregion
			}

			return possui;
		}

		#endregion
	}
}