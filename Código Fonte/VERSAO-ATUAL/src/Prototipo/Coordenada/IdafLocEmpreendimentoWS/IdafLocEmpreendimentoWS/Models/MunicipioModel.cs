using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Tecnomapas.Blocos.Data;

namespace IdafLocEmpreendimentoWS.Models
{
    public class MunicipioModel
    {

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

        /*public Envelope[] BuscarEnvelopesMunicipios()
        {
            Comando comando = null;
            IDataReader reader = null;
            List<Envelope> envelopes = null;
            try
            {
                string schemaUsuarioGeo = ConfigurationManager.AppSettings["SchemaUsuarioGeo"];
                BancoDeDados bancoGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");

                string campoGeometrico = string.Empty;

                comando = bancoGeo.GetComandoSql(@"select a.objectid, a.nome, to_char(gerarenvelope(sdo_geom.sdo_mbr(a.shape))) envelope from " + schemaUsuarioGeo + @".lim_municipio15 a order by a.nome ");

                reader = bancoGeo.ExecutarReader(comando);
                if (reader == null) return null;

                envelopes = new List<Envelope>();
                Envelope envelope = null;
                while (reader.Read())
                {
                    envelope = new Envelope();

                    envelope.Id = Convert.ToInt32(reader["objectid"]);
                    envelope.Nome = Convert.ToString(reader["nome"]);
                    String[] env = Convert.ToString(reader["envelope"]).Split(';');
                    envelope.MinX = decimal.Parse(env[0]);
                    envelope.MinY = decimal.Parse(env[1]);
                    envelope.MaxX = decimal.Parse(env[2]);
                    envelope.MaxY = decimal.Parse(env[3]);
                    envelopes.Add(envelope);
                }
                reader.Close();
                reader.Dispose();
                comando.Dispose();
            }
            finally
            {
                if (comando != null)
                {
                    comando.Dispose();
                }
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }

            }
            return envelopes.ToArray();
        }




        public Envelope BuscarCoordenadasGeometriaMunicipio(int objectid)
        {
            Comando comando = null;
            IDataReader reader = null;
            Envelope envelope = null;
            try
            {
                string schemaUsuarioGeo = ConfigurationManager.AppSettings["SchemaUsuarioGeo"];
                BancoDeDados bancoGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");

                string campoGeometrico = string.Empty;

                comando = bancoGeo.GetComandoSql(@"select a.objectid, gerarenvelope(a.shape) geometria from lim_municipio15 a where a.objectid = :objectid");

                comando.AdicionarParametroEntrada("objectid", objectid, DbType.Int32);

                reader = bancoGeo.ExecutarReader(comando);
                if (reader == null) return null;

                envelope = new Envelope();

                if (reader.Read())
                {
                    envelope.Id = Convert.ToInt32(reader["objectid"]);
                    List<decimal[]> coords = new List<decimal[]>();

                    String[] coordenadas = reader["GEOMETRIA"].ToString().Split(';');

                    for (int i = 0; i < coordenadas.Length - 1; i += 2)
                    {
                        coords.Add(new decimal[] { decimal.Parse(coordenadas[i]), decimal.Parse(coordenadas[i + 1]) });
                    }

                    envelope.Coordenadas = coords.ToArray();
                }

                reader.Close();
                reader.Dispose();
                comando.Dispose();
            }
            finally
            {
                if (comando != null)
                {
                    comando.Dispose();
                }
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }

            }
            return envelope;
        }*/

    }
}