using System;
using System.Collections.Generic;
using System.Data;

namespace Tecnomapas.Blocos.Etx.ModuloExtensao.Data
{
	public static class ExtensaoIDataReader
	{
		public static T GetValue<T>(this IDataReader reader, string key)
		{
			string strType = "";

			if (reader[key] != null && reader[key] != DBNull.Value)
			{
				strType = typeof(T).ToString();

			if (strType.Contains("Nullable"))
				{
					strType = strType.Substring(strType.IndexOf('[') + 1, strType.IndexOf(']') - strType.IndexOf('[') - 1);
				}

				return (T)Convert.ChangeType(reader[key], Type.GetType(strType));
			}
			return default(T);
		}

		public static Dictionary<string, object> CriarDictionary(this IDataReader reader)
		{
			Dictionary<string, object> dic = new Dictionary<string, object>();
			if (reader.FieldCount > 0)
			{
				for (var i = 0; i < reader.FieldCount; i++)
				{
					dic.Add(reader.GetName(i), reader.GetValue(i));
				}
			}
			return dic;
		}

		public static Dictionary<string, T> CriarDictionary<T>(this IDataReader reader)
		{
			Dictionary<string, T> dic = new Dictionary<string, T>();
			if (reader.FieldCount > 0)
			{
				for (var i = 0; i < reader.FieldCount; i++)
				{
					dic.Add(reader.GetName(i), (T)reader.GetValue<T>(reader.GetName(i)));
				}
			}
			return dic;
		}
	}
}