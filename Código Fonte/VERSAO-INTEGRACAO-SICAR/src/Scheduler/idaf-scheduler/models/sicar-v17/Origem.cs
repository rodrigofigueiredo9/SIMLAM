using System;
using Newtonsoft.Json;
using Tecnomapas.EtramiteX.Scheduler.models.misc;

namespace Tecnomapas.EtramiteX.Scheduler.models
{
	public class Origem
	{
		//Domínio do campo TIPO
		public const string TipoOffline = "OFF";
		public const string TipoEstadual = "EST";
		public const string TipoMunicipal = "MUN";

		//Domínio do campo STATUS
		public const string StatusInscrito = "IN";
		public const string StatusAtivo = "AT";
		public const string StatusPendente = "PE";
		public const string StatusCancelado = "CA";

		[JsonConverter(typeof(DateTimeDMYHMS))]
		public DateTime dataProtocolo { get; set; }
		public string codigoProtocolo { get; set; }
		public string status { get; set; }
		public string tipo { get; set; }

		public Origem()
		{
			tipo = Origem.TipoEstadual;
			status = Origem.StatusInscrito;

			var agora = DateTime.Now;
			dataProtocolo = agora;
			codigoProtocolo = CarUtils.GerarCodigoProtocolo(agora);
		}
	}
}