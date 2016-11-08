using System.ComponentModel;

namespace Tecnomapas.EtramiteX.Scheduler.models
{
	public class EnderecoCorrespondencia
	{
		[DefaultValue("")]
		public string logradouro { get; set; }

		[DefaultValue("S/N")]
		public string numero { get; set; }

		[DefaultValue("")]
		public string complemento { get; set; }

		[DefaultValue("")]
		public string bairro { get; set; }

		[DefaultValue("")]
		public string cep { get; set; }

		[DefaultValue(0)]
		public int? codigoMunicipio { get; set; }
	}
}