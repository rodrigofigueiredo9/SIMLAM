using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tecnomapas.Geobases.WebServices.Models.Entities
{

	public class ListAttributes
	{
		public Attributes Attributes { get; set; }
	}

	public class Attributes
	{
		public string Nome { get; set; }
		public string Cod_Ibge { get; set; }
	}
}