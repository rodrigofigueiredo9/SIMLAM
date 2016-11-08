using System;
using System.Data;
using Tecnomapas.Blocos.Data;

namespace Tecnomapas.Blocos.Arquivo.Data
{
	public class ArquivoDa
	{
		private string _esquema;

		public ArquivoDa(string esquema = null)
		{
			_esquema = esquema;
		}

		public string ObterCaminho(int id, BancoDeDados banco = null)
		{
			string caminho = string.Empty;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, _esquema))
			{
				#region Arquivo

				Comando comando = bancoDeDados.CriarComando(@"begin select caminho into :caminho from tab_arquivo a where a.id = :id; end;");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroSaida("caminho", DbType.String, 500);

				bancoDeDados.ExecutarNonQuery(comando);

				caminho = comando.ObterValorParametro("caminho").ToString();

				#endregion
			}

			return caminho;
		}

		public Arquivo Obter(int id, BancoDeDados banco = null)
		{
			Arquivo arquivo = new Arquivo();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, _esquema))
			{
				#region Arquivo
				Comando comando = bancoDeDados.CriarComando(@"
					select a.id,
						   a.nome,
						   a.caminho,
						   a.diretorio,
						   a.extensao,
						   a.tipo,
						   a.tamanho,
						   a.raiz,
						   a.tid,
						   c.raiz diretorioConfig
					  from tab_arquivo a, 
						   cnf_arquivo c
					 where a.raiz = c.id(+)
					   and a.id = :id");
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						arquivo.Id = id;
						arquivo.Tid = reader["tid"].ToString();
						arquivo.Nome = reader["nome"].ToString();
						arquivo.Caminho = reader["caminho"].ToString();
						arquivo.Diretorio = reader["diretorio"].ToString();
						arquivo.Extensao = reader["extensao"].ToString();
						arquivo.ContentType = reader["tipo"].ToString();
						arquivo.ContentLength = Convert.ToInt32(reader["tamanho"]);
						arquivo.Raiz = Convert.ToInt32(reader["raiz"]);
						arquivo.DiretorioConfiguracao = reader["diretorioConfig"].ToString();
					}

					reader.Close();
				}

				#endregion
			}

			return arquivo;
		}

		public Boolean Existe(int id, BancoDeDados banco = null)
		{			
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, _esquema))
			{
				#region Arquivo
				Comando comando = bancoDeDados.CriarComando(@"select count(a.id) qtd from tab_arquivo a, cnf_arquivo c where a.raiz = c.id(+) and a.id = :id");
				
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarScalar(comando).ToString() != "0";

				#endregion
			}
		}

		public void Salvar(Arquivo arquivo, int? executor, string executorNome, string executorLogin, int? executorTipo, string executorTid, BancoDeDados banco = null)
		{
			if (arquivo == null)
			{
				throw new Exception("Arquivo é nulo.");
			}

			if ((arquivo.Id ?? 0) <= 0)
			{
				Criar(arquivo, executor, executorNome, executorLogin, executorTipo, executorTid, banco);
			}
			else
			{
				Editar(arquivo, executor, executorNome, executorLogin, executorTipo, executorTid, banco);
			}
		}

		private void Criar(Arquivo arquivo, int? executor, string executorNome, string executorLogin, int? executorTipo, string executorTid, BancoDeDados banco = null)
		{
			if (banco == null)
			{
				GerenciadorTransacao.GerarNovoID();
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, _esquema))
			{
				#region Arquivo

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into tab_arquivo a (id, nome, caminho, diretorio, extensao, tipo, tamanho, raiz, tid, 
				executor_id, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao, executor_tid)
				values (seq_arquivo.nextval, :nome, :caminho, :diretorio, :extensao, :tipo, :tamanho, :raiz, :tid,
				:executor_id, :executor_nome, :executor_login, :executor_tipo_id, (select ltf.texto from lov_executor_tipo ltf where ltf.id = :executor_tipo_id), 
				(select la.id from lov_historico_artefatos_acoes la where la.acao = 1 and la.artefato = 6), systimestamp, :executor_tid) returning a.id into :id");

				comando.AdicionarParametroEntrada("nome", DbType.String, 250, arquivo.Nome);
				comando.AdicionarParametroEntrada("caminho", DbType.String, 500, arquivo.Caminho);
				comando.AdicionarParametroEntrada("diretorio", DbType.String, 500, arquivo.Diretorio);
				comando.AdicionarParametroEntrada("extensao", DbType.String, 10, arquivo.Extensao);
				comando.AdicionarParametroEntrada("tipo", DbType.String, 80, arquivo.ContentType);
				comando.AdicionarParametroEntrada("tamanho", arquivo.ContentLength, DbType.Int32);
				comando.AdicionarParametroEntrada("raiz", arquivo.Raiz, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				//
				comando.AdicionarParametroEntrada("executor_id", executor.HasValue ? executor : DBNull.Value as object, DbType.Int32);
				comando.AdicionarParametroEntrada("executor_nome", DbType.String, 80, executorNome);
				comando.AdicionarParametroEntrada("executor_login", DbType.String, 30, executorLogin);
				comando.AdicionarParametroEntrada("executor_tipo_id", executorTipo.HasValue ? executorTipo : DBNull.Value as object, DbType.Int32);
				comando.AdicionarParametroEntrada("executor_tid", DbType.String, 36, executorTid);

				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				arquivo.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				bancoDeDados.Commit();
			}
		}

		private void Editar(Arquivo arquivo, int? executor, string executorNome, string executorLogin, int? executorTipo, string executorTid, BancoDeDados banco = null)
		{
			if (banco == null)
			{
				GerenciadorTransacao.GerarNovoID();
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, _esquema))
			{
				#region Arquivo

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update tab_arquivo a set a.nome =:nome, a.caminho =:caminho, diretorio =:diretorio, 
				a.extensao =:extensao, tipo =:tipo, tamanho =:tamanho, raiz =:raiz, tid =:tid,
				executor_id =:executor_id, executor_nome=:executor_nome, executor_login=:executor_login, executor_tipo_id=:executor_tipo_id, 
				executor_tipo_texto = (select ltf.texto from lov_executor_tipo ltf where ltf.id = :executor_tipo_id), 
				acao_executada = (select la.id from lov_historico_artefatos_acoes la where la.acao = 2 and la.artefato = 6), 
				data_execucao = systimestamp, executor_tid =:executor_tid where a.id = :id");

				comando.AdicionarParametroEntrada("id", arquivo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("nome", DbType.String, 250, arquivo.Nome);
				comando.AdicionarParametroEntrada("caminho", DbType.String, 500, arquivo.Caminho);
				comando.AdicionarParametroEntrada("diretorio", DbType.String, 500, arquivo.Diretorio);
				comando.AdicionarParametroEntrada("extensao", DbType.String, 10, arquivo.Extensao);
				comando.AdicionarParametroEntrada("tipo", DbType.String, 80, arquivo.ContentType);
				comando.AdicionarParametroEntrada("tamanho", arquivo.ContentLength, DbType.Int32);
				comando.AdicionarParametroEntrada("raiz", arquivo.Raiz, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				//
				comando.AdicionarParametroEntrada("executor_id", executor.HasValue ? executor : DBNull.Value as object, DbType.Int32);
				comando.AdicionarParametroEntrada("executor_nome", DbType.String, 80, executorNome);
				comando.AdicionarParametroEntrada("executor_login", DbType.String, 30, executorLogin);
				comando.AdicionarParametroEntrada("executor_tipo_id", executorTipo.HasValue ? executorTipo : DBNull.Value as object, DbType.Int32);
				comando.AdicionarParametroEntrada("executor_tid", DbType.String, 36, executorTid);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				bancoDeDados.Commit();
			}
		}

		public int ObterSeparadorQtd(BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, _esquema))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) qtd, c.diretorio from tab_arquivo c 
					where c.diretorio = (select a.diretorio from tab_arquivo a where a.id = (select max(t.id) from tab_arquivo t) )
					group by c.diretorio");

				int num = 0;

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						int qtd = Convert.ToInt32(reader["qtd"]);
						string diretorio = reader["diretorio"].ToString();

						num = Int32.Parse(diretorio.Substring(diretorio.LastIndexOf("\\") + 1));

						if (qtd > 1000)
						{
							num++;
						}
					}

					reader.Close();

					return num;
				}
			}
		}

		public void MarcarDeletado(int arquivoId, string arquivoCaminho, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, _esquema))
			{
				#region Arquivo

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into tab_arquivo_excluido (id, caminho) values (:id, :caminho)");

				comando.AdicionarParametroEntrada("id", arquivoId, DbType.Int32);
				comando.AdicionarParametroEntrada("caminho", DbType.String, 500, arquivoCaminho);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				bancoDeDados.Commit();
			}
		}

		public void AtualizarCaminho(Arquivo arquivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, _esquema))
			{
				#region Arquivo

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update tab_arquivo a set a.caminho = :caminho where a.id = :id");

				comando.AdicionarParametroEntrada("id", arquivo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("caminho", DbType.String, 500, arquivo.Caminho);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				bancoDeDados.Commit();
			}
		}
	}
}