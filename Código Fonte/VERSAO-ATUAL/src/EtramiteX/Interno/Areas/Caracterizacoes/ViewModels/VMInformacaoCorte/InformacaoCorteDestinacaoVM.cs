using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMInformacaoCorte
{
	public class InformacaoCorteDestinacaoVM
	{
		public List<SelectListItem> DestinacaoMaterial { get; set; } = new List<SelectListItem>();
		public List<SelectListItem> Produto { get; set; } = new List<SelectListItem>();
		public Decimal Quantidade { get; set; }
	}
}