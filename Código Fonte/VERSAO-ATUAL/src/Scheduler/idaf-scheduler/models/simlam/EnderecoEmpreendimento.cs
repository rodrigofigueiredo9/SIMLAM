using System.ComponentModel;

namespace Tecnomapas.EtramiteX.Scheduler.models.simlam
{
	public class EnderecoEmpreendimento
	{
		[DefaultValue(0)]
		public int correspondencia { get; set; }

		[DefaultValue(0)]
		public int municipio { get; set; }

		[DefaultValue("")]
		public string cep { get; set; }

		[DefaultValue("")]
		public string logradouro { get; set; }

		[DefaultValue("")]
		public string numero { get; set; }

		[DefaultValue("")]
		public string complemento { get; set; }

		[DefaultValue("")]
		public string bairro { get; set; }

		[DefaultValue("")]
		public string distrito { get; set; }

		[DefaultValue("")]
		public string corrego { get; set; }

		[DefaultValue(0)]
		public int zona { get; set; }

		[DefaultValue("")]
		public string caixaPostal { get; set; }
	}
}
