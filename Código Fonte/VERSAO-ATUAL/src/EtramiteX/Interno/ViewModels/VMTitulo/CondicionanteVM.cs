using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo
{
	public class CondicionanteVM
	{
		private List<SelectListItem> _periodicidades = new List<SelectListItem>();
		public List<SelectListItem> Periodicidades
		{
			get { return _periodicidades; }
			set { _periodicidades = value; }
		}

		private TituloCondicionante _condicionante = new TituloCondicionante();
		public TituloCondicionante Condicionante
		{
			get { return _condicionante; }
			set { _condicionante = value; }
		}

		public CondicionanteVM(List<TituloCondicionantePeriodTipo> periodicidades)
		{
			Periodicidades = ViewModelHelper.CriarSelectList(periodicidades, true);
		}

		public CondicionanteVM() { }

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
				});
			}
		}
	}
}