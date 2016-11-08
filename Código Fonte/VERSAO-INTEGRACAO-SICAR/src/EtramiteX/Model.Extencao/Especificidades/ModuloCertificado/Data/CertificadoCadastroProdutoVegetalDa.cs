using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertificado;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertificado.Data
{
	public class CertificadoCadastroProdutoVegetalDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();

		internal Historico Historico { get { return _historico; } }
		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private string EsquemaBanco { get; set; }
		#endregion

		public CertificadoCadastroProdutoVegetalDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(CertificadoCadastroProdutoVegetal certificado, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título de Certificado de Cadastro de Produto Vegetal

				eHistoricoAcao acao;
				object id;

				bancoDeDados.IniciarTransacao();
				//Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_certif_cad_prod_vegetal e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", certificado.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);


				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"
						update {0}esp_certif_cad_prod_vegetal t
						   set t.protocolo           = :protocolo,
							   t.destinatario        = :destinatario,
							   t.nome                = :nome,
							   t.fabricante         = :fabricante,
							   t.classe_toxicologica = :classe_toxicologica,
							   t.classe              = :classe,
							   t.ingrediente         = :ingrediente,
							   t.classificacao       = :classificacao,
							   t.cultura             = :cultura,
							   t.tid                 = :tid
						 where t.titulo = :titulo", EsquemaBanco);
					acao = eHistoricoAcao.atualizar;
					certificado.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"
						insert into {0}esp_certif_cad_prod_vegetal e
						  (id,
						   titulo,
						   protocolo,
						   destinatario,
						   nome,
						   fabricante,
						   classe_toxicologica,
						   classe,
						   ingrediente,
						   classificacao,
						   cultura,
						   tid)
						values
						  (seq_esp_certif_cad_prod_veg.nextval,
						   :titulo,
						   :protocolo,
						   :destinatario,
						   :nome,
						   :fabricante,
						   :classe_toxicologica,
						   :classe,
						   :ingrediente,
						   :classificacao,
						   :cultura,
						   :tid)
						returning e.id into :id", EsquemaBanco);
					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("titulo", certificado.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", certificado.ProtocoloReq.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario", certificado.Destinatario, DbType.Int32);
				comando.AdicionarParametroEntrada("nome", DbType.String, 80, certificado.Nome);
				comando.AdicionarParametroEntrada("fabricante", DbType.String, 150, certificado.Fabricante);
				comando.AdicionarParametroEntrada("classe_toxicologica", DbType.String, 80, certificado.ClasseToxicologica);
				comando.AdicionarParametroEntrada("classe", DbType.String, 80, certificado.Classe);
				comando.AdicionarParametroEntrada("ingrediente", DbType.String, 80, certificado.Ingrediente);
				comando.AdicionarParametroEntrada("classificacao", DbType.String, 150, certificado.Classificacao);
				comando.AdicionarParametroEntrada("cultura", DbType.String, 80, certificado.Cultura);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					certificado.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				#region Histórico

				Historico.Gerar(Convert.ToInt32(certificado.Titulo.Id), eHistoricoArtefatoEspecificidade.certificadocadastroprodvegetal, acao, bancoDeDados);

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

				Comando comando = bancoDeDados.CriarComando(@"update {0}esp_certif_cad_prod_vegetal c set c.tid = :tid where c.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Histórico

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.certificadocadastroprodvegetal, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComando(@"delete from {0}esp_certif_cad_prod_vegetal e where e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal CertificadoCadastroProdutoVegetal Obter(int titulo, BancoDeDados banco = null)
		{
			CertificadoCadastroProdutoVegetal especificidade = new CertificadoCadastroProdutoVegetal();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título de Ofício de Notificação

				Comando comando = bancoDeDados.CriarComando(@"
					select e.id,
						e.tid,
						e.protocolo,
						e.nome,
						e.fabricante,
						e.classe_toxicologica,
						e.classe,
						e.ingrediente,
						e.classificacao,
						e.cultura,
						n.numero,
						n.ano,
						p.requerimento,
						p.protocolo protocolo_tipo, 
						e.destinatario,
						(select distinct nvl(pe.nome, pe.razao_social) from {0}hst_esp_certif_cad_prd_vegetal he, {0}hst_pessoa pe where he.destinatario_id = pe.pessoa_id and he.destinatario_tid = pe.tid
						and pe.data_execucao = (select max(h.data_execucao) from {0}hst_pessoa h where h.pessoa_id = pe.pessoa_id and h.tid = pe.tid) and he.especificidade_id = e.id
						and not exists(select 1 from {0}lov_historico_artefatos_acoes l where l.id = he.acao_executada and l.acao = 3) 
						and he.titulo_tid = (select ht.tid from {0}hst_titulo ht where ht.titulo_id = e.titulo and ht.data_execucao = (select min(htt.data_execucao) from {0}hst_titulo htt where htt.titulo_id = e.titulo 
						and htt.data_execucao > (select max(httt.data_execucao) from {0}hst_titulo httt where httt.titulo_id = e.titulo and httt.situacao_id = 1)))) destinatario_nome_razao 
					from {0}esp_certif_cad_prod_vegetal e, 
						{0}tab_titulo_numero            n, 
						{0}tab_protocolo                p
					 where n.titulo(+) = e.titulo
						and e.protocolo = p.id(+)
						and e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						especificidade.Id = reader.GetValue<int>("id");
						especificidade.Titulo.Id = titulo;
						especificidade.Tid = reader.GetValue<string>("tid");
						especificidade.ProtocoloReq.Id = reader.GetValue<int>("protocolo");
						especificidade.ProtocoloReq.IsProcesso = reader.GetValue<int>("protocolo_tipo") == 1;
						especificidade.ProtocoloReq.RequerimentoId = reader.GetValue<int>("requerimento");
						especificidade.ProtocoloReq.Id = reader.GetValue<int>("protocolo");
						especificidade.Destinatario = reader.GetValue<int>("destinatario");
						especificidade.DestinatarioNomeRazao = reader.GetValue<string>("destinatario_nome_razao");
						especificidade.Titulo.Numero.Inteiro = reader.GetValue<int>("numero");
						especificidade.Titulo.Numero.Ano = reader.GetValue<int>("ano");

						especificidade.Nome = reader.GetValue<string>("nome");
						especificidade.Fabricante = reader.GetValue<string>("fabricante");
						especificidade.ClasseToxicologica = reader.GetValue<string>("classe_toxicologica");
						especificidade.Classe = reader.GetValue<string>("classe");
						especificidade.Ingrediente = reader.GetValue<string>("ingrediente");
						especificidade.Classificacao = reader.GetValue<string>("classificacao");
						especificidade.Cultura = reader.GetValue<string>("cultura");
					}

					reader.Close();
				}

				#endregion
			}

			return especificidade;
		}

		#endregion
	}
}