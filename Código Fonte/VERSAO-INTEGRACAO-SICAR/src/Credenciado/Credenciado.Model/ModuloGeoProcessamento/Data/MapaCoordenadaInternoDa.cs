using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloGeoProcessamento;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloGeoProcessamento.Data
{
	public class MapaCoordenadaInternoDa
	{
		private string EsquemaBancoGeo { get; set; }

		public MapaCoordenadaInternoDa(string strBancoDeDadosGeo = null)
		{
			EsquemaBancoGeo = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDadosGeo))
			{
				EsquemaBancoGeo = strBancoDeDadosGeo;
			}
		}

		public Lote ObterLote(string codfiscal, string codquadra, string codlote)
		{
			Lote result = null;

			using (BancoDeDados bd = BancoDeDados.ObterInstancia())
			{
				string queryStr =
					@"SELECT l.objectid,l.cod_sfisca,l.cod_quadra,l.cod_lote,l.nom_propri,l.tpo_lograd,l.nom_lograd,l.num_numera,l.nom_bairro 
					FROM {0}lotes l WHERE l.cod_sfisca = :codfiscal AND l.cod_quadra = :codquadra AND l.cod_lote = :codlote";

				Comando comando = bd.CriarComando(queryStr, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("codfiscal", codfiscal, DbType.String);
				comando.AdicionarParametroEntrada("codquadra", codquadra, DbType.String);
				comando.AdicionarParametroEntrada("codlote", codlote, DbType.String);

				using (DbDataReader reader = bd.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						result = new Lote();

						result.id = Convert.ToInt32(reader["objectid"]);

						result.codfiscal = Convert.ToInt32(reader["cod_sfisca"]);
						result.codquadras = Convert.ToInt32(reader["cod_quadra"]);
						result.codlotes = Convert.ToInt32(reader["cod_lote"]);

						result.proprietario = Convert.ToString(reader["nom_propri"]);

						result.tipo_logradouro = Convert.ToString(reader["tpo_lograd"]);

						result.logradouro = Convert.ToString(reader["nom_lograd"]);
						result.numero = Convert.ToInt32(reader["num_numera"]);

						result.bairro = Convert.ToString(reader["nom_bairro"]);
					}
					reader.Close();
				}
			}

			if (result != null)
			{
				ObterLoteXY(result);
			}
			else
			{
				result = new Lote();
			}

			return result;
		}

		private void ObterLoteXY(Lote lote)
		{
			List<Decimal> ordernadas = new List<decimal>();

			using (BancoDeDados bd = BancoDeDados.ObterInstancia())
			{
				string queryStr =
					"SELECT column_value ordenada FROM table (SELECT sdo_geom.sdo_mbr(t.geometry).sdo_ordinates FROM {0}lotes t WHERE t.objectid = :id AND rownum = 1)";

				Comando comando = bd.CriarComando(queryStr, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("id", lote.id, DbType.Int32);

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

			lote.x = (easting1 + easting2)/2;
			lote.y = (northing1 + northing2)/2;
		}

		public List<Logradouro> ObterLogradouros(string logradouro)
		{
			List<Logradouro> listResult = new List<Logradouro>();

			using (BancoDeDados bd = BancoDeDados.ObterInstancia())
			{
				string queryStr = "SELECT t.objectid, t.tpo_lograd, t.nom_lograd FROM {0}logradouros t ";

				if (logradouro != "")
				{
					queryStr += "WHERE ";
				}

				List<string> conditions = new List<string>();

				if (logradouro != "")
				{
					conditions.Add("UPPER(t.nom_lograd) LIKE UPPER('%'||:logradouro||'%') ");
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
				queryStr += "ORDER BY t.nom_lograd";

				Comando comando = bd.CriarComando(queryStr, EsquemaBancoGeo);

				if (logradouro != "")
				{
					comando.AdicionarParametroEntrada("logradouro", logradouro, DbType.String);
				}

				using (DbDataReader reader = bd.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						Logradouro result = new Logradouro();
						result.id = Convert.ToInt32(reader["objectid"]);
						result.tipo_logradouro = Convert.ToString(reader["tpo_lograd"]);
						result.nome = Convert.ToString(reader["nom_lograd"]);
						listResult.Add(result);
					}
					reader.Close();
				}
			}

			foreach (Logradouro logr in listResult)
			{
				ObterLogradouroXY(logr);
			}

			return listResult;
		}

		private void ObterLogradouroXY(Logradouro logradouro)
		{
			List<Decimal> ordernadas = new List<decimal>();

			using (BancoDeDados bd = BancoDeDados.ObterInstancia())
			{
				string queryStr =
					"SELECT column_value ordenada FROM table (SELECT sdo_geom.sdo_mbr(t.geometry).sdo_ordinates FROM {0}logradouros t WHERE t.objectid = :id AND rownum = 1)";

				Comando comando = bd.CriarComando(queryStr, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("id", logradouro.id, DbType.Int32);

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

			logradouro.x = (easting1 + easting2)/2;
			logradouro.y = (northing1 + northing2)/2;
		}
	}
}