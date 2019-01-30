using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Tecnomapas.Geobases.WebServices.Models.Entities;

namespace Tecnomapas.Geobases.WebServices.Models.Business
{
	public class MunicipioBus
	{
		MunicipioDa _da = new MunicipioDa();

		public List<Municipio> listarMunicipios(string municipio)
		{
			return _da.listarMunicipios(municipio);
		}

		public Municipio ObterMunicipio(decimal easting, decimal northing)
		{
			return _da.ObterMunicipio(easting, northing);
		}

		public Municipio GetMunicipioGeobases(Decimal easting, Decimal northing)
		{
			string url = ConfigurationManager.AppSettings["urlGeobases"] + ConfigurationManager.AppSettings["urlServiceMunicipios"]
				+ @"?geometry={'x':"+ easting.ToString() + ",'y':"+ northing.ToString() +
				"}&geometryType=esriGeometryPoint&inSR="+ ConfigurationManager.AppSettings["srid"] +
				"&spatialRel=esriSpatialRelIntersects&returnGeometry=false&outFields=nome,cod_ibge&f=json";
			WebRequest request = HttpWebRequest.Create(url);
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			if (response.StatusCode == HttpStatusCode.OK)
				throw new Exception("Não foi possível se conectar ao servidor do Geobases, favor entrar em contato com o administrador do sistema");

			StreamReader responseReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
			string json = responseReader.ReadToEnd();

			GeoServiceJson serviceJson = JsonConvert.DeserializeObject<GeoServiceJson>(json);

			responseReader.Close();
			response.Close();

			Municipio result = new Municipio();
			result.nome = serviceJson.Features[0].Attributes.Nome;
			result.IBGE = serviceJson.Features[0].Attributes.Cod_Ibge;

			return result;
		}
	}
}