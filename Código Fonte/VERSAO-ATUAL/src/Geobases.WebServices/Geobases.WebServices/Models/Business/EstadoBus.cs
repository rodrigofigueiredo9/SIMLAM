using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Geobases.WebServices.Models.Data;

namespace Tecnomapas.Geobases.WebServices.Models.Business
{
	public class EstadoBus
	{
		EstadoDa _da = new EstadoDa();

		public bool EstaNoEstado(decimal easting, decimal northing)
		{
			return _da.EstaNoEstado(easting, northing);
		}
	}
}