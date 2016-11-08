using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloNotificacao;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloNotificacao.Data
{
	public class NotificacaoEmbargoDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();

		internal Historico Historico { get { return _historico; } }

		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private string EsquemaBanco { get; set; }

		#endregion

		public NotificacaoEmbargoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(NotificacaoEmbargo notificacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cadastro do Titulo

				eHistoricoAcao acao;
				object id;

				//Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_notificacao_embargo e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", notificacao.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"update {0}esp_notificacao_embargo e set e.titulo = :titulo, e.protocolo = :protocolo, e.atividade_embargo = 
						:atividade_embargo, e.tid = :tid where e.titulo = :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					notificacao.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}esp_notificacao_embargo(id, titulo, protocolo, atividade_embargo, tid) values ({0}seq_esp_notificacao_embargo.nextval, :titulo, 
                        :protocolo, :atividade_embargo, :tid) returning id into :id", EsquemaBanco);

					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("titulo", notificacao.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", notificacao.ProtocoloReq.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade_embargo", notificacao.AtividadeEmbargo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					notificacao = notificacao ?? new NotificacaoEmbargo();
					notificacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				#region Destinatário

				comando = bancoDeDados.CriarComando("delete from {0}esp_notificacao_embargo_dest t ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where t.especificidade = :especificidade {0}", comando.AdicionarNotIn("and", "t.id", DbType.Int32, notificacao.Destinatarios.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("especificidade", notificacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				foreach (var item in notificacao.Destinatarios)
				{
					if (item.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}esp_notificacao_embargo_dest t set t.destinatario = :destinatario, t.tid = :tid where t.id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}esp_notificacao_embargo_dest (id, especificidade, destinatario, tid) values ({0}seq_esp_notificacao_embar_dest.nextval, 
							:especificidade, :destinatario, :tid)", EsquemaBanco);
						comando.AdicionarParametroEntrada("especificidade", notificacao.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("destinatario", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Histórico

				Historico.Gerar(Convert.ToInt32(notificacao.Titulo.Id), eHistoricoArtefatoEspecificidade.notificacaoembargo, acao, bancoDeDados);

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

				Comando comando = bancoDeDados.CriarComando(@"update {0}esp_notificacao_embargo c set c.tid = :tid where c.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.notificacaoembargo, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComando(@"delete from {0}esp_notificacao_embargo e where e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal NotificacaoEmbargo Obter(int titulo, BancoDeDados banco = null)
		{
			NotificacaoEmbargo especificidade = new NotificacaoEmbargo();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.tid, e.protocolo, e.atividade_embargo, n.numero, n.ano, p.requerimento, p.protocolo 
					protocolo_tipo from {0}esp_notificacao_embargo e, {0}tab_titulo_numero n, {0}tab_protocolo p where n.titulo(+) = e.titulo and e.protocolo = p.id(+) and e.titulo = :titulo", EsquemaBanco);

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

						if (reader["atividade_embargo"] != null && !Convert.IsDBNull(reader["atividade_embargo"]))
						{
							especificidade.AtividadeEmbargo = Convert.ToInt32(reader["atividade_embargo"]);
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

				#region Destinatario

				comando = bancoDeDados.CriarComando(@"select d.id, p.id pessoaId, nvl(p.nome, p.razao_social) pessoaNome 
					from {0}esp_notificacao_embargo_dest d, {0}tab_pessoa p 
					where d.destinatario = p.id and d.especificidade = :especificidade", EsquemaBanco);
				comando.AdicionarParametroEntrada("especificidade", especificidade.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						especificidade.Destinatarios.Add(new DestinatarioEspecificidade()
						{
							IdRelacionamento = reader.GetValue<int>("id"),
							Id = reader.GetValue<int>("pessoaId"),
							Nome = reader.GetValue<String>("pessoaNome")
						});
					}

					reader.Close();
				}

				#endregion
			}

			return especificidade;
		}

		internal NotificacaoEmbargo ObterHistorico(int titulo, int situacao, BancoDeDados banco = null)
		{
			NotificacaoEmbargo especificidade = new NotificacaoEmbargo();
			Comando comando = null;
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				if (situacao > 0)
				{
					comando = bancoDeDados.CriarComando(@"
					select e.id,
						e.tid,
						e.protocolo_id,
						e.atividade_embargo_id,
						n.numero,
						n.ano,
						p.requerimento_id,
						p.protocolo_id protocolo_tipo
					from hst_esp_notificacao_embargo e, hst_titulo_numero n, hst_protocolo p
					where e.titulo_id = n.titulo_id(+)
						and e.titulo_tid = n.titulo_tid(+)
						and e.protocolo_id = p.id_protocolo(+)
						and e.protocolo_tid = p.tid(+)
						and not exists(select 1 from {0}lov_historico_artefatos_acoes l where l.id = e.acao_executada and l.acao = 3) 
						and e.titulo_tid = (select ht.tid from hst_titulo ht where ht.titulo_id = e.titulo_id and ht.data_execucao =
							(select max(httt.data_execucao) from hst_titulo httt where httt.titulo_id = e.titulo_id and httt.situacao_id = :situacao)) and e.titulo_id = :titulo", EsquemaBanco);

					comando.AdicionarParametroEntrada("situacao", situacao, DbType.Int32);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"
					select e.id,
						e.tid,
						e.protocolo_id protocolo,
						e.atividade_embargo_id atividade_embargo,
						n.numero,
						n.ano,
						p.requerimento_id requerimento,
						p.protocolo_id protocolo_tipo
					from {0}hst_esp_notificacao_embargo e,
						{0}hst_titulo_numero            n,
						{0}hst_protocolo                p
					where e.titulo_id = n.titulo_id(+)
						and e.titulo_tid = n.titulo_tid(+)
						and e.protocolo_id = p.id_protocolo(+)
						and e.protocolo_tid = p.tid(+)
						and not exists(select 1 from {0}lov_historico_artefatos_acoes l where l.id = e.acao_executada and l.acao = 3) 
						and e.titulo_tid = (select ht.tid from {0}hst_titulo ht where ht.titulo_id = e.titulo_id and ht.data_execucao =
							(select min(htt.data_execucao) from {0}hst_titulo htt where htt.titulo_id = e.titulo_id and htt.data_execucao >
							(select max(httt.data_execucao) from {0}hst_titulo httt where httt.titulo_id = e.titulo_id and httt.situacao_id = 1))) and e.titulo_id = :titulo", EsquemaBanco);
				}

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst = reader.GetValue<int>("id");

						especificidade.Titulo.Id = titulo;
						especificidade.Id = Convert.ToInt32(reader["id"]);
						especificidade.Tid = reader["tid"].ToString();

						if (reader["protocolo"] != null && !Convert.IsDBNull(reader["protocolo"]))
						{
							especificidade.ProtocoloReq.IsProcesso = (reader["protocolo_tipo"] != null && Convert.ToInt32(reader["protocolo_tipo"]) == 1);
							especificidade.ProtocoloReq.RequerimentoId = Convert.ToInt32(reader["requerimento"]);
							especificidade.ProtocoloReq.Id = Convert.ToInt32(reader["protocolo"]);
						}

						if (reader["atividade_embargo"] != null && !Convert.IsDBNull(reader["atividade_embargo"]))
						{
							especificidade.AtividadeEmbargo = Convert.ToInt32(reader["atividade_embargo"]);
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

				#region Destinatario

				comando = bancoDeDados.CriarComando(@"select distinct(p.pessoa_id), d.notificacao_destin_id id, nvl(p.nome, p.razao_social) nome_razao
													from hst_esp_notificacao_embar_dest d, hst_pessoa p where d.destinatario_id = p.pessoa_id
													and d.destinatario_tid = p.tid and p.data_execucao = (select max(h.data_execucao)
													from hst_pessoa h where h.pessoa_id = p.pessoa_id and h.tid = p.tid) and d.id_hst = :hst", EsquemaBanco);

				comando.AdicionarParametroEntrada("hst", hst, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						especificidade.Destinatarios.Add(new DestinatarioEspecificidade()
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

		internal Notificacao ObterDadosPDF(int titulo, BancoDeDados banco = null)
		{
			Notificacao notificacao = new Notificacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados do Titulo

				DadosPDF dados = DaEsp.ObterDadosTitulo(titulo, bancoDeDados);

				notificacao.Titulo = dados.Titulo;
				notificacao.Titulo.SetorEndereco = DaEsp.ObterEndSetor(notificacao.Titulo.SetorId);
				notificacao.Protocolo = dados.Protocolo;
				notificacao.Empreendimento = dados.Empreendimento;

				#endregion

				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.id, a.atividade from {0}esp_notificacao_embargo e, {0}tab_atividade a where e.titulo = :id and e.atividade_embargo = a.id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						notificacao.Id = Convert.ToInt32(reader["id"]);
						notificacao.AtividadeEmbargada = reader.GetValue<String>("atividade");
					}

					reader.Close();
				}

				#endregion

				#region Destinatarios

				comando = bancoDeDados.CriarComando(@"select e.destinatario from {0}esp_notificacao_embargo_dest e where e.especificidade = :especificidade", EsquemaBanco);
				comando.AdicionarParametroEntrada("especificidade", notificacao.Id, DbType.Int32);
				List<Int32> destinatarios = bancoDeDados.ExecutarList<Int32>(comando);
				notificacao.Destinatarios = destinatarios.Select(x => DaEsp.ObterDadosPessoa(x, notificacao.Empreendimento.Id, bancoDeDados)).ToList();

				#endregion
			}

			return notificacao;
		}

		#endregion
	}
}