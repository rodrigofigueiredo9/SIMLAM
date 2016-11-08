using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;

namespace Tecnomapas.EtramiteX.WindowsService.Relatorio.Model.Data
{
	public static class ExtencaoBancoDeDados
	{
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

		public static DbType GetType(this Comando comando, object tipo)
		{
			TypeCode tCode = Convert.GetTypeCode(tipo);
			System.Data.DbType dataType = DbType.Int32;

			switch (tCode)
			{
				case TypeCode.Boolean:
					dataType = DbType.Boolean;
					break;

				case TypeCode.Byte:
					dataType = DbType.Byte;
					break;

				case TypeCode.Char:
				case TypeCode.String:
					dataType = DbType.String;
					break;

				case TypeCode.DateTime:
					dataType = DbType.DateTime;
					break;

				case TypeCode.Decimal:
					dataType = DbType.Decimal;
					break;

				case TypeCode.Double:
					dataType = DbType.Double;
					break;

				case TypeCode.Int16:
					dataType = DbType.Int16;
					break;

				case TypeCode.Int32:
					dataType = DbType.Int32;
					break;

				case TypeCode.Int64:
					dataType = DbType.Int64;
					break;

				case TypeCode.SByte:
					dataType = DbType.SByte;
					break;

				case TypeCode.Single:
					dataType = DbType.Single;
					break;

				case TypeCode.UInt16:
					dataType = DbType.UInt16;
					break;

				case TypeCode.UInt32:
					dataType = DbType.UInt32;
					break;

				case TypeCode.UInt64:
					dataType = DbType.UInt64;
					break;
			}

			return dataType;
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

		public static T GetValue<T>(this IDataReader reader, string key)
		{
			if (reader.ContainsColumn(key) && reader[key] != null && reader[key] != DBNull.Value)
			{
				return (T)Convert.ChangeType(reader[key], typeof(T));
			}

			return default(T);
		}

		public static bool IsDBNull(this IDataReader reader, string key)
		{
			object valor = reader[key];
			return valor == null || Convert.IsDBNull(valor);
		}
	}
}