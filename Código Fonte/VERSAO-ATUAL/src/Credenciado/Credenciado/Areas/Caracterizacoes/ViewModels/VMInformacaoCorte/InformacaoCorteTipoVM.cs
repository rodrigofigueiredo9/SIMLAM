using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels.VMInformacaoCorte
{
	public class InformacaoCorteTipoVM
	{
		public List<SelectListItem> TipoCorte { get; set; } = new List<SelectListItem>();
		public List<SelectListItem> Especie { get; set; } = new List<SelectListItem>();
		public Decimal AreaCorte { get; set; }
		public Decimal IdadePlantio { get; set; }
	}
}