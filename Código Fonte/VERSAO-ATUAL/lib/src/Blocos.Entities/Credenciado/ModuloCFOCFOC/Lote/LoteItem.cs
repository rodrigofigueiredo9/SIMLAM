using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC.Lote
{
	public class LoteItem
	{
		public int Id { get; set; }
		public int OrigemTipo { get; set; }
		public string OrigemTipoTexto { get; set; }
		public int Origem { get; set; }
		public string OrigemNumero { get; set; }
		public int Cultura { get; set; }
		public string CulturaTexto { get; set; }
		public int Cultivar { get; set; }
		public string CultivarTexto { get; set; }
		public int UnidadeMedida { get; set; }
		public string UnidadeMedidaTexto { get; set; }
		public decimal Quantidade { get; set; }
        public bool ExibeKg { get; set; }
        public string Serie { get; set; }
		
	}
}