using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloAnaliseItens;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloAnaliseItens.Data
{
	class AnaliseItensDa
	{
		#region Propriedades

		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());
		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();

		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }

		private string EsquemaBanco { get; set; }

		private string EsquemaBancoCredenciado
		{
			get
			{
				return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado);
			}
		}

		#endregion

		public AnaliseItensDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Criar(AnaliseItem analise, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Análise de itens de processo/documento

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_analise a (id, protocolo, checagem, situacao, tid) 
				values ({0}seq_analise.nextval, :protocolo, :checagem, :situacao, :tid) returning a.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", analise.Protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("checagem", analise.Checagem.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", 1, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				analise.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Roteiros

				if (analise.Roteiros != null && analise.Roteiros.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_analise_roteiro tc (id, analise, roteiro, roteiro_tid, tid) 
					values ({0}seq_analise_roteiro.nextval, :analise, :roteiro, :roteiro_tid, :tid) returning tc.id into :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("analise", analise.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("roteiro", DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroSaida("id", DbType.Int32);

					foreach (Roteiro item in analise.Roteiros)
					{
						comando.SetarValorParametro("roteiro", item.Id);
						comando.AdicionarParametroEntrada("roteiro_tid", DbType.String, 36, item.Tid);
						bancoDeDados.ExecutarNonQuery(comando);
						item.IdRelacionamento = Convert.ToInt32(comando.ObterValorParametro("id"));
					}
				}

				#endregion

				#region Itens

				if (analise.Itens != null && analise.Itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_analise_itens c 
					(id, analise, checagem, item_id, item_tid, descricao, motivo, situacao, tid, avulso, setor) 
					values 
					({0}seq_analise_itens.nextval, :analise, :checagem, :item_id, :item_tid, :descricao, :motivo, :situacao, :tid, :avulso, :setor) 
					returning c.id into :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("analise", analise.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("checagem", analise.Checagem.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("item_id", DbType.Int32);
					comando.AdicionarParametroEntrada("item_tid", DbType.String, 36);
					comando.AdicionarParametroEntrada("descricao", DbType.String, 500);
					comando.AdicionarParametroEntrada("motivo", DbType.String);
					comando.AdicionarParametroEntrada("situacao", DbType.Int32);
					comando.AdicionarParametroEntrada("setor", DbType.Int32);
					comando.AdicionarParametroEntrada("avulso", DbType.Int32);

					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroSaida("id", DbType.Int32);

					foreach (Item item in analise.Itens)
					{
						comando.SetarValorParametro("item_id", item.Id);
						comando.SetarValorParametro("item_tid", item.Tid);
						comando.SetarValorParametro("descricao", item.Descricao);
						comando.AdicionarParametroEntClob("motivo", item.Motivo);
						comando.SetarValorParametro("situacao", item.Situacao);
						comando.SetarValorParametro("setor", item.SetorId);
						comando.SetarValorParametro("avulso", item.Avulso ? 1 : 0);

						bancoDeDados.ExecutarNonQuery(comando);
						item.IdRelacionamento = Convert.ToInt32(comando.ObterValorParametro("id"));
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(analise.Id, eHistoricoArtefato.analiseitens, eHistoricoAcao.analisar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Editar(AnaliseItem analise, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Análise de itens de processo/documento

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_analise a set a.protocolo = :protocolo, 
				a.checagem = :checagem, a.situacao = :situacao, a.atualizado = :atualizado, a.tid = :tid where a.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", analise.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", analise.Protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("checagem", analise.Checagem.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", analise.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("atualizado", analise.Atualizado ? 1 : 0, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco

				//Itens da analise

				comando = bancoDeDados.CriarComando("delete from {0}tab_analise_itens c ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where c.analise = :analise{0}",
					comando.AdicionarNotIn("and", "c.id", DbType.Int32, analise.Itens.Select(x => x.IdRelacionamento).ToList()), EsquemaBanco);
				comando.AdicionarParametroEntrada("analise", analise.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Roteiros

				if (analise.Roteiros != null && analise.Roteiros.Count > 0)
				{
					foreach (Roteiro roteiro in analise.Roteiros)
					{
						if (roteiro.IdRelacionamento > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_analise_roteiro c set c.analise  = :analise, c.roteiro = :roteiro, 
							c.roteiro_tid = :roteiro_tid, c.tid = :tid where c.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", roteiro.IdRelacionamento, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_analise_roteiro(id, analise, roteiro, roteiro_tid, tid) 
							values (seq_analise_roteiro.nextval, :analise, :roteiro, :roteiro_tid, :tid)", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("analise", analise.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("roteiro_tid", DbType.String, 36, roteiro.Tid);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Itens

				if (analise.Itens != null && analise.Itens.Count > 0)
				{
					foreach (Item item in analise.Itens)
					{
						if (item.IdRelacionamento > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_analise_itens c set c.analise = :analise, c.checagem = :checagem, c.data_analise = :data_analise,
							c.item_id = :item_id, c.item_tid = :item_tid, c.descricao = :descricao, c.motivo = :motivo, c.situacao = :situacao, c.setor = :setor, c.avulso = :avulso, c.tid = :tid, c.analista = :analista, c.recebido = :recebido
							where c.id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
						}
						else
						{

							comando = bancoDeDados.CriarComando(@"insert into {0}tab_analise_itens c (id, analise, checagem, item_id, item_tid, descricao, motivo, situacao, setor, avulso, tid, data_analise, analista, recebido) 
							values ({0}seq_analise_itens.nextval, :analise, :checagem, :item_id, :item_tid, :descricao, :motivo, :situacao, :setor, :avulso, :tid, :data_analise, :analista, :recebido) returning id into :id", EsquemaBanco);

							comando.AdicionarParametroSaida("id", DbType.Int32);
						}

						comando.AdicionarParametroEntrada("analise", analise.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("checagem", analise.Checagem.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("item_id", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("item_tid", DbType.String, 36, item.Tid);

						if (String.IsNullOrEmpty(item.DataAnalise))
						{
							comando.AdicionarParametroEntrada("data_analise", DBNull.Value, DbType.DateTime);
						}
						else
						{
							comando.AdicionarParametroEntrada("data_analise", Convert.ToDateTime(item.DataAnalise), DbType.DateTime);
						}

						comando.AdicionarParametroEntrada("descricao", DbType.String, 500, item.Descricao);
						comando.AdicionarParametroEntClob("motivo", item.Motivo);
						comando.AdicionarParametroEntrada("situacao", item.Situacao, DbType.Int32);
						comando.AdicionarParametroEntrada("setor", item.SetorId, DbType.Int32);
						comando.AdicionarParametroEntrada("avulso", item.Avulso ? 1 : 0, DbType.Int32);
						comando.AdicionarParametroEntrada("analista", item.Analista, DbType.String);
						comando.AdicionarParametroEntrada("recebido", item.Recebido, DbType.Boolean);

						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						if (item.IdRelacionamento <= 0) 
						{
							item.IdRelacionamento = Convert.ToInt32(comando.ObterValorParametro("id"));
						}
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(analise.Id, eHistoricoArtefato.analiseitens, eHistoricoAcao.analisar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void AlterarSituacao(AnaliseItem analise, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Análise de itens de processo/documento

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_analise a set a.situacao = :situacao, a.tid = :tid where a.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", analise.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", analise.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(analise.Id, eHistoricoArtefato.analiseitens, eHistoricoAcao.alterarsituacao, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		public void Salvar(AnaliseItem analise, BancoDeDados banco = null)
		{
			if (analise == null)
			{
				throw new Exception("Análise de itens é nula.");
			}

			if (analise.Id == 0)
			{
				Criar(analise, banco);
			}
			else
			{
				Editar(analise, banco);
			}
		}

		internal void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_analise c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Histórico

				Historico.Gerar(id, eHistoricoArtefato.analiseitens, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				List<String> lista = new List<string>();
				lista.Add("delete {0}tab_analise_roteiro a where a.analise = :analise;");
				lista.Add("delete {0}tab_analise_itens a where a.analise = :analise;");
				lista.Add("delete {0}tab_analise a where a.id = :analise;");

				#region Apaga os dados da analise
				comando = bancoDeDados.CriarComando(@"begin " + string.Join(" ", lista) + " end;", EsquemaBanco);
				comando.AdicionarParametroEntrada("analise", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
				#endregion
			}
		}

		#endregion

		#region Obter / Filtrar

		internal AnaliseItem Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			AnaliseItem analise = new AnaliseItem();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Análise de itens de processo/documento

				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.protocolo, ls.id situacao_id, ls.texto situacao_texto, c.tid,
				c.checagem from {0}tab_analise c, {0}lov_analise_situacao ls where c.situacao = ls.id and c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						analise.Id = id;
						analise.Tid = reader["tid"].ToString();

						if (reader["protocolo"] != null && !Convert.IsDBNull(reader["protocolo"]))
						{
							analise.Protocolo.Id = Convert.ToInt32(reader["protocolo"]);
						}

						if (reader["checagem"] != null && !Convert.IsDBNull(reader["checagem"]))
						{
							analise.ChecagemId = Convert.ToInt32(reader["checagem"]);
							analise.Checagem.Id = Convert.ToInt32(reader["checagem"]);
						}

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							analise.Situacao = Convert.ToInt32(reader["situacao_id"]);
							analise.SituacaoTexto = reader["situacao_texto"].ToString();
						}
					}
					reader.Close();
				}
				if (simplificado)
				{
					return analise;
				}

				#endregion

				if (analise.Id > 0)
				{
					#region Roteiros

					comando = bancoDeDados.CriarComando(@"select ta.id, ta.roteiro, ta.tid, ta.roteiro_tid, tr.numero, tr.nome nome_cadastrado, r.nome nome_atual,
														tr.versao versao_cadastrada, tr.situacao_id situacao, to_char(trh.data_execucao, 'dd/mm/yyyy') data_atualizacao, r.versao versao_atual from {0}tab_analise_roteiro ta, 
														{0}hst_roteiro tr, {0}hst_roteiro trh, {0}tab_roteiro r where ta.roteiro_tid = tr.tid and trh.tid = r.tid and trh.roteiro_id = r.id 
														and ta.roteiro = tr.roteiro_id and ta.roteiro = r.id and ta.analise = :analise", EsquemaBanco);

					comando.AdicionarParametroEntrada("analise", analise.Id, DbType.Int32);

					List<Roteiro> roteiros = new List<Roteiro>();

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						Roteiro roteiro;
						while (reader.Read())
						{
							roteiro = new Roteiro();
							roteiro.Id = Convert.ToInt32(reader["roteiro"]);
							roteiro.IdRelacionamento = Convert.ToInt32(reader["id"]);
							roteiro.Tid = reader["roteiro_tid"].ToString();

							roteiro.Nome = reader["nome_cadastrado"].ToString();
							roteiro.NomeAtual = reader["nome_atual"].ToString();

							roteiro.Versao = Convert.ToInt32(reader["versao_atual"]);
							roteiro.VersaoAtual = Convert.ToInt32(reader["versao_cadastrada"]);

							roteiro.Situacao = Convert.ToInt32(reader["situacao"]);

							if (reader["data_atualizacao"] != null && !Convert.IsDBNull(reader["data_atualizacao"]))
							{
								roteiro.DataAtualizacao = reader["data_atualizacao"].ToString();
							}

							analise.Roteiros.Add(roteiro);
						}
						reader.Close();
					}

					#endregion

					#region Itens

					comando = bancoDeDados.CriarComando(@"select tai.id, tai.recebido, tai.analista, tai.analise, hri.item_id idRelacionamento,
					tai.data_analise, tai.item_tid tid, tai.checagem, hri.nome, hri.condicionante, hri.procedimento, hri.tipo_id, lrip.texto tipo_texto,
					ls.id situacao_id, ls.texto situacao_texto, tai.descricao, tai.motivo, tai.avulso, tai.setor, hri.caracterizacao_tipo_id from {0}tab_analise_itens tai,
					{0}hst_roteiro_item hri, {0}lov_roteiro_item_tipo lrip, {0}lov_analise_item_situacao ls where tai.item_tid = hri.tid
					and tai.situacao = ls.id and hri.tipo_id = lrip.id and tai.analise = :analise", EsquemaBanco);

					comando.AdicionarParametroEntrada("analise", analise.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{

						Item item;
						while (reader.Read())
						{
							item = new Item();
							item.Id = Convert.ToInt32(reader["idRelacionamento"]);
							item.IdRelacionamento = Convert.ToInt32(reader["id"]);
							item.Tid = reader["tid"].ToString();
							item.Nome = reader["nome"].ToString();
							item.Condicionante = reader["condicionante"].ToString();
							item.ProcedimentoAnalise = reader["procedimento"].ToString();
							item.Analista = reader["analista"].ToString();
							item.CaracterizacaoTipoId = reader.GetValue<Int32>("caracterizacao_tipo_id");

							item.DataAnalise = DateTime.Today.ToString();
							if (reader["data_analise"] != null && !Convert.IsDBNull(reader["data_analise"]))
							{
								item.DataAnalise = reader["data_analise"].ToString();
							}

							item.Descricao = reader["descricao"].ToString();
							item.Motivo = reader["motivo"].ToString();

							if (reader["checagem"] != null && !Convert.IsDBNull(reader["checagem"]))
							{
								item.ChecagemId = Convert.ToInt32(reader["checagem"]);
							}

							if (reader["tipo_id"] != null && !Convert.IsDBNull(reader["tipo_id"]))
							{
								item.Tipo = Convert.ToInt32(reader["tipo_id"]);
								item.TipoTexto = reader["tipo_texto"].ToString();
							}

							item.Avulso = Convert.ToBoolean(reader["avulso"]);
							item.Recebido = Convert.ToBoolean(reader["recebido"]);

							if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
							{
								item.Situacao = Convert.ToInt32(reader["situacao_id"]);
								item.SituacaoTexto = reader["situacao_texto"].ToString();
							}

							if (reader["setor"] != null && !Convert.IsDBNull(reader["setor"]))
							{
								item.SetorId = Convert.ToInt32(reader["setor"]);
							}

							analise.Itens.Add(item);
						}
						reader.Close();
					}
					#endregion
				}
			}
			return analise;
		}

		internal AnaliseItem ObterSimplificado(int id)
		{
			return Obter(id, true);
		}

		internal AnaliseItem ObterPorChecagem(int checagemId, bool simplificado = false, BancoDeDados banco = null)
		{
			AnaliseItem analise = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Análise de itens de processo/documento

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}tab_analise c where c.checagem = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", checagemId, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (!Convert.IsDBNull(valor) && Convert.ToInt32(valor) > 0)
				{
					analise = Obter(Convert.ToInt32(valor), simplificado);
				}
				#endregion

			}
			return analise;
		}

		internal AnaliseItem Obter(Protocolo protocolo, bool simplificado = false)
		{
			AnaliseItem analise = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Análise de itens de processo/documento

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}tab_analise c where c.protocolo = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", protocolo.Id, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (!Convert.IsDBNull(valor) && Convert.ToInt32(valor) > 0)
				{
					analise = Obter(Convert.ToInt32(valor), simplificado);
				}
				#endregion

			}
			return analise;
		}

		internal AnaliseItem ObterSimplificado(Protocolo protocolo)
		{
			return Obter(protocolo, true);
		}

		internal AnaliseItem ObterHistorico(int id, string tid = null, bool simplificado = false, BancoDeDados banco = null)
		{
			AnaliseItem analise = new AnaliseItem();
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Análise de itens de processo/documento

				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.protocolo_id, c.situacao_id, c.situacao_texto, c.tid, c.checagem_id 
				from {0}hst_analise c where c.analise_id = :id and c.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst = Convert.ToInt32(reader["id"]);

						analise.Id = id;
						analise.Tid = reader["tid"].ToString();

						if (reader["protocolo_id"] != null && !Convert.IsDBNull(reader["protocolo_id"]))
						{
							analise.Protocolo.Id = Convert.ToInt32(reader["protocolo_id"]);
						}

						if (reader["checagem_id"] != null && !Convert.IsDBNull(reader["checagem_id"]))
						{
							analise.ChecagemId = Convert.ToInt32(reader["checagem_id"]);
							analise.Checagem.Id = Convert.ToInt32(reader["checagem_id"]);
						}

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							analise.Situacao = Convert.ToInt32(reader["situacao_id"]);
							analise.SituacaoTexto = reader["situacao_texto"].ToString();
						}
					}
					reader.Close();
				}

				#endregion

				if (analise.Id <= 0 || simplificado)
				{
					return analise;
				}

				#region Roteiros

				comando = bancoDeDados.CriarComando(@"select ta.analise_rot_id id, ta.roteiro_id, ta.tid, ta.roteiro_tid, tr.numero, tr.nome nome_cadastrado, tr.nome nome_atual, 
				tr.versao versao_cadastrada, tr.situacao_id situacao, to_char(tr.data_criacao, 'dd/mm/yyyy') data_atualizacao, tr.versao versao_atual
				from {0}hst_analise_roteiro ta, {0}hst_roteiro tr where ta.roteiro_tid = tr.tid and ta.roteiro_id = tr.roteiro_id and ta.id_hst = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", hst, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Roteiro roteiro;

					while (reader.Read())
					{
						roteiro = new Roteiro();
						roteiro.Id = Convert.ToInt32(reader["roteiro_id"]);
						roteiro.IdRelacionamento = Convert.ToInt32(reader["id"]);
						roteiro.Tid = reader["roteiro_tid"].ToString();

						roteiro.Nome = reader["nome_cadastrado"].ToString();
						roteiro.NomeAtual = reader["nome_atual"].ToString();

						roteiro.Versao = Convert.ToInt32(reader["versao_atual"]);
						roteiro.VersaoAtual = Convert.ToInt32(reader["versao_cadastrada"]);

						roteiro.Situacao = Convert.ToInt32(reader["situacao"]);

						if (reader["data_atualizacao"] != null && !Convert.IsDBNull(reader["data_atualizacao"]))
						{
							roteiro.DataAtualizacao = reader["data_atualizacao"].ToString();
						}

						analise.Roteiros.Add(roteiro);
					}

					reader.Close();
				}

				#endregion

				#region Itens

				comando = bancoDeDados.CriarComando(@"select tai.itens_id id, tai.recebido, tai.analista, tai.analise_id, hri.item_id idrelacionamento, tai.data_analise, tai.item_tid tid,
				tai.checagem_id, hri.nome, hri.condicionante, hri.procedimento, hri.tipo_id, hri.tipo_texto, tai.situacao_id, tai.situacao_texto, tai.descricao, tai.motivo,
				tai.avulso, tai.setor_id from {0}hst_analise_itens tai, {0}hst_roteiro_item hri where tai.item_tid = hri.tid and tai.id_hst = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", hst, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{

					Item item;
					while (reader.Read())
					{
						item = new Item();
						item.Id = Convert.ToInt32(reader["idRelacionamento"]);
						item.IdRelacionamento = Convert.ToInt32(reader["id"]);
						item.Tid = reader["tid"].ToString();
						item.Nome = reader["nome"].ToString();
						item.Condicionante = reader["condicionante"].ToString();
						item.ProcedimentoAnalise = reader["procedimento"].ToString();
						item.Analista = reader["analista"].ToString();

						item.DataAnalise = DateTime.Today.ToString();
						if (reader["data_analise"] != null && !Convert.IsDBNull(reader["data_analise"]))
						{
							item.DataAnalise = reader["data_analise"].ToString();
						}

						item.Descricao = reader["descricao"].ToString();
						item.Motivo = reader["motivo"].ToString();

						if (reader["checagem_id"] != null && !Convert.IsDBNull(reader["checagem_id"]))
						{
							item.ChecagemId = Convert.ToInt32(reader["checagem_id"]);
						}

						if (reader["tipo_id"] != null && !Convert.IsDBNull(reader["tipo_id"]))
						{
							item.Tipo = Convert.ToInt32(reader["tipo_id"]);
							item.TipoTexto = reader["tipo_texto"].ToString();
						}

						item.Avulso = Convert.ToBoolean(reader["avulso"]);
						item.Recebido = Convert.ToBoolean(reader["recebido"]);

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							item.Situacao = Convert.ToInt32(reader["situacao_id"]);
							item.SituacaoTexto = reader["situacao_texto"].ToString();
						}

						if (reader["setor_id"] != null && !Convert.IsDBNull(reader["setor_id"]))
						{
							item.SetorId = Convert.ToInt32(reader["setor_id"]);
						}

						analise.Itens.Add(item);
					}
					reader.Close();
				}

				#endregion
			}

			return analise;
		}

		internal List<Item> ObterHistoricoAnalise(int itemId, int checagemId)
		{
			List<Item> analises = new List<Item>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				int acao = Historico.ObterHistoricoAcaoId(eHistoricoAcao.analisar, eHistoricoArtefato.analiseitens, bancoDeDados);

				#region Analise dos itens
				Comando comando = bancoDeDados.CriarComando(@"select ta.id_hst, ta.id, nvl(ta.data_analise, aa.data_execucao) data_analise, ta.item_id, ta.situacao_id,
															ta.situacao_texto, ta.item_id, ta.analista, ta.descricao, ta.tid,
															ta.motivo, s.nome setor_nome, s.sigla setor_sigla from {0}hst_analise_itens ta, 
															{0}hst_analise aa, hst_setor s where aa.id = ta.id_hst and ta.checagem_id = :checagem_id
															and ta.item_id = :item_id and ta.analista is not null and aa.acao_executada = :acao
															and s.tid(+) = ta.setor_tid and s.setor_id(+) = ta.setor_id order by ta.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("item_id", itemId, DbType.Int32);
				comando.AdicionarParametroEntrada("checagem_id", checagemId, DbType.Int32);
				comando.AdicionarParametroEntrada("acao", acao, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Item analiseItem;
					while (reader.Read())
					{
						analiseItem = new Item();
						analiseItem.Id = Convert.ToInt32(reader["item_id"]);

						if (reader["data_analise"] != null && !Convert.IsDBNull(reader["data_analise"]))
						{
							analiseItem.DataAnalise = reader["data_analise"].ToString();
						}

						analiseItem.Analista = reader["analista"].ToString();

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							analiseItem.Situacao = Convert.ToInt32(reader["situacao_id"]);
						}
						analiseItem.SituacaoTexto = reader["situacao_texto"].ToString();

						if (reader["descricao"] != null && !Convert.IsDBNull(reader["descricao"]))
						{
							analiseItem.Descricao = reader["descricao"].ToString();
						}

						if (reader["motivo"] != null && !Convert.IsDBNull(reader["motivo"]))
						{
							analiseItem.Motivo = reader["motivo"].ToString();
						}

						if (reader["setor_nome"] != null && !Convert.IsDBNull(reader["setor_nome"]))
						{
							analiseItem.SetorNome = reader["setor_nome"].ToString();
						}

						if (reader["setor_sigla"] != null && !Convert.IsDBNull(reader["setor_sigla"]))
						{
							analiseItem.SetorSigla = reader["setor_sigla"].ToString();
						}

						analiseItem.Tid = reader["tid"].ToString();

						analises.Add(analiseItem);
					}
					reader.Close();
				}
				#endregion
			}

			return analises;
		}

		internal AnaliseItem ObterAnaliseTitulo(int tituloId, BancoDeDados banco = null)
		{
			AnaliseItem analise = new AnaliseItem();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select n.analise_id, n.analise_tid
				from {0}esp_oficio_notificacao n, {0}tab_protocolo p where p.id = n.protocolo and n.titulo = :tituloId", EsquemaBanco);

				comando.AdicionarParametroEntrada("tituloId", tituloId);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						analise.Id = Convert.ToInt32(reader["analise_id"]);
						analise.Tid = reader["analise_tid"].ToString();
					}

					reader.Close();
				}
			}

			return analise;
		}

		public List<Item> ObterItensProjetoDigital(List<Caracterizacao> caracterizacoes)
		{
			List<Item> itens = new List<Item>();

			if (caracterizacoes.Count == 0)
			{
				return itens;
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Itens do roteiro

				Comando comando = bancoDeDados.CriarComando(@"select r.id, r.nome, r.condicionante, r.procedimento, r.tipo, l.texto tipo_texto,
															r.caracterizacao_tipo, lc.texto caracterizacao_tipo_texto, li.id situacao_id, li.texto situacao_texto,
															r.tid from tab_roteiro_item r, lov_roteiro_item_tipo l, lov_caracterizacao_tipo lc, lov_analise_item_situacao li
															where r.tipo = l.id and lc.id = r.caracterizacao_tipo and li.id = 1/*Recebido*/ and r.tipo = 3 /*Projeto Digital*/", EsquemaBanco);

				comando.DbCommand.CommandText += comando.AdicionarIn("and", "r.caracterizacao_tipo", DbType.Int32, caracterizacoes.Select(x => (int)x.Tipo).ToList());
				
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Item item = new Item();

					while (reader.Read())
					{
						item = new Item();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Nome = reader["nome"].ToString();
						item.DataAnalise = DateTime.Today.ToString();
						item.Recebido = true;
						item.Situacao = reader.GetValue<Int32>("situacao_id");
						item.SituacaoTexto = reader.GetValue<String>("situacao_texto");
						item.Condicionante = reader["condicionante"].ToString();
						item.ProcedimentoAnalise = reader["procedimento"].ToString();
						item.Tipo = reader.GetValue<int>("tipo");
						item.TipoTexto = reader["tipo_texto"].ToString();
						item.CaracterizacaoTipoId = reader.GetValue<int>("caracterizacao_tipo");
						item.CaracterizacaoTipoTexto = reader.GetValue<string>("caracterizacao_tipo_texto");
						item.Tid = reader["tid"].ToString();
						item.TemProjetoGeografico = caracterizacoes.Any(x => (int)x.Tipo == item.CaracterizacaoTipoId && x.ProjetoId > 0);

						itens.Add(item);
					}

					reader.Close();
				}

				#endregion
			}

			return itens;

		}

		#endregion

		#region Validações

		public int ExisteAnalise(int checagemId, BancoDeDados banco = null)
		{
			int retorno = 0;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select a.id from {0}tab_analise a where a.checagem = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", checagemId, DbType.Int32);
				object obj = bancoDeDados.ExecutarScalar(comando);

				if (obj != null && !Convert.IsDBNull(obj))
				{
					retorno = Convert.ToInt32(obj);
				}
			}
			return retorno;
		}

		public bool ExisteTituloPendencia(int requerimentoId, BancoDeDados banco = null)
		{
			bool retorno = false;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select count(1) from tab_titulo t, esp_oficio_notificacao e 
				where e.protocolo = (select id from tab_protocolo where requerimento = :requerimento) 
				and t.situacao in (2, 3, 4, 6) /*Emitido, Concluído, Assinado e Prorrogado*/ and t.id = e.titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("requerimento", requerimentoId, DbType.Int32);
				object obj = bancoDeDados.ExecutarScalar(comando);

				if (obj != null && !Convert.IsDBNull(obj))
				{
					retorno = Convert.ToBoolean(obj);
				}
			}

			return retorno;
		}

		public bool IsImportadoCaracterizacao(int projetoDigitalId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(1) from tmp_projeto_digital where id = :projeto_digital and etapa_importacao = 3", EsquemaBanco);

				comando.AdicionarParametroEntrada("projeto_digital", projetoDigitalId, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		#endregion
	}
}