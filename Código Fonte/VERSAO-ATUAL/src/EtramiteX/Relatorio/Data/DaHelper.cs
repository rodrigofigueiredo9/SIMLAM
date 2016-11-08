using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.Data
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

		public static String Ordenar(List<String> colunas, List<String> orderm)
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

			return " order by " + String.Join(",", orderm);
		}

		#region Converter

		public static decimal ConverterParaDecimal(object valor)
		{
			if (Convert.IsDBNull(valor) && (valor == null))
			{
				return 0;
			}

			try
			{
				return Convert.ToDecimal(valor);
			}
			catch
			{
				return 0;
			}
		}

		public static int ConverterParaInt32(object valor)
		{
			if (valor == null && !Convert.IsDBNull(valor))
			{
				return 0;
			}

			try
			{
				return Convert.ToInt32(valor);
			}
			catch
			{
				return 0;
			}
		}

		public static DateTime ConverterParaDateTime(object valor)
		{
			if (valor is DBNull)
				return DateTime.MinValue;

			try
			{
				return (Convert.ToDateTime(valor));
			}
			catch
			{
				return DateTime.MinValue;
			}
		}

		#endregion
	}
}