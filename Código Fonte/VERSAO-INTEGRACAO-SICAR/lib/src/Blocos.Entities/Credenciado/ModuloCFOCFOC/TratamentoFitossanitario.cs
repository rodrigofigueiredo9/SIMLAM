using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC
{
	public class TratamentoFitossanitario
	{
		public int Id { get; set; }
		public string Tid { get; set; }
		public string ProdutoComercial { get; set; }
		public string IngredienteAtivo { get; set; }
		public decimal Dose { get; set; }
		public string PragaProduto { get; set; }
		public string ModoAplicacao { get; set; }
	}
}