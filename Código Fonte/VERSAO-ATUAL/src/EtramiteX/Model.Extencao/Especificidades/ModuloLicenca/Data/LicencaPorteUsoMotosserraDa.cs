using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLicenca;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLicenca.Data
{
	public class LicencaPorteUsoMotosserraDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();

		internal Historico Historico { get { return _historico; } }

		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private string EsquemaBanco { get; set; }

		#endregion

		public LicencaPorteUsoMotosserraDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(LicencaPorteUsoMotosserra licenca, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cadastro do Titulo

				eHistoricoAcao acao;
				object id;

				// Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_licenca_motosserra e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", licenca.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"update {0}esp_licenca_motosserra e set e.titulo = :titulo, e.protocolo = :protocolo, e.destinatario = :destinatario, e.via = :via, e.ano_exercicio
                        = :ano_exercicio, e.motosserra = :motosserra, e.motosserra_tid = :motosserra_tid, e.tid = :tid where e.titulo = :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					licenca.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}esp_licenca_motosserra (id, titulo, protocolo, destinatario, via, ano_exercicio, motosserra, motosserra_tid, tid)
                        values (seq_esp_licenca_motosserra.nextval, :titulo, :protocolo, :destinatario, :via, :ano_exercicio, :motosserra, :motosserra_tid, :tid) returning id into :id", EsquemaBanco);

					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("titulo", licenca.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", licenca.ProtocoloReq.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario", licenca.Destinatario, DbType.Int32);
				comando.AdicionarParametroEntrada("via", licenca.Vias, DbType.Int32);
				comando.AdicionarParametroEntrada("ano_exercicio", DbType.String, 4, licenca.AnoExercicio);
				comando.AdicionarParametroEntrada("motosserra", licenca.Motosserra.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("motosserra_tid", licenca.Motosserra.Tid, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					licenca = licenca ?? new LicencaPorteUsoMotosserra();
					licenca.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				#region Histórico

				Historico.Gerar(Convert.ToInt32(licenca.Titulo.Id), eHistoricoArtefatoEspecificidade.licencaporteusomotosserra, acao, bancoDeDados);

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

				Comando comando = bancoDeDados.CriarComando(@"update {0}esp_licenca_motosserra c set c.tid = :tid where c.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.licencaporteusomotosserra, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComando(@"delete from {0}esp_licenca_motosserra e where e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal LicencaPorteUsoMotosserra Obter(int titulo, BancoDeDados banco = null)
		{
			LicencaPorteUsoMotosserra especificidade = new LicencaPorteUsoMotosserra();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.tid, e.protocolo, e.via, e.ano_exercicio, n.numero, e.motosserra, 
				h.motosserra_id motosserra_id_hst, h.motosserra_tid motosserra_tid_hst, e.motosserra_tid, n.ano, p.requerimento, p.protocolo protocolo_tipo, e.destinatario, 
				(select distinct nvl(pe.nome, pe.razao_social) from {0}hst_esp_licenca_motosserra he, {0}hst_pessoa pe where he.destinatario_id = pe.pessoa_id and he.destinatario_tid = pe.tid
				and pe.data_execucao = (select max(h.data_execucao) from {0}hst_pessoa h where h.pessoa_id = pe.pessoa_id and h.tid = pe.tid) and he.especificidade_id = e.id
				and not exists(select 1 from {0}lov_historico_artefatos_acoes l where l.id = he.acao_executada and l.acao = 3) 
				and he.titulo_tid = (select ht.tid from {0}hst_titulo ht where ht.titulo_id = e.titulo and ht.data_execucao = (select min(htt.data_execucao) from {0}hst_titulo htt where htt.titulo_id = e.titulo 
				and htt.data_execucao > (select max(httt.data_execucao) from {0}hst_titulo httt where httt.titulo_id = e.titulo and httt.situacao_id = 1)))) destinatario_nome_razao 
				from {0}esp_licenca_motosserra e, {0}hst_esp_licenca_motosserra h, {0}tab_titulo_numero n, {0}tab_protocolo p where n.titulo(+) = e.titulo and e.protocolo = p.id 
				and h.especificidade_id(+) = e.id and h.tid(+) = e.tid and e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						especificidade.Titulo.Id = titulo;
						especificidade.Id = Convert.ToInt32(reader["id"]);
						especificidade.Tid = reader["tid"].ToString();
						especificidade.Vias = Convert.ToInt32(reader["via"]);
						especificidade.AnoExercicio = reader["ano_exercicio"].ToString();
						especificidade.Motosserra.Id = Convert.ToInt32(reader["motosserra"]);
						especificidade.Motosserra.Tid = reader["motosserra_tid"].ToString();

						int motosserraIdHst = 0;
						string motosserraTidHst = String.Empty;

						if (reader["motosserra_id_hst"] != null && !Convert.IsDBNull(reader["motosserra_id_hst"]))
						{
							motosserraIdHst = Convert.ToInt32(reader["motosserra_id_hst"]);
							motosserraTidHst = reader["motosserra_tid_hst"].ToString();
						}
						

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

						especificidade.Motosserra = ObterMotosserraPorHistorico(motosserraIdHst, motosserraTidHst);

						reader.Close();
					}
				}

				#endregion
			}

			return especificidade;
		}

		internal Motosserra ObterMotosserra(int id, BancoDeDados banco = null)
		{
			Motosserra motosserra = new Motosserra();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				Comando comando = bancoDeDados.CriarComando(@"select t.registro_numero, t.serie_numero, t.modelo, 
															t.nota_fiscal_numero, t.proprietario, t.situacao, t.tid from 
															tab_motosserra t where t.id = :motosserra", EsquemaBanco);

				comando.AdicionarParametroEntrada("motosserra", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						motosserra.Id = id;
						motosserra.Marca = reader.GetValue<String>("modelo");
						motosserra.NotaFiscal = reader.GetValue<String>("nota_fiscal_numero");
						motosserra.NumeroFabricacao = reader.GetValue<String>("serie_numero");
						motosserra.NumeroRegistro = reader.GetValue<String>("registro_numero");
						motosserra.ProprietarioId = reader.GetValue<Int32>("proprietario");
						motosserra.SituacaoId = reader.GetValue<Int32>("situacao");
						motosserra.Tid = reader.GetValue<String>("tid");
					}

					reader.Close();
				}
			}

			return motosserra;

		}

		internal Motosserra ObterMotosserraPorHistorico(int id, string tid, BancoDeDados banco = null)
		{
			Motosserra motosserra = new Motosserra();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				Comando comando = bancoDeDados.CriarComando(@"select t.registro_numero, t.serie_numero, t.modelo, 
															t.nota_fiscal_numero, t.proprietario_id, t.situacao from 
															hst_motosserra t where t.motosserra = :motosserra 
															and t.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("motosserra", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", tid, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						motosserra.Id = id;
						motosserra.Marca = reader.GetValue<String>("modelo");
						motosserra.NotaFiscal = reader.GetValue<String>("nota_fiscal_numero");
						motosserra.NumeroFabricacao = reader.GetValue<String>("serie_numero");
						motosserra.NumeroRegistro = reader.GetValue<String>("registro_numero");
						motosserra.ProprietarioId = reader.GetValue<Int32>("proprietario_id");
						motosserra.SituacaoId = reader.GetValue<Int32>("situacao");
						motosserra.Tid = tid;
					}

					reader.Close();
				}
			}

			return motosserra;

		}

		internal Licenca ObterDadosPDF(int titulo, BancoDeDados banco = null)
		{
			Licenca licenca = new Licenca();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados do Titulo

				DadosPDF dados = DaEsp.ObterDadosTitulo(titulo, bancoDeDados);

				licenca.Titulo = dados.Titulo;
				licenca.Titulo.SetorEndereco = DaEsp.ObterEndSetor(licenca.Titulo.SetorId);
				licenca.Protocolo = dados.Protocolo;
				licenca.Empreendimento = dados.Empreendimento;

				#endregion

				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.tid, e.protocolo, e.destinatario, e.via, e.ano_exercicio 
															from {0}esp_licenca_motosserra e where e.titulo = :titulo ", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						licenca.Destinatario.Id = Convert.ToInt32(reader["destinatario"]);
						licenca.Vias = reader["via"].ToString();
						licenca.AnoExercicio = reader["ano_exercicio"].ToString();
					}

					reader.Close();
				}

				#endregion

				licenca.Destinatario = DaEsp.ObterDadosPessoa(licenca.Destinatario.Id, licenca.Empreendimento.Id, bancoDeDados);
			}

			return licenca;
		}

		#endregion

		#region Validacoes

		internal List<TituloAssociadoEsp> ObterTitulosAssociados(int motosserraId, BancoDeDados banco = null)
		{
			List<TituloAssociadoEsp> titulos = new List<TituloAssociadoEsp>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, tn.numero, tn.ano, t.situacao, ls.texto situacao_texto, tm.sigla, t.modelo, ttm.codigo, t.empreendimento
															from {0}tab_titulo t, {0}tab_titulo_modelo ttm, {0}tab_titulo_numero tn, tab_titulo_modelo tm, {0}lov_titulo_situacao ls
															where t.modelo = ttm.id and tm.id = t.modelo and tn.titulo(+) = t.id and ls.id = t.situacao and t.id in 
															(select t.id from tab_titulo t, esp_licenca_motosserra e, tab_motosserra m where t.id = e.titulo and m.id = e.motosserra 
															and m.situacao = 1/*Ativo*/ and e.motosserra = :motosserra) and t.situacao <> 5 /*Encerrado*/", EsquemaBanco);

				comando.AdicionarParametroEntrada("motosserra", motosserraId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						TituloAssociadoEsp titulo = new TituloAssociadoEsp();
						titulo.Id = Convert.ToInt32(reader["id"]);
						titulo.ModeloId = Convert.ToInt32(reader["modelo"]);
						titulo.ModeloSigla = reader["sigla"].ToString();
						titulo.ModeloCodigo = Convert.ToInt32(reader["codigo"]);
						titulo.Situacao = Convert.ToInt32(reader["situacao"]);
						titulo.SituacaoTexto = reader["situacao_texto"].ToString();

						if (reader["empreendimento"] != null && !Convert.IsDBNull(reader["empreendimento"]))
						{
							titulo.EmpreendimentoId = Convert.ToInt32(reader["empreendimento"]);
						}

						if (reader["numero"] != null && !Convert.IsDBNull(reader["numero"]))
						{
							titulo.TituloNumero = reader["numero"].ToString() + "/" + reader["ano"].ToString();
						}

						titulos.Add(titulo);

					}

					reader.Close();

				}

			}

			return titulos;
		}

		internal bool PossuiDestinatarioIgualProprietarioMotosserra(int destinatario, int motosserra, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_motosserra m where m.id = :motosserra and m.proprietario = :destinatario", EsquemaBanco);
				comando.AdicionarParametroEntrada("destinatario", destinatario, DbType.Int32);
				comando.AdicionarParametroEntrada("motosserra", motosserra, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal bool MotosserraIsAtivo(int motosserraId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_motosserra m where m.id = :motosserra and m.situacao = 1 /*Ativo*/", EsquemaBanco);
				comando.AdicionarParametroEntrada("motosserra", motosserraId, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		#endregion
	}
}