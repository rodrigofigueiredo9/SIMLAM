using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertidao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertidao.Data
{
	public class CertidaoDebitoDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();

		internal Historico Historico { get { return _historico; } }

		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private string EsquemaBanco { get; set; }

		#endregion

		public CertidaoDebitoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(CertidaoDebito certidao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cadastro do Titulo

				eHistoricoAcao acao;
				object id;

				//Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_certidao_debito e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", certidao.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"update {0}esp_certidao_debito e set e.titulo = :titulo, e.protocolo = :protocolo, 
														e.destinatario = :destinatario, e.tid = :tid where e.titulo = :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					certidao.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}esp_certidao_debito (id, titulo, protocolo, destinatario, tid) 
														values ({0}seq_esp_certidao_debito.nextval, :titulo,  :protocolo, :destinatario, :tid) 
														returning id into :id", EsquemaBanco);

					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("titulo", certidao.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", certidao.ProtocoloReq.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario", certidao.Destinatario, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					certidao = certidao ?? new CertidaoDebito();
					certidao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				#region Fiscalizacoes

				#region Limpar os dados do banco

				comando = bancoDeDados.CriarComando(@"delete from {0}esp_certidao_deb_fisc c where c.especificidade = :especificidade", EsquemaBanco);

				comando.AdicionarParametroEntrada("especificidade", certidao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				foreach (Fiscalizacao fiscalizacao in certidao.Fiscalizacoes)
				{
					comando = bancoDeDados.CriarComando(@"insert into esp_certidao_deb_fisc(id, especificidade, fiscalizacao_id, 
														fiscalizacao_tid, protocolo_id, protocolo_tid, tid) values(seq_esp_certidao_deb_fisc.nextval, :especificidade,
														:fiscalizacao_id, :fiscalizacao_tid, :protocolo_id, :protocolo_tid, :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("especificidade", certidao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("fiscalizacao_id", fiscalizacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("fiscalizacao_tid", fiscalizacao.Tid, DbType.String);
					comando.AdicionarParametroEntrada("protocolo_id", fiscalizacao.ProtocoloId.HasValue ? fiscalizacao.ProtocoloId.Value : (Object)DBNull.Value, DbType.Int32);
					comando.AdicionarParametroEntrada("protocolo_tid", !String.IsNullOrWhiteSpace(fiscalizacao.ProtocoloTid) ? fiscalizacao.ProtocoloTid : (Object)DBNull.Value, DbType.String);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Histórico

				Historico.Gerar(Convert.ToInt32(certidao.Titulo.Id), eHistoricoArtefatoEspecificidade.certidaodebito, acao, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int titulo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}esp_certidao_debito c set c.tid = :tid where c.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.certidaodebito, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComando("begin " +
													"delete from {0}esp_certidao_deb_fisc f where f.especificidade = (select id from esp_certidao_debito where titulo = :titulo);" +
													"delete from {0}esp_certidao_debito e where e.titulo = :titulo;" +
													"end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal CertidaoDebito Obter(int titulo, BancoDeDados banco = null)
		{
			CertidaoDebito especificidade = new CertidaoDebito();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.tid, e.protocolo, n.numero, n.ano, p.requerimento, p.protocolo protocolo_tipo, e.destinatario, 
				(select distinct nvl(pe.nome, pe.razao_social) from {0}hst_esp_certidao_debito he, {0}hst_pessoa pe where he.destinatario_id = pe.pessoa_id and he.destinatario_tid = pe.tid
				and pe.data_execucao = (select max(h.data_execucao) from {0}hst_pessoa h where h.pessoa_id = pe.pessoa_id and h.tid = pe.tid) and he.especificidade_id = e.id
				and not exists(select 1 from {0}lov_historico_artefatos_acoes l where l.id = he.acao_executada and l.acao = 3) 
				and he.titulo_tid = (select ht.tid from {0}hst_titulo ht where ht.titulo_id = e.titulo and ht.data_execucao = (select min(htt.data_execucao) from {0}hst_titulo htt where htt.titulo_id = e.titulo 
				and htt.data_execucao > (select max(httt.data_execucao) from {0}hst_titulo httt where httt.titulo_id = e.titulo and httt.situacao_id = 1)))) destinatario_nome_razao 
				from {0}esp_certidao_debito e, {0}tab_titulo_numero n, {0}tab_protocolo p where n.titulo(+) = e.titulo and e.protocolo = p.id(+) and e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						especificidade.Titulo.Id = titulo;
						especificidade.Id = Convert.ToInt32(reader["id"]);
						especificidade.Tid = reader["tid"].ToString();

						if (reader["protocolo"] != null && !Convert.IsDBNull(reader["protocolo"]))
						{
							especificidade.ProtocoloReq.IsProcesso = (reader["protocolo_tipo"] != null && Convert.ToInt32(reader["protocolo_tipo"]) == 1);
							especificidade.ProtocoloReq.RequerimentoId = Convert.ToInt32(reader["requerimento"]);
							especificidade.ProtocoloReq.Id = Convert.ToInt32(reader["protocolo"]);
						}

						if (reader["destinatario"] != null && !Convert.IsDBNull(reader["destinatario"]))
						{
							especificidade.Destinatario = Convert.ToInt32(reader["destinatario"]);
							especificidade.DestinatarioNomeRazao = Convert.ToString(reader["destinatario_nome_razao"]);
						}

						if (reader["numero"] != null && !Convert.IsDBNull(reader["numero"]))
						{
							especificidade.Titulo.Numero.Inteiro = Convert.ToInt32(reader["numero"]);
						}

						if (reader["ano"] != null && !Convert.IsDBNull(reader["ano"]))
						{
							especificidade.Titulo.Numero.Ano = Convert.ToInt32(reader["ano"]);
						}

						comando = bancoDeDados.CriarComando(@"select f.fiscalizacao_id, f.fiscalizacao_tid, protocolo_id, protocolo_tid 
															from {0}esp_certidao_deb_fisc f  where f.especificidade = :especificidade", EsquemaBanco);

						comando.AdicionarParametroEntrada("especificidade", especificidade.Id, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							while (readerAux.Read())
							{
								Int32 fiscalizacaoId = readerAux.GetValue<Int32>("fiscalizacao_id");
								String fiscalizacaoTid = readerAux.GetValue<String>("fiscalizacao_tid");
								Int32? protocoloId = readerAux.GetValue<Int32?>("protocolo_id");
								String protocoloTid = readerAux.GetValue<String>("protocolo_tid");

								Fiscalizacao fiscalizacao = new Fiscalizacao();
								fiscalizacao = ObterFiscalizacaoPorHistorico(fiscalizacaoId, fiscalizacaoTid, protocoloId.GetValueOrDefault(0), protocoloTid);

								especificidade.Fiscalizacoes.Add(fiscalizacao);
							}

							readerAux.Close();
						}
					}

					reader.Close();
				}

				#endregion
			}

			return especificidade;
		}

		internal Certidao ObterDadosPDF(int titulo, BancoDeDados banco = null)
		{
			Certidao certidao = new Certidao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados do Titulo

				DadosPDF dados = DaEsp.ObterDadosTitulo(titulo, bancoDeDados);

				certidao.Titulo = dados.Titulo;
				certidao.Titulo.SetorEndereco = DaEsp.ObterEndSetor(certidao.Titulo.SetorId);
				certidao.Protocolo = dados.Protocolo;
				certidao.Empreendimento = dados.Empreendimento;

				#endregion

				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.destinatario, e.protocolo from {0}esp_certidao_debito e where e.titulo = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						certidao.Destinatario.Id = Convert.ToInt32(reader["destinatario"]);
						certidao.Protocolo.Id = Convert.ToInt32(reader["protocolo"]);
					}

					reader.Close();
				}

				#endregion

				certidao.Destinatario = DaEsp.ObterDadosPessoa(certidao.Destinatario.Id, certidao.Empreendimento.Id, bancoDeDados);
			}

			return certidao;
		}

		#endregion

		#region Auxiliares

		internal List<Fiscalizacao> ObterFiscalizacoesPorAutuado(int autuadoId, BancoDeDados banco = null)
		{
			List<Fiscalizacao> fiscalizacoes = new List<Fiscalizacao>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id Id, t.id NumeroFiscalizacao, t.situacao SituacaoId, ls.texto SituacaoTexto,
															t.tid Tid, coalesce(i.infracao_autuada, i.possui_infracao) InfracaoAutuada, (case when p.numero is not null then p.numero 
															|| '/' ||p.ano end) NumeroProcesso, p.tid ProtocoloTid, p.Id ProtocoloId, l.data DataFiscalizacao 
															from {0}tab_fiscalizacao t, {0}lov_fiscalizacao_situacao ls, {0}tab_fisc_local_infracao l,
															{0}tab_fisc_infracao i, {0}tab_protocolo p where t.situacao = ls.id(+) and l.fiscalizacao(+) = t.id
															and i.fiscalizacao = t.id and p.fiscalizacao(+) = t.id and t.id in (select fiscalizacao
															from tab_fisc_local_infracao where pessoa = :autuadoId union all select fiscalizacao
															from tab_fisc_local_infracao where responsavel = :autuadoId)", EsquemaBanco);

				comando.AdicionarParametroEntrada("autuadoId", autuadoId, DbType.Int32);

				fiscalizacoes = bancoDeDados.ObterEntityList<Fiscalizacao>(comando, (IDataReader reader, Fiscalizacao fiscalizacaoItem) =>
				{
					fiscalizacaoItem.DataFiscalizacao = Convert.ToDateTime(reader.GetValue<String>("DataFiscalizacao")).ToShortDateString();
					fiscalizacaoItem.InfracaoAutuada = Convert.ToBoolean(reader.GetValue<Int32>("InfracaoAutuada"));
					fiscalizacaoItem.ProtocoloId = reader.GetValue<Int32?>("ProtocoloId");
				});
			}

			return fiscalizacoes;
		}

		internal Fiscalizacao ObterFiscalizacaoPorHistorico(int fiscalizacaoId, string fiscalizacaoTid, int protocoloId, string protocoloTid, BancoDeDados banco = null)
		{
			Fiscalizacao fiscalizacoes = new Fiscalizacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select f.fiscalizacao_id Id, f.fiscalizacao_id NumeroFiscalizacao, f.situacao_id situacaoId,
															f.situacao_texto situacaoTexto, l.data DataFiscalizacao, coalesce(i.infracao_autuada, i.possui_infracao) InfracaoAutuada,
															(select p.numero || '/' || p.ano from hst_protocolo p where p.fiscalizacao_id = f.fiscalizacao_id and 
															p.id_protocolo = :protocoloId and p.tid = :protocoloTid) NumeroProcesso, f.tid Tid from hst_fiscalizacao f,
															hst_fisc_local_infracao l, hst_fisc_infracao i where f.fiscalizacao_id = :fiscalizacao_id and 
															f.tid = :fiscalizacao_tid and f.data_execucao = (select max(data_execucao) from hst_fiscalizacao
															where fiscalizacao_id = :fiscalizacao_id and tid = f.tid) and l.id_hst = f.id and i.fiscalizacao_id_hst = f.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao_id", fiscalizacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("fiscalizacao_tid", fiscalizacaoTid, DbType.String);
				comando.AdicionarParametroEntrada("protocoloId", protocoloId, DbType.Int32);
				comando.AdicionarParametroEntrada("protocoloTid", protocoloTid, DbType.String);

				fiscalizacoes = bancoDeDados.ObterEntity<Fiscalizacao>(comando, (IDataReader reader, Fiscalizacao fiscalizacaoItem) =>
				{
					fiscalizacaoItem.DataFiscalizacao = Convert.ToDateTime(reader.GetValue<String>("DataFiscalizacao")).ToShortDateString();
				});
			}

			return fiscalizacoes;
		}

		#endregion
	}
}