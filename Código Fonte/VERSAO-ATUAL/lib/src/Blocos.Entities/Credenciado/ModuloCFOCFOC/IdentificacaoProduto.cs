using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC
{
	public class IdentificacaoProduto
	{
		public int Id { get; set; }
		public string Tid { get; set; }
		public int UnidadeProducao { get; set; }
		public string CodigoUP { get; set; }
		public int LoteId { get; set; }
		public string LoteCodigo { get; set; }
		public int CulturaId { get; set; }
		public string CulturaTexto { get; set; }
		public int CultivarId { get; set; }
		public string CultivarTexto { get; set; }
		public int UnidadeMedidaId { get; set; }
		public string UnidadeMedida { get; set; }
		public decimal Quantidade { get; set; }
		public DateTecno DataInicioColheita { get; set; }
		public DateTecno DataFimColheita { get; set; }
		public DateTecno DataConsolidacao { get; set; }

        public bool ExibeQtdKg { get; set; }

		public IdentificacaoProduto()
		{
			DataInicioColheita = new DateTecno();
			DataFimColheita = new DateTecno();
			DataConsolidacao = new DateTecno();
		}
	}
}