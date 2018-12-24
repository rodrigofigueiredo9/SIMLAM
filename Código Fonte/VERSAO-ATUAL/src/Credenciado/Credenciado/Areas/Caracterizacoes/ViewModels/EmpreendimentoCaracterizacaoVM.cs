using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels
{
	public class EmpreendimentoCaracterizacaoVM
	{
		public long EmpreendimentoId { get; set; }
		public int? EmpreendimentoCodigo { get; set; }
		public string DenominadorTexto { get; set; }
		public string DenominadorValor { get; set; }
		public string EmpreendimentoCNPJ { get; set; }
		public Decimal AreaImovel { get; set; }
		public Decimal AreaPlantada { get; set; }

		public List<SelectListItem> EmpreendimentoUf { get; set; } = new List<SelectListItem>();
		public List<SelectListItem> EmpreendimentoZonaLocalizacao { get; set; } = new List<SelectListItem>();
		public List<SelectListItem> EmpreendimentoMunicipio { get; set; } = new List<SelectListItem>();

	}
}