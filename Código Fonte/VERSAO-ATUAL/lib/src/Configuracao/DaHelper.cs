using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Data
{
	internal class DaHelper
	{
		public static IEnumerable<IDataReader> ObterLista(Comando comando)
		{
			BancoDeDados bancoDeDados = null;
			IDataReader reader = null;

			try
			{
				bancoDeDados = BancoDeDados.ObterInstancia();
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

		public static List<string> tablesCache = new List<string>() { "tab_profissao", "tab_cargo", "tab_setor", "tab_funcionario", "tab_orgao_classe", "tab_atividade", "tab_meio_contato"};

		public static IEnumerable<IDataReader> ObterLista(string strComando, string valor = null, string schema = "default")
		{
			if (strComando.IndexOf("tab_", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
				( strComando.IndexOf("crt_", StringComparison.InvariantCultureIgnoreCase) >= 0 && 
					strComando.IndexOf("lov_crt_", StringComparison.InvariantCultureIgnoreCase) < 0 ))
			{
				if (!tablesCache.Any(x => strComando.IndexOf(x, StringComparison.InvariantCultureIgnoreCase) >= 0))
				{
					throw new Exception("Não é permitido fazer CACHE de TABELA!");
				}
			}

			BancoDeDados bancoDeDados = null;
			IDataReader reader = null;
			Comando comando = null;

			try
			{
				bancoDeDados = BancoDeDados.ObterInstancia(schema);
				comando = bancoDeDados.CriarComando(strComando);

				if (valor != null && !string.IsNullOrEmpty(valor))
				{
					comando.AdicionarParametroEntrada("valor", valor, DbType.String);
				}

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
	}
}