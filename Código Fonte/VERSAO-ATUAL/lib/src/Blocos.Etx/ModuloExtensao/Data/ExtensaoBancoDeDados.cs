using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Tecnomapas.Blocos.Data;

namespace Tecnomapas.Blocos.Etx.ModuloExtensao.Data
{
	public delegate void AcaoReader(IDataReader reader);
	public delegate void AcaoReader<T>(IDataReader reader, T obj);

	public static class ExtensaoBancoDeDados
	{
		public static Comando CriarComando(this BancoDeDados bancoDeDados, string sql, params string[] schemas)
		{
			if (schemas == null)
			{
				throw new Exception("Configuração de esquema é inválida");
			}

			IEnumerable<string> lstsChemas = schemas.Select(x => ((!string.IsNullOrEmpty(x)) ? String.Concat(x, ".") : String.Concat(x, "")));

			return bancoDeDados.CriarComando(String.Format(sql, lstsChemas.ToArray()));
		}

		public static Comando CriarComandoPlSql(this BancoDeDados bancoDeDados, string sql, params string[] schemas)
		{
			if (!String.IsNullOrEmpty(sql))
			{
				sql = sql.Replace('\r', ' ');
				//sql = sql.Replace('\n', ' ');
				//sql = sql.Replace('\t', ' ');
				//sql = sql.Replace("  ", " ");
			}

			return CriarComando(bancoDeDados, sql, schemas);
		}

		public static T ObterEntity<T>(this BancoDeDados bancoDados, Comando comando, AcaoReader<T> acao = null) where T : class
		{
			string strType = "";

			using (IDataReader reader = bancoDados.ExecutarReader(comando))
			{
				#region [ Tipo Classe ]

				T tObject = Activator.CreateInstance<T>();
				PropertyInfo[] props = typeof(T).GetProperties();

				if (reader.Read())
				{
					foreach (PropertyInfo prop in props)
					{
						if (reader.ContainsColumn(prop.Name))
						{
							strType = prop.PropertyType.ToString();

							if (strType.Contains("Nullable"))
							{
								strType = strType.Substring(strType.IndexOf('[') + 1, strType.IndexOf(']') - strType.IndexOf('[') - 1);
							}

							prop.SetValue(tObject, Convert.ChangeType(reader[prop.Name], Type.GetType(strType)), null);
						}
					}

					if (acao != null)
					{
						acao(reader, tObject);
					}
				}

				reader.Close();

				return tObject;

				#endregion
			}
		}

		public static T ObterEntity<T>(this IDataReader reader) where T : class
		{
			string strType = "";

			T tObject = Activator.CreateInstance<T>();
			PropertyInfo[] props = typeof(T).GetProperties();

			foreach (PropertyInfo prop in props)
			{
				if (reader.ContainsColumn(prop.Name))
				{
					strType = prop.PropertyType.ToString();

					if (strType.Contains("Nullable"))
					{
						strType = strType.Substring(strType.IndexOf('[') + 1, strType.IndexOf(']') - strType.IndexOf('[') - 1);
					}

					prop.SetValue(tObject, Convert.ChangeType(reader[prop.Name], Type.GetType(strType)), null);
				}
			}


			return tObject;
		}

		public static List<T> ObterEntityList<T>(this BancoDeDados bancoDados, Comando comando, AcaoReader<T> acao = null)
		{
			using (IDataReader reader = bancoDados.ExecutarReader(comando))
			{
				List<T> lstRetorno = new List<T>();
				T tObject;
				string strType = "";

				#region Tipo Primitivo

				if (reader.FieldCount == 1)
				{
					while (reader.Read())
					{
						lstRetorno.Add((T)Convert.ChangeType(reader[0], typeof(T)));

						if (acao != null)
						{
							acao(reader, lstRetorno.Last());
						}
					}

					reader.Close();

					return lstRetorno;
				}

				#endregion

				#region Tipo Classe

				PropertyInfo[] props = typeof(T).GetProperties();

				while (reader.Read())
				{
					tObject = Activator.CreateInstance<T>();

					foreach (PropertyInfo prop in props)
					{
						if (reader.ContainsColumn(prop.Name))
						{
							strType = prop.PropertyType.ToString();

							if (strType.Contains("Nullable"))
							{
								strType = strType.Substring(strType.IndexOf('[') + 1, strType.IndexOf(']') - strType.IndexOf('[') - 1);
							}

							prop.SetValue(tObject, Convert.ChangeType(reader[prop.Name], Type.GetType(strType)), null);

						}
					}

					lstRetorno.Add(tObject);
					if (acao != null)
					{
						acao(reader, lstRetorno.Last());
					}
				}

				reader.Close();

				return lstRetorno;

				#endregion
			}
		}

		public static bool ContainsColumn(this IDataReader reader, string key)
		{
			for (int i = 0; i < reader.FieldCount; i++)
			{
				if (reader.GetName(i).ToLower() == key.ToLower())
				{
					return reader[key] != null && reader[key] != DBNull.Value;
				}
			}

			return false;
		}

		public static Dictionary<string, object> ExecutarDictionary(this BancoDeDados bancoDeDados, Comando comando)
		{
			Dictionary<string, object> dic = new Dictionary<string, object>();
			using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
			{
				if (reader.Read())
				{
					dic = reader.CriarDictionary();
				}
				reader.Close();
			}
			return dic;
		}

		public static List<Dictionary<string, object>> ExecutarDictionaryList(this BancoDeDados bancoDeDados, Comando comando)
		{
			List<Dictionary<string, object>> dic = new List<Dictionary<string, object>>();
			using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
			{
				while (reader.Read())
				{
					dic.Add(reader.CriarDictionary());
				}
				reader.Close();
			}
			return dic;
		}

		public static void ExecutarReader(this BancoDeDados bancoDeDados, Comando comando, AcaoReader acao)
		{
			using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
			{
				while (reader.Read())
				{
					acao(reader);
				}

				reader.Close();
			}
		}
	}
}