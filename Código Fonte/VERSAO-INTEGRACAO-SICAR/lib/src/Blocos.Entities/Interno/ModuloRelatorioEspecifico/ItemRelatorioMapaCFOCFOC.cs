using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloLiberacaoCFOCFOC;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloRelatorioEspecifico
{
	public class ItemRelatorioMapaCFOCFOC
	{
		public string DataEmissao { get; set; }
		public string CulturaCultivar { get; set; }
		public string Tipo { get; set; }
		public string NumeroCFO { get; set; }
		public string NumeroCFOC { get; set; }
		public decimal Quantidade { get; set; }
		public string UnidadeMedida { get; set; }
		public string UnidadeOrigem { get; set; }
		public string Municipio { get; set; }

		public ItemRelatorioMapaCFOCFOC()
		{
			
		}

	}
}
