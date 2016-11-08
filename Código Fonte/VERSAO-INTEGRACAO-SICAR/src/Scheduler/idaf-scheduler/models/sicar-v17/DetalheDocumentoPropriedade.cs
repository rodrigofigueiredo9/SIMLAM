using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Tecnomapas.EtramiteX.Scheduler.models.misc;

namespace Tecnomapas.EtramiteX.Scheduler.models
{
	public class DetalheDocumentoPropriedade
	{
		[JsonConverter(typeof (DateTimeDMY)), DefaultValue("")]
		public DateTime? dataRegistro { get; set; }

		[DefaultValue("")]
		public string numeroMatricula { get; set; }

		[DefaultValue("")]
		public string livro { get; set; }

		[DefaultValue("")]
		public string folha { get; set; }

		[DefaultValue(0)]
		public int? municipioCartorio { get; set; }
	}
}