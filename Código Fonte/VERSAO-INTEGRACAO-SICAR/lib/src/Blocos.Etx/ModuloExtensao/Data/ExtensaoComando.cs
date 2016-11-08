using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;

namespace Tecnomapas.Blocos.Etx.ModuloExtensao.Data
{
	public static class ExtensaoComando
	{
		/*public static string AdicionarParametroArray2<T>(this Comando cmd, string prefixo, DbType tipo, List<T> Lst)
		{
			List<String> strParam = new List<string>();

			for (int i = 0; i < Lst.Count; i++)
			{
				cmd.AdicionarParametroEntrada(prefixo + i, Lst[i], tipo);
				strParam.Add(prefixo + i);
			}

			return String.Join(",", strParam);
		}*/

		public static string AdicionarNotIn<T>(this Comando cmd, string operador, string coluna, DbType tipo, List<T> Lst)
		{
            string retorno = string.Empty;
            int limiteItens = 999; //limite máximo de itens em uma operação IN ou NOT IN é de 1000 itens.

			if (Lst == null || Lst.Count == 0)
			{
				return string.Empty;
			}

			Lst.RemoveAll(x => x == null);

			if (Lst.Count == 0)
			{
				return string.Empty;
			}

			List<String> strParam = new List<string>();
			string pref = coluna.Replace(".", "pref");

			for (int i = 0; i < Lst.Count; i++)
			{
				cmd.AdicionarParametroEntrada(pref + i, Lst[i], tipo);
				strParam.Add(":" + pref + i);
			}

            if (Lst.Count < limiteItens)
            {
                retorno = String.Format(" {0} {1} not in ({2}) ", operador, coluna, String.Join(",", strParam));
            }
            else
            {
                retorno = String.Format(" {0} ( ", operador);
                int i = 0;
                while (i < strParam.Count)
                {
                    retorno += String.Format(" {0} not in (", coluna);

                    for (int j = i; ((j < i + limiteItens) && j < strParam.Count); j++)
                    {
                        retorno += strParam[j] + ",";
                    }

                    retorno = retorno.Substring(0, retorno.Length - 1);

                    retorno += (i + limiteItens < strParam.Count) ? ") or " : ") ";

                    if ((i + limiteItens) > strParam.Count)
                    {
                        i = strParam.Count;
                    }
                    else
                    {
                        i += limiteItens;
                    }
                }

                retorno += ")";
            }

            return retorno;
		}

		public static string AdicionarIn<T>(this Comando cmd, string operador, string coluna, DbType tipo, List<T> Lst)
		{
            string retorno = string.Empty;
            int limiteItens = 999;//limite máximo de itens em uma operação IN ou NOT IN é de 1000 itens.

			if (Lst == null || Lst.Count == 0)
			{
				return string.Empty;
			}

			Lst.RemoveAll(x => x == null);

			if (Lst.Count == 0)
			{
				return string.Empty;
			}

			List<String> strParam = new List<string>();
			string pref = coluna.Replace(".", "pref");

			for (int i = 0; i < Lst.Count; i++)
			{
				cmd.AdicionarParametroEntrada(pref + i, Lst[i], tipo);
				strParam.Add(":" + pref + i);
			}

            if (Lst.Count < limiteItens)
            {
                retorno = String.Format(" {0} {1} in ({2}) ", operador, coluna, String.Join(",", strParam));
            }
            else
            {
                retorno = String.Format(" {0} ( ", operador);
                int i = 0;
                while (i < strParam.Count)
                {
                    retorno += String.Format(" {0} in (", coluna);

                    for (int j = i; ((j < i + limiteItens) && j < strParam.Count); j++)
                    {
                        retorno += strParam[j] + ",";
                    }

                    retorno = retorno.Substring(0, retorno.Length - 1);

                    retorno += (i + limiteItens < strParam.Count) ? ") or " : ") ";

                    if ((i + limiteItens) > strParam.Count)
                    {
                        i = strParam.Count;
                    }
                    else
                    {
                        i += limiteItens;
                    }
                }

                retorno += ")";
            }

            return retorno;
		}

		public static DbType ToDbType(Object obj)
		{
			TypeCode tCode = Convert.GetTypeCode(obj);
			DbType dataType = DbType.Int32;

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

				case TypeCode.Empty:
				case TypeCode.Object:
				case TypeCode.DBNull:
				default:
					throw new Exception(String.Format("Tipo de objeto {0} não reconhecido para tipo de banco", tCode));
			}

			return dataType;
		}

