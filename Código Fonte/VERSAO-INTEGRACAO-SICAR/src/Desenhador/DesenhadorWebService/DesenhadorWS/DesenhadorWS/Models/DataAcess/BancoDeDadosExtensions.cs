using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;

namespace Tecnomapas.DesenhadorWS.Models.DataAcess
{
	public static class BancoDeDadosExtensions
	{
		public static Comando GetComandoPlSql(this BancoDeDados bancoDeDados, string sql)
		{
			if (!String.IsNullOrEmpty(sql))
			{
				sql = sql.Replace('\r', ' ');
			}

			return bancoDeDados.GetComandoSql(sql);
		}
	}
}