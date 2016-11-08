using System;
using System.Collections.Generic;
using System.Linq;
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
	}
}