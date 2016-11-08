using System.ComponentModel;

namespace Tecnomapas.EtramiteX.Scheduler.models.simlam
{
	public class Empreendimento
	{
		[DefaultValue(0)]
		public int id { get; set; }

		[DefaultValue("")]
		public string cnpj { get; set; }

		[DefaultValue("")]
		public string denominador { get; set; }

		[DefaultValue("")]
		public string nomeFantasia { get; set; }

		[DefaultValue(0)]
		public int codigo { get; set; }

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

		[DefaultValue("")]
		public string email { get; set; }

		[DefaultValue("")]
		public string telefone { get; set; }
	}
}
