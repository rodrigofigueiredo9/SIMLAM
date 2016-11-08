using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;

namespace Tecnomapas.Blocos.Etx.ModuloCore.Data
{
	public class DaHelper
	{
		public static IEnumerable<IDataReader> ObterLista(Comando comando, BancoDeDados banco = null)
		{
			BancoDeDados bancoDeDados = null;
			IDataReader reader = null;

			try
			{
				bancoDeDados = BancoDeDados.ObterInstancia(banco);
				reader = bancoDeDados.ExecutarReader(comando);

				while (reader.Read())
				{
					yield return reader;
				}
			}
			finally
			{
				if (reader != null)
				{
					reader.Close();
					reader.Dispose();
				}

				if (bancoDeDados != null)
				{
					bancoDeDados.Dispose();
				}
			}
		}

		public static IEnumerable<IDataReader> ObterLista(string strComando, BancoDeDados banco = null)
		{
			BancoDeDados bancoDeDados = null;
			IDataReader reader = null;
			Comando comando = null;

			try
			{
				bancoDeDados = BancoDeDados.ObterInstancia(banco);
				comando = bancoDeDados.CriarComando(strComando);
				reader = bancoDeDados.ExecutarReader(comando);

				while (reader.Read())
				{
					yield return reader;
				}
			}
			finally
			{
				if (reader != null)
				{
					reader.Close();
					reader.Dispose();
				}

				if (comando != null)
				{
					comando.Dispose();
				}

				if (bancoDeDados != null)
				{
					bancoDeDados.Dispose();
				}
			}
		}

		public static String Ordenar(List<String> colunas, List<String> orderm, bool desc = false)
		{
			if (colunas == null || colunas.Count == 0)
			{
				throw new Exception("Colunas a serem ordenas não foram definidas pelo sistema");
			}

			if (orderm == null || orderm.Count == 0 ||
				orderm.Any(x => !colunas.Exists(y => y.Equals(x, StringComparison.InvariantCultureIgnoreCase))))
			{
				throw new Exception("Colunas de ordenação não existente");
			}

			return " order by " + String.Join(",", orderm) + (desc? " desc": "");
		}

		public static String FormatarSql(string sql, params string[] schemas)
		{
			IEnumerable<string> lstsChemas = schemas.Select(x => ((!string.IsNullOrEmpty(x)) ? String.Concat(x, ".") : String.Concat(x, "")));
			return String.Format(sql, lstsChemas.ToArray());
		}
	}
}