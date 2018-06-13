using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Tecnomapas.EtramiteX.Scheduler.models.misc;

namespace Tecnomapas.EtramiteX.Scheduler.models
{
	public class DadosReserva
	{
		[DefaultValue("")]
		public string numero { get; set; }

		[JsonConverter(typeof (DateTimeDMY)), DefaultValue("")]
		public DateTime data { get; set; }

		[DefaultValue("")]
		public string area { get; set; }

		[DefaultValue("")]
		public string reservaDentroImovel { get; set; }

		[DefaultValue("")]
		public string numeroCAR { get; set; }

		[DefaultValue("")]
		private string cedentePossuiCodEmpreendimento { get; set; }

		public void setCedentePossuiCodEmpreendimento(string cedente)
		{
			cedentePossuiCodEmpreendimento = cedente;
		}

		public string getCedentePossuiCodEmpreendimento() => cedentePossuiCodEmpreendimento;
	}
}