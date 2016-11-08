using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOficio;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOficio.Data
{
	public class OficioNotificacaoDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();

		internal Historico Historico { get { return _historico; } }
		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private string EsquemaBanco { get; set; }
		#endregion

		public OficioNotificacaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(OficioNotificacao oficio, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título de Ofício de Notificação

				eHistoricoAcao acao;
				object id;

				bancoDeDados.IniciarTransacao();
				//Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_oficio_notificacao e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", oficio.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				//Verifica a existencia da análise
				comando = bancoDeDados.CriarComando(@"select a.id, a.tid from {0}tab_analise a, {0}tab_protocolo p where p.checagem = a.checagem and a.protocolo = :protocolo", EsquemaBanco);
				comando.AdicionarParametroEntrada("protocolo", oficio.ProtocoloReq.Id, DbType.Int32);

				AnaliseItemEsp analise = new AnaliseItemEsp();
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						analise.Id = Convert.ToInt32(reader["id"]);
						analise.Tid = reader["tid"].ToString();
					}
					reader.Close();
				}

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"update {0}esp_oficio_notificacao e set e.titulo = :titulo, e.protocolo = :protocolo, e.destinatario = :destinatario, 
					e.analise_id = :analise_id, e.analise_tid = :analise_tid, e.tid = :tid where e.titulo = :titulo", EsquemaBanco);
					acao = eHistoricoAcao.atualizar;
					oficio.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}esp_oficio_notificacao e (id, titulo, protocolo, destinatario, analise_id, analise_tid, tid) 
					values ({0}seq_esp_oficio_notificacao.nextval, :titulo, :protocolo, :destinatario, :analise_id, :analise_tid, :tid) returning e.id into :id", EsquemaBanco);
					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("titulo", oficio.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", oficio.ProtocoloReq.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario", oficio.Destinatario, DbType.Int32);
				comando.AdicionarParametroEntrada("analise_id", analise.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("analise_tid", DbType.String, 36, analise.Tid);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					oficio = oficio ?? new OficioNotificacao();
					oficio.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				#region Histórico

				Historico.Gerar(Convert.ToInt32(oficio.Titulo.Id), eHistoricoArtefatoEspecificidade.oficionotificacao, acao, bancoDeDados);

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
				Comando comando = bancoDeDados.CriarComando(@"update {0}esp_oficio_notificacao c set c.tid = :tid where c.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Histórico

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.oficionotificacao, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComando(@"delete from {0}esp_oficio_notificacao e where e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal OficioNotificacao Obter(int titulo, BancoDeDados banco = null)
		{
			OficioNotificacao especificidade = new OficioNotificacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título de Ofício de Notificação

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.tid, e.protocolo, n.numero, n.ano, p.requerimento, p.protocolo protocolo_tipo, e.destinatario,
				(select distinct(nvl(pe.nome, pe.razao_social)) from hst_esp_oficio_notificacao he, hst_pessoa pe where he.destinatario_id = pe.pessoa_id
				and he.destinatario_tid = pe.tid and pe.data_execucao = (select max(h.data_execucao) from hst_pessoa h where h.pessoa_id = pe.pessoa_id and h.tid = pe.tid) and he.especificidade_id = e.id 
				and not exists(select 1 from {0}lov_historico_artefatos_acoes l where l.id = he.acao_executada and l.acao = 3) and he.titulo_tid = 
				(select ht.tid	from hst_titulo ht where ht.titulo_id = e.titulo and ht.data_execucao = (select min(htt.data_execucao)
				from hst_titulo htt where htt.titulo_id = e.titulo and htt.data_execucao > (select max(httt.data_execucao)
				from hst_titulo httt where httt.titulo_id = e.titulo and httt.situacao_id = 1)))) destinatario_nome_razao
				from {0}esp_oficio_notificacao e, {0}tab_titulo_numero n, {0}tab_protocolo p where n.titulo(+) = e.titulo 
				and e.protocolo = p.id(+) and e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						especificidade.Id = Convert.ToInt32(reader["id"]);
						especificidade.Titulo.Id = titulo;
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
							especificidade.DestinatarioNomeRazao = reader["destinatario_nome_razao"].ToString();
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

				#region Itens da analise
				comando = bancoDeDados.CriarComando(@"select ri.item_id, ri.tid item_tid, ri.nome, i.situacao situacao_id, lis.texto situacao_texto 
				from {0}esp_oficio_notificacao p, {0}tab_analise_itens i, {0}hst_roteiro_item ri, {0}lov_analise_item_situacao lis
				where i.analise = p.analise_id and p.titulo = :id and i.item_id = ri.item_id and i.item_tid = ri.tid 
				and i.situacao = lis.id and i.situacao in (2,4)", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					AnaliseItemEsp item;
					while (reader.Read())
					{

						item = new AnaliseItemEsp();
						item.Id = Convert.ToInt32(reader["item_id"]);
						item.Nome = reader["nome"].ToString();
						item.Situacao = Convert.ToInt32(reader["situacao_id"]);
						item.SituacaoTexto = reader["situacao_texto"].ToString();
						item.Tid = reader["item_tid"].ToString();
						especificidade.Itens.Add(item);
					}
					reader.Close();
				}
				#endregion

			}
			return especificidade;
		}

		internal Oficio ObterDadosPDF(int titulo, BancoDeDados banco = null)
		{
			Oficio oficio = new Oficio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados do Título

				DadosPDF dados = DaEsp.ObterDadosTitulo(titulo, bancoDeDados);

				oficio.Titulo = dados.Titulo;
				oficio.Titulo.SetorEndereco = DaEsp.ObterEndSetor(oficio.Titulo.SetorId);
				oficio.Protocolo = dados.Protocolo;
				oficio.Empreendimento = dados.Empreendimento;

				#endregion

				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.destinatario from {0}esp_oficio_notificacao e where e.titulo = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						oficio.Destinatario.Id = Convert.ToInt32(reader["destinatario"]);
					}

					reader.Close();
				}

				#endregion

				oficio.Destinatario = DaEsp.ObterDadosPessoa(oficio.Destinatario.Id, oficio.Empreendimento.Id, bancoDeDados);

				#region Itens da analise

				comando = bancoDeDados.CriarComando(@"select ri.item_id, ri.nome, i.situacao situacao_id, lis.texto situacao_texto, i.motivo
				from {0}esp_oficio_notificacao p, {0}tab_analise_itens i, {0}hst_roteiro_item ri, {0}lov_analise_item_situacao lis
				where i.analise = p.analise_id and p.titulo = :id and i.item_id = ri.item_id and i.item_tid = ri.tid 
				and i.situacao = lis.id and i.situacao in (2,4)", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", titulo, DbType.Int32);

				oficio.SituacoesGrupo = new List<AnaliseSituacaoGrupoPDF>();
				AnaliseSituacaoGrupoPDF situacaoGrupo = null;

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					AnaliseItemPDF item;

					while (reader.Read())
					{
						item = new AnaliseItemPDF();
						item.Id = Convert.ToInt32(reader["item_id"]);
						item.Nome = reader["nome"].ToString();
						item.Motivo = reader["motivo"].ToString();

						int situacaoId = Convert.ToInt32(reader["situacao_id"]);
						situacaoGrupo = oficio.SituacoesGrupo.FirstOrDefault(x => x.Situacao == situacaoId);

						if (situacaoGrupo == null)
						{
							situacaoGrupo = new AnaliseSituacaoGrupoPDF();
							situacaoGrupo.Situacao = Convert.ToInt32(reader["situacao_id"]);
							situacaoGrupo.SituacaoTexto = reader["situacao_texto"].ToString();
							oficio.SituacoesGrupo.Add(situacaoGrupo);
						}

						situacaoGrupo.Itens.Add(item);
					}

					reader.Close();
				}

				#endregion
			}

			return oficio;
		}

		#endregion

		#region Validações

		public bool ProcDocPossuiItemPendenteReprovado(int requerimento, bool isProcesso)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) qtd from {0}tab_analise a, {0}tab_analise_itens tai, {0}tab_protocolo
				p where a.id = tai.analise and a.protocolo = p.id and p.requerimento = :requerimento and tai.situacao in (2, 4)", EsquemaBanco);

				comando.AdicionarParametroEntrada("requerimento", requerimento, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		#endregion
	}
}