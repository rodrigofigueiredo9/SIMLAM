using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Tecnomapas.EtramiteX.Scheduler.models.misc;

namespace Tecnomapas.EtramiteX.Scheduler.models
{
	public class DetalheDocumentoPosse
	{
		[DefaultValue("")]
		public string emissorDocumento { get; set; }

		[JsonConverter(typeof (DateTimeDMY)), DefaultValue("")]
		public DateTime? dataDocumento { get; set; }

		[DefaultValue("")]
		public string nomeVendedor { get; set; }

		[DefaultValue(0)]
		public int? cpfVendedor { get; set; }

		[DefaultValue("")]
		public string nomeDeclarante { get; set; }

		[DefaultValue("")]
		public string cpfCnpjDeclarante { get; set; }

		[DefaultValue("")]
		public string autodeclaracao { get; set; }

		public EnderecoDeclarante enderecoDeclarante { get; set; }
	}
}