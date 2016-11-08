using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class CulturaFlorestalPDF
	{
		public String AreaTotalHa { get; set; }

		private List<CulturaFlorestalTipoPDF> _tipos = new List<CulturaFlorestalTipoPDF>();
		public List<CulturaFlorestalTipoPDF> Tipos
		{
			get { return _tipos; }
			set { _tipos = value; }
		}
	}
}