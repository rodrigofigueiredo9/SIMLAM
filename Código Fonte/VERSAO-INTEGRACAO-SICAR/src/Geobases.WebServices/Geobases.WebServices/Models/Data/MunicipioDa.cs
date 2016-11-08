using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Geobases.WebServices.Models.Entities;
using System.Configuration;

namespace Tecnomapas.Geobases.WebServices.Models.Business
{
    public class MunicipioDa
    {
		private void getMunicipioEnvelope(Municipio municipio)
		{
			List<Decimal> ordernadas = new List<decimal>();

			using (BancoDeDados bd = BancoDeDados.ObterInstancia())
			{
				string queryStr = "SELECT column_value ordenada FROM table (SELECT sdo_geom.sdo_mbr(t.shape).sdo_ordinates FROM lim_municipio15 t WHERE t.objectid = :id AND rownum = 1)";

				Comando comando = bd.CriarComando(queryStr);

				comando.AdicionarParametroEntrada("id", municipio.id, DbType.Int32);

				using (DbDataReader reader = bd.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						ordernadas.Add(Convert.ToDecimal(reader["ordenada"]));
					}
					reader.Close();
				}
			}

			Decimal easting1 = ordernadas[0];
			Decimal northing1 = ordernadas[1];
			Decimal easting2 = ordernadas[2];
			Decimal northing2 = ordernadas[3];

			municipio.x = (easting1 + easting2) / 2;
			municipio.y = (northing1 + northing2) / 2;
		}

        public List<Municipio> listarMunicipios(string municipio)
        {
            List<Municipio> listResult = new List<Municipio>();

            using (BancoDeDados bd = BancoDeDados.ObterInstancia())
            {
                string queryStr = "SELECT t.objectid, t.nome FROM lim_municipio15 t ";

                if (municipio != "")
                {
                    queryStr += "WHERE ";
                }

                List<string> conditions = new List<string>();

                if (municipio != "")
                {
                    conditions.Add("UPPER(t.nome) LIKE UPPER('%'||:municipio||'%') ");
                }

                int m = conditions.Count;

                if (m != 0)
                {
                    queryStr += conditions[0];
                    for (int i = 1; i < m; i++)
                    {
                        queryStr += "AND " + conditions[i];
                    }
                }
                queryStr += "ORDER BY t.nome";

                Comando comando = bd.CriarComando(queryStr);

                if (municipio != "")
                {
                    comando.AdicionarParametroEntrada("municipio", municipio, DbType.String);
                }

                using (DbDataReader reader = bd.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        Municipio result = new Municipio();
                        result.id = Convert.ToInt32(reader["objectid"]);
                        result.nome = Convert.ToString(reader["nome"]);
                        listResult.Add(result);
                    }
                    reader.Close();
                }
            }

            foreach (Municipio mun in listResult)
            {
                getMunicipioEnvelope(mun);
            }

            return listResult;
        }

		public Municipio ObterMunicipio(decimal easting, decimal northing)
		{
			Municipio result = null;

            int srid = Convert.ToInt32(ConfigurationManager.AppSettings["srid"]);
			using (BancoDeDados bd = BancoDeDados.ObterInstancia())
			{
				string queryStr = string.Format(@"select m.cod_ibge, m.nome from lim_municipio15 m where 
					sdo_relate(m.shape, mdsys.sdo_geometry(2001, {0}, mdsys.sdo_point_type(:easting, :northing, null), null, null), 'MASK=ANYINTERACT') = 'TRUE'", srid); ;

				Comando comando = bd.CriarComando(queryStr);
				comando.AdicionarParametroEntrada("easting", easting, DbType.Decimal);
				comando.AdicionarParametroEntrada("northing", northing, DbType.Decimal);

				using (DbDataReader reader = bd.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						result = new Municipio();
						result.IBGE = reader["cod_ibge"].ToString();
						result.nome = Convert.ToString(reader["nome"]);
					}
					reader.Close();
				}
			}

			return result;
		}
    }
}