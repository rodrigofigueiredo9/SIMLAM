using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPTV.Destinatario
{
	public class DestinatarioListarFiltro
	{
		public string Nome { get; set; }
		public string CPFCNPJ { get; set; }
		public bool IsCPF { get; set; }
		public Boolean StraggDestinatario { get; set; }
	}
}