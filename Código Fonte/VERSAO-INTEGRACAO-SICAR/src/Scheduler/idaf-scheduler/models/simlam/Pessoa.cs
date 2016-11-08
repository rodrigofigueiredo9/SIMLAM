using System;

namespace Tecnomapas.EtramiteX.Scheduler.models.simlam
{
	public class Pessoa
	{
		public int id { get; set; }
		public int tipo { get; set; }
		public string nome { get; set; }
		public string cpf { get; set; }
		public DateTime dataNascimento { get; set; }
		public string nomeMae { get; set; }
	}
}