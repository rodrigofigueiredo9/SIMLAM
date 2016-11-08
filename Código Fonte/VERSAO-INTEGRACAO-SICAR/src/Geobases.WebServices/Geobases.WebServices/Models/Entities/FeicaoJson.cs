using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tecnomapas.Geobases.WebServices.Models.Entities
{
	public class FeicaoJson
	{
		public string Nome { get; set; }
		public List<GeometriaJson> Geometrias { get; set; }
	}
}