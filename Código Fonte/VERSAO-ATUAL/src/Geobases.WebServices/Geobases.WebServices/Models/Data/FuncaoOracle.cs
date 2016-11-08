using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Oracle.DataAccess.Client;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.TecnoGeo.Acessadores;

namespace Tecnomapas.Geobases.WebServices.Models.Data
{
	public class FuncaoOracle: Expressao
	{
		Object[] _parametros;
		string _funcaoFormato;
		const string FORMATO_NOME_PARAMETRO = "{0}_F{1}";


		public FuncaoOracle(String format, params Object[] parametros)
		{
			_funcaoFormato = format;
			_parametros = parametros ?? new Object[0];
		}

		protected override void CarregarParametros(OracleCommand cmd, ref int param, string nome)
		{
			for (int i = 0; i < _parametros.Length; i++)
			{
				int size = 0;

				if (_parametros[i] is String && !String.IsNullOrEmpty(_parametros[i].ToString()))
				{
					size = _parametros[i].ToString().Length;
				}
				
				cmd.Parameters.Add(String.Format(FORMATO_NOME_PARAMETRO, nome, i), TipoOracle(ExtensaoComando.ToDbType(_parametros[i])), _parametros[i], System.Data.ParameterDirection.Input);
				cmd.Parameters[String.Format(FORMATO_NOME_PARAMETRO, nome, i)].Size = size;

				param++;
			}
		}

		protected override string GerarComando(ref int param, string nome)
		{
			if (_parametros == null || _parametros.Length == 0)
			{
				return String.Empty;
			}

			List<String> lstParans = new List<string>();

			for (int i = 0; i < _parametros.Length; i++)
			{
				lstParans.Add(String.Format(":"+FORMATO_NOME_PARAMETRO, nome, i));
				param++;
			}

			return String.Format(_funcaoFormato, lstParans.ToArray());
		}

		private OracleDbType TipoOracle(DbType tipo)
		{
			OracleDbType resp;

			switch (tipo)
			{
				case System.Data.DbType.Date:
				case System.Data.DbType.DateTime:
					resp = OracleDbType.Date;
					break;
				case System.Data.DbType.Boolean:
					resp = OracleDbType.Int16;
					break;
				case System.Data.DbType.Single:
				case System.Data.DbType.Double:
				case System.Data.DbType.Decimal:
					resp = OracleDbType.Decimal;
					break;
				case System.Data.DbType.Int16:
				case System.Data.DbType.UInt16:
					resp = OracleDbType.Decimal;
					break;
				case System.Data.DbType.Int32:
				case System.Data.DbType.UInt32:
					resp = OracleDbType.Decimal;
					break;
				case System.Data.DbType.Int64:
				case System.Data.DbType.UInt64:
					resp = OracleDbType.Decimal;
					break;
				case System.Data.DbType.String:
					resp = OracleDbType.Clob;
					break;
				case DbType.Object:
					resp = OracleDbType.Clob;
					break;
				default:
					throw new Exception("Tipo de atributo incorreto \"" + tipo.ToString() + "\"");
			}

			return resp;
		}
	}
}