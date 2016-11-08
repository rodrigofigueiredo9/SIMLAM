using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Termo
{
	public class TermoAprovacaoMedicaoVM
	{
		public bool IsVisualizar { get; set; }
		public List<SelectListItem> Destinatarios { get; set; }
		public TermoAprovacaoMedicao Termo { get; set; }
		public List<SelectListItem> Tecnicos { get; set; }
		public List<SelectListItem> Setores { get; set; }
		public List<SelectListItem> Funcionario { get; set; }

		public TermoAprovacaoMedicaoVM()
		{
			Destinatarios = new List<SelectListItem>();
			Termo = new TermoAprovacaoMedicao();
			Tecnicos = new List<SelectListItem>();
			Setores = new List<SelectListItem>();
			Funcionario = new List<SelectListItem>();
		}

		public String GetJSON(object obj)
		{
			return ViewModelHelper.Json(obj);
		}
	}
}