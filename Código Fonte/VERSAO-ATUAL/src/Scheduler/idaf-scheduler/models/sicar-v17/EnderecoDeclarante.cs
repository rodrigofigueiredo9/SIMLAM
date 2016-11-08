using System.ComponentModel;

namespace Tecnomapas.EtramiteX.Scheduler.models
{
	public class EnderecoDeclarante
	{
		[DefaultValue(0)]
		public int? codigoMunicipio { get; set; }

		[DefaultValue("")]
		public string cep { get; set; }

		[DefaultValue("")]
		public string bairro { get; set; }

		[DefaultValue("")]
		public string numero { get; set; }

		[DefaultValue("")]
		public string complemento { get; set; }

		[DefaultValue("")]
		public string logradouro { get; set; }
	}
}