		public static void AdicionarParametroEntrada(this Comando cmd, string nome, object valor)
		{
			if (valor == null)
			{
				return;
			}

			cmd.AdicionarParametroEntrada(nome, valor, ToDbType(valor));
		}

		public static void AdicionarParametroEntClob(this Comando cmd, string nome, String valor)
		{
			if (String.IsNullOrEmpty(valor))
			{
				cmd.AdicionarParametroEntrada(nome, DBNull.Value, DbType.String);
				return;
			}

			cmd.AdicionarParametroEntrada(nome, DbType.StringFixedLength, valor.Length, valor);
		}

		public static string FiltroAndLike(this Comando cmd, string coluna, string nome, string valor, bool upper = false, bool likeInicio = false)
		{
			if (String.IsNullOrWhiteSpace(valor))
			{
				return string.Empty;
			}

			cmd.AdicionarParametroEntrada(nome, (likeInicio ? "%" : string.Empty)+ valor.Replace("%", "") + "%", DbType.String);

			if (upper)
			{
				return String.Format(" and upper({0}) like upper(:{1})", coluna, nome);
			}
			else
			{
				return String.Format(" and {0} like :{1}", coluna, nome);
			}
		}

		public static string FiltroNull(this Comando cmd, string coluna, bool valor, bool isNull = true, string complemento = null)
		{			
			return (valor) ? String.Format(" and ({0} {1} {2})", coluna, (isNull) ? "is null" : "is not null", complemento) : string.Empty;
		}

		public static string FiltroAnd(this Comando cmd, string coluna, string nome, Object valor, string complemento = null, bool isDiferente = false)
		{
			if (valor == null)
			{
				return string.Empty;
			}

			DbType dbType = ToDbType(valor);

			if (!FiltroValidar(valor, dbType))
			{
				return string.Empty;
			}

			cmd.AdicionarParametroEntrada(nome, valor, DbType.String);

			if (isDiferente)
			{
				return String.Format(" and ({0} != :{1} {2})", coluna, nome, complemento);
			}

			return String.Format(" and ({0} = :{1} {2})", coluna, nome, complemento);
		}

		public static string FiltroIn(this Comando cmd, string coluna, string sql, string nome, Object valor, string complemento = null)
		{
			if (valor == null || valor.ToString() == "0")
			{
				return string.Empty;
			}

			DbType dbType = ToDbType(valor);

			if (!FiltroValidar(valor, dbType))
			{
				return string.Empty;
			}

			if (valor.ToString() == "0")
			{
				return string.Empty;
			}

			cmd.AdicionarParametroEntrada(nome, valor, ToDbType(valor));

			return String.Format(" and ({0} in (" + sql + ") {1})", coluna, complemento);
		}

		public static string FiltroNotIn(this Comando cmd, string coluna, string sql, string nome, Object valor, string complemento = null)
		{
			if (valor == null || valor.ToString() == "0")
			{
				return string.Empty;
			}

			DbType dbType = ToDbType(valor);

			if (!FiltroValidar(valor, dbType))
			{
				return string.Empty;
			}

			cmd.AdicionarParametroEntrada(nome, valor, ToDbType(valor));

			return String.Format(" and ({0} not in (" + sql + ") {1})", coluna, complemento);
		}

		public static bool FiltroValidar(Object valor, DbType tipoBanco)
		{
			switch (tipoBanco)
			{
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
				case DbType.Guid:
				case DbType.String:
					return !String.IsNullOrWhiteSpace(valor.ToString());

				case DbType.Date:
				case DbType.DateTime:
				case DbType.DateTime2:
				case DbType.DateTimeOffset:
					DateTime date = new DateTime();
					return DateTime.TryParse(valor.ToString(), out date);

				case DbType.Binary:
				case DbType.Boolean:
				case DbType.Byte:
					return Convert.ToInt32(valor) > 0;

				case DbType.Double:
					return Convert.ToDouble(valor) > 0;

				case DbType.Decimal:
				case DbType.Int16:
				case DbType.Int32:
				case DbType.Int64:
				case DbType.SByte:
				case DbType.Single:
				case DbType.UInt16:
				case DbType.UInt32:
				case DbType.UInt64:
					return Convert.ToDecimal(valor) > 0;

				case DbType.Object:
				case DbType.StringFixedLength:
				case DbType.Time:
				case DbType.VarNumeric:
				case DbType.Xml:
				default:
					throw new Exception(String.Format("Tipo de banco {0} não validado", tipoBanco));
			}
		}
	}
}
