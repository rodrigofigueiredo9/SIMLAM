using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV
{
	public class FuncionarioRelatorio
	{
		public int Id { get; set; }
		public string Nome { get; set; }

		public string Numero { get; set; }

		public string Registro { get; set; }

        public int? ArquivoId { get; set; }

        public int UFHablitacao { get; set; }

        public string NumeroVistoCrea { get; set; }
	}
}
