using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV
{
	public class TratamentosRelatorio
	{
		public int Id { get; set; }
		public string ProdutoComercial { get; set; }
		public string IngredienteAtivo { get; set; }
		public decimal Dose { get; set; }
		public string PragaProduto { get; set; }
		public string ModoAplicacao { get; set; }
	}
}
