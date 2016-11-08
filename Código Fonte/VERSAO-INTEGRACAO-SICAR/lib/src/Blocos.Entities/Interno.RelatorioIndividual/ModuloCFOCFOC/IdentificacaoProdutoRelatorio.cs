using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCFOCFOC
{
	public class IdentificacaoProdutoRelatorio
	{
		public int Id { get; set; }
		public int UnidadeProducaoID { get; set; }
		public string UnidadeProducaoTID { get; set; }
		public string CodigoUP { get; set; }
		public string LoteCodigo { get; set; }
		public string CulturaTexto { get; set; }
		public string CultivarTexto { get; set; }
		public string UnidadeMedida { get; set; }
		public decimal Quantidade { get; set; }
		public string DataInicioColheita { get; set; }
		public string DataFimColheita { get; set; }
		public string DataConsolidacao { get; set; }

		public string CulturaCultivar
		{
			get
			{
				string retorno = CultivarTexto;

				if (CulturaTexto.ToLower() != "citros")
				{
					retorno = CulturaTexto + " " + CultivarTexto;
				}
				else if (String.IsNullOrWhiteSpace(retorno))
				{
					retorno = CulturaTexto;
				}

				return retorno;
			}
		}
	}
}