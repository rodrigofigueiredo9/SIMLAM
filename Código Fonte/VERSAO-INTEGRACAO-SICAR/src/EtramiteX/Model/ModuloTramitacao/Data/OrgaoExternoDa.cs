using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Data
{
	class TramitacaoExternoDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();		
		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }		
		private string EsquemaBanco { get; set; }

		#endregion

		public TramitacaoExternoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void EnviarExterno(Tramitacao tramitacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Enviar processos/documentos

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_tramitacao a (id, protocolo, tipo, objetivo, situacao, despacho,
				executor, remetente, remetente_setor, destinatario, destinatario_setor, tid, data_envio) values ({0}seq_tramitacao.nextval, :protocolo, :tipo, :objetivo, 
				:situacao, :despacho, :executor, :remetente, :remetente_setor, :destinatario, :destinatario_setor, :tid, sysdate) returning a.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", tramitacao.Protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", tramitacao.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("objetivo", tramitacao.Objetivo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", tramitacao.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntClob("despacho", tramitacao.Despacho);
				comando.AdicionarParametroEntrada("executor", tramitacao.Executor.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("remetente", tramitacao.Remetente.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("remetente_setor", tramitacao.RemetenteSetor.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario", tramitacao.Destinatario.Id > 0 ? (int?)tramitacao.Destinatario.Id : null, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario_setor", tramitacao.DestinatarioSetor.Id > 0 ? (int?)tramitacao.DestinatarioSetor.Id : null, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				int id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#region Enviar tramitação externa

				comando = bancoDeDados.CriarComando(@"insert into {0}tab_tramitacao_externo t (id, tramitacao, orgao, funcionario, tid) 
				values ({0}seq_tramitacao_externo.nextval, :tramitacao, :orgao, :funcionario, :tid) ", EsquemaBanco);

				comando.AdicionarParametroEntrada("tramitacao", id, DbType.Int32);
				comando.AdicionarParametroEntrada("orgao", tramitacao.OrgaoExterno.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("funcionario", tramitacao.Destinatario.Nome, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#endregion

				#region Histórico

				Historico.Gerar(id, eHistoricoArtefato.tramitacao, eHistoricoAcao.enviarexterno, bancoDeDados);
				
				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void ReceberExterno(Tramitacao tramitacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualiza a tabela transacional

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_tramitacao t set t.situacao = :situacao, t.remetente = null, t.remetente_setor = null, t.destinatario = :destinatario, 
				t.destinatario_setor = :destinatario_setor, t.data_envio = null, t.objetivo = null where t.id = :id ", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", tramitacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", (int)eTramitacaoSituacao.Tramitando, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario", tramitacao.Destinatario.Id > 0 ? (int?)tramitacao.Destinatario.Id : null, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario_setor", tramitacao.DestinatarioSetor.Id > 0 ? (int?)tramitacao.DestinatarioSetor.Id : null, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(tramitacao.Id, eHistoricoArtefato.tramitacao, eHistoricoAcao.retirarexterno, bancoDeDados);
				
				#endregion

				#region Atualizar o id da ultima tramitação no protocolo

				comando = bancoDeDados.CriarComando(@"begin "
				+ " update {0}tab_protocolo p set p.tramitacao = (select max(id) id from {0}hst_tramitacao h where h.protocolo_id = :protocolo), "
				+ " p.setor = :setor, p.emposse = :emposse where p.id = :protocolo;"
				+ " update {0}lst_protocolo p set p.tramitacao = (select max(id) id from {0}hst_tramitacao h where h.protocolo_id = :protocolo), "
				+ " p.setor_id = :setor, p.emposse_id = :emposse where p.protocolo_id = :protocolo; end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", tramitacao.Protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("setor", tramitacao.DestinatarioSetor.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("emposse", tramitacao.Destinatario.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Finalizar a tramitaçao de processo/documento

				List<String> lista = new List<string>();
				lista.Add("delete from {0}tab_tramitacao_externo e where e.tramitacao = :id;");
				lista.Add("delete from {0}tab_tramitacao t where t.id = :id;");
				comando = bancoDeDados.CriarComando(@"begin " + string.Join(" ", lista) + "end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", tramitacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Cancelar(Tramitacao tramitacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_tramitacao c set c.situacao = :situacao, c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", tramitacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", (int)eTramitacaoSituacao.Tramitando, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Histórico
				Historico.Gerar(tramitacao.Id, eHistoricoArtefato.tramitacao, eHistoricoAcao.cancelar, bancoDeDados);
				#endregion

				#region Cancelar tramitaçao de processo/documento

				comando = bancoDeDados.CriarComando(@"begin
					delete from {0}tab_tramitacao_arquivar e where e.tramitacao = :id;
					delete from {0}tab_tramitacao_externo e where e.tramitacao = :id;
					delete from {0}tab_tramitacao t where t.id = :id;
				end;", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", tramitacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		internal List<TramitacaoArquivoLista> ObterArquivos(int setorId = 0, BancoDeDados banco = null)
		{
			List<TramitacaoArquivoLista> arquivos = new List<TramitacaoArquivoLista>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.nome from tab_tramitacao_arquivo c", EsquemaBanco);

				if (setorId > 0)
				{
					comando.DbCommand.CommandText = " and c.setor = :setor";
					comando.AdicionarParametroEntrada("setor", DbType.Int32, setorId);
				}

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						arquivos.Add(new TramitacaoArquivoLista()
						{
							Id = Convert.ToInt32(reader["id"]),
							Texto = reader["nome"].ToString(),
							IsAtivo = true
						});
					}

					reader.Close();
				}
			}
			return arquivos;
		}

		internal List<OrgaoClasse> ObterOrgaosExterno()
		{
			List<OrgaoClasse> lst = new List<OrgaoClasse>();

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.nome from tab_tramitacao_orgao t");
			foreach (var item in daReader)
			{
				lst.Add(new OrgaoClasse()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["nome"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		#endregion
	}
}