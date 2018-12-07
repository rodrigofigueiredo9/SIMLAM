using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Data
{
	class ArquivarDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();
		
		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }
		
		private string EsquemaBanco { get; set; }

		#endregion

		public ArquivarDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Arquivar(Tramitacao tramitacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Enviar processos/documentos

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_tramitacao a (id, protocolo, tipo, objetivo, situacao, despacho,
				executor, remetente, remetente_setor, destinatario_setor, tid, data_envio) values ({0}seq_tramitacao.nextval, :protocolo, :tipo, :objetivo, 
				:situacao, :despacho, :executor, :remetente, :remetente_setor, :destinatario_setor, :tid, sysdate) returning a.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", tramitacao.Protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", tramitacao.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("objetivo", tramitacao.Objetivo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", tramitacao.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntClob("despacho", tramitacao.Despacho);
				comando.AdicionarParametroEntrada("executor", tramitacao.Executor.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("remetente", tramitacao.Remetente.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("remetente_setor", tramitacao.RemetenteSetor.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario_setor", tramitacao.DestinatarioSetor.Id > 0 ? (int?)tramitacao.DestinatarioSetor.Id : null, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				int id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#region Arquivar

				comando = bancoDeDados.CriarComando(@"insert into {0}tab_tramitacao_arquivar(id, tramitacao, arquivo, estante, prateleira, tid)
				values ({0}seq_tramitacao_arquivar.nextval, :tramitacao, :arquivo, :estante, :prateleira, :tid)", EsquemaBanco);

				comando.AdicionarParametroEntrada("tramitacao", id, DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo", tramitacao.Arquivamento.ArquivoId, DbType.Int32);
				comando.AdicionarParametroEntrada("estante", tramitacao.Arquivamento.EstanteId, DbType.Int32);
				comando.AdicionarParametroEntrada("prateleira", tramitacao.Arquivamento.PrateleiraId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Atualizar Posse
				comando = bancoDeDados.CriarComandoPlSql(@"begin 
					update {0}tab_protocolo p set p.setor = null, p.emposse = null where p.id = :protocolo; 
					update {0}lst_protocolo p set p.setor_id = null, p.emposse_id = null where p.protocolo_id = :protocolo; 
				end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", tramitacao.Protocolo.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#endregion

				#region Histórico

				Historico.Gerar(id, eHistoricoArtefato.tramitacao, eHistoricoAcao.arquivar, bancoDeDados);
				
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

				List<String> lista = new List<string>();
				lista.Add("delete from {0}tab_tramitacao_arquivar e where e.tramitacao = :id;");
				lista.Add("delete from {0}tab_tramitacao_externo e where e.tramitacao = :id;");
				lista.Add("delete from {0}tab_tramitacao t where t.id = :id;");
				comando = bancoDeDados.CriarComando(@"begin " + string.Join(" ", lista) + "end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", tramitacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Desarquivar(Tramitacao tramitacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualiza a tabela transacional

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_tramitacao t set t.situacao = :situacao, t.remetente = null, t.data_envio = null, t.objetivo = null,
					t.remetente_setor = t.destinatario_setor, t.destinatario = :destinatario,  t.destinatario_setor = :destinatario_setor where t.id = :id ", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", tramitacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", (int)eTramitacaoSituacao.Tramitando, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario", tramitacao.Destinatario.Id > 0 ? (int?)tramitacao.Destinatario.Id : null, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario_setor", tramitacao.DestinatarioSetor.Id > 0 ? (int?)tramitacao.DestinatarioSetor.Id : null, DbType.Int32);
				
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(tramitacao.Id, eHistoricoArtefato.tramitacao, eHistoricoAcao.desarquivar, bancoDeDados);
				
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
				lista.Add("delete from {0}tab_tramitacao_arquivar e where e.tramitacao = :id;");
				lista.Add("delete from {0}tab_tramitacao t where t.id = :id;");
				comando = bancoDeDados.CriarComando(@"begin " + string.Join(" ", lista) + "end;", EsquemaBanco);

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
				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.nome from {0}tab_tramitacao_arquivo c", EsquemaBanco);

				if (setorId > 0)
				{
					comando.DbCommand.CommandText += " where c.setor = :setor";
					comando.AdicionarParametroEntrada("setor", setorId, DbType.Int32);
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

		internal List<Prateleira> ObterArquivoPrateleiras(int estante = 0, int modo = 0, BancoDeDados banco = null)
		{
			List<Prateleira> prateleiras = new List<Prateleira>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.id, p.arquivo, p.identificacao, p.tid from {0}tab_tramitacao_arq_prateleira p where p.id > 0 ", EsquemaBanco);

				if (estante > 0)
				{
					comando.DbCommand.CommandText += " and p.estante = :estante";
					comando.AdicionarParametroEntrada("estante", estante, DbType.Int32);
				}

				if (modo > 0)
				{
					comando.DbCommand.CommandText += " and p.modo = :modo";
					comando.AdicionarParametroEntrada("modo", modo, DbType.Int32);
				}

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						prateleiras.Add(new Prateleira()
						{
							Id = Convert.ToInt32(reader["id"]),
							Arquivo = Convert.ToInt32(reader["arquivo"]),
							Texto = reader["identificacao"].ToString(),
							Tid = reader["tid"].ToString(),
							IsAtivo = true
						});
					}

					reader.Close();
				}
			}
			return prateleiras;
		}

		internal List<Estante> ObterArquivoEstantes(int arquivoId = 0, BancoDeDados banco = null)
		{
			List<Estante> estantes = new List<Estante>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.arquivo, e.nome, e.tid from {0}tab_tramitacao_arq_estante e", EsquemaBanco);

				if (arquivoId > 0)
				{
					comando.DbCommand.CommandText += " where e.arquivo = :arquivoid";
					comando.AdicionarParametroEntrada("arquivoid", arquivoId, DbType.Int32);
				}

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						estantes.Add(new Estante()
						{
							Id = Convert.ToInt32(reader["id"]),
							Arquivo = Convert.ToInt32(reader["arquivo"]),
							Texto = reader["nome"].ToString(),
							Tid = reader["tid"].ToString(),
							IsAtivo = true
						});
					}

					reader.Close();
				}
			}
			return estantes;
		}

		internal Arquivar ObterArquivamento(int tramitacaoId, BancoDeDados banco = null)
		{
			Arquivar arquivar = new Arquivar();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
									select f.id          funcionario_id,
											f.nome        funcionario_nome,
											ta.tramitacao tamitacao_id,
											a.id          arquivo_id,
											a.nome        arquivo_nome,
											te.id         estante_id,
											te.nome       estante_nome,
											tp.id         prateleira_id,
											tp.identificacao       prateleira_nome,
											s.id          setor_id,
											s.nome        setor_nome
										from {0}tab_funcionario               f,
											 {0}tab_tramitacao                t,
											 {0}tab_tramitacao_arquivar       ta,
											 {0}tab_tramitacao_arquivo        a,
											 {0}tab_tramitacao_arq_estante    te,
											 {0}tab_tramitacao_arq_prateleira tp,
											 {0}tab_setor                     s
										where te.id = ta.estante
										and tp.id = ta.prateleira
										and a.id = ta.arquivo
										and s.id = a.setor
										and t.id = ta.tramitacao
										and f.id = t.remetente
										and ta.tramitacao = :tramitacaoid", EsquemaBanco);

				comando.AdicionarParametroEntrada("tramitacaoid", tramitacaoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						arquivar.ArquivoId = Convert.ToInt32(reader["arquivo_id"]);
						arquivar.ArquivoNome = reader["arquivo_nome"].ToString();

						arquivar.EstanteId = Convert.ToInt32(reader["estante_id"]);
						arquivar.EstanteNome = reader["estante_nome"].ToString();

						arquivar.PrateleiraId = Convert.ToInt32(reader["prateleira_id"]);
						arquivar.PrateleiraNome = reader["prateleira_nome"].ToString();

						arquivar.SetorId = Convert.ToInt32(reader["setor_id"]);
						arquivar.SetorNome = reader["setor_nome"].ToString();

						arquivar.Funcionario.Id = Convert.ToInt32(reader["funcionario_id"]);
						arquivar.Funcionario.Nome = reader["funcionario_nome"].ToString();
					}

					reader.Close();
				}
			}

			return arquivar;
		}

		internal Arquivar ObterArquivamentoAutomatico(string despacho, int objetivoId, BancoDeDados banco = null)
		{
			Arquivar arquivar = new Arquivar();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
									select  a.id          arquivo_id,
											a.nome        arquivo_nome,
											te.id         estante_id,
											te.nome       estante_nome,
											tp.id         prateleira_id,
											tp.identificacao       prateleira_nome,
											s.id          setor_id,
											s.nome        setor_nome
										from {0}tab_tramitacao_arquivo        a,
											 {0}tab_tramitacao_arq_estante    te,
											 {0}tab_tramitacao_arq_prateleira tp,
											 {0}tab_setor                     s
										where te.arquivo = a.id
										and tp.arquivo = a.id
										and tp.estante = te.id
										and s.id = a.setor
										and a.arquivar_automatico = 1
										and rownum = 1", EsquemaBanco);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						arquivar.ArquivoId = Convert.ToInt32(reader["arquivo_id"]);
						arquivar.ArquivoNome = reader["arquivo_nome"].ToString();

						arquivar.EstanteId = Convert.ToInt32(reader["estante_id"]);
						arquivar.EstanteNome = reader["estante_nome"].ToString();

						arquivar.PrateleiraId = Convert.ToInt32(reader["prateleira_id"]);
						arquivar.PrateleiraModoId = arquivar.PrateleiraId;
						arquivar.PrateleiraNome = reader["prateleira_nome"].ToString();

						arquivar.SetorId = Convert.ToInt32(reader["setor_id"]);
						arquivar.SetorNome = reader["setor_nome"].ToString();

						arquivar.ObjetivoId = objetivoId;
						arquivar.Despacho = despacho;
					}

					reader.Close();
				}
			}

			return arquivar;
		}

		#endregion
	}
}