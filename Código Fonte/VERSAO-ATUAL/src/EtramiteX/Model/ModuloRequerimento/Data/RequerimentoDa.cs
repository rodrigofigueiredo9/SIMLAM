using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloRequerimento.Data
{
	public class RequerimentoDa
	{
		#region Requerimento

		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();
		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }
		private string EsquemaBanco { get; set; }
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		public Int32 RoteiroPadrao
		{
			get { return _configSys.Obter<Int32>(ConfiguracaoSistema.KeyRoteiroPadrao); }
		}

		#endregion

		public RequerimentoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		public void Salvar(Requerimento requerimento, BancoDeDados banco = null)
		{
			if (requerimento == null)
			{
				throw new Exception("Requerimento é nulo.");
			}

			if (requerimento.Id <= 0)
			{
				Criar(requerimento, banco);
			}
			else
			{
				Editar(requerimento, banco);
			}
		}

		internal int? Criar(Requerimento requerimento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Requerimento

				Comando comando;

				bancoDeDados.IniciarTransacao();

				comando = bancoDeDados.CriarComando(@"insert into {0}tab_requerimento e (id, numero, data_criacao, situacao, tid, agendamento, setor, informacoes) values 
				({0}seq_requerimento.nextval, {0}seq_requerimento.currval, sysdate, :situacao, :tid, :agendamento, :setor, :informacoes) returning e.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("situacao", 1, DbType.Int32); // 1 - Em andamento
				comando.AdicionarParametroEntrada("agendamento", requerimento.AgendamentoVistoria, DbType.Int32);
				comando.AdicionarParametroEntrada("setor", requerimento.SetorId, DbType.Int32);
				comando.AdicionarParametroEntrada("informacoes", DbType.String, 500, requerimento.Informacoes);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				requerimento.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Atividades

				if (requerimento.Atividades != null && requerimento.Atividades.Count > 0)
				{
					foreach (Atividade item in requerimento.Atividades)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_requerimento_atividade t (id, requerimento, atividade, tid)
						values ({0}seq_requerimento_atividade.nextval, :requerimento, :atividade, :tid) returning t.id into :id", EsquemaBanco);
						comando.AdicionarParametroSaida("id", DbType.Int32);

						comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("atividade", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						item.IdRelacionamento = Convert.ToInt32(comando.ObterValorParametro("id"));

						#region Modelo de Título

						if (item.Finalidades.Count > 0)
						{
							foreach (Finalidade itemAux in item.Finalidades)
							{
								comando = bancoDeDados.CriarComando(@"insert into {0}tab_requerimento_ativ_finalida (id, requerimento_ativ, modelo,
								titulo_anterior_tipo, titulo_anterior_id, titulo_anterior_numero, modelo_anterior_id, modelo_anterior_nome, modelo_anterior_sigla, orgao_expedidor, finalidade, tid) values 
								({0}seq_requerimento_ativ_fin.nextval, :requerimento_ativ, :modelo, :titulo_anterior_tipo, :titulo_anterior_id, :titulo_anterior_numero, :modelo_anterior_id, 
								:modelo_anterior_nome, :modelo_anterior_sigla, :orgao_expedidor, :finalidade, :tid)", EsquemaBanco);

								comando.AdicionarParametroEntrada("requerimento_ativ", item.IdRelacionamento, DbType.Int32);
								comando.AdicionarParametroEntrada("modelo", itemAux.TituloModelo, DbType.Int32);
								comando.AdicionarParametroEntrada("titulo_anterior_tipo", itemAux.TituloAnteriorTipo, DbType.Int32);
								comando.AdicionarParametroEntrada("titulo_anterior_id", itemAux.TituloAnteriorId, DbType.Int32);
								comando.AdicionarParametroEntrada("titulo_anterior_numero", DbType.String, 20, itemAux.TituloAnteriorNumero);
								comando.AdicionarParametroEntrada("modelo_anterior_id", itemAux.TituloModeloAnteriorId, DbType.Int32);
								comando.AdicionarParametroEntrada("modelo_anterior_nome", DbType.String, 100, itemAux.TituloModeloAnteriorTexto);
								comando.AdicionarParametroEntrada("modelo_anterior_sigla", DbType.String, 100, itemAux.TituloModeloAnteriorSigla);
								comando.AdicionarParametroEntrada("orgao_expedidor", DbType.String, 100, itemAux.OrgaoExpedidor);
								comando.AdicionarParametroEntrada("finalidade", itemAux.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
								comando.AdicionarParametroEntrada("modelo_anterior_sigla", DbType.String, 100, itemAux.TituloModeloAnteriorSigla);

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}

						#endregion
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(requerimento.Id, eHistoricoArtefato.requerimento, eHistoricoAcao.criar, bancoDeDados);

				Consulta.Gerar(requerimento.Id, eHistoricoArtefato.requerimento, bancoDeDados);

				#endregion

				bancoDeDados.Commit();

				return requerimento.Id;
			}
		}

		internal void Editar(Requerimento requerimento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Requerimento

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_requerimento r set r.interessado = :interessado, r.empreendimento = :empreendimento, 
				r.situacao = :situacao, r.tid = :tid, r.agendamento = :agendamento, r.informacoes = :informacoes where r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", requerimento.Id, DbType.Int32);

				if (requerimento.Interessado.Id > 0)
				{
					comando.AdicionarParametroEntrada("interessado", requerimento.Interessado.Id, DbType.Int32);
				}
				else
				{
					comando.AdicionarParametroEntrada("interessado", DBNull.Value, DbType.Int32);
				}

				if (requerimento.Empreendimento.Id > 0)
				{
					comando.AdicionarParametroEntrada("empreendimento", requerimento.Empreendimento.Id, DbType.Int32);
				}
				else
				{
					comando.AdicionarParametroEntrada("empreendimento", DBNull.Value, DbType.Int32);
				}

				comando.AdicionarParametroEntrada("agendamento", requerimento.AgendamentoVistoria, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", requerimento.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("informacoes", DbType.String, 500, requerimento.Informacoes);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco

				#region Atividade/Modelos

				comando = bancoDeDados.CriarComando(@"delete from {0}tab_requerimento_ativ_finalida c 
				where c.requerimento_ativ in (select a.id from {0}tab_requerimento_atividade a where a.requerimento = :requerimento ", EsquemaBanco);

				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "a.atividade", DbType.Int32, requerimento.Atividades.Select(x => x.Id).ToList());

				comando.DbCommand.CommandText += ")";

				comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				if (requerimento.Atividades != null && requerimento.Atividades.Count > 0)
				{
					foreach (Atividade item in requerimento.Atividades)
					{
						comando = bancoDeDados.CriarComando(@"delete from {0}tab_requerimento_ativ_finalida c 
						where c.requerimento_ativ in (select a.id from {0}tab_requerimento_atividade a where a.requerimento = :requerimento and a.atividade = :atividade)", EsquemaBanco);
						comando.DbCommand.CommandText += String.Format(" {0}", comando.AdicionarNotIn("and", "c.id", DbType.Int32, item.Finalidades.Select(x => x.IdRelacionamento).ToList()));

						comando.AdicionarParametroEntrada("atividade", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				//Atividades
				comando = bancoDeDados.CriarComando("delete from {0}tab_requerimento_atividade c ", EsquemaBanco);

				comando.DbCommand.CommandText += String.Format("where c.requerimento = :requerimento{0}",
				comando.AdicionarNotIn("and", "c.id", DbType.Int32, requerimento.Atividades.Select(x => x.IdRelacionamento).ToList()));

				comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				//Responsáveis
				comando = bancoDeDados.CriarComando("delete from {0}tab_requerimento_responsavel c ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where c.requerimento = :requerimento{0}",
				comando.AdicionarNotIn("and", "c.id", DbType.Int32, requerimento.Responsaveis.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Atividades
				if (requerimento.Atividades != null && requerimento.Atividades.Count > 0)
				{
					foreach (Atividade item in requerimento.Atividades)
					{
						if (item.IdRelacionamento > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_requerimento_atividade e set e.requerimento = :requerimento, e.atividade = :atividade, e.tid =:tid where e.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_requerimento_atividade t (id, requerimento, atividade, tid)
							values ({0}seq_requerimento_atividade.nextval, :requerimento, :atividade, :tid) returning t.id into :id", EsquemaBanco);
							comando.AdicionarParametroSaida("id", DbType.Int32);
						}

						comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("atividade", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						if (item.IdRelacionamento <= 0)
						{
							item.IdRelacionamento = Convert.ToInt32(comando.ObterValorParametro("id"));
						}

						#region Modelo de Título

						if (item.Finalidades != null)
						{
							foreach (Finalidade itemAux in item.Finalidades)
							{
								if (itemAux.IdRelacionamento > 0)
								{
									comando = bancoDeDados.CriarComando(@"update {0}tab_requerimento_ativ_finalida a set requerimento_ativ = :requerimento_ativ,
									modelo = :modelo, titulo_anterior_tipo = :titulo_anterior_tipo, titulo_anterior_id = :titulo_anterior_id, titulo_anterior_numero = :titulo_anterior_numero, 
									modelo_anterior_id = :modelo_anterior_id, modelo_anterior_nome = :modelo_anterior_nome, modelo_anterior_sigla = :modelo_anterior_sigla, orgao_expedidor = :orgao_expedidor, finalidade = :finalidade, 
									tid = :tid where id = :id", EsquemaBanco);
									comando.AdicionarParametroEntrada("id", itemAux.IdRelacionamento, DbType.Int32);
								}
								else
								{
									comando = bancoDeDados.CriarComando(@"insert into {0}tab_requerimento_ativ_finalida (id, requerimento_ativ, modelo,
									titulo_anterior_tipo, titulo_anterior_id, titulo_anterior_numero, modelo_anterior_id, modelo_anterior_nome, modelo_anterior_sigla, orgao_expedidor, finalidade, tid) values 
									({0}seq_requerimento_ativ_fin.nextval, :requerimento_ativ, :modelo, :titulo_anterior_tipo, :titulo_anterior_id, :titulo_anterior_numero, :modelo_anterior_id, 
									:modelo_anterior_nome, :modelo_anterior_sigla, :orgao_expedidor, :finalidade, :tid)", EsquemaBanco);
								}

								comando.AdicionarParametroEntrada("finalidade", itemAux.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("requerimento_ativ", item.IdRelacionamento, DbType.Int32);
								comando.AdicionarParametroEntrada("modelo", itemAux.TituloModelo, DbType.Int32);
								comando.AdicionarParametroEntrada("titulo_anterior_tipo", itemAux.TituloAnteriorTipo, DbType.Int32);
								comando.AdicionarParametroEntrada("titulo_anterior_id", itemAux.TituloAnteriorId, DbType.Int32);
								comando.AdicionarParametroEntrada("titulo_anterior_numero", DbType.String, 20, itemAux.TituloAnteriorNumero);
								comando.AdicionarParametroEntrada("modelo_anterior_id", itemAux.TituloModeloAnteriorId, DbType.Int32);
								comando.AdicionarParametroEntrada("modelo_anterior_nome", DbType.String, 100, itemAux.TituloModeloAnteriorTexto);
								comando.AdicionarParametroEntrada("modelo_anterior_sigla", DbType.String, 100, itemAux.TituloModeloAnteriorSigla);
								comando.AdicionarParametroEntrada("orgao_expedidor", DbType.String, 100, itemAux.OrgaoExpedidor);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
								bancoDeDados.ExecutarNonQuery(comando);
							}
						}
						#endregion
					}
				}

				#endregion

				#region Responsáveis

				if (requerimento.Responsaveis != null && requerimento.Responsaveis.Count > 0)
				{
					foreach (ResponsavelTecnico responsavel in requerimento.Responsaveis)
					{
						if (responsavel.IdRelacionamento > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_requerimento_responsavel r set r.requerimento = :requerimento, r.responsavel = :responsavel, 
							r.funcao = :funcao, r.numero_art = :numero_art, r.tid = :tid where r.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", responsavel.IdRelacionamento, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_requerimento_responsavel(id, requerimento, responsavel, funcao, numero_art, tid) values
							({0}seq_requerimento_responsavel.nextval, :requerimento, :responsavel, :funcao, :numero_art, :tid )", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("responsavel", responsavel.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("funcao", responsavel.Funcao, DbType.Int32);
						comando.AdicionarParametroEntrada("numero_art", responsavel.NumeroArt, DbType.String);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());


						bancoDeDados.ExecutarNonQuery(comando);
					}
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"delete {0}tab_requerimento_responsavel p where p.requerimento = :id", EsquemaBanco);
					comando.AdicionarParametroEntrada("id", requerimento.Id, DbType.Int32);
					bancoDeDados.ExecutarNonQuery(comando);
				}
				#endregion

				#region Histórico

				Historico.Gerar(requerimento.Id, eHistoricoArtefato.requerimento, eHistoricoAcao.atualizar, bancoDeDados);
				Consulta.Gerar(requerimento.Id, eHistoricoArtefato.requerimento, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal int? Importar(Requerimento requerimento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Requerimento

				Comando comando;

				bancoDeDados.IniciarTransacao();

				comando = bancoDeDados.CriarComando(@"insert into {0}tab_requerimento e (id, numero, data_criacao, situacao, tid, agendamento, setor, interessado, empreendimento, informacoes, autor) 
				values (:requerimento, :requerimento, sysdate, :situacao, :tid, :agendamento, :setor, :interessado, :empreendimento, :informacoes, :autor)", EsquemaBanco);

				comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", (int)eRequerimentoSituacao.Finalizado, DbType.Int32); // 1 - Finalizado
				comando.AdicionarParametroEntrada("agendamento", requerimento.AgendamentoVistoria, DbType.Int32);
				comando.AdicionarParametroEntrada("setor", requerimento.SetorId, DbType.Int32);
				comando.AdicionarParametroEntrada("interessado", requerimento.Interessado.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento", (requerimento.Empreendimento.Id > 0) ? (object)requerimento.Empreendimento.Id : DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("informacoes", DbType.String, 500, requerimento.Informacoes);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("autor", requerimento.CredenciadoId, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Atividades

				if (requerimento.Atividades != null && requerimento.Atividades.Count > 0)
				{
					foreach (Atividade item in requerimento.Atividades)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_requerimento_atividade t (id, requerimento, atividade, tid)
						values ({0}seq_requerimento_atividade.nextval, :requerimento, :atividade, :tid) returning t.id into :id", EsquemaBanco);
						comando.AdicionarParametroSaida("id", DbType.Int32);

						comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("atividade", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						item.IdRelacionamento = Convert.ToInt32(comando.ObterValorParametro("id"));

						#region Modelo de Título

						if (item.Finalidades.Count > 0)
						{
							foreach (Finalidade itemAux in item.Finalidades)
							{
								comando = bancoDeDados.CriarComando(@"insert into {0}tab_requerimento_ativ_finalida (id, requerimento_ativ, modelo,
								titulo_anterior_tipo, titulo_anterior_id, titulo_anterior_numero, modelo_anterior_id, modelo_anterior_nome, modelo_anterior_sigla, orgao_expedidor, finalidade, tid) values 
								({0}seq_requerimento_ativ_fin.nextval, :requerimento_ativ, :modelo, :titulo_anterior_tipo, :titulo_anterior_id, :titulo_anterior_numero, :modelo_anterior_id, 
								:modelo_anterior_nome, :modelo_anterior_sigla, :orgao_expedidor, :finalidade, :tid)", EsquemaBanco);

								comando.AdicionarParametroEntrada("requerimento_ativ", item.IdRelacionamento, DbType.Int32);
								comando.AdicionarParametroEntrada("modelo", itemAux.TituloModelo, DbType.Int32);
								comando.AdicionarParametroEntrada("titulo_anterior_tipo", itemAux.TituloAnteriorTipo, DbType.Int32);
								comando.AdicionarParametroEntrada("titulo_anterior_id", itemAux.TituloAnteriorId, DbType.Int32);
								comando.AdicionarParametroEntrada("titulo_anterior_numero", DbType.String, 20, itemAux.TituloAnteriorNumero);
								comando.AdicionarParametroEntrada("modelo_anterior_id", itemAux.TituloModeloAnteriorId, DbType.Int32);
								comando.AdicionarParametroEntrada("modelo_anterior_nome", DbType.String, 100, itemAux.TituloModeloAnteriorTexto);
								comando.AdicionarParametroEntrada("modelo_anterior_sigla", DbType.String, 100, itemAux.TituloModeloAnteriorSigla);
								comando.AdicionarParametroEntrada("orgao_expedidor", DbType.String, 100, itemAux.OrgaoExpedidor);
								comando.AdicionarParametroEntrada("finalidade", itemAux.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
								comando.AdicionarParametroEntrada("modelo_anterior_sigla", DbType.String, 100, itemAux.TituloModeloAnteriorSigla);

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}

						#endregion
					}
				}

				#endregion

				#region Responsáveis

				if (requerimento.Responsaveis != null && requerimento.Responsaveis.Count > 0)
				{
					foreach (ResponsavelTecnico responsavel in requerimento.Responsaveis)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_requerimento_responsavel(id, requerimento, responsavel, funcao, numero_art, tid) values
						({0}seq_requerimento_responsavel.nextval, :requerimento, :responsavel, :funcao, :numero_art, :tid )", EsquemaBanco);

						comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("responsavel", responsavel.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("funcao", responsavel.Funcao, DbType.Int32);
						comando.AdicionarParametroEntrada("numero_art", responsavel.NumeroArt, DbType.String);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"delete {0}tab_requerimento_responsavel p where p.requerimento = :id", EsquemaBanco);
					comando.AdicionarParametroEntrada("id", requerimento.Id, DbType.Int32);
					bancoDeDados.ExecutarNonQuery(comando);
				}
				#endregion

				#region Histórico

				Historico.Gerar(requerimento.Id, eHistoricoArtefato.requerimento, eHistoricoAcao.importar, bancoDeDados);

				Consulta.Gerar(requerimento.Id, eHistoricoArtefato.requerimento, bancoDeDados);

				#endregion

				bancoDeDados.Commit();

				return requerimento.Id;
			}
		}

		internal void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_requerimento c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Histórico

				Historico.Gerar(id, eHistoricoArtefato.requerimento, eHistoricoAcao.excluir, bancoDeDados);
				Consulta.Deletar(id, eHistoricoArtefato.requerimento, bancoDeDados);

				#endregion

				#region Apaga os dados de requerimento

				List<String> lista = new List<string>();

				lista.Add(@"delete from {0}tab_requerimento_ativ_finalida a where a.requerimento_ativ in 
								(select b.id from {0}tab_requerimento_atividade b where b.requerimento = :requerimento);");
				lista.Add("delete from {0}tab_requerimento_atividade b where b.requerimento = :requerimento;");
				lista.Add("delete from {0}tab_requerimento_responsavel c where c.requerimento = :requerimento;");
				lista.Add("delete from {0}tab_requerimento e where e.id = :requerimento;");

				comando = bancoDeDados.CriarComando(@" begin " + string.Join(" ", lista) + " end;", EsquemaBanco);
				comando.AdicionarParametroEntrada("requerimento", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter / Filtrar

		internal int ObterNovoID(BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando("select {0}seq_requerimento.nextval from dual", EsquemaBanco);
				return bancoDeDados.ExecutarScalar<int>(comando);
			}
		}

		internal Requerimento Obter(int id, BancoDeDados banco = null)
		{
			Requerimento requerimento = new Requerimento();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Requerimento

				Comando comando = bancoDeDados.CriarComando(@"
											select r.id,
													r.numero,
													trunc(r.data_criacao) data_criacao,
													r.interessado,
													r.tid,
													nvl(p.nome, p.razao_social) interessado_nome,
													nvl(p.cpf, p.cnpj) interessado_cpf_cnpj,
													p.tipo interessado_tipo,
													r.empreendimento,
													e.cnpj empreendimento_cnpj,
													e.denominador,
													e.codigo,
													r.situacao,
													ls.texto situacao_texto,
													r.agendamento,
													r.setor,
													r.informacoes,
													r.autor,
													(select etapa_importacao from tmp_projeto_digital where requerimento_id = r.id) etapa_importacao
												from {0}tab_requerimento          r,
													{0}tab_pessoa                p,
													{0}tab_empreendimento        e,
													{0}lov_requerimento_situacao ls
												where r.interessado = p.id(+)
												and r.empreendimento = e.id(+)
												and r.situacao = ls.id
												and r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						requerimento.Id = id;
						requerimento.Tid = reader["tid"].ToString();
						requerimento.DataCadastro = reader.GetValue<DateTime>("data_criacao");
						requerimento.SetorId = reader.GetValue<int>("setor");
						requerimento.AutorId = reader.GetValue<int>("autor");
						requerimento.AgendamentoVistoria = reader.GetValue<int>("agendamento");
						requerimento.Interessado.Id = reader.GetValue<int>("interessado");
						requerimento.Interessado.Tipo = reader.GetValue<int>("interessado_tipo");
						requerimento.EtapaImportacao = reader.GetValue<int>("etapa_importacao");

						if (requerimento.Interessado.Tipo == PessoaTipo.FISICA)
						{
							requerimento.Interessado.Fisica.Nome = reader.GetValue<string>("interessado_nome");
							requerimento.Interessado.Fisica.CPF = reader.GetValue<string>("interessado_cpf_cnpj");
						}
						else
						{
							requerimento.Interessado.Juridica.RazaoSocial = reader.GetValue<string>("interessado_nome");
							requerimento.Interessado.Juridica.CNPJ = reader.GetValue<string>("interessado_cpf_cnpj");
						}

						requerimento.Empreendimento.Id = reader.GetValue<int>("empreendimento");
						requerimento.Empreendimento.Codigo = reader.GetValue<int>("codigo");
						requerimento.Empreendimento.Denominador = reader.GetValue<string>("denominador");
						requerimento.Empreendimento.CNPJ = reader.GetValue<string>("empreendimento_cnpj");
						requerimento.SituacaoId = reader.GetValue<int>("situacao");
						requerimento.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						requerimento.Informacoes = reader["informacoes"].ToString();
					}

					reader.Close();
				}

				#endregion

				#region Atividades

				comando = bancoDeDados.CriarComando(@"select a.id, a.atividade, a.tid, b.atividade atividade_texto, b.setor, 
													b.situacao from {0}tab_requerimento_atividade a, {0}tab_atividade b
													where a.atividade = b.id and a.requerimento = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Atividade atividade;

					while (reader.Read())
					{
						atividade = new Atividade();
						atividade.Id = Convert.ToInt32(reader["atividade"]);
						atividade.IdRelacionamento = Convert.ToInt32(reader["id"]);
						atividade.Tid = reader["tid"].ToString();
						atividade.NomeAtividade = reader["atividade_texto"].ToString();
						atividade.SetorId = Convert.ToInt32(reader["setor"]);
						atividade.SituacaoId = Convert.ToInt32(reader["situacao"]);

						#region Atividades/Finalidades/Modelos
						comando = bancoDeDados.CriarComando(@"
											select a.id,
													a.finalidade,
													ltf.texto                finalidade_texto,
													a.modelo,
													tm.nome                  modelo_nome,
													a.titulo_anterior_tipo,
													a.titulo_anterior_id,
													a.titulo_anterior_numero,
													a.modelo_anterior_id,
													a.modelo_anterior_nome,
													a.modelo_anterior_sigla,
													a.orgao_expedidor,
													ltf.codigo
												from tab_requerimento_ativ_finalida a,
													tab_titulo_modelo              tm,
													lov_titulo_finalidade          ltf
												where a.modelo = tm.id
												and a.finalidade = ltf.id(+)
												and a.requerimento_ativ = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", atividade.IdRelacionamento, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							Finalidade fin;

							while (readerAux.Read())
							{
								fin = new Finalidade();

								fin.IdRelacionamento = Convert.ToInt32(readerAux["id"]);

								fin.Codigo = Convert.ToInt32(readerAux["codigo"]);

								fin.OrgaoExpedidor = readerAux["orgao_expedidor"].ToString();

								if (readerAux["finalidade"] != DBNull.Value)
								{
									fin.Id = Convert.ToInt32(readerAux["finalidade"]);
									fin.Texto = readerAux["finalidade_texto"].ToString();
								}

								if (readerAux["modelo"] != DBNull.Value)
								{
									fin.TituloModelo = Convert.ToInt32(readerAux["modelo"]);
									fin.TituloModeloTexto = readerAux["modelo_nome"].ToString();
								}

								if (readerAux["modelo_anterior_id"] != DBNull.Value)
								{
									fin.TituloModeloAnteriorId = Convert.ToInt32(readerAux["modelo_anterior_id"]);
								}

								fin.TituloModeloAnteriorTexto = readerAux["modelo_anterior_nome"].ToString();
								fin.TituloModeloAnteriorSigla = readerAux["modelo_anterior_sigla"].ToString();

								if (readerAux["titulo_anterior_tipo"] != DBNull.Value)
								{
									fin.TituloAnteriorTipo = Convert.ToInt32(readerAux["titulo_anterior_tipo"]);
								}

								if (readerAux["titulo_anterior_id"] != DBNull.Value)
								{
									fin.TituloAnteriorId = Convert.ToInt32(readerAux["titulo_anterior_id"]);
								}

								fin.TituloAnteriorNumero = readerAux["titulo_anterior_numero"].ToString();
								fin.EmitidoPorInterno = (fin.TituloAnteriorTipo != 3);
								atividade.Finalidades.Add(fin);
							}
							readerAux.Close();
						}
						#endregion

						requerimento.Atividades.Add(atividade);
					}

					reader.Close();
				}

				#endregion

				#region Responsáveis

				comando = bancoDeDados.CriarComando(@"select pr.id, pr.responsavel, pr.funcao, nvl(p.nome, p.razao_social) nome, pr.numero_art, 
				nvl(p.cpf, p.cnpj) cpf_cnpj, p.tipo from {0}tab_requerimento_responsavel pr, {0}tab_pessoa p where pr.responsavel = p.id and pr.requerimento = :requerimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ResponsavelTecnico responsavel;
					while (reader.Read())
					{
						responsavel = new ResponsavelTecnico();
						responsavel.IdRelacionamento = Convert.ToInt32(reader["id"]);
						responsavel.Id = Convert.ToInt32(reader["responsavel"]);
						responsavel.Funcao = Convert.ToInt32(reader["funcao"]);
						responsavel.CpfCnpj = reader["cpf_cnpj"].ToString();
						responsavel.NomeRazao = reader["nome"].ToString();
						responsavel.NumeroArt = reader["numero_art"].ToString();
						requerimento.Responsaveis.Add(responsavel);
					}
					reader.Close();
				}

				#endregion
			}

			return requerimento;
		}

		public Requerimento ObterSimplificado(int id, BancoDeDados banco = null)
		{
			Requerimento requerimento = new Requerimento();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Requerimento
				Comando comando = bancoDeDados.CriarComando(@"
												select r.id,
														r.numero,
														trunc(r.data_criacao) data_criacao,
														r.interessado,
														nvl(p.nome, p.razao_social) interessado_nome,
														p.tipo interessado_tipo,
														r.empreendimento,
														e.denominador,
														r.situacao,
														ls.texto situacao_texto,
														r.agendamento,
														r.autor,
                                                        e.nome_fantasia
													from {0}tab_requerimento          r,
														{0}tab_pessoa                p,
														{0}tab_empreendimento        e,
														{0}lov_requerimento_situacao ls
													where r.interessado = p.id(+)
													and r.empreendimento = e.id(+)
													and r.situacao = ls.id
													and r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						requerimento.Id = id;
						requerimento.DataCadastro = Convert.ToDateTime(reader["data_criacao"]);
						requerimento.AutorId = reader.GetValue<int>("autor");

						if (reader["agendamento"] != null && !Convert.IsDBNull(reader["agendamento"]))
						{
							requerimento.AgendamentoVistoria = Convert.ToInt32(reader["agendamento"]);
						}

						if (reader["interessado"] != null && !Convert.IsDBNull(reader["interessado"]))
						{
							requerimento.Interessado.Id = Convert.ToInt32(reader["interessado"]);

							if (reader["interessado_tipo"].ToString() == "1")
							{
								requerimento.Interessado.Fisica.Nome = reader["interessado_nome"].ToString();
							}
							else
							{
								requerimento.Interessado.Juridica.RazaoSocial = reader["interessado_nome"].ToString();
							}
						}

						if (reader["empreendimento"] != null && !Convert.IsDBNull(reader["empreendimento"]))
						{
							requerimento.Empreendimento.Id = Convert.ToInt32(reader["empreendimento"]);
							requerimento.Empreendimento.Denominador = reader["denominador"].ToString();
                            requerimento.Empreendimento.NomeFantasia = reader["nome_fantasia"].ToString();
						}

						if (reader["situacao"] != null && !Convert.IsDBNull(reader["situacao"]))
						{
							requerimento.SituacaoId = Convert.ToInt32(reader["situacao"]);
							requerimento.SituacaoTexto = reader["situacao_texto"].ToString();
						}
					}
					reader.Close();
				}
				#endregion
			}
			return requerimento;
		}

		internal Resultados<Requerimento> Filtrar(Filtro<RequerimentoListarFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<Requerimento> retorno = new Resultados<Requerimento>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("e.numero", "numero", filtros.Dados.Numero);

				comandtxt += comando.FiltroAndLike("e.interessado_cpf_cnpj", "interessado_cpf_cnpj", filtros.Dados.InteressadoCpfCnpj);

				comandtxt += comando.FiltroAndLike("e.interessado_nome_razao", "interessado_nome_razao", filtros.Dados.InteressadoNomeRazao, true);

				comandtxt += comando.FiltroAndLike("e.empreendimento_cnpj", "empreendimento_cnpj", filtros.Dados.EmpreendimentoCnpj);

				comandtxt += comando.FiltroAndLike("e.empreendimento_denominador", "empreendimento_denominador", filtros.Dados.EmpreendimentoDenominador, true);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "numero", "interessado_nome_razao", "empreendimento_denominador", "situacao_texto" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("numero");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format("select count(*) from {0}lst_requerimento e where e.id > 0" + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select id, requerimento_id, numero, data_criacao, interessado_id, interessado_tipo, 
					interessado_nome_razao, interessado_cpf_cnpj, interessado_rg_ie, empreendimento_id, e.autor_id,
					empreendimento_denominador, empreendimento_cnpj, situacao_id, situacao_texto from {0}lst_requerimento e where e.id > 0"
				+ comandtxt + DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Requerimento req;

					while (reader.Read())
					{
						req = new Requerimento();
						req.Id = Convert.ToInt32(reader["requerimento_id"]);
						req.DataCadastro = Convert.ToDateTime(reader["data_criacao"]);
						req.AutorId = reader.GetValue<Int32>("autor_id");

						if (reader["interessado_id"] != null && !Convert.IsDBNull(reader["interessado_id"]))
						{
							req.Interessado.Id = Convert.ToInt32(reader["interessado_id"]);
							req.Interessado.Tipo = Convert.ToInt32(reader["interessado_tipo"]);

							if (reader["interessado_tipo"].ToString() == "1")
							{
								req.Interessado.Fisica.Nome = reader["interessado_nome_razao"].ToString();
							}
							else
							{
								req.Interessado.Juridica.RazaoSocial = reader["interessado_nome_razao"].ToString();
							}
						}

						if (reader["empreendimento_id"] != null && !Convert.IsDBNull(reader["empreendimento_id"]))
						{
							req.Empreendimento.Id = Convert.ToInt32(reader["empreendimento_id"]);
							req.Empreendimento.Denominador = reader["empreendimento_denominador"].ToString();
						}

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							req.SituacaoId = Convert.ToInt32(reader["situacao_id"]);
							req.SituacaoTexto = reader["situacao_texto"].ToString();
						}

						retorno.Itens.Add(req);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		internal List<TituloModeloLst> ObterNumerosTitulos(string numero, int modeloId, BancoDeDados banco = null)
		{
			List<TituloModeloLst> lst = new List<TituloModeloLst>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"	
									select tt.id id,
											ttn.numero || '/' || ttn.ano || ' - ' || tt.data_emissao texto,
											1 ativo,
											ttm.sigla
										from {0}tab_titulo tt, {0}tab_titulo_numero ttn, {0}tab_titulo_modelo ttm
										where tt.id = ttn.titulo
										and tt.situacao in (3, 6) -- Concluído e Prorrogado
										and ttn.numero || '/' || ttn.ano = :numero
										and tt.modelo = ttm.id
										and ttn.modelo = :modelo", EsquemaBanco);

				comando.AdicionarParametroEntrada("numero", numero, DbType.String);
				comando.AdicionarParametroEntrada("modelo", modeloId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					int count = 1;
					while (reader.Read())
					{
						lst.Add(new TituloModeloLst()
						{
							Id = count++,
							Texto = reader["texto"].ToString(),
							IsAtivo = Convert.ToBoolean(reader["ativo"]),
							IdRelacionamento = Convert.ToInt32(reader["id"]),
							Sigla = reader["sigla"].ToString()
						});
					}

					reader.Close();
				}
			}
			return lst;
		}

		internal List<Roteiro> ObterRequerimentoRoteirosHistorico(int requerimento, int situacao, BancoDeDados banco = null)
		{
			List<Roteiro> roteiros = new List<Roteiro>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando;

				comando = bancoDeDados.CriarComando(@"select r.roteiro_id roteiro, r.nome roteiro_nome, r.versao, a.atividade_id atividade, ha.atividade atividade_nome, r.tid
				from {0}hst_roteiro r, {0}hst_roteiro_atividades a, {0}hst_roteiro_modelos m, {0}hst_atividade ha, (select lf.codigo finalidade, fa.modelo_id, a.atividade_id
				from {0}hst_requerimento_atividade a, {0}hst_requerimento_ativ_finalida fa, {0}lov_titulo_finalidade lf  where a.id = fa.id_hst and fa.finalidade = lf.id
				and a.id_hst = (select max(h.id) from {0}hst_requerimento h where h.requerimento_id = :requerimento)) x where r.id = a.id_hst and r.id = m.id_hst
				and a.atividade_id = x.atividade_id and bitand(r.finalidade, x.finalidade) > 0 and m.modelo_id = x.modelo_id and a.atividade_tid = ha.tid
				and a.atividade_id = ha.atividade_id and r.id in (select max(h.id) from {0}hst_roteiro h, {0}hst_roteiro_atividades aa, {0}hst_roteiro_modelos mm
				where h.id = aa.id_hst and h.id = mm.id_hst and aa.atividade_id = x.atividade_id and mm.modelo_id = x.modelo_id and bitand(h.finalidade, x.finalidade) > 0 
				and h.data_execucao <= (select max(hr.data_execucao) from {0}hst_requerimento hr where hr.requerimento_id = :requerimento)) order by r.roteiro_id, a.atividade_id", EsquemaBanco);

				comando.AdicionarParametroEntrada("requerimento", requerimento, DbType.Int32);

				#region Buscar todas as atividades que estão configuradas com um roteiro
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						roteiros.Add(new Roteiro()
						{
							Id = Convert.ToInt32(reader["roteiro"]),
							VersaoAtual = Convert.ToInt32(reader["versao"]),
							Nome = reader["roteiro_nome"].ToString(),
							AtividadeId = Convert.ToInt32(reader["atividade"]),
							AtividadeTexto = reader["atividade_nome"].ToString(),
							Tid = reader["tid"].ToString()
						});
					}

					reader.Close();
				}
				#endregion

				comando = bancoDeDados.CriarComando(@"
									select ta.id atividade_id, ta.atividade atividade_nome, r.roteiro_id roteiro, r.versao, r.nome roteiro_nome, r.roteiro_tid
									  from {0}tab_requerimento_atividade tra,
										   {0}tab_atividade              ta", EsquemaBanco);

				comando.DbCommand.CommandText += String.Format(
							@", (select hr.roteiro_id, hr.tid, hr.versao, hr.numero, hr.nome, hr.tid roteiro_tid
									from {0}tab_checagem_roteiro t, {0}hst_roteiro hr
								where t.roteiro_tid = hr.tid
									and t.roteiro = hr.roteiro_id
									and t.checagem = (select t.checagem from {0}tab_protocolo t where t.requerimento = :requerimento) {1}) r 
									where tra.atividade = ta.id and tra.requerimento = :requerimento {2}",
							EsquemaBanco, comando.AdicionarNotIn("and", "t.roteiro", DbType.Int32, roteiros.Select(x => x.Id).ToList()),
							comando.AdicionarNotIn("and", "tra.atividade", DbType.Int32, roteiros.Select(x => x.AtividadeId).ToList()));

				comando.AdicionarParametroEntrada("requerimento", requerimento, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						roteiros.Add(new Roteiro()
						{
							Id = Convert.ToInt32(reader["roteiro"]),
							VersaoAtual = Convert.ToInt32(reader["versao"]),
							Nome = reader["roteiro_nome"].ToString(),
							AtividadeId = Convert.ToInt32(reader["atividade_id"]),
							AtividadeTexto = reader["atividade_nome"].ToString(),
							Tid = reader["roteiro_tid"].ToString()
						});
					}

					reader.Close();
				}
			}
			return roteiros;
		}

		#endregion

		#region Validações

		public string PossuiTituloDeclaratorio(int requerimentoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select l.texto from tab_titulo t, lov_titulo_situacao l where l.id = t.situacao and t.requerimento = :requerimento and t.situacao != 7");//Em cadastro

				comando.AdicionarParametroEntrada("requerimento", requerimentoId, DbType.Int32);
				return Convert.ToString(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal bool Existe(int requerimentoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_requerimento t where t.id =:id");

				comando.AdicionarParametroEntrada("id", requerimentoId, DbType.Int32);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal bool ValidarModeloAnteriorNumero(int tituloAnteriorId, int tituloAnteriorTipo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_requerimento_ativ_finalida t where t.titulo_anterior_id = :tituloAnteriorId and t.titulo_anterior_tipo = :tituloAnteriorTipo");

				comando.AdicionarParametroEntrada("tituloAnteriorId", tituloAnteriorId, DbType.Int32);
				comando.AdicionarParametroEntrada("tituloAnteriorTipo", tituloAnteriorTipo, DbType.Int32);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal bool ValidarNumeroSemAnoExistente(string numero, int modeloId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"	
					select count(*) from {0}tab_titulo tt, {0}tab_titulo_numero ttn
                    where tt.id = ttn.titulo
                    and tt.situacao in (3, 6) -- Concluído e Prorrogado
                    and ttn.numero = :numero
                    and ttn.modelo = :modelo", EsquemaBanco);

				comando.AdicionarParametroEntrada("numero", numero, DbType.String);
				comando.AdicionarParametroEntrada("modelo", modeloId, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal string AssociadoProcesso(int requerimentoId)
		{
			object objeto = null;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.numero||'/'||t.ano numero from {0}tab_protocolo t where t.protocolo = 1 and t.requerimento = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", requerimentoId, DbType.Int32);
				objeto = bancoDeDados.ExecutarScalar(comando);
			}
			return (objeto == null ? string.Empty : objeto.ToString());
		}

		internal string AssociadoDocumento(int requerimentoId)
		{
			object objeto = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.numero||'/'||t.ano numero from {0}tab_protocolo t where t.protocolo = 2 and t.requerimento = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", requerimentoId, DbType.Int32);

				objeto = bancoDeDados.ExecutarScalar(comando);
			}
			return (objeto == null ? string.Empty : objeto.ToString());
		}

		internal string AssociadoTitulo(int requerimentoId)
		{
			object objeto = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select l.texto from tab_titulo t, lov_titulo_situacao l where l.id = t.situacao and t.requerimento = :requerimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("requerimento", requerimentoId, DbType.Int32);

				objeto = bancoDeDados.ExecutarScalar(comando);
			}
			return (objeto == null ? string.Empty : objeto.ToString());
		}

		internal bool IsRequerimentoAtividadeCorte(int requerimento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				SELECT COUNT(1) FROM TAB_REQUERIMENTO_ATIVIDADE WHERE ATIVIDADE = 209 /* Informação de Corte */AND REQUERIMENTO = :requerimento");

				comando.AdicionarParametroEntrada("requerimento", requerimento, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		#endregion
	}
}