using log4net;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Data;

namespace Tecnomapas.EtramiteX.Scheduler.models.misc
{
	/// <summary>
	/// Adaptação dos métodos de salvar arquivo do Blocos.Arquivo.Data e GerenciadorArquivo. Modificados para uso neste projeto sem intervenção de usuário.
	/// </summary>
	public class ArquivoManager
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public string Salvar(string arquivoNome, FileStream arquivoStream, OracleConnection conn)
		{
			var diretorio = BuscarDiretorioArquivo(conn);

			var diretoriosCopy = diretorio.ToDictionary(x => x.Key, y => y.Value);
			var atual = diretoriosCopy.First();

			var arquivo = new Arquivo
			{
				Nome = arquivoNome,
				TemporarioNome = Path.GetRandomFileName(),
				Diretorio = GerarPathFinal(ObterSeparadorQtd(conn)),
				Raiz = atual.Key,
				ContentType = "application / octet - stream"
			};
			arquivo.Caminho = String.Join("\\", new[] {atual.Value, arquivo.Diretorio, arquivo.TemporarioNome});
			arquivo.Extensao = Path.GetExtension(arquivo.TemporarioNome);

			if (!Directory.Exists(Path.GetDirectoryName(arquivo.Caminho)))
				Directory.CreateDirectory(Path.GetDirectoryName(arquivo.Caminho));


			const int numeroTentativas = 3;
			const int delayParaNovaTentativa = 1000;

			for (var i = 1; i <= numeroTentativas; ++i)
			{
				try
				{
					SalvarNoDisco(arquivoStream, arquivo.Caminho);
					break;
				}
				catch (IOException)
				{
					if (i == numeroTentativas) // Ao final, tentar uma última vez
						throw;

					Thread.Sleep(delayParaNovaTentativa);
				}
			}
			
			SalvarNoBanco(conn, arquivo);

			return arquivo.Id.ToString();
		}

		private static void SalvarNoDisco(Stream st, string fileName)
		{
			if (st == null)
				throw new ArgumentNullException("Stream de arquivo é nulo.");

			FileStream fs = null;

			try
			{
				fs = File.Create(fileName);
				st.Seek(0, SeekOrigin.Begin);
				st.CopyTo(fs);
				fs.Flush();
			}
			finally
			{
				if (fs != null)
				{
					fs.Close();
					fs.Dispose();
				}

				st.Close();
				st.Dispose();
			}
		}

		private static void SalvarNoBanco(OracleConnection conn,Arquivo arquivo)
		{
			var schema = CarUtils.GetEsquemaInstitucional();

			var sqlBuilder = new StringBuilder();
			sqlBuilder.Append("INSERT INTO " + schema + ".TAB_ARQUIVO ");
			sqlBuilder.Append("(id, nome, caminho, diretorio, extensao, tipo, tamanho, raiz, tid,");
			sqlBuilder.Append("executor_nome, executor_login, executor_tid,");
			sqlBuilder.Append("acao_executada, data_execucao) ");
			sqlBuilder.Append("VALUES ");
			sqlBuilder.Append("(" + schema + ".seq_arquivo.nextval, :nome, :caminho, :diretorio,");
			sqlBuilder.Append(":extensao, :tipo, :tamanho, :raiz, :tid,");
			sqlBuilder.Append(":executor_nome, :executor_login, :executor_tid,");
			sqlBuilder.Append(":acao_executada, CURRENT_TIMESTAMP) ");
			sqlBuilder.Append("RETURNING id INTO :novo_id");

			try
			{
				using (var cmd = new OracleCommand(sqlBuilder.ToString(), conn))
				{
					cmd.Parameters.Add(new OracleParameter("nome", arquivo.Nome));
					cmd.Parameters.Add(new OracleParameter("caminho", arquivo.Caminho));
					cmd.Parameters.Add(new OracleParameter("diretorio", arquivo.Diretorio));
					cmd.Parameters.Add(new OracleParameter("extensao", arquivo.Extensao));
					cmd.Parameters.Add(new OracleParameter("tipo", arquivo.ContentType));
					cmd.Parameters.Add(new OracleParameter("tamanho", arquivo.ContentLength));
					cmd.Parameters.Add(new OracleParameter("raiz", arquivo.Raiz));
					cmd.Parameters.Add(new OracleParameter("tid", GerenciadorTransacao.ObterIDAtual()));

					cmd.Parameters.Add(new OracleParameter("executor_nome", "idaf-scheduler"));
					cmd.Parameters.Add(new OracleParameter("executor_login", "idaf-scheduler"));
					cmd.Parameters.Add(new OracleParameter("executor_tid", GerenciadorTransacao.ObterIDAtual()));
					cmd.Parameters.Add(new OracleParameter("acao_executada", 19));
					
					cmd.Parameters.Add(new OracleParameter("novo_id", OracleDbType.Decimal, ParameterDirection.ReturnValue));

					cmd.ExecuteNonQuery();

					arquivo.Id = Convert.ToInt32(cmd.Parameters["novo_id"].Value.ToString());
				}
			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados:" + exception.Message, exception);
			}
		}
		
		private Dictionary<int, string> BuscarDiretorioArquivo(OracleConnection conn)
		{
			var schema = CarUtils.GetEsquemaInstitucional();

			var result = new Dictionary<int, string>();

			using (var cmd = new OracleCommand("SELECT c.id, c.raiz FROM " + schema + ".CNF_ARQUIVO c WHERE c.ativo = 1 AND tipo = 2", conn))
			{
				using (var dr = cmd.ExecuteReader())
				{
					while (dr.Read())
					{
						result.Add(Convert.ToInt32(dr["id"]), dr["raiz"].ToString());
					}
				}
			}

			return result;
		}

		private string GerarPathFinal(string divPorQtd)
		{
			var anoMes = DateTime.Now.ToString("yyyy\\\\MM");
			var semanaDoAno = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday).ToString();

			return String.Join("\\", new[] { anoMes, semanaDoAno, divPorQtd }).Replace("\\\\", "\\");
		}

		private string ObterSeparadorQtd(OracleConnection conn)
		{
			var schema = CarUtils.GetEsquemaInstitucional();

			var result = 0;

			using (
				var cmd =
					new OracleCommand(
						"SELECT count(*) qtd, c.diretorio FROM " + schema + ".tab_arquivo c WHERE c.diretorio = (SELECT a.diretorio FROM " +
						schema + ".tab_arquivo a WHERE a.id = (SELECT max(t.id) FROM " + schema + ".tab_arquivo t)) group by c.diretorio",
						conn))
			{
				using (var dr = cmd.ExecuteReader())
				{
					while (dr.Read())
					{
						var qtd = Convert.ToInt32(dr["qtd"]);
						var diretorio = dr["diretorio"].ToString();

						result = Int32.Parse(diretorio.Substring(diretorio.LastIndexOf("\\") + 1));

						if (qtd > 1000)
						{
							result++;
						}
					}
				}
			}

			return result.ToString();
		}



		public string BuscarDiretorioArquivoTemporario(OracleConnection conn)
		{
			var schema = CarUtils.GetEsquemaInstitucional();

			using (var cmd = new OracleCommand("SELECT c.raiz FROM " + schema + ".CNF_ARQUIVO c WHERE c.ativo = 1 AND tipo = 1", conn))
			{
				using (var dr = cmd.ExecuteReader())
				{
					if (dr.Read())
					{
						return dr["raiz"].ToString();
					}
				}
			}

			return string.Empty;
		}
	}
}
