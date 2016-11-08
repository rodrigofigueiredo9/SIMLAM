using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class SilviculturaSilvicultPDF
	{
		public String AreaCroqui { get; set; }

		private List<CulturaPDF> _culturas = new List<CulturaPDF>();
		public List<CulturaPDF> Culturas
		{
			get { return _culturas; }
			set { _culturas = value; }
		}
	}
}