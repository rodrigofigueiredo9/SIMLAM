using System.Collections.Generic;
using System.Web.Mvc;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.AtividadeEspecificidade
{
	public class EspBarragemVM
	{
		public List<SelectListItem> Barragens { set; get; }
		public int? BarragemId { set; get; }
		public bool IsVisualizar { get; set; }

		public EspBarragemVM()
		{
			this.Barragens = new List<SelectListItem>();
		}
	}
}