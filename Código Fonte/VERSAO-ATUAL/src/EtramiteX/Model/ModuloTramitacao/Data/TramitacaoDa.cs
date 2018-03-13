using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Data
{
	public class TramitacaoDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();
		
		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }
		
		private string EsquemaBanco { get; set; }

		#endregion

		public TramitacaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Enviar(Tramitacao tramitacao, BancoDeDados banco = null)
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

				tramitacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Histórico/Consulta

				Historico.Gerar(tramitacao.Id, eHistoricoArtefato.tramitacao, eHistoricoAcao.enviar, bancoDeDados);
				
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

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_tramitacao c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", tramitacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Histórico/Consulta
				Historico.Gerar(tramitacao.Id, eHistoricoArtefato.tramitacao, eHistoricoAcao.cancelar, bancoDeDados);				
				#endregion

				#region Cancelar tramitaçao do protocolo

				List<String> lista = new List<string>();
				lista.Add("delete from {0}tab_tramitacao_arquivar e where e.tramitacao = :id;");
				lista.Add("delete from {0}tab_tramitacao_externo e where e.tramitacao = :id;");
				lista.Add("delete from {0}tab_tramitacao t where t.id = :id;");

				comando = bancoDeDados.CriarComando(@"begin " + string.Join(" ", lista) + " end;", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", tramitacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Receber(Tramitacao tramitacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Histórico/Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_tramitacao tt set tt.destinatario = :destinatario, tt.tid = :tid where tt.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("destinatario", tramitacao.Destinatario.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("id", tramitacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(tramitacao.Id, eHistoricoArtefato.tramitacao, eHistoricoAcao.receber, bancoDeDados);
				
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
				lista.Add("begin");
				lista.Add("delete from {0}tab_tramitacao_arquivar a where a.tramitacao = :id;");
				lista.Add("delete from {0}tab_tramitacao t where t.id = :id;");
				lista.Add("end;");
				comando = bancoDeDados.CriarComando(String.Join(" ", lista), EsquemaBanco);
				comando.AdicionarParametroEntrada("id", tramitacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void MudarTipoTramitacaoSetor(List<Setor> setores, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();
				eHistoricoAcao acao;

				#region Setores/Funcionários

				if (setores != null && setores.Count > 0)
				{
					foreach (Setor item in setores)
					{
						Comando comando = bancoDeDados.CriarComando("delete from {0}tab_tramitacao_setor_func c ", EsquemaBanco);
						comando.DbCommand.CommandText += String.Format("where c.setor = :setor{0}", comando.AdicionarNotIn("and", "c.funcionario", DbType.Int32, item.Funcionarios.Select(y => y.Id).ToList()));
						comando.AdicionarParametroEntrada("setor", item.Id, DbType.Int32);
						bancoDeDados.ExecutarNonQuery(comando);

						if (item.IdRelacao > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_tramitacao_setor s set s.tipo = :tipo, s.tid = :tid where s.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", item.IdRelacao, DbType.Int32);

							acao = eHistoricoAcao.atualizar;
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_tramitacao_setor s(id, setor, tipo, tid) values ({0}seq_tramitacao_setor.nextval, :setor, :tipo, :tid) returning s.id into :id", EsquemaBanco);
							comando.AdicionarParametroSaida("id", DbType.Int32);
							comando.AdicionarParametroEntrada("setor", item.Id, DbType.Int32);

							acao = eHistoricoAcao.criar;
						}

						comando.AdicionarParametroEntrada("tipo", item.TramitacaoTipoId, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						if (item.IdRelacao <= 0)
						{
							item.IdRelacao = Convert.ToInt32(comando.ObterValorParametro("id"));
						}

						#region Funcionários

						if (item.Funcionarios != null)
						{
							foreach (FuncionarioLst itemAux in item.Funcionarios)
							{
								comando = bancoDeDados.CriarComando(@"insert into {0}tab_tramitacao_setor_func(id, setor, funcionario, tid)
								(select {0}seq_tramitacao_setor_func.nextval, :setor, :funcionario, :tid from dual where not exists
								(select id from {0}tab_tramitacao_setor_func r where r.setor = :setor and r.funcionario = :funcionario))", EsquemaBanco);

								comando.AdicionarParametroEntrada("setor", item.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("funcionario", itemAux.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}

						#endregion

						#region Histórico

						Historico.Gerar(item.Id, eHistoricoArtefato.tramitacaosetor, acao, bancoDeDados);

						#endregion
					}
				}

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void SalvarMotivo(Motivo motivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				eHistoricoAcao acao = eHistoricoAcao.atualizar;
				Comando comando;

				if (motivo.Id > 0)
				{
					comando = bancoDeDados.CriarComando(@"update tab_tramitacao_motivo t set t.texto = :texto, t.ativo = :ativo, t.tid = :tid where t.id =:id returning id into :idSaida", EsquemaBanco);
					comando.AdicionarParametroEntrada("id", motivo.Id, DbType.Int32);
				}
				else
				{
					acao = eHistoricoAcao.criar;
					comando = bancoDeDados.CriarComando(@"insert into tab_tramitacao_motivo (id, texto, ativo, tid) values (seq_tramitacao_motivo.nextval, :texto, :ativo, :tid) returning id into :idSaida", EsquemaBanco);
				}

				comando.AdicionarParametroEntrada("texto", motivo.Nome, DbType.String);
				comando.AdicionarParametroEntrada("ativo", motivo.IsAtivo ? 1 : 0, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("idSaida", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				if (motivo.Id <= 0)
				{
					motivo.Id = Convert.ToInt32(comando.ObterValorParametro("idSaida"));
				}

				Historico.Gerar(motivo.Id, eHistoricoArtefato.tramitacaomotivo, acao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		internal Resultados<Tramitacao> FiltrarEmPosse(Filtro<ListarTramitacaoFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<Tramitacao> retorno = new Resultados<Tramitacao>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				string esquema = (string.IsNullOrEmpty(EsquemaBanco) ? "" : ".");
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("p.emposse_id", "emposse_id", filtros.Dados.EmposseId);

				comandtxt += comando.FiltroAnd("p.tipo_id", "tipo_id", filtros.Dados.ProtocoloTipo);

				comandtxt += comando.FiltroAnd("p.numero", "numero", filtros.Dados.Protocolo.Numero);

				comandtxt += comando.FiltroAnd("p.ano", "ano", filtros.Dados.Protocolo.Ano);

				comandtxt += comando.FiltroAnd("t.remetente_id", "remetente_id", filtros.Dados.RemetenteId);

				comandtxt += comando.FiltroAnd("p.setor_id", "remetente_setor_id", filtros.Dados.RemetenteSetorId); // Setor atual da posse, é onde se encontra o protocolo

				comandtxt += comando.FiltroAnd("p.setor_id", "setor_id", filtros.Dados.EmposseSetorId); // Setor atual da posse, é onde se encontra o protocolo

				comandtxt += comando.FiltroAnd("t.destinatario_id", "destinatario_id", filtros.Dados.DestinatarioId);

				comandtxt += comando.FiltroAnd("t.destinatario_setor_id", "destinatario_setor_id", filtros.Dados.DestinatarioSetorId);

				comandtxt += comando.FiltroAnd("t.tipo_id", "tipo_id", filtros.Dados.TramitacaoTipoId);

				comandtxt += comando.FiltroIn("nvl(t.destinatario_setor_id, p.setor_id)", String.Format("select s.setor from {0}tab_funcionario_setor s where s.funcionario = :funcionario_setor_destino", esquema),
				"funcionario_setor_destino", filtros.Dados.FuncionarioSetorDestinoId);

				comandtxt += comando.FiltroIn("p.setor_id", String.Format(@"select s.setor from {0}tab_funcionario_setor s, {0}tab_tramitacao_setor_func fs where fs.funcionario = s.funcionario 
				and s.funcionario = :registrador_setor", esquema), "registrador_setor", filtros.Dados.RegistradorDestinatarioId);

				comandtxt += comando.FiltroIn("p.emposse_id", String.Format(@"select fs.funcionario from {0}tab_funcionario_setor fs where fs.setor in (select f.setor from {0}tab_tramitacao_setor_func f 
				where f.funcionario = :emposse_registrador)", esquema), "emposse_registrador", filtros.Dados.RegistradorDestinatarioSetorId, "or p.emposse_id = :emposse_registrador");

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "numero,ano", "protocolo", "data_envio", "objetivo_texto" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("numero,ano");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format(@"select count(*) from {0}lst_protocolo p, {0}lst_hst_tramitacao t where p.tramitacao = t.tramitacao_hst_id(+)
				and not exists (select 1 from {0}tab_tramitacao t where t.protocolo = p.protocolo_id)" + comandtxt, esquema);
				
				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.DbCommand.CommandText = String.Format(@"select p.protocolo_id, p.protocolo, p.numero, p.ano, p.numero_completo, t.tid, t.tramitacao_id, t.tramitacao_hst_id, 
				p.tipo_texto, t.remetente_id, t.remetente_tid, t.remetente_nome, t.remetente_setor_id, t.remetente_setor_sigla, t.remetente_setor_nome, 
				t.destinatario_id, t.destinatario_tid, t.destinatario_nome, t.destinatario_setor_id, t.destinatario_setor_sigla, t.destinatario_setor_nome, t.objetivo_id, 
				t.objetivo_texto, t.situacao_id, t.situacao_texto, t.acao_executada, t.data_envio, t.data_execucao data_recebimento from {0}lst_protocolo p, 
				{0}lst_hst_tramitacao t where p.tramitacao = t.tramitacao_hst_id(+) and not exists (select 1 from {0}tab_tramitacao t where t.protocolo = p.protocolo_id) "

				+ comandtxt + DaHelper.Ordenar(colunas, ordenar), esquema);

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Tramitacao tramitacao;

					while (reader.Read())
					{
						tramitacao = new Tramitacao();
						if (reader["tramitacao_id"] != null && !Convert.IsDBNull(reader["tramitacao_id"]))
						{
							tramitacao.Id = Convert.ToInt32(reader["tramitacao_id"]);
							tramitacao.HistoricoId = Convert.ToInt32(reader["tramitacao_hst_id"]);

							tramitacao.DataEnvio.Data = Convert.IsDBNull(reader["data_envio"]) ? null : (DateTime?)Convert.ToDateTime(reader["data_envio"]);
							tramitacao.DataRecebido.Data = Convert.IsDBNull(reader["data_recebimento"]) ? null : (DateTime?)Convert.ToDateTime(reader["data_recebimento"]);
							tramitacao.DataExecucao.Data = tramitacao.DataRecebido.Data;

							tramitacao.Tid = reader["tid"].ToString();

							if (reader["objetivo_id"] != null && !Convert.IsDBNull(reader["objetivo_id"]))
							{
								tramitacao.Objetivo.Id = Convert.ToInt32(reader["objetivo_id"]);
								tramitacao.Objetivo.Texto = Convert.ToString(reader["objetivo_texto"]);
							}

							if (reader["remetente_id"] != null && !Convert.IsDBNull(reader["remetente_id"]))
							{
								tramitacao.Remetente.Id = Convert.ToInt32(reader["remetente_id"]);
							}

							tramitacao.Remetente.Nome = reader["remetente_nome"].ToString();

							if (reader["remetente_setor_id"] != null && !Convert.IsDBNull(reader["remetente_setor_id"]))
							{
								tramitacao.RemetenteSetor.Id = Convert.ToInt32(reader["remetente_setor_id"]);
							}

							tramitacao.RemetenteSetor.Sigla = Convert.ToString(reader["remetente_setor_sigla"]);
							tramitacao.RemetenteSetor.Nome = Convert.ToString(reader["remetente_setor_nome"]);


							if (reader["destinatario_id"] != null && !Convert.IsDBNull(reader["destinatario_id"]))
							{
								tramitacao.Destinatario.Id = Convert.ToInt32(reader["destinatario_id"]);
							}

							tramitacao.Destinatario.Nome = Convert.ToString(reader["destinatario_nome"]);
							tramitacao.DestinatarioSetor.Sigla = Convert.ToString(reader["destinatario_setor_sigla"]);
							tramitacao.DestinatarioSetor.Nome = Convert.ToString(reader["destinatario_setor_nome"]);
						}
						else
						{
							tramitacao.DataEnvio = new DateTecno() { Data = new DateTime(), DataTexto = String.Empty, DataHoraTexto = String.Empty };
							tramitacao.DataRecebido = new DateTecno() { Data = new DateTime(), DataTexto = String.Empty, DataHoraTexto = String.Empty };
							tramitacao.Remetente = new Funcionario { Id = 0, Nome = String.Empty };
							tramitacao.Destinatario = new Funcionario { Id = 0, Nome = String.Empty };
							tramitacao.Objetivo = new Objetivo { Id = 0, Texto = String.Empty };
						}

						if (reader["destinatario_setor_id"] != null && !Convert.IsDBNull(reader["destinatario_setor_id"]))
						{
							tramitacao.DestinatarioSetor.Id = Convert.ToInt32(reader["destinatario_setor_id"]);
						}

						tramitacao.IsExisteHistorico = (reader["tramitacao_hst_id"] != null && !Convert.IsDBNull(reader["tramitacao_hst_id"]));
						tramitacao.Protocolo.IsProcesso = (reader["protocolo"] != null && reader["protocolo"].ToString() == "1");
						tramitacao.Protocolo.Id = Convert.ToInt32(reader["protocolo_id"]);
						tramitacao.Protocolo.NumeroProtocolo = Convert.ToInt32(reader["numero"]);
						tramitacao.Protocolo.Ano = Convert.ToInt32(reader["ano"]);
						tramitacao.Protocolo.Tipo.Texto = reader["tipo_texto"].ToString();

						retorno.Itens.Add(tramitacao);
					}

					reader.Close();

					#endregion
				}
			}
			return retorno;
		}

		internal Resultados<Tramitacao> Filtrar(Filtro<ListarTramitacaoFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<Tramitacao> retorno = new Resultados<Tramitacao>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				string esquema = (string.IsNullOrEmpty(EsquemaBanco) ? "" : ".");
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				#region Filtros de protocolo

				comandtxt += comando.FiltroAnd("t.protocolo", "protocolo", filtros.Dados.ProtocoloTipo);

				comandtxt += comando.FiltroAnd("t.protocolo_id", "protocolo_id", filtros.Dados.Protocolo.Id);

				comandtxt += comando.FiltroAnd("t.protocolo_numero", "numero", filtros.Dados.Protocolo.Numero);

				comandtxt += comando.FiltroAnd("t.protocolo_ano", "ano", filtros.Dados.Protocolo.Ano);

				#endregion

				comandtxt += comando.FiltroAnd("t.remetente_id", "remetente_id", filtros.Dados.RemetenteId);

				comandtxt += comando.FiltroAnd("t.remetente_setor_id", "remetente_setor_id", filtros.Dados.RemetenteSetorId);

				comandtxt += comando.FiltroAnd("t.destinatario_id", "destinatario_id", filtros.Dados.DestinatarioId);

				comandtxt += comando.FiltroAnd("t.destinatario_setor_id", "destinatario_setor_id", filtros.Dados.DestinatarioSetorId);

				comandtxt += comando.FiltroAnd("t.tipo_id", "tipo_id", filtros.Dados.TramitacaoTipoId);

				comandtxt += comando.FiltroIn("t.destinatario_setor_id", String.Format("select s.setor from {0}tab_funcionario_setor s where s.funcionario = :funcionario_setor_destino", esquema),
				"funcionario_setor_destino", filtros.Dados.FuncionarioSetorDestinoId);

				comandtxt += comando.FiltroIn("t.remetente_id", String.Format("select fs.funcionario from {0}tab_tramitacao_setor_func f, {0}tab_funcionario_setor fs where fs.setor = f.setor and f.funcionario = :registrador_remetente", esquema),
				"registrador_remetente", filtros.Dados.RegistradorRemetenteId);

				comandtxt += comando.FiltroIn("t.tramitacao_id", String.Format("select te.tramitacao from {0}tab_tramitacao_externo te where te.orgao = :orgao_externo", esquema),
				"orgao_externo", filtros.Dados.OrgaoExternoId);

				comandtxt += comando.FiltroNull("t.destinatario_id", filtros.Dados.DestinatarioNulo);

				comandtxt += comando.FiltroNull("t.destinatario_id", filtros.Dados.DestinatarioNaoNulo, false);

				#region Desarquivar

				comandtxt += comando.FiltroIn("a.id", String.Format("select ta.tramitacao from {0}tab_tramitacao_arquivar ta where ta.arquivo = :arquivo", esquema),
				"arquivo", filtros.Dados.ArquivoId);

				comandtxt += comando.FiltroIn("a.id", String.Format("select tarq.tramitacao from {0}tab_tramitacao_arquivar tarq where tarq.prateleira in (select id from {0}tab_tramitacao_arq_prateleira where estante = :estante)", esquema),
				"estante", filtros.Dados.ArquivoEstanteId);

				comandtxt += comando.FiltroIn("a.id", String.Format("select tarq.tramitacao from {0}tab_tramitacao_arquivar tarq where tarq.prateleira in (select id from {0}tab_tramitacao_arq_prateleira where modo = :prateleira_modo)", esquema),
				"prateleira_modo", filtros.Dados.ArquivoPrateleiraModoId);

				comandtxt += comando.FiltroIn("a.id", String.Format("select tarq.tramitacao from {0}tab_tramitacao_arquivar tarq where tarq.prateleira in (select id from {0}tab_tramitacao_arq_prateleira where identificacao = :identificacao)", esquema),
				"identificacao", filtros.Dados.ArquivoIdentificacao);

				comandtxt += comando.FiltroIn("t.protocolo_id", String.Format(@"select a.protocolo from {0}tab_protocolo_atividades a where a.atividade = :atividade", esquema),
				"atividade", filtros.Dados.AtividadeId);

				comandtxt += comando.FiltroIn("t.protocolo_id", String.Format(@"select a.protocolo id from {0}tab_protocolo_atividades a where a.situacao = :atividade_situacao", esquema),
				"atividade_situacao", filtros.Dados.AtividadeSituacaoId);

				comandtxt += comando.FiltroIn("t.protocolo_id", String.Format(@"select tp.id from tab_protocolo tp where tp.interessado in (select p.id from tab_pessoa p 
				where lower(p.nome) like lower(:interessado_nome_razao) || '%' or lower(p.razao_social) like lower(:interessado_nome_razao) || '%')", esquema), "interessado_nome_razao", filtros.Dados.InteressadoNomeRazao);

				comandtxt += comando.FiltroIn("t.protocolo_id", String.Format(@"select tp.id from tab_protocolo tp where tp.interessado in (select p.id from tab_pessoa p 
				where lower(p.cpf) like lower(:interessado_cpf_cnpj) || '%' or lower(p.cnpj) like lower(:interessado_cpf_cnpj) || '%')", esquema), "interessado_cpf_cnpj", filtros.Dados.InteressadoCpfCnpj);

				comandtxt += comando.FiltroIn("t.protocolo_id", String.Format(@"select tp.id from {0}tab_protocolo tp where tp.empreendimento in 
				(select te.empreendimento from {0}tab_empreendimento_endereco te where te.correspondencia = 0 and te.estado = :empreendimento_uf)", esquema), "empreendimento_uf", filtros.Dados.EmpreendimentoUf);

				comandtxt += comando.FiltroIn("t.protocolo_id", String.Format(@"select tp.id from {0}tab_protocolo tp where tp.empreendimento in 
				(select te.empreendimento from {0}tab_empreendimento_endereco te where te.correspondencia = 0 and te.municipio = :empreendimento_municipio)", esquema), "empreendimento_municipio", filtros.Dados.EmpreendimentoMunicipio);

				#endregion

				#endregion

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "protocolo_numero,protocolo_ano", "protocolo", "data_envio", "objetivo_texto" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("protocolo_numero,protocolo_ano");
				}

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format(@"select count(*) from {0}tab_tramitacao a, {0}lst_hst_tramitacao t where a.id = t.tramitacao_id and a.tid = t.tid 
				and t.situacao_id = :situacao_tramitacao " + comandtxt, esquema);

				comando.AdicionarParametroEntrada("situacao_tramitacao", (filtros.Dados.TramitacaoSituacaoId == 0) ? (int)eTramitacaoSituacao.Tramitando : filtros.Dados.TramitacaoSituacaoId, DbType.Int32);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.DbCommand.CommandText = String.Format(@"select t.tramitacao_hst_id, t.tramitacao_id, t.protocolo, t.protocolo_id, t.protocolo_tid, 
				t.protocolo_numero, t.protocolo_ano, t.protocolo_numero_completo, t.protocolo_numero_autuacao, t.protocolo_tipo_id, t.protocolo_tipo_texto, 
				t.remetente_id, t.remetente_tid, t.remetente_nome, t.remetente_setor_id, t.remetente_setor_sigla, t.remetente_setor_nome, t.destinatario_id, 
				t.destinatario_tid, t.destinatario_nome, t.destinatario_setor_id, t.destinatario_setor_sigla, t.destinatario_setor_nome, t.objetivo_id, t.objetivo_texto, 
				t.situacao_id, t.situacao_texto, t.data_execucao data_recebimento, t.data_envio, t.data_execucao, t.tid from {0}tab_tramitacao a, {0}lst_hst_tramitacao t 
				where a.tid = t.tid and a.id = t.tramitacao_id and t.situacao_id = :situacao_tramitacao "
					+ comandtxt + DaHelper.Ordenar(colunas, ordenar), esquema);

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Tramitacao tramitacao;

					while (reader.Read())
					{
						tramitacao = new Tramitacao();
						tramitacao.Id = Convert.ToInt32(reader["tramitacao_id"]);

						if (reader["tramitacao_hst_id"] != null && !Convert.IsDBNull(reader["tramitacao_hst_id"]))
						{
							tramitacao.HistoricoId = Convert.ToInt32(reader["tramitacao_hst_id"]);
						}

						tramitacao.DataEnvio.Data = Convert.IsDBNull(reader["data_envio"]) ? null : (DateTime?)Convert.ToDateTime(reader["data_envio"]);
						tramitacao.DataRecebido.Data = Convert.IsDBNull(reader["data_recebimento"]) ? null : (DateTime?)Convert.ToDateTime(reader["data_recebimento"]);
						tramitacao.DataExecucao.Data = tramitacao.DataRecebido.Data;

						tramitacao.Protocolo.IsProcesso = (reader["protocolo"] != null && reader["protocolo"].ToString() == "1");
						tramitacao.Protocolo.Id = Convert.ToInt32(reader["protocolo_id"]);
						tramitacao.Protocolo.NumeroProtocolo = Convert.ToInt32(reader["protocolo_numero"]);
						tramitacao.Protocolo.Ano = Convert.ToInt32(reader["protocolo_ano"]);
						tramitacao.Protocolo.Tipo.Texto = reader["protocolo_tipo_texto"].ToString();

						if (reader["objetivo_id"] != null && !Convert.IsDBNull(reader["objetivo_id"]))
						{
							tramitacao.Objetivo.Id = Convert.ToInt32(reader["objetivo_id"]);
							tramitacao.Objetivo.Texto = reader["objetivo_texto"].ToString();
						}

						tramitacao.RemetenteSetor.Id = Convert.ToInt32(reader["remetente_setor_id"]);
						tramitacao.Remetente.Nome = reader["remetente_nome"].ToString();
						tramitacao.RemetenteSetor.Sigla = reader["remetente_setor_sigla"].ToString();
						tramitacao.RemetenteSetor.Nome = reader["remetente_setor_nome"].ToString();

						if (reader["destinatario_id"] != null && !Convert.IsDBNull(reader["destinatario_id"]))
						{
							tramitacao.Destinatario.Id = Convert.ToInt32(reader["destinatario_id"]);
						}

						tramitacao.Destinatario.Nome = reader["destinatario_nome"].ToString();
						tramitacao.DestinatarioSetor.Sigla = reader["destinatario_setor_sigla"].ToString();
						tramitacao.DestinatarioSetor.Nome = reader["destinatario_setor_nome"].ToString();

						if (reader["destinatario_setor_id"] != null && !Convert.IsDBNull(reader["destinatario_setor_id"]))
						{
							tramitacao.DestinatarioSetor.Id = Convert.ToInt32(reader["destinatario_setor_id"]);
						}

						retorno.Itens.Add(tramitacao);
					}

					reader.Close();

					#endregion
				}
			}
			return retorno;
		}

		internal Resultados<Tramitacao> FiltrarHistorico(Filtro<ListarTramitacaoFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<Tramitacao> retorno = new Resultados<Tramitacao>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				string esquema = (string.IsNullOrEmpty(EsquemaBanco) ? "" : ".");
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				#region Filtros de protocolo

				comandtxt += comando.FiltroAnd("t.protocolo_id", "protocolo_id", filtros.Dados.Protocolo.Id);

				comandtxt += comando.FiltroAnd("t.protocolo", "protocolo", filtros.Dados.ProtocoloTipo);

				comandtxt += comando.FiltroAnd("t.protocolo_numero", "numero", filtros.Dados.Protocolo.Numero);

				comandtxt += comando.FiltroAnd("t.protocolo_ano", "ano", filtros.Dados.Protocolo.Ano);

				#endregion

				comandtxt += comando.FiltroAnd("t.remetente_id", "remetente_id", filtros.Dados.RemetenteId);

				comandtxt += comando.FiltroAnd("t.remetente_setor_id", "remetente_setor_id", filtros.Dados.RemetenteSetorId);

				comandtxt += comando.FiltroAnd("t.destinatario_id", "destinatario_id", filtros.Dados.DestinatarioId);

				comandtxt += comando.FiltroAnd("t.destinatario_setor_id", "destinatario_setor_id", filtros.Dados.DestinatarioSetorId);

				comandtxt += comando.FiltroIn("t.destinatario_setor_id", String.Format("select s.setor from {0}tab_funcionario_setor s where s.funcionario = :funcionario_setor_destino", esquema),
				"funcionario_setor_destino", filtros.Dados.FuncionarioSetorDestinoId);

				comandtxt += comando.FiltroIn("t.destinatario_setor_id", String.Format("select s.setor from {0}tab_funcionario_setor s, {0}tab_tramitacao_setor_func fs where fs.funcionario = s.funcionario and s.funcionario = :registrador_setor", esquema),
				"registrador_setor", filtros.Dados.RegistradorDestinatarioSetorId);

				comandtxt += comando.FiltroIn("t.destinatario_setor_id", String.Format(@"select s.setor from {0}tab_funcionario_setor s, {0}tab_tramitacao_setor_func fs where fs.funcionario = s.funcionario and s.funcionario = :emposse_registrador", esquema),
				"emposse_registrador", filtros.Dados.RegistradorDestinatarioId, "or t.destinatario_id = :emposse_registrador");

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "protocolo_numero,protocolo_ano", "protocolo", "data_envio", "objetivo_texto" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("protocolo_numero,protocolo_ano");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format(@"select count(*) from {0}lst_hst_tramitacao t where t.id > 0" + comandtxt, esquema);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.DbCommand.CommandText = String.Format(@"select t.id, t.tid, t.tramitacao_id, t.tramitacao_hst_id, t.protocolo, t.protocolo_id, t.protocolo_numero, t.protocolo_ano, 
				t.protocolo_numero_completo, t.protocolo_tipo_texto, t.remetente_id, t.remetente_tid, t.remetente_nome, t.remetente_setor_id, 
				t.remetente_setor_sigla, t.remetente_setor_nome, t.destinatario_id, t.destinatario_tid, t.destinatario_nome, t.destinatario_setor_id, 
				t.destinatario_setor_sigla, t.destinatario_setor_nome, t.objetivo_id, t.objetivo_texto, t.situacao_id, t.situacao_texto, 
				t.acao_executada, (select aa.texto from {0}lov_historico_artefatos_acoes a, {0}lov_historico_acao aa where a.acao = aa.id and a.id =  t.acao_executada) acao_executada_texto,
				t.data_envio, t.data_execucao data_recebimento from {0}lst_hst_tramitacao t where t.id > 0"
				+ comandtxt + DaHelper.Ordenar(colunas, ordenar), esquema);

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Tramitacao tramitacao;

					while (reader.Read())
					{
						tramitacao = new Tramitacao();
						tramitacao.Id = Convert.ToInt32(reader["tramitacao_id"]);
						tramitacao.HistoricoId = Convert.ToInt32(reader["tramitacao_hst_id"]);

						tramitacao.DataEnvio.Data = Convert.IsDBNull(reader["data_envio"]) ? null : (DateTime?)Convert.ToDateTime(reader["data_envio"]);
						tramitacao.DataRecebido.Data = Convert.IsDBNull(reader["data_recebimento"]) ? null : (DateTime?)Convert.ToDateTime(reader["data_recebimento"]);
						tramitacao.DataExecucao.Data = tramitacao.DataRecebido.Data;

						tramitacao.Tid = reader["tid"].ToString();

						if (reader["objetivo_id"] != null && !Convert.IsDBNull(reader["objetivo_id"]))
						{
							tramitacao.Objetivo.Id = Convert.ToInt32(reader["objetivo_id"]);
							tramitacao.Objetivo.Texto = Convert.ToString(reader["objetivo_texto"]);
						}

						if (reader["remetente_id"] != null && !Convert.IsDBNull(reader["remetente_id"]))
						{
							tramitacao.Remetente.Id = Convert.ToInt32(reader["remetente_id"]);
						}

						tramitacao.Remetente.Nome = reader["remetente_nome"].ToString();

						if (reader["remetente_setor_id"] != null && !Convert.IsDBNull(reader["remetente_setor_id"]))
						{
							tramitacao.RemetenteSetor.Id = Convert.ToInt32(reader["remetente_setor_id"]);
						}

						tramitacao.RemetenteSetor.Sigla = Convert.ToString(reader["remetente_setor_sigla"]);
						tramitacao.RemetenteSetor.Nome = Convert.ToString(reader["remetente_setor_nome"]);


						if (reader["destinatario_id"] != null && !Convert.IsDBNull(reader["destinatario_id"]))
						{
							tramitacao.Destinatario.Id = Convert.ToInt32(reader["destinatario_id"]);
						}

						tramitacao.Destinatario.Nome = Convert.ToString(reader["destinatario_nome"]);

						if (reader["destinatario_setor_id"] != null && !Convert.IsDBNull(reader["destinatario_setor_id"]))
						{
							tramitacao.DestinatarioSetor.Id = Convert.ToInt32(reader["destinatario_setor_id"]);
						}

						tramitacao.DestinatarioSetor.Sigla = Convert.ToString(reader["destinatario_setor_sigla"]);
						tramitacao.DestinatarioSetor.Nome = Convert.ToString(reader["destinatario_setor_nome"]);

						tramitacao.IsExisteHistorico = (reader["tramitacao_hst_id"] != null && !Convert.IsDBNull(reader["tramitacao_hst_id"]));
						tramitacao.Protocolo.IsProcesso = (reader["protocolo"] != null && reader["protocolo"].ToString() == "1");
						tramitacao.Protocolo.Id = Convert.ToInt32(reader["protocolo_id"]);
						tramitacao.Protocolo.NumeroProtocolo = Convert.ToInt32(reader["protocolo_numero"]);
						tramitacao.Protocolo.Ano = Convert.ToInt32(reader["protocolo_ano"]);
						tramitacao.Protocolo.Tipo.Texto = reader["protocolo_tipo_texto"].ToString();

						if (reader["acao_executada"] != null && !Convert.IsDBNull(reader["acao_executada"]))
						{
							tramitacao.AcaoId = Convert.ToInt32(reader["acao_executada"]);
							tramitacao.AcaoExecutada = reader["acao_executada_texto"].ToString();
						}

						retorno.Itens.Add(tramitacao);
					}

					reader.Close();

					#endregion
				}
			}
			return retorno;
		}

		internal Tramitacao Obter(int id, BancoDeDados banco = null)
		{
			Tramitacao tramitacao = new Tramitacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Tramitacao

				Comando comando = bancoDeDados.CriarComando(@"select t.tramitacao_hst_id, t.tramitacao_id id, t.protocolo, t.protocolo_id, t.protocolo_tid, 
				t.protocolo_numero, t.protocolo_ano, t.protocolo_numero_completo, t.protocolo_numero_autuacao, t.protocolo_tipo_id, t.protocolo_tipo_texto, 
				t.remetente_id, t.remetente_tid, t.remetente_nome, t.remetente_setor_id, t.remetente_setor_sigla, t.remetente_setor_nome, t.destinatario_id, 
				t.destinatario_tid, t.destinatario_nome, t.destinatario_setor_id, t.destinatario_setor_sigla, t.destinatario_setor_nome, t.objetivo_id, 
				t.objetivo_texto, t.situacao_id, t.situacao_texto, t.data_execucao data_recebimento, t.data_envio, t.data_execucao, t.despacho, t.remetente_municipio_nome,
				t.remetente_estado_sigla, t.destinatario_municipio_nome,t.destinatario_estado_sigla, t.orgao_externo_id, t.orgao_externo_nome, a.executor, t.tid 
				from {0}tab_tramitacao a, {0}lst_hst_tramitacao t where a.tid = t.tid and a.id = t.tramitacao_id and a.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						tramitacao.Id = id;

						if (reader["tramitacao_hst_id"] != null && !Convert.IsDBNull(reader["tramitacao_hst_id"]))
						{
							tramitacao.HistoricoId = Convert.ToInt32(reader["tramitacao_hst_id"]);
						}

						tramitacao.Tid = reader["tid"].ToString();
						tramitacao.DataEnvio.Data = Convert.IsDBNull(reader["data_envio"]) ? null : (DateTime?)Convert.ToDateTime(reader["data_envio"]);
						tramitacao.Despacho = reader["despacho"].ToString();
						tramitacao.Protocolo.NumeroProtocolo = Convert.ToInt32(reader["protocolo_numero"]);
						tramitacao.Protocolo.Ano = Convert.ToInt32(reader["protocolo_ano"]);
						tramitacao.Protocolo.Tipo.Texto = reader["protocolo_tipo_texto"].ToString();

						if (reader["protocolo_id"] != null && !Convert.IsDBNull(reader["protocolo_id"]))
						{
							tramitacao.Protocolo.Id = Convert.ToInt32(reader["protocolo_id"]);
							tramitacao.Protocolo.IsProcesso = reader["protocolo"].ToString()=="1";
						}

						if (reader["objetivo_id"] != null && !Convert.IsDBNull(reader["objetivo_id"]))
						{
							tramitacao.Objetivo.Id = Convert.ToInt32(reader["objetivo_id"]);
							tramitacao.Objetivo.Texto = reader["objetivo_texto"].ToString();
						}

						if (reader["remetente_id"] != null && !Convert.IsDBNull(reader["remetente_id"]))
						{
							tramitacao.Remetente.Id = Convert.ToInt32(reader["remetente_id"]);							
						}
						tramitacao.Remetente.Nome = reader["remetente_nome"].ToString();

						if (reader["destinatario_id"] != null && !Convert.IsDBNull(reader["destinatario_id"]))
						{
							tramitacao.Destinatario.Id = Convert.ToInt32(reader["destinatario_id"]);
						}
						tramitacao.Destinatario.Nome = reader["destinatario_nome"].ToString();

						if (reader["remetente_setor_id"] != null && !Convert.IsDBNull(reader["remetente_setor_id"]))
						{
							tramitacao.RemetenteSetor.Id = Convert.ToInt32(reader["remetente_setor_id"]);							
						}
						tramitacao.RemetenteSetor.Nome = reader["remetente_setor_nome"].ToString();

						if (reader["destinatario_setor_id"] != null && !Convert.IsDBNull(reader["destinatario_setor_id"]))
						{
							tramitacao.DestinatarioSetor.Id = Convert.ToInt32(reader["destinatario_setor_id"]);							
						}
						tramitacao.DestinatarioSetor.Nome = reader["destinatario_setor_nome"].ToString();

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							tramitacao.SituacaoId = Convert.ToInt32(reader["situacao_id"]);
						}

						if (reader["orgao_externo_id"] != null && !Convert.IsDBNull(reader["orgao_externo_id"]))
						{
							tramitacao.OrgaoExterno.Id = Convert.ToInt32(reader["orgao_externo_id"]);
						}
						tramitacao.OrgaoExterno.Texto = reader["orgao_externo_nome"].ToString();

						if (reader["executor"] != null && !Convert.IsDBNull(reader["executor"]))
						{
							tramitacao.Executor.Id = Convert.ToInt32(reader["executor"]);
						}
					}

					reader.Close();
				}

				#endregion
			}
			return tramitacao;
		}

		internal Tramitacao ObterHistorico(int id, BancoDeDados banco = null)
		{
			Tramitacao tramitacao = new Tramitacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Tramitacao

				Comando comando = bancoDeDados.CriarComando(@"select t.tramitacao_hst_id, t.tramitacao_id id, t.protocolo, t.protocolo_id, t.protocolo_tid, 
				t.protocolo_numero, t.protocolo_ano, t.protocolo_numero_completo, t.protocolo_numero_autuacao, t.protocolo_tipo_id, t.protocolo_tipo_texto, 
				t.remetente_id, t.remetente_tid, t.remetente_nome, t.remetente_setor_id, t.remetente_setor_sigla, t.remetente_setor_nome, t.destinatario_id, 
				t.destinatario_tid, t.destinatario_nome, t.destinatario_setor_id, t.destinatario_setor_sigla, t.destinatario_setor_nome, t.objetivo_id, 
				t.objetivo_texto, t.situacao_id, t.situacao_texto, t.data_execucao data_recebimento, t.data_envio, t.data_execucao, t.arquivo_nome, 
				t.prateleira_nome, t.despacho, t.remetente_municipio_nome, t.remetente_estado_sigla, t.destinatario_municipio_nome, t.destinatario_estado_sigla, 
				t.orgao_externo_id, t.orgao_externo_nome, t.tid from {0}lst_hst_tramitacao t where t.tramitacao_hst_id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						tramitacao.Id = id;

						if (reader["tramitacao_hst_id"] != null && !Convert.IsDBNull(reader["tramitacao_hst_id"]))
						{
							tramitacao.HistoricoId = Convert.ToInt32(reader["tramitacao_hst_id"]);
						}

						tramitacao.Tid = reader["tid"].ToString();
						tramitacao.DataEnvio.Data = Convert.IsDBNull(reader["data_envio"]) ? null : (DateTime?)Convert.ToDateTime(reader["data_envio"]);

						tramitacao.Despacho = reader["despacho"].ToString();
						tramitacao.Protocolo.NumeroProtocolo = Convert.ToInt32(reader["protocolo_numero"]);
						tramitacao.Protocolo.Ano = Convert.ToInt32(reader["protocolo_ano"]);
						tramitacao.Protocolo.Tipo.Texto = reader["protocolo_tipo_texto"].ToString();

						if (reader["objetivo_id"] != null && !Convert.IsDBNull(reader["objetivo_id"]))
						{
							tramitacao.Objetivo.Id = Convert.ToInt32(reader["objetivo_id"]);
							tramitacao.Objetivo.Texto = reader["objetivo_texto"].ToString();
						}

						if (reader["remetente_id"] != null && !Convert.IsDBNull(reader["remetente_id"]))
						{
							tramitacao.Remetente.Id = Convert.ToInt32(reader["remetente_id"]);
						}
						tramitacao.Remetente.Nome = reader["remetente_nome"].ToString();

						if (reader["destinatario_id"] != null && !Convert.IsDBNull(reader["destinatario_id"]))
						{
							tramitacao.Destinatario.Id = Convert.ToInt32(reader["destinatario_id"]);
						}
						tramitacao.Destinatario.Nome = reader["destinatario_nome"].ToString();

						if (reader["remetente_setor_id"] != null && !Convert.IsDBNull(reader["remetente_setor_id"]))
						{
							tramitacao.RemetenteSetor.Id = Convert.ToInt32(reader["remetente_setor_id"]);
						}
						tramitacao.RemetenteSetor.Nome = reader["remetente_setor_nome"].ToString();

						if (reader["destinatario_setor_id"] != null && !Convert.IsDBNull(reader["destinatario_setor_id"]))
						{
							tramitacao.DestinatarioSetor.Id = Convert.ToInt32(reader["destinatario_setor_id"]);
						}
						tramitacao.DestinatarioSetor.Nome = reader["destinatario_setor_nome"].ToString();

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							tramitacao.SituacaoId = Convert.ToInt32(reader["situacao_id"]);
						}

						if (reader["orgao_externo_id"] != null && !Convert.IsDBNull(reader["orgao_externo_id"]))
						{
							tramitacao.OrgaoExterno.Id = Convert.ToInt32(reader["orgao_externo_id"]);
						}
						tramitacao.OrgaoExterno.Texto = reader["orgao_externo_nome"].ToString();
					}

					reader.Close();
				}

				#endregion
			}
			return tramitacao;
		}

		internal TramitacaoPosse ObterProtocoloPosse(int protocoloId, BancoDeDados banco = null)
		{
			TramitacaoPosse posse = new TramitacaoPosse();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select f.id funcionario_id, f.nome funcionario_nome, s.id setor_id, s.nome setor_nome
				from {0}tab_protocolo p, {0}tab_funcionario f, {0}tab_setor s where f.id(+) = p.emposse and s.id = p.setor and p.id = :protocolo_id", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo_id", protocoloId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						posse.FuncionarioId = Convert.ToInt32(reader["funcionario_id"]);
						posse.FuncionarioNome = reader["funcionario_nome"].ToString();

						posse.SetorId = Convert.ToInt32(reader["setor_id"]);
						posse.SetorNome = reader["setor_nome"].ToString();
					}

					reader.Close();
				}
			}
			return posse;
		}

		internal List<Setor> ObterSetores(BancoDeDados banco = null)
		{
			List<Setor> setores = new List<Setor>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select s.id, ts.id IdRelacao, s.nome, s.sigla, s.responsavel, nvl(ts.tipo,1) tipo, s.tid from {0}tab_setor s, 
				{0}tab_tramitacao_setor ts where s.id = ts.setor(+) order by s.nome", EsquemaBanco);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Setor setor;

					while (reader.Read())
					{
						setor = new Setor();
						setor.Id = Convert.ToInt32(reader["id"]);
						setor.Nome = reader["nome"].ToString();
						setor.Sigla = reader["sigla"].ToString();
						setor.TramitacaoTipoId = Convert.ToInt32(reader["tipo"]);

						if (reader["IdRelacao"] != null && !Convert.IsDBNull(reader["IdRelacao"]))
						{
							setor.IdRelacao = Convert.ToInt32(reader["IdRelacao"]);
						}

						#region Funcionários

						comando = bancoDeDados.CriarComando(@"select s.id, s.setor, s.funcionario funcionario_id, f.nome funcionario_nome, s.tid 
						from {0}tab_tramitacao_setor_func s, {0}tab_funcionario f where s.funcionario = f.id and s.setor = :setor", EsquemaBanco);

						comando.AdicionarParametroEntrada("setor", setor.Id, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							FuncionarioLst funcionario;
							while (readerAux.Read())
							{
								funcionario = new FuncionarioLst();
								funcionario.Id = Convert.ToInt32(readerAux["funcionario_id"]);
								funcionario.Texto = readerAux["funcionario_nome"].ToString();
								setor.Funcionarios.Add(funcionario);
							}
							readerAux.Close();
						}

						#endregion

						setores.Add(setor);
					}
					reader.Close();

					#endregion
				}
			}

			return setores;
		}

		internal List<FuncionarioLst> ObterFuncionariosRegistrador(int funcionarioId, BancoDeDados banco = null)
		{
			List<FuncionarioLst> funcionarios = new List<FuncionarioLst>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Funcionarios dos setores onde o usuário logado é registrador

				Comando comando = bancoDeDados.CriarComando(@"select distinct tf.id, tf.nome from tab_tramitacao_setor_func t, tab_funcionario_setor tfs,
					tab_funcionario tf where t.setor = tfs.setor and tfs.funcionario = tf.id and t.funcionario = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", funcionarioId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					FuncionarioLst funcionario;

					while (reader.Read())
					{
						funcionario = new FuncionarioLst();
						funcionario.Id = Convert.ToInt32(reader["id"]);
						funcionario.Texto = reader["nome"].ToString();
						funcionario.IsAtivo = true;
						funcionarios.Add(funcionario);
					}
					reader.Close();
				}
				#endregion
			}
			return funcionarios;
		}

		public int ObterTipoSetor(int setorId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.tipo from {0}tab_tramitacao_setor t where t.setor = :setor", EsquemaBanco);

				comando.AdicionarParametroEntrada("setor", setorId, DbType.Int32);

				object resultado = bancoDeDados.ExecutarScalar(comando);

				if (resultado != null && !Convert.IsDBNull(resultado))
				{
					return Convert.ToInt32(resultado);
				}
			}
			return 0;
		}

		internal int ObterSetorProtocolo(int protocolo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.setor from {0}tab_protocolo t where t.id = :protocolo", EsquemaBanco);
				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);

				object resultado = bancoDeDados.ExecutarScalar(comando);

				if (resultado != null && !Convert.IsDBNull(resultado))
				{
					return Convert.ToInt32(resultado);
				}
			}
			return 0;
		}

		internal int ObterFuncionarioIdPosse(int protocolo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.emposse from {0}tab_protocolo t where t.id = :protocolo", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);

				object resultado = bancoDeDados.ExecutarScalar(comando);

				if (resultado != null && !Convert.IsDBNull(resultado))
				{
					return Convert.ToInt32(resultado);
				}
			}
			return 0;
		}

		internal List<Motivo> ObterMotivos()
		{
			List<Motivo> lst = new List<Motivo>();

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto, t.ativo from tab_tramitacao_motivo t order by t.texto");
			foreach (var item in daReader)
			{
				lst.Add(new Motivo()
				{
					Id = Convert.ToInt32(item["id"]),
					Nome = item["texto"].ToString(),
					IsAtivo = Convert.ToBoolean(item["ativo"]),
				});
			}

			return lst;
		}

		#endregion

		#region Validações

		internal int ExisteTramitacao(int protocolo, BancoDeDados banco = null)
		{
			int retorno = 0;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select a.id from {0}tab_tramitacao a where a.protocolo = :protocolo", EsquemaBanco);
				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);

				object obj = bancoDeDados.ExecutarScalar(comando);

				if (obj != null && !Convert.IsDBNull(obj))
				{
					retorno = Convert.ToInt32(obj);
				}
			}
			return retorno;
		}

		internal int ExisteTramitacao(int protocolo, int situacao, BancoDeDados banco = null)
		{
			int retorno = 0;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select a.id from {0}tab_tramitacao a where a.protocolo = :protocolo and a.situacao = :situacao", EsquemaBanco);
				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", situacao, DbType.Int32);

				object obj = bancoDeDados.ExecutarScalar(comando);

				if (obj != null && !Convert.IsDBNull(obj))
				{
					retorno = Convert.ToInt32(obj);
				}
			}
			return retorno;
		}

		internal bool Registrador(int funcionarioId, int setorId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_tramitacao_setor_func t where t.setor = :setor and t.funcionario = :funcionario", EsquemaBanco);

				comando.AdicionarParametroEntrada("setor", setorId, DbType.Int32);
				comando.AdicionarParametroEntrada("funcionario", funcionarioId, DbType.Int32);

				object resultado = bancoDeDados.ExecutarScalar(comando);

				if (resultado != null && !Convert.IsDBNull(resultado))
				{
					return Convert.ToBoolean(resultado);
				}
			}
			return false;
		}

		internal bool Registrador(int funcionarioId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_tramitacao_setor_func t where t.funcionario = :funcionario", EsquemaBanco);

				comando.AdicionarParametroEntrada("funcionario", funcionarioId, DbType.Int32);

				object resultado = bancoDeDados.ExecutarScalar(comando);

				if (resultado != null && !Convert.IsDBNull(resultado))
				{
					return Convert.ToInt32(resultado) > 0;
				}
			}
			return false;
		}

		internal bool SetorPorRegistrado(int setorId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_tramitacao_setor t where t.tipo = 2 and t.setor = :setor", EsquemaBanco);

				comando.AdicionarParametroEntrada("setor", setorId, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		internal bool ProtocoloMesmoSetorSelecionado(Protocolo protocolo, int funcSetorId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_protocolo p where p.id = :protocolo and p.setor = :setor", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("setor", funcSetorId, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}			
		}

		internal bool ExisteArquivadoEmArquivo(int arquivoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_tramitacao_arquivar where arquivo = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", arquivoId, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public Protocolo ExisteProtocolo(string numero, int excetoId = 0)
		{
			Protocolo protocolo = new Protocolo();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.id, p.protocolo isprocesso from {0}tab_protocolo p where p.numero = :numero and p.ano = :ano and p.id != :exceto_id", EsquemaBanco);

				ProtocoloNumero protocoloNum = new ProtocoloNumero(numero);
				comando.AdicionarParametroEntrada("numero", protocoloNum.Numero, DbType.Int32);
				comando.AdicionarParametroEntrada("ano", protocoloNum.Ano, DbType.Int32);
				comando.AdicionarParametroEntrada("exceto_id", excetoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						protocolo.Id = Convert.ToInt32(reader["id"]);
						protocolo.IsProcesso = Convert.ToInt32(reader["isprocesso"]) == 1;
						protocolo.NumeroProtocolo = protocoloNum.Numero;
						protocolo.Ano = protocoloNum.Ano;
					}

					reader.Close();
				}
			}
			return protocolo;
		}

		internal bool ExisteMotivo(Motivo motivo)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_tramitacao_motivo t where lower(t.texto) = :motivoTexto and t.id <> :motivoId", EsquemaBanco);

				comando.AdicionarParametroEntrada("motivoId", motivo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("motivoTexto", motivo.Nome.ToLower(), DbType.String);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal bool NotificacaoIsValida(int protocolo)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_protocolo p
															where p.id = :protocolo
															and
															(
																p.fiscalizacao is null
																or exists
																(
																	select 1 from tab_fiscalizacao f
																	where f.id = p.fiscalizacao
																	and 
																	(
																		not exists
																		(
																			select 1 from tab_fisc_multa m
																			where f.id = m.fiscalizacao
																		)
																		or exists
																		(
																			select 1 from tab_fisc_notificacao n
																			where f.id = n.fiscalizacao
																		)
																	)  
																)
															)", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		#endregion
	}
}