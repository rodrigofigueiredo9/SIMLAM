using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPapel;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloPapel.Data
{
	class PapelDa
	{
		#region Propriedades

		private Historico _historico = new Historico();
		private Historico Historico { get { return _historico; } }
		private string EsquemaBanco { get; set; }

		#endregion

		#region Buscar / Listar

		internal Resultados<Papel> Filtrar(Filtro<PapelListarFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<Papel> retorno = new Resultados<Papel>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAndLike("p.nome", "nome", filtros.Dados.Nome, true);

				List<String> ordenar = new List<String>() { "nome" };
				List<String> colunas = new List<String>() { "nome" };

				#endregion

				#region Executa a pesquisa nas tabelas

				comando.DbCommand.CommandText = "select count(*) from tab_autenticacao_papel p where p.tipo = 1 and p.funcionario_tipo = 3 " + comandtxt;

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@" select p.id, p.nome, p.tid from tab_autenticacao_papel p where p.tipo = 1 and p.funcionario_tipo = 3 {0} {1}", 
					comandtxt, DaHelper.Ordenar(colunas, ordenar));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Papel papel;
					while (reader.Read())
					{
						papel = new Papel();

						papel.Id = Convert.ToInt32(reader["id"]);
						papel.Tid = reader["tid"].ToString();
						papel.Nome = reader["nome"].ToString();

						retorno.Itens.Add(papel);
					}

					reader.Close();
				}
			}

			return retorno;
		}

		internal string ObterNome(int id)
		{
			string nome = string.Empty;
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				comando = bancoDeDados.CriarComando(@" select t.nome from tab_autenticacao_papel t where t.id = :id ");
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				nome = bancoDeDados.ExecutarScalar(comando).ToString();
			}

			return nome;
		}

		internal List<PermissaoGrupo> ObterPermissaoGrupoColecao()
		{
			List<PermissaoGrupo> grupoColecao = null;
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Grupo

				comando = bancoDeDados.CriarComando(@"select distinct l.grupo from lov_autenticacao_permissao l where l.funcionario_tipo = 3 and l.tipo = 1 order by l.grupo");

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					grupoColecao = new List<PermissaoGrupo>();

					while (reader.Read())
					{
						grupoColecao.Add(new PermissaoGrupo { Nome = reader["grupo"].ToString() });
					}

					reader.Close();
				}

				#endregion

				#region Permissão

				comando = bancoDeDados.CriarComando(@" select l.id, l.nome from lov_autenticacao_permissao l where l.funcionario_tipo = 3 and l.tipo = 1 and l.grupo = :grupo order by l.grupo, l.nome ");

				comando.AdicionarParametroEntrada("grupo", DbType.String);

				foreach (var grupo in grupoColecao)
				{
					comando.DbCommand.Parameters["grupo"].Value = grupo.Nome;

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						grupo.PermissaoColecao = new List<Permissao>();

						while (reader.Read())
						{
							grupo.PermissaoColecao.Add(new Permissao { ID = Convert.ToInt32(reader["id"]), Nome = reader["nome"].ToString() });
						}

						reader.Close();
					}
				}

				#endregion
			}

			return grupoColecao;
		}

		internal List<PermissaoGrupo> ObterPermissaoGrupoColecao(int id)
		{
			List<PermissaoGrupo> grupoColecao = null;
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{

				#region Grupo

				comando = bancoDeDados.CriarComando(@"select distinct l.grupo from lov_autenticacao_permissao l where l.funcionario_tipo = 3 and l.tipo = 1 order by l.grupo");

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					grupoColecao = new List<PermissaoGrupo>();

					while (reader.Read())
					{
						grupoColecao.Add(new PermissaoGrupo { Nome = reader["grupo"].ToString() });
					}

					reader.Close();
				}

				#endregion

				#region Permissão

				comando = bancoDeDados.CriarComando(@" select l.id, l.nome, nvl((select 1 from tab_autenticacao_papel_perm t where t.papel = :papel and t.permissao = l.id), 0) is_permitido from 
                    lov_autenticacao_permissao l where l.funcionario_tipo = 3 and l.tipo = 1 and l.grupo = :grupo  order by l.grupo, l.nome ");

				comando.AdicionarParametroEntrada("grupo", DbType.String);
				comando.AdicionarParametroEntrada("papel", id, DbType.Int32);

				foreach (var grupo in grupoColecao)
				{
					comando.DbCommand.Parameters["grupo"].Value = grupo.Nome;

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						grupo.PermissaoColecao = new List<Permissao>();

						while (reader.Read())
						{
							grupo.PermissaoColecao.Add(new Permissao
							{
								ID = Convert.ToInt32(reader["id"]),
								Nome = reader["nome"].ToString(),
								IsPermitido = reader["is_permitido"].ToString().Equals("1")
							});
						}

						reader.Close();
					}
				}

				#endregion
			}

			return grupoColecao;
		}

		#endregion

		#region DMLs

		internal void Salvar(Papel papel, BancoDeDados banco = null)
		{
			if (papel == null)
			{
				throw new Exception("Papel nulo.");
			}

			if (papel.Id <= 0)
			{
				Criar(papel, banco);
			}
			else
			{
				Editar(papel, banco);
			}
		}

		internal void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação
				
				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_autenticacao_papel c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Histórico / Consulta

				Historico.Gerar(id, eHistoricoArtefato.papel, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Apaga os dados de papel

				List<String> lista = new List<string>();
				lista.Add("delete {0}tab_autenticacao_papel_perm a where a.papel = :papel;");
				lista.Add("delete {0}tab_autenticacao_papel a where a.id = :papel;");
				comando = bancoDeDados.CriarComando(@"begin " + string.Join(" ", lista) + "end;", EsquemaBanco);
			
				comando.AdicionarParametroEntrada("papel", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();

				#endregion
			}
		}

		private void Editar(Papel papel, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Papel
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@" update tab_autenticacao_papel set nome = :nome, tid= :tid where id = :id ");

				comando.AdicionarParametroEntrada("nome", DbType.String, 80, papel.Nome);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", papel.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Permissão

				comando = bancoDeDados.CriarComando(@" delete tab_autenticacao_papel_perm where papel = :id ");
				comando.AdicionarParametroEntrada("id", papel.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@" insert into tab_autenticacao_papel_perm (id, papel, permissao, tid) values (seq_autenticacao_papel_perm.nextval, :papel, :permissao, :tid) ");

				comando.AdicionarParametroEntrada("papel", papel.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("permissao", DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				foreach (var item in papel.Permissoes)
				{
					comando.DbCommand.Parameters["permissao"].Value = item.ID;
					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				Historico.Gerar(papel.Id, eHistoricoArtefato.papel, eHistoricoAcao.atualizar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		private void Criar(Papel papel, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Papel
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@" insert into tab_autenticacao_papel (id, nome, funcionario_tipo, tipo, tid) 
					values (seq_autenticacao_papel.nextval, :nome, 3, 1, :tid) returning id into :id ");

				comando.AdicionarParametroEntrada("nome", papel.Nome, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				papel.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Permissão

				comando = bancoDeDados.CriarComando(@" insert into tab_autenticacao_papel_perm (id, papel, permissao, tid) values (seq_autenticacao_papel_perm.nextval, :papel, :permissao, :tid) ");

				comando.AdicionarParametroEntrada("papel", papel.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("permissao", DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				foreach (var item in papel.Permissoes)
				{
					comando.DbCommand.Parameters["permissao"].Value = item.ID;
					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				Historico.Gerar(papel.Id, eHistoricoArtefato.papel, eHistoricoAcao.criar, bancoDeDados);

				bancoDeDados.Commit();
			}

		}

		#endregion

		internal bool PossuiFuncionario(int id, out string funcionarios)
		{
			funcionarios = string.Empty;
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				comando = bancoDeDados.CriarComando(@" select stragg(tf.nome) funcionarios from tab_funcionario tf, tab_funcionario_papel tfp where tf.id =
				tfp.funcionario and tfp.papel = :papel ");

				comando.AdicionarParametroEntrada("papel", id, DbType.Int32);

				funcionarios = bancoDeDados.ExecutarScalar(comando).ToString();
			}

			return !string.IsNullOrWhiteSpace(funcionarios);
		}
	}
}