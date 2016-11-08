using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Data
{
	public class TermoCPFARLRDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();
		GerenciadorConfiguracao<ConfiguracaoSistema> _config = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		internal Historico Historico { get { return _historico; } }

		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private string EsquemaBanco { get; set; }

		private string EsquemaBancoGeo { get; set; }

		#endregion

		public TermoCPFARLRDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			EsquemaBancoGeo = _config.Obter<string>(ConfiguracaoSistema.KeyUsuarioGeo);

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(TermoCPFARLR termo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cadastro da Especificidade

				eHistoricoAcao acao;
				object id;

				// Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_termo_comp_pfarlr e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", termo.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"update {0}esp_termo_comp_pfarlr t set t.protocolo = :protocolo, t.data_emissao_titulo_anterior = :data_emissao_titulo_anterior, numero_averbacao = :numero_averbacao,t.tid = :tid where t.titulo = :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					termo.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@" insert into {0}esp_termo_comp_pfarlr (id, titulo, protocolo, data_emissao_titulo_anterior, numero_averbacao, tid) values ({0}seq_esp_termo_comp_pfarlr.nextval, :titulo, :protocolo,
						:data_emissao_titulo_anterior, :numero_averbacao, :tid) returning id into :id", EsquemaBanco);

					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("titulo", termo.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", termo.ProtocoloReq.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("data_emissao_titulo_anterior", termo.DataTituloAnterior.Data.GetValueOrDefault(), DbType.Date);
				comando.AdicionarParametroEntrada("numero_averbacao", termo.NumeroAverbacao);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					termo = termo ?? new TermoCPFARLR();
					termo.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				#region Destinatário

				comando = bancoDeDados.CriarComando("delete from {0}esp_termo_comp_pfarlr_destina t ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where t.especificidade = :especificidade {0}", comando.AdicionarNotIn("and", "t.id", DbType.Int32, termo.Destinatarios.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("especificidade", termo.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				foreach (var item in termo.Destinatarios)
				{
					if (item.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}esp_termo_comp_pfarlr_destina t set t.destinatario = :destinatario, t.tid = :tid where t.id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}esp_termo_comp_pfarlr_destina (id, especificidade, destinatario, tid) values ({0}seq_esp_termo_comp_pfarlr_dest.nextval, 
							:especificidade, :destinatario, :tid)", EsquemaBanco);
						comando.AdicionarParametroEntrada("especificidade", termo.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("destinatario", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				Historico.Gerar(termo.Titulo.Id, eHistoricoArtefatoEspecificidade.termocomppfarlr, acao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int titulo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComandoPlSql(
				@"begin 
					update {0}esp_termo_comp_pfarlr c set c.tid = :tid where c.titulo = :titulo;
					update {0}esp_termo_comp_pfarlr_destina c set c.tid = :tid where c.especificidade = (select id from esp_termo_comp_pfarlr where titulo = :titulo);
				end;", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.termocomppfarlr, eHistoricoAcao.excluir, bancoDeDados);

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComando(
					"begin " +
					  "delete from {0}esp_termo_comp_pfarlr_destina c where c.especificidade = (select id from esp_termo_comp_pfarlr where titulo = :titulo); " +
					  "delete from {0}esp_termo_comp_pfarlr e where e.titulo = :titulo; " +
					"end; ", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion Ações de DML

		#region Obter

		internal TermoCPFARLR Obter(int titulo, BancoDeDados banco = null)
		{
			TermoCPFARLR especificidade = new TermoCPFARLR();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.titulo, e.protocolo, e.data_emissao_titulo_anterior, e.tid, n.numero, e.numero_averbacao, n.ano, p.requerimento, 
															p.protocolo protocolo_tipo from {0}esp_termo_comp_pfarlr e, {0}tab_titulo_numero n, {0}tab_protocolo p where n.titulo(+) = e.titulo 
															and e.protocolo = p.id and e.titulo = :titulo ", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						especificidade.Titulo.Id = titulo;
						especificidade.Id = reader.GetValue<int>("id");
						especificidade.Tid = reader.GetValue<string>("tid");
						especificidade.DataTituloAnterior.Data = reader.GetValue<DateTime>("data_emissao_titulo_anterior");
						especificidade.ProtocoloReq.IsProcesso = reader.GetValue<int>("protocolo_tipo") == 1;
						especificidade.ProtocoloReq.RequerimentoId = reader.GetValue<int>("requerimento");
						especificidade.ProtocoloReq.Id = reader.GetValue<int>("protocolo");
						especificidade.Titulo.Numero.Inteiro = reader.GetValue<int>("numero");
						especificidade.Titulo.Numero.Ano = reader.GetValue<int>("ano");
						especificidade.NumeroAverbacao = reader.GetValue<string>("numero_averbacao");
					}

					reader.Close();
				}

				#region Destinatário

				comando = bancoDeDados.CriarComando(@" select p.id Id, t.id IdRelacionamento, nvl(p.nome, p.razao_social) Nome from 
													{0}esp_termo_comp_pfarlr_destina t, {0}esp_termo_comp_pfarlr e, {0}tab_pessoa p where 
													t.especificidade = e.id and e.titulo = :titulo and p.id = t.destinatario", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				especificidade.Destinatarios = bancoDeDados.ObterEntityList<TermoCPFARLRDestinatario>(comando);

				#endregion

				#endregion
			}

			return especificidade;
		}

		internal TermoCPFARLR ObterHistorico(int titulo, int situacao, BancoDeDados banco = null)
		{
			TermoCPFARLR especificidade = new TermoCPFARLR();
			Comando comando = null;
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				if (situacao > 0)
				{
					comando = bancoDeDados.CriarComando(@"select e.id,
						e.especificidade_id,
						e.tid,
						p.id_protocolo,
						e.data_emissao_titulo_anterior,
						e.dominio_id,
						n.numero,
						n.ano,
						p.requerimento_id,
						p.protocolo_id protocolo_tipo
					from {0}hst_esp_termo_comp_pfarlr e,
						{0}hst_titulo_numero           n,
						{0}hst_protocolo               p
					where e.titulo_id = n.titulo_id(+)
					and e.titulo_tid = n.titulo_tid(+)
					and e.protocolo_id = p.id_protocolo(+)
					and e.protocolo_tid = p.tid(+)
					and not exists(select 1 from {0}lov_historico_artefatos_acoes l where l.id = e.acao_executada and l.acao = 3) 
					and e.titulo_tid = (select ht.tid from {0}hst_titulo ht where ht.titulo_id = e.titulo_id and ht.data_execucao =
					(select max(htt.data_execucao) from {0}hst_titulo htt where htt.titulo_id = e.titulo_id and htt.situacao_id = :situacao))
					and e.titulo_id = :titulo", EsquemaBanco);

					comando.AdicionarParametroEntrada("situacao", situacao, DbType.Int32);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"select e.id,
						e.especificidade_id,
						e.tid,
						p.id_protocolo,
						e.data_emissao_titulo_anterior,
						e.dominio_id,
						n.numero,
						n.ano,
						p.requerimento_id,
						p.protocolo_id protocolo_tipo
					from {0}hst_esp_termo_comp_pfarlr e,
						{0}hst_titulo_numero           n,
						{0}hst_protocolo               p
					where e.titulo_id = n.titulo_id(+)
					and e.titulo_tid = n.titulo_tid(+)
					and e.protocolo_id = p.id_protocolo(+)
					and e.protocolo_tid = p.tid(+)
					and not exists(select 1 from {0}lov_historico_artefatos_acoes l where l.id = e.acao_executada and l.acao = 3) 
					and e.titulo_tid = (select ht.tid from {0}hst_titulo ht where ht.titulo_id = e.titulo_id and ht.data_execucao =
					(select min(htt.data_execucao) from {0}hst_titulo htt where htt.titulo_id = e.titulo_id and htt.data_execucao >
					(select max(httt.data_execucao) from {0}hst_titulo httt where httt.titulo_id = e.titulo_id and httt.situacao_id = 1)))
					and e.titulo_id = :titulo", EsquemaBanco);
				}

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst = reader.GetValue<int>("id");

						especificidade.Titulo.Id = titulo;
						especificidade.Id = reader.GetValue<int>("especificidade_id");
						especificidade.Tid = reader.GetValue<string>("tid");
						especificidade.DataTituloAnterior.Data = reader.GetValue<DateTime>("data_emissao_titulo_anterior");
						especificidade.ProtocoloReq.IsProcesso = reader.GetValue<int>("protocolo_tipo") == 1;
						especificidade.ProtocoloReq.RequerimentoId = reader.GetValue<int>("requerimento_id");
						especificidade.ProtocoloReq.Id = reader.GetValue<int>("id_protocolo");
						especificidade.Titulo.Numero.Inteiro = reader.GetValue<int>("numero");
						especificidade.Titulo.Numero.Ano = reader.GetValue<int>("ano");
					}

					reader.Close();
				}

				#endregion

				#region Destinatario

				comando = bancoDeDados.CriarComando(@"select distinct(p.pessoa_id), d.termo_destinatario_id id, nvl(p.nome, p.razao_social) nome_razao
				from {0}hst_esp_termo_comp_pfarlr_des d, {0}hst_pessoa p where d.destinatario_id = p.pessoa_id and d.destinatario_tid = p.tid
				and p.data_execucao = (select max(h.data_execucao) from {0}hst_pessoa h where h.pessoa_id = p.pessoa_id and h.tid = p.tid) and d.id_hst = :hst", EsquemaBanco);

				comando.AdicionarParametroEntrada("hst", hst, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						especificidade.Destinatarios.Add(new TermoCPFARLRDestinatario()
						{
							IdRelacionamento = reader.GetValue<int>("id"),
							Id = reader.GetValue<int>("pessoa_id"),
							Nome = reader.GetValue<string>("nome_razao")
						});
					}

					reader.Close();
				}

				#endregion
			}

			return especificidade;
		}

		internal Termo ObterDadosPDF(int titulo, BancoDeDados banco = null)
		{
			Termo termo = new Termo();
			termo.Dominialidade = new DominialidadePDF();
			List<int> destinatarioIds = new List<int>();
			PessoaPDF pessoa = null;
			DateTime dataEmissao;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título

				DadosPDF dados = DaEsp.ObterDadosTitulo(titulo, bancoDeDados);
				termo.Titulo = dados.Titulo;
				termo.Protocolo = dados.Protocolo;
				termo.Empreendimento = dados.Empreendimento;
				termo.Dominialidade = new DominialidadePDF(new DominialidadeBus().ObterPorEmpreendimento(dados.Empreendimento.Id.GetValueOrDefault()));

				#endregion

				#region Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.data_emissao_titulo_anterior, e.numero_averbacao from {0}esp_termo_comp_pfarlr e where e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						dataEmissao = reader.GetValue<DateTime>("data_emissao_titulo_anterior");
						termo.NumeroAverbacao = reader.GetValue<string>("numero_averbacao");

						GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());
						termo.Titulo.TituloAnteriorMesEmissao = _config.Obter<List<String>>(ConfiguracaoSistema.KeyMeses).ElementAt(dataEmissao.Month - 1);
						termo.Titulo.TituloAnteriorDiaEmissao = dataEmissao.Day.ToString();
						termo.Titulo.TituloAnteriorAnoEmissao = dataEmissao.Year.ToString();
					}

					reader.Close();
				}

				#endregion

				#region Informacões do cartorio

				comando = bancoDeDados.CriarComando(@"select stragg(distinct ' nº ' || b.numero_cartorio || ', folha(s) ' || b.numero_folha || ' do livro nº ' || b.numero_livro ||
													', no ' || b.nome_cartorio) cartorio from {0}esp_termo_comp_pfarlr a, {0}crt_dominialidade_reserva b, {0}tab_titulo t
													where b.dominio in (select id from {0}crt_dominialidade_dominio where dominialidade =  (select id from {0}crt_dominialidade 
													where empreendimento = t.empreendimento and t.id = a.titulo)) and a.titulo = :titulo and t.id = a.titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				termo.InformacoesRegistro = bancoDeDados.ExecutarScalar<string>(comando);

				#endregion

				#region Interessado

				comando = bancoDeDados.CriarComando(@" select t.destinatario from {0}esp_termo_comp_pfarlr_destina t, {0}esp_termo_comp_pfarlr e where t.especificidade = e.id and e.titulo = :titulo ", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				destinatarioIds = bancoDeDados.ExecutarList<int>(comando);
				termo.Interessados = new List<PessoaPDF>();
				foreach (int destId in destinatarioIds)
				{
					pessoa = _daEsp.ObterDadosPessoa(destId, dados.Empreendimento.Id, banco);
					pessoa.VinculoTipoTexto = pessoa.VinculoTipoTexto == "Outro" ? "Representante" : pessoa.VinculoTipoTexto;
					termo.Interessados.Add(pessoa);
				}

				#endregion

				#region ARLs

				comando = bancoDeDados.CriarComando(@"select r.identificacao, r.arl_croqui, null coordenadaN, null coordenadaE,
													r.situacao_vegetal from {0}crt_dominialidade_reserva r, {0}esp_termo_comp_pfarlr e where e.titulo = :titulo
													and r.dominio in (select id from {0}crt_dominialidade_dominio where dominialidade = :dominialidade)", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("dominialidade", termo.Dominialidade.Id, DbType.Int32);

				AreaReservaLegalPDF areaARLPdf = null;

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					termo.RLFormacao = new List<AreaReservaLegalPDF>();
					termo.RLPreservada = new List<AreaReservaLegalPDF>();

					while (reader.Read())
					{
						areaARLPdf = new AreaReservaLegalPDF()
						{
							Tipo = reader.GetValue<int>("situacao_vegetal"),
							AreaCroqui = Convert.ToDecimal(reader.GetValue<string>("arl_croqui")).ToStringTrunc(),
							CoordenadaE = reader.GetValue<string>("coordenadaE"),
							CoordenadaN = reader.GetValue<string>("coordenadaN"),
							Identificacao = reader.GetValue<string>("identificacao")
						};

						if (areaARLPdf.Tipo == (int)eReservaLegalSituacaoVegetal.Preservada)
						{
							termo.RLPreservada.Add(areaARLPdf);
							
						}
						else if (areaARLPdf.Tipo == (int)eReservaLegalSituacaoVegetal.EmRecuperacao)
						{
							termo.RLFormacao.Add(areaARLPdf);
							
						}
					}

					reader.Close();
				}

				termo.RLTotalPreservada = termo.RLPreservada.Sum(x => Convert.ToDecimal(x.AreaCroqui)).ToStringTrunc();
				termo.RLTotalFormacao = termo.RLFormacao.Sum(x => Convert.ToDecimal(x.AreaCroqui)).ToStringTrunc();

				#endregion

				#region ARLs - Coordenadas

				comando = bancoDeDados.CriarComando(@"select arl.codigo, c.empreendimento, c.id dominialidade, pr.id, trunc(arl_o.column_value, 2) coordenada
													from {0}crt_dominialidade c, {0}crt_projeto_geo pr, {0}crt_dominialidade_reserva cr, {1}geo_arl arl,
													table(geometria9i.pontoIdeal(arl.geometry).SDO_ORDINATES) arl_o where c.id = :dominialidade and pr.empreendimento
													= c.empreendimento and arl.codigo = :codigo and pr.caracterizacao = 1 and cr.dominio in (select id from 
													{0}crt_dominialidade_dominio where dominialidade = c.id) and cr.identificacao = arl.codigo 
													and arl.projeto = pr.id", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("dominialidade", termo.Dominialidade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("codigo", DbType.String);

				foreach (var item in termo.RLFormacao)
				{
					comando.SetarValorParametro("codigo", item.Identificacao);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							item.CoordenadaE = reader.GetValue<string>("coordenada");
						}

						if (reader.Read())
						{
							item.CoordenadaN = reader.GetValue<string>("coordenada");
						}

						reader.Close();
					}
				}

				foreach (var item in termo.RLPreservada)
				{
					comando.SetarValorParametro("codigo", item.Identificacao);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							item.CoordenadaE = reader.GetValue<string>("coordenada");
						}

						if (reader.Read())
						{
							item.CoordenadaN = reader.GetValue<string>("coordenada");
						}

						reader.Close();
					}
				}

				#endregion
			}

			return termo;
		}

		internal TermoCPFARLRTituloAnterior ObterTituloAnterior(int atividadeId, int protocoloId)
		{
			TermoCPFARLRTituloAnterior titulo = null;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@" select tpaf.titulo_anterior_id is_emitido_orgao, tpaf.titulo_anterior_numero numero, tpaf.modelo_anterior_nome nome
				  , tpaf.orgao_expedidor orgao from {0}tab_protocolo_ativ_finalida tpaf, {0}tab_protocolo_atividades tpa where tpa.id = tpaf.protocolo_ativ and tpa.atividade = :atividadeId and 
				  tpa.protocolo = :protocoloId and exists (select 1 from {0}tab_titulo_modelo ttm where ttm.codigo = 43 and ttm.id = tpaf.modelo) ", EsquemaBanco);

				comando.AdicionarParametroEntrada("atividadeId", atividadeId, DbType.Int32);
				comando.AdicionarParametroEntrada("protocoloId", protocoloId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						titulo = new TermoCPFARLRTituloAnterior
						{
							TituloNome = reader.GetValue<string>("nome"),
							TituloNumero = reader.GetValue<string>("numero"),
							TituloOrgao = reader.GetValue<string>("orgao"),
							IsEmitidoPeloOrgao = reader.GetValue<bool>("is_emitido_orgao")
						};
					}

					reader.Close();
				}
			}

			return titulo;
		}

		#endregion
	}
}