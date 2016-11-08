using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemRoteiro;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloChecagemRoteiro.Data
{
	class ChecagemRoteiroDa
	{
		#region Propriedades
		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();
		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }
		private string EsquemaBanco { get; set; }
		#endregion

		public ChecagemRoteiroDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML
		
		internal void Criar(ChecagemRoteiro checagem, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Checagem itens de roteiro

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_checagem a (id, interessado, situacao, tid) 
				values ({0}seq_checagem.nextval, :interessado, :situacao, :tid) returning a.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("interessado", DbType.String, 80, checagem.Interessado);
				comando.AdicionarParametroEntrada("situacao", checagem.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				checagem.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Roteiros

				if (checagem.Roteiros != null && checagem.Roteiros.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_checagem_roteiro tc (id, checagem, roteiro, roteiro_tid, tid) 
					values ({0}seq_checagem_roteiro.nextval, :checagem, :roteiro, :roteiro_tid, :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("checagem", checagem.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("roteiro", DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					foreach (Roteiro item in checagem.Roteiros)
					{
						comando.SetarValorParametro("roteiro", item.Id);
						comando.AdicionarParametroEntrada("roteiro_tid", DbType.String, 36, item.Tid);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Itens

				if (checagem.Itens != null && checagem.Itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_checagem_itens c (id, checagem, item_id, item_tid, situacao, tid, motivo) 
					values ({0}seq_checagem_itens.nextval, :checagem, :item_id, :item_tid, :situacao, :tid, :motivo)", EsquemaBanco);

					comando.AdicionarParametroEntrada("checagem", checagem.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("item_id", DbType.Int32);
					comando.AdicionarParametroEntrada("item_tid", DbType.String, 36);
					comando.AdicionarParametroEntrada("motivo", DbType.String, 500);
					comando.AdicionarParametroEntrada("situacao", DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					foreach (Item item in checagem.Itens)
					{
						comando.SetarValorParametro("item_id", item.Id);
						comando.SetarValorParametro("item_tid", item.Tid);
						comando.SetarValorParametro("motivo", item.Motivo);
						comando.SetarValorParametro("situacao", item.Situacao);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(checagem.Id, eHistoricoArtefato.checagem, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Editar(ChecagemRoteiro checagem, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Checagem itens de roteiro

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_checagem a set a.interessado = :interessado, a.situacao = :situacao, a.tid = :tid where a.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", checagem.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("interessado", DbType.String, 80, checagem.Interessado);
				comando.AdicionarParametroEntrada("situacao", checagem.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco
				//Roteiros da checagem
				comando = bancoDeDados.CriarComando(String.Empty);
				comando.DbCommand.CommandText = String.Format("delete from {1}tab_checagem_roteiro c where c.checagem = :checagem{0}",
					comando.AdicionarNotIn("and", "c.id", DbType.Int32, checagem.Roteiros.Select(x => x.IdRelacionamento).ToList()), EsquemaBanco);
				comando.AdicionarParametroEntrada("checagem", checagem.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				//Itens da checagem
				comando = bancoDeDados.CriarComando(String.Empty);
				comando.DbCommand.CommandText = String.Format("delete from {1}tab_checagem_itens c where c.checagem = :checagem{0}",
					comando.AdicionarNotIn("and", "c.id", DbType.Int32, checagem.Itens.Select(x => x.IdRelacionamento).ToList()), EsquemaBanco);
				comando.AdicionarParametroEntrada("checagem", checagem.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Roteiros

				if (checagem.Roteiros != null && checagem.Roteiros.Count > 0)
				{
					foreach (Roteiro item in checagem.Roteiros)
					{
						if (item.IdRelacionamento > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_checagem_roteiro c set c.checagem  = :checagem, c.roteiro = :roteiro, 
							c.roteiro_tid = :roteiro_tid, c.tid = :tid where c.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_checagem_roteiro(id, checagem, roteiro, roteiro_tid, tid) 
							values (seq_checagem_roteiro.nextval, :checagem, :roteiro, :roteiro_tid, :tid)", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("checagem", checagem.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("roteiro", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("roteiro_tid", DbType.String, 36, item.Tid);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}

				}

				#endregion

				#region Itens

				if (checagem.Itens != null && checagem.Itens.Count > 0)
				{
					foreach (Item item in checagem.Itens)
					{
						if (item.IdRelacionamento > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_checagem_itens c set c.checagem = :checagem, c.motivo = :motivo, c.item_id = :item_id, 
							c.item_tid = :item_tid, c.situacao = :situacao, c.tid = :tid where c.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_checagem_itens c (id, checagem, item_id, item_tid, situacao, tid, motivo) 
							values ({0}seq_checagem_itens.nextval, :checagem, :item_id, :item_tid, :situacao, :tid, :motivo)", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("checagem", checagem.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("item_id", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("item_tid", DbType.String, 36, item.Tid);
						comando.AdicionarParametroEntrada("situacao", item.Situacao, DbType.Int32);
						comando.AdicionarParametroEntrada("motivo", DbType.String, 500, item.Tid);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(checagem.Id, eHistoricoArtefato.checagem, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void AlterarSituacao(ChecagemRoteiro checagem, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Checagem itens de roteiro

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_checagem a set a.situacao = :situacao, a.tid = :tid where a.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", checagem.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", checagem.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				if (checagem.Situacao == 3)//inválida
				{
					Historico.Gerar(checagem.Id, eHistoricoArtefato.checagem, eHistoricoAcao.invalidar, bancoDeDados);
				}
				else
				{
					Historico.Gerar(checagem.Id, eHistoricoArtefato.checagem, eHistoricoAcao.alterarsituacao, bancoDeDados);
				}

				#endregion

				bancoDeDados.Commit();
			}
		}

		public void Salvar(ChecagemRoteiro checagem, BancoDeDados banco = null)
		{
			if (checagem == null)
			{
				throw new Exception("Checagem de itens de roteiro é nula.");
			}

			if (checagem.Id == 0)
			{
				Criar(checagem, banco);
			}
		}

		internal void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação
				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_checagem c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Histórico
				Historico.Gerar(id, eHistoricoArtefato.checagem, eHistoricoAcao.excluir, bancoDeDados);
				#endregion

				#region Apaga os dados da checagem
				List<String> lista = new List<string>();
				lista.Add("delete {0}tab_checagem_roteiro a where a.checagem = :checagem;");
				lista.Add("delete {0}tab_checagem_itens a where a.checagem = :checagem;");
				lista.Add("delete {0}tab_checagem a where a.id = :checagem;");
				comando = bancoDeDados.CriarComando(@"begin " + string.Join(" ", lista) + "end;", EsquemaBanco);
			
				comando.AdicionarParametroEntrada("checagem", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
				#endregion
			}
		}

		#endregion

		#region Obter / Filtrar

		internal ChecagemRoteiro ObterSimplificado(int id)
		{
			ChecagemRoteiro checagem = new ChecagemRoteiro();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Checagem itens de roteiro
				Comando comando = bancoDeDados.CriarComando(@"select c.interessado, ls.id situacao_id, ls.texto situacao_texto, c.tid from {0}tab_checagem c, {0}lov_checagem_situacao ls 
				where c.situacao = ls.id and c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						checagem.Id = id;
						checagem.Tid = reader["tid"].ToString();
						checagem.Interessado = reader["interessado"].ToString();
						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							checagem.Situacao = Convert.ToInt32(reader["situacao_id"]);
							checagem.SituacaoTexto = reader["situacao_texto"].ToString();
						}
					}
					reader.Close();
				}
				#endregion
			}
			return checagem;
		}

		internal ChecagemRoteiro Obter(int id)
		{
			ChecagemRoteiro checagem = new ChecagemRoteiro();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Checagem itens de roteiro
				Comando comando = bancoDeDados.CriarComando(@"select c.interessado, ls.id situacao_id, ls.texto situacao_texto, c.tid from {0}tab_checagem c, {0}lov_checagem_situacao ls 
				where c.situacao = ls.id and c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						checagem.Id = id;
						checagem.Tid = reader["tid"].ToString();
						checagem.Interessado = reader["interessado"].ToString();
						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							checagem.Situacao = Convert.ToInt32(reader["situacao_id"]);
							checagem.SituacaoTexto = reader["situacao_texto"].ToString();
						}
					}
					reader.Close();
				}
				#endregion

				#region Roteiros
				comando = bancoDeDados.CriarComando(@"select c.id, c.roteiro, r.numero, r.nome, c.tid, c.roteiro_tid, r.versao versao_cadastrada, 
				(select hr.versao from {0}tab_roteiro hr where hr.id = c.roteiro) versao_atual,
				r.situacao_id from {0}tab_checagem_roteiro c, {0}hst_roteiro r where c.roteiro = r.roteiro_id and c.roteiro_tid = r.tid
				and c.checagem = :checagem order by 4", EsquemaBanco);

				comando.AdicionarParametroEntrada("checagem", checagem.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Roteiro roteiro;
					while (reader.Read())
					{
						roteiro = new Roteiro();
						roteiro.Id = Convert.ToInt32(reader["roteiro"]);
						roteiro.IdRelacionamento = Convert.ToInt32(reader["id"]);
						roteiro.Tid = reader["roteiro_tid"].ToString();
						roteiro.Nome = reader["nome"].ToString();
						
						if (reader["versao_atual"] != null && !Convert.IsDBNull(reader["versao_atual"]))
						{
							roteiro.VersaoAtual = Convert.ToInt32(reader["versao_atual"]);
						}

						if (reader["versao_cadastrada"] != null && !Convert.IsDBNull(reader["versao_cadastrada"]))
						{
							roteiro.Versao = Convert.ToInt32(reader["versao_cadastrada"]);
						}

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							roteiro.Situacao = Convert.ToInt32(reader["situacao_id"]);
						}

						checagem.Roteiros.Add(roteiro);
					}
					reader.Close();
				}
				#endregion

				#region Itens
				comando = bancoDeDados.CriarComando(@"select tri.item_id id, tri.tid, tri.nome, tri.condicionante, tri.procedimento, tri.tipo_id, tri.tipo_texto, 
				ls.id situacao_id, ls.texto situacao_texto, i.motivo from {0}tab_checagem_itens i, {0}hst_roteiro_item tri, {0}lov_checagem_item_situacao ls 
				where i.item_tid = tri.tid and i.situacao = ls.id and i.checagem = :checagem", EsquemaBanco);

				comando.AdicionarParametroEntrada("checagem", checagem.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Item item;
					while (reader.Read())
					{
						item = new Item();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Tid = reader["tid"].ToString();
						item.Nome = reader["nome"].ToString();
						item.Condicionante = reader["condicionante"].ToString();
						item.ProcedimentoAnalise = reader["procedimento"].ToString();
						item.Motivo = reader["motivo"].ToString();

						if (reader["tipo_id"] != null && !Convert.IsDBNull(reader["tipo_id"]))
						{
							item.Tipo = Convert.ToInt32(reader["tipo_id"]);
							item.TipoTexto = reader["tipo_texto"].ToString();
						}

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							item.Situacao = Convert.ToInt32(reader["situacao_id"]);
							item.SituacaoTexto = reader["situacao_texto"].ToString();
						}

						checagem.Itens.Add(item);
					}
					reader.Close();
				}
				#endregion
			}
			return checagem;
		}

		internal Resultados<ChecagemRoteiro> Filtrar(Filtro<ChecagemRoteiroListarFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<ChecagemRoteiro> retorno = new Resultados<ChecagemRoteiro>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("r.id", "numero", filtros.Dados.Id);

				comandtxt += comando.FiltroAnd("r.situacao", "situacao", filtros.Dados.SituacaoId);

				comandtxt += comando.FiltroAndLike("r.interessado", "interessado", filtros.Dados.Interessado, true);

				comandtxt += comando.FiltroIn("r.id", String.Format("select c.checagem from {0}tab_checagem_roteiro c, tab_roteiro r where c.roteiro = r.id and upper(r.nome) like upper(:nome || '%') ",
					EsquemaBanco), "nome", filtros.Dados.NomeRoteiro);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "numero", "interessado", "situacao" };

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

				comando.DbCommand.CommandText = String.Format("select count(*) from {0}tab_checagem r where r.id > 0" + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select r.id, r.id numero, r.interessado, ls.id situacao_id, ls.texto situacao, r.tid from {0}tab_checagem r, {0}lov_checagem_situacao ls 
				where r.situacao = ls.id" + comandtxt + DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = @"select x.*, nvl((select count(*) from tab_checagem_itens t where t.situacao = 1 and t.checagem = x.id), 0) temPendencia 
													from (select a.*, rownum rnum from ( " + comandtxt + @") a) x where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					ChecagemRoteiro checagem;

					while (reader.Read())
					{
						checagem = new ChecagemRoteiro();
						checagem.Id = Convert.ToInt32(reader["id"]);
						checagem.Interessado = reader["interessado"].ToString();
						checagem.Situacao = Convert.ToInt32(reader["situacao_id"]);
						checagem.SituacaoTexto = reader["situacao"].ToString();
						checagem.TemPendencia = Convert.ToBoolean(reader["temPendencia"]);

						retorno.Itens.Add(checagem);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		#endregion

		#region Validações

		public List<String> ObterAssociacoesCheckList(int id, BancoDeDados banco = null)
		{
			List<String> list = new List<String>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select 
				(case when p.protocolo = 1 then 'processo' else 'documento' end) || ' número: ' || p.numero || '/' || p.ano numero 
				from {0}tab_protocolo p where p.checagem = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						list.Add(reader["numero"].ToString());
					}
					reader.Close();
				}
			}
			return list;
		}

		#endregion
	}
}