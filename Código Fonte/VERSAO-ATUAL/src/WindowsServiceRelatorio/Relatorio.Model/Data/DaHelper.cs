using System;
using Tecnomapas.Blocos.Data;

namespace Tecnomapas.EtramiteX.WindowsService.Relatorio.Model.Data
{
	public static class DaHelper
	{
		public static void SetarValorParametroClob(this Comando comando, string parametro, object valor)
		{
			if (valor is string && String.IsNullOrEmpty(valor.ToString()))
			{
				comando.DbCommand.Parameters[parametro].DbType = System.Data.DbType.StringFixedLength;
				comando.DbCommand.Parameters[parametro].Value = valor;
				comando.DbCommand.Parameters[parametro].Size = valor.ToString().Length;
			}
			else
			{
				comando.DbCommand.Parameters[parametro].DbType = System.Data.DbType.String;
				comando.DbCommand.Parameters[parametro].Value = DBNull.Value;
				comando.DbCommand.Parameters[parametro].Size = 0;
			}
		}
	}
}