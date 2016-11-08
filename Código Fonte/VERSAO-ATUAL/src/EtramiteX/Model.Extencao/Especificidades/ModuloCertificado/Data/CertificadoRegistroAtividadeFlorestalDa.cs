using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertificado;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertificado.Data
{
	public class CertificadoRegistroAtividadeFlorestalDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();

		internal Historico Historico { get { return _historico; } }

		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private string EsquemaBanco { get; set; }

		#endregion

		public CertificadoRegistroAtividadeFlorestalDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(CertificadoRegistroAtividadeFlorestal certificado, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cadastro do Titulo

				eHistoricoAcao acao;
				object id;

				// Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_cer_reg_ati_florestal e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", certificado.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"update {0}esp_cer_reg_ati_florestal e set e.titulo = :titulo, e.protocolo = :protocolo, e.destinatario = :destinatario, e.via = :via, e.ano_exercicio
                        = :ano_exercicio, e.tid = :tid where e.titulo = :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					certificado.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}esp_cer_reg_ati_florestal (id, titulo, protocolo, destinatario, via, ano_exercicio, tid)
                        values (seq_esp_cer_reg_ati_florestal.nextval, :titulo, :protocolo, :destinatario, :via, :ano_exercicio, :tid) returning id into :id", EsquemaBanco);

					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("titulo", certificado.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", certificado.ProtocoloReq.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario", certificado.Destinatario, DbType.Int32);
				comando.AdicionarParametroEntrada("via", certificado.Vias, DbType.Int32);
				comando.AdicionarParametroEntrada("ano_exercicio", DbType.String, 4, certificado.AnoExercicio);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					certificado = certificado ?? new CertificadoRegistroAtividadeFlorestal();
					certificado.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				#region Histórico

				Historico.Gerar(Convert.ToInt32(certificado.Titulo.Id), eHistoricoArtefatoEspecificidade.cerregistroatividadeflorestal, acao, bancoDeDados);

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

				Comando comando = bancoDeDados.CriarComando(@"update {0}esp_cer_reg_ati_florestal c set c.tid = :tid where c.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.cerregistroatividadeflorestal, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComando(@"delete from {0}esp_cer_reg_ati_florestal e where e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal CertificadoRegistroAtividadeFlorestal Obter(int titulo, BancoDeDados banco = null)
		{
			CertificadoRegistroAtividadeFlorestal especificidade = new CertificadoRegistroAtividadeFlorestal();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.tid, e.protocolo, e.via, e.ano_exercicio, n.numero, n.ano, p.requerimento, p.protocolo protocolo_tipo, e.destinatario, 
				(select distinct nvl(pe.nome, pe.razao_social) from {0}hst_esp_cer_reg_ati_florestal he, {0}hst_pessoa pe where he.destinatario_id = pe.pessoa_id and he.destinatario_tid = pe.tid
				and pe.data_execucao = (select max(h.data_execucao) from {0}hst_pessoa h where h.pessoa_id = pe.pessoa_id and h.tid = pe.tid) and he.especificidade_id = e.id
				and not exists(select 1 from {0}lov_historico_artefatos_acoes l where l.id = he.acao_executada and l.acao = 3) 
				and he.titulo_tid = (select ht.tid from {0}hst_titulo ht where ht.titulo_id = e.titulo and ht.data_execucao = (select min(htt.data_execucao) from {0}hst_titulo htt where htt.titulo_id = e.titulo 
				and htt.data_execucao > (select max(httt.data_execucao) from {0}hst_titulo httt where httt.titulo_id = e.titulo and httt.situacao_id = 1)))) destinatario_nome_razao 
				from {0}esp_cer_reg_ati_florestal e, {0}tab_titulo_numero n, {0}tab_protocolo p where n.titulo(+) = e.titulo and e.protocolo = p.id and e.titulo = :titulo ", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						especificidade.Titulo.Id = titulo;
						especificidade.Id = Convert.ToInt32(reader["id"]);
						especificidade.Tid = reader["tid"].ToString();
						especificidade.Vias = reader["via"].ToString();
						especificidade.AnoExercicio = reader["ano_exercicio"].ToString();

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
					}

					reader.Close();
				}

				#endregion
			}

			return especificidade;
		}

		internal Certificado ObterDadosPDF(int titulo, BancoDeDados banco = null)
		{
			Certificado certificado = new Certificado();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados do Titulo

				DadosPDF dados = DaEsp.ObterDadosTitulo(titulo, bancoDeDados);

				certificado.Titulo = dados.Titulo;
				certificado.Titulo.SetorEndereco = DaEsp.ObterEndSetor(certificado.Titulo.SetorId);
				certificado.Protocolo = dados.Protocolo;
				certificado.Empreendimento = dados.Empreendimento;

				#endregion

				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.tid, e.protocolo, e.destinatario, e.via, e.ano_exercicio
                    from {0}esp_cer_reg_ati_florestal e where e.titulo = :titulo ", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						certificado.Destinatario = new PessoaPDF();
						certificado.Destinatario.Id = Convert.ToInt32(reader["destinatario"]);
						certificado.Vias = reader["via"].ToString();
						certificado.AnoExercicio = reader["ano_exercicio"].ToString();
					}

					reader.Close();
				}

				#endregion

				certificado.Destinatario = DaEsp.ObterDadosPessoa(certificado.Destinatario.Id, certificado.Empreendimento.Id, bancoDeDados);
			}

			return certificado;
		}

		#endregion
	}
}