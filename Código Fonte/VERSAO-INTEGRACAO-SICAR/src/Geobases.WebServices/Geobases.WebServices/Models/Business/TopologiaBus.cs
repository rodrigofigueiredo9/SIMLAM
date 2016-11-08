using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Geobases.WebServices.Models.Data;
using Tecnomapas.TecnoGeo.Geografico;
using Tecnomapas.Geobases.WebServices.Models.Entities;

namespace Tecnomapas.Geobases.WebServices.Models.Business
{
	public class TopologiaBus
	{
		private TopologiaDa _da = new TopologiaDa();

		public List<FeicaoJson> ObterAtributosGeoRelacao(List<string> feicoes, string wkt)
		{
			Dictionary<string, List<Feicao>> feicoesBanco = _da.Relacao(feicoes, wkt);
			List<FeicaoJson> feicoesJson = new List<FeicaoJson>();


			foreach (string feicaoKey in feicoesBanco.Keys)
			{
				FeicaoJson feicaoItem = new FeicaoJson();
				feicaoItem.Nome = feicaoKey;
				feicaoItem.Geometrias = new List<GeometriaJson>();

				foreach (Feicao feicao in feicoesBanco[feicaoKey])
				{
					GeometriaJson geoItem = new GeometriaJson();
					geoItem.Atributos = new List<AtributoJson>();

					for (int i = 0; i < feicao.Atributos.Count; i++)
					{
						if (feicao.Atributos[i].Nome != "NOME" && feicao.Atributos[i].Nome != "ADMINISTRADOR")
							continue;
						geoItem.Atributos.Add(new AtributoJson() { Nome = feicao.Atributos[i].Nome, Valor = feicao.Atributos[i].Valor });
					}

					feicaoItem.Geometrias.Add(geoItem);
				}

				feicoesJson.Add(feicaoItem);
			}

			return feicoesJson;
		}
	}
}