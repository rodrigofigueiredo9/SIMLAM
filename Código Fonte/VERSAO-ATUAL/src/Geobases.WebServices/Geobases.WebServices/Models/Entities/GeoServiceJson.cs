using System;
using System.Collections.Generic;

namespace Tecnomapas.Geobases.WebServices.Models.Entities
{
	public class GeoServiceJson
	{
		public string DisplayFieldName { get; set; }
		public Object FieldAliases { get; set; }
		public List<ListAttributes> Features { get; set; }		
	}
}