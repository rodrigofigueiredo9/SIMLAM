using System.Collections.Generic;

namespace Tecnomapas.EtramiteX.Scheduler.models
{
	public class Resposta
	{
		public string codigo { get; set; }
		public List<object> respostas { get; set; }

		public Resposta()
		{
			respostas = new List<object>();
		}
	}
}