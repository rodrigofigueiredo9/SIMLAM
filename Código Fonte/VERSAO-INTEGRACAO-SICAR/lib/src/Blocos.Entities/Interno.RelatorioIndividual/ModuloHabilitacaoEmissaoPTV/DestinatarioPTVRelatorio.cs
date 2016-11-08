using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV
{
	public class DestinatarioPTVRelatorio
	{
		public int Id { get; set; }
		public string Nome { get; set; }
		public string Endereco { get; set; }
		public string Municipio { get; set; }
		public string CPFCNPJ { get; set; }
		public string UF { get; set; }
	}
}
