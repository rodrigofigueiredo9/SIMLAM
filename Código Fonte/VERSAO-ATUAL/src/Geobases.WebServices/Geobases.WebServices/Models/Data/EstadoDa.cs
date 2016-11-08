using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Geobases.WebServices.Models.Entities;
using System.Configuration;

namespace Tecnomapas.Geobases.WebServices.Models.Data
{
	public class EstadoDa
	{
		public bool EstaNoEstado(decimal easting, decimal northing)
		{
			int srid = Convert.ToInt32(ConfigurationManager.AppSettings["srid"]);

			using (BancoDeDados bd = BancoDeDados.ObterInstancia())
			{
				string queryStr = string.Format(@"select count(*) from LIM_ESTADUAL m where 
					sdo_relate(m.shape, mdsys.sdo_geometry(2001, {0}, mdsys.sdo_point_type(:easting, :northing, null), null, null), 'MASK=ANYINTERACT') = 'TRUE'", srid);

				Comando comando = bd.CriarComando(queryStr);
				comando.AdicionarParametroEntrada("easting", easting, DbType.Decimal);
				comando.AdicionarParametroEntrada("northing", northing, DbType.Decimal);

				return Convert.ToBoolean(bd.ExecutarScalar(comando));
			}
		}
	}
}