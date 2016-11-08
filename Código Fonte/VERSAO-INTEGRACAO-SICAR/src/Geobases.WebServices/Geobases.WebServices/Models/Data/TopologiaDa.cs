using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.TecnoGeo.Acessadores.OracleSpatial;
using System.Configuration;
using Tecnomapas.TecnoGeo.Acessadores.Shape;
using Tecnomapas.TecnoGeo.Acessadores;
using Tecnomapas.TecnoGeo.Geografico;
using System.Data;

namespace Tecnomapas.Geobases.WebServices.Models.Data
{
	public class TopologiaDa
	{
		private FonteFeicaoOracleSpatial GetDatabaseFontFeicao(string connectionKey = "default")
		{
			FonteFeicaoOracleSpatial fonteFeicao = new FonteFeicaoOracleSpatial();
			bool[] parameters = new bool[] { false, false, false };


			string[] arrayStrConnection = ConfigurationManager.ConnectionStrings[connectionKey].ConnectionString.Split(';');

			for (int i = 0; i < arrayStrConnection.Length; i++)
			{
				string[] param = arrayStrConnection[i].Split('=');

				param[0] = param[0].ToLower();

				if (param[0].IndexOf("source") >= 0)
				{
					fonteFeicao.Fonte = param[1];
					parameters[0] = true;
				}
				else if (param[0].IndexOf("user") >= 0)
				{
					fonteFeicao.Usuario = param[1];
					parameters[1] = true;
				}
				else if (param[0].IndexOf("password") >= 0)
				{
					fonteFeicao.Senha = param[1];
					parameters[2] = true;
				}
			}


			if (!(parameters[0] && parameters[1] && parameters[2]))
			{
				throw new Exception("Conexão não encontrada.");
			}

			return fonteFeicao;
		}

		public Dictionary<string, List<Feicao>> Relacao(List<string> feicoes, string wkt)
		{
			FonteFeicaoOracleSpatial origem = GetDatabaseFontFeicao();
			Dictionary<string, List<Feicao>> lstRetorno = new Dictionary<string, List<Feicao>>();

			int srid = Convert.ToInt32(ConfigurationManager.AppSettings["srid"]);
			origem.Abrir();

			LeitorFeicao leitor = null;
			ClasseFeicao classeFeicao = null;

			Expressao geometria = new FuncaoOracle("addsrid(mdsys.sdo_util.from_wktgeometry({0}), {1})", wkt, srid);

			Expressao filtro;

			List<Feicao> lstGeoAtributos = null;

			for (int i = 0; i < feicoes.Count; i++)
			{
				lstGeoAtributos = new List<Feicao>();
				string feicao = feicoes[i];

				try
				{
					classeFeicao = origem.ObterClasseFeicao(feicao);
				}
				catch
				{
					classeFeicao = null;
				}

				if (classeFeicao == null)
					continue;

				string campoGeo = classeFeicao.CampoGeometrico;

				filtro = new ExpressaoRelacionalOracleSpatial(new OperacaoEspacialRelacao(new Campo(campoGeo), geometria, TipoRelacaoEspacial.ANYINTERACT), TipoOperadorRelacional.Igual, new ConstanteOracleSpatial(DbType.String, "TRUE"));

				try
				{
					leitor = origem.ObterLeitorFeicao(feicao, null, filtro);
				}
				catch (Exception)
				{
					for (int k = 0; k < 5; k++)
					{
						try
						{
							ExpressaoRelacionalOracleSpatial termoAux = new ExpressaoRelacionalOracleSpatial(new Campo("1"), TipoOperadorRelacional.Igual, new Campo("1"));
							filtro = new ExpressaoLogicaOracleSpatial(filtro, TipoOperadorLogico.E, termoAux);
							leitor = origem.ObterLeitorFeicao(feicao, null, filtro);
						}
						catch (Exception)
						{
							if (k < 4)
							{
								continue;
							}

							throw;
						}

						break;
					}
				}

				while (leitor.Ler())
				{
					if (leitor.Atual.Geometria == null)
						continue;

					lstGeoAtributos.Add(leitor.Atual);
				}

				leitor.Fechar();

				if (lstGeoAtributos.Count > 0)
				{
					lstRetorno.Add(feicao, lstGeoAtributos);
				}
			}

			origem.Fechar();

			return lstRetorno;
		}
	}
}