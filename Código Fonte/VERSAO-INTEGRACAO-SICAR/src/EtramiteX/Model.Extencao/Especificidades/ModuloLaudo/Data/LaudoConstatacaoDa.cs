using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLaudo;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLaudo.Data
{
	public class LaudoConstatacaoDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();

		internal Historico Historico { get { return _historico; } }

		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private string EsquemaBanco { get; set; }

		#endregion

		public LaudoConstatacaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(LaudoConstatacao laudo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				
				#region Cadastrar Dados da Especificidade

				eHistoricoAcao acao;
				object id;
				
				bancoDeDados.IniciarTransacao();
				//Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_laudo_constatacao e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", laudo.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"update {0}esp_laudo_constatacao e set e.titulo = :titulo, e.protocolo = :protocolo, 
					e.objetivo = :objetivo, e.constatacao_parecer = :constatacao_parecer, e.destinatario = :destinatario, e.data_vistoria = :data_vistoria, 
					e.tid = :tid where e.titulo = :titulo", EsquemaBanco);
					acao = eHistoricoAcao.atualizar;
					laudo.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}esp_laudo_constatacao e (id, titulo, protocolo, objetivo, constatacao_parecer, destinatario, data_vistoria, tid) 
					values ({0}seq_esp_laudo_constatacao.nextval, :titulo, :protocolo, :objetivo, :constatacao_parecer, :destinatario, :data_vistoria, :tid) returning e.id into :id", EsquemaBanco);
					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}
				
				comando.AdicionarParametroEntrada("titulo", laudo.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", laudo.ProtocoloReq.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario", laudo.Destinatario, DbType.Int32);
				comando.AdicionarParametroEntrada("objetivo", DbType.String, 500, laudo.Objetivo);
				comando.AdicionarParametroEntClob("constatacao_parecer", laudo.Constatacao);
				comando.AdicionarParametroEntrada("data_vistoria", laudo.DataVistoria.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());


				
				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					laudo = laudo ?? new LaudoConstatacao();
					laudo.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				#region Histórico

				Historico.Gerar(Convert.ToInt32(laudo.Titulo.Id), eHistoricoArtefatoEspecificidade.laudoconstatacao, acao, bancoDeDados);

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
				//
				Comando comando = bancoDeDados.CriarComando(@"update {0}esp_laudo_constatacao c set c.tid = :tid where c.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Histórico

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.laudoconstatacao, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComando(@"delete from {0}esp_laudo_constatacao e where e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal LaudoConstatacao Obter(int titulo, BancoDeDados banco = null)
		{
			LaudoConstatacao especificidade = new LaudoConstatacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.tid, e.objetivo, e.constatacao_parecer, e.data_vistoria, e.protocolo, n.numero, n.ano, p.requerimento, p.protocolo protocolo_tipo, 
				e.destinatario, (select distinct nvl(pe.nome, pe.razao_social) from {0}hst_esp_laudo_constatacao he, {0}hst_pessoa pe where he.destinatario_id = pe.pessoa_id and he.destinatario_tid = pe.tid
				and pe.data_execucao = (select max(h.data_execucao) from {0}hst_pessoa h where h.pessoa_id = pe.pessoa_id and h.tid = pe.tid) and he.especificidade_id = e.id
				and not exists(select 1 from {0}lov_historico_artefatos_acoes l where l.id = he.acao_executada and l.acao = 3) 
				and he.titulo_tid = (select ht.tid from {0}hst_titulo ht where ht.titulo_id = e.titulo and ht.data_execucao = (select min(htt.data_execucao) from {0}hst_titulo htt where htt.titulo_id = e.titulo 
				and htt.data_execucao > (select max(httt.data_execucao) from {0}hst_titulo httt where httt.titulo_id = e.titulo and httt.situacao_id = 1)))) destinatario_nome_razao 
				from {0}esp_laudo_constatacao e, {0}tab_titulo_numero n, {0}tab_protocolo p where n.titulo(+) = e.titulo and e.protocolo = p.id(+) and e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						especificidade.Id = Convert.ToInt32(reader["id"]);
						especificidade.Titulo.Id = titulo;
						especificidade.Tid = reader["tid"].ToString();
						especificidade.Objetivo = reader["objetivo"].ToString();
						especificidade.Constatacao = reader["constatacao_parecer"].ToString();

						if (reader["destinatario"] != null && !Convert.IsDBNull(reader["destinatario"]))
						{
							especificidade.Destinatario = Convert.ToInt32(reader["destinatario"]);
							especificidade.DestinatarioNomeRazao = Convert.ToString(reader["destinatario_nome_razao"]);
						}

						if (reader["data_vistoria"] != null && !Convert.IsDBNull(reader["data_vistoria"]))
						{
							especificidade.DataVistoria.Data = Convert.ToDateTime(reader["data_vistoria"]);
						}

						if (reader["protocolo"] != null && !Convert.IsDBNull(reader["protocolo"]))
						{
							especificidade.ProtocoloReq.IsProcesso = (reader["protocolo_tipo"] != null && Convert.ToInt32(reader["protocolo_tipo"]) == 1);
							especificidade.ProtocoloReq.RequerimentoId = Convert.ToInt32(reader["requerimento"]);
							especificidade.ProtocoloReq.Id = Convert.ToInt32(reader["protocolo"]);
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

		internal Laudo ObterDadosPDF(int titulo, BancoDeDados banco = null)
		{
			Laudo laudo = new Laudo();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados do Título

				DadosPDF dados = DaEsp.ObterDadosTitulo(titulo, bancoDeDados);
				laudo.Titulo = dados.Titulo;
				laudo.Protocolo = dados.Protocolo;
				laudo.Empreendimento = dados.Empreendimento;

				#endregion

				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.objetivo, e.constatacao_parecer, e.destinatario, e.data_vistoria 
				from {0}esp_laudo_constatacao e where e.titulo = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						laudo.Objetivo = reader["objetivo"].ToString();
						laudo.Parecer = reader["constatacao_parecer"].ToString();
						laudo.DescricaoParecer = reader["constatacao_parecer"].ToString();
						laudo.Destinatario.Id = Convert.ToInt32(reader["destinatario"]);
						
						if (reader["data_vistoria"] != null && !Convert.IsDBNull(reader["data_vistoria"]))
						{
							laudo.DataVistoria = Convert.ToDateTime(reader["data_vistoria"]).ToShortDateString();
						}
					}

					reader.Close();
				}

				#endregion

				laudo.Destinatario = DaEsp.ObterDadosPessoa(laudo.Destinatario.Id, laudo.Empreendimento.Id, bancoDeDados);

				laudo.Anexos = DaEsp.ObterAnexos(titulo, bancoDeDados);
			}

			return laudo;
		}

		#endregion
	}
}