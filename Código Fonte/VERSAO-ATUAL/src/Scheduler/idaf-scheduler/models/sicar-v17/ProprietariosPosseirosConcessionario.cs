
namespace Tecnomapas.EtramiteX.Scheduler.models
{
	public class ProprietariosPosseirosConcessionario
	{
		//Domínio do campo TIPO
		public const string TipoPessoaFisica = "PF";
		public const string TipoPessoaJuridica = "PJ";

		public string tipo { get; set; }

		public string cpfCnpj { get; set; }

		public string nome { get; set; }
		public string nomeFantasia { get; set; }
		public string dataNascimento { get; set; }
		public string nomeMae { get; set; }
		public object nomeConjuge { get; set; }
		public object cpfConjuge { get; set; }

		public ProprietariosPosseirosConcessionario()
		{
			nomeMae = "Não informado";
			dataNascimento = "01/01/1900";
		}
	}
}