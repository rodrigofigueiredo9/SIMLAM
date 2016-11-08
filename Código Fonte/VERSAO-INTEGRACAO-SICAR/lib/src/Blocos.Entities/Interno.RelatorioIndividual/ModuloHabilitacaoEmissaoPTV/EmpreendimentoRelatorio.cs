using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV
{
	public class EmpreendimentoRelatorio
	{
		public string NomeRazao { get; set; }
		public string ResponsavelRazaoSocial { get; set; }
		public string EndLogradouro { get; set; }
		public string EndBairro { get; set; }
		public string EndDistrito { get; set; }
		public string EndMunicipio { get; set; }
		public string EndUF { get; set; }
		public string CNPJ { get; set; }
		public string ResponsavelCPF { get; set; }
		public EmpreendimentoRelatorio() { }
	}
}
