using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo
{
	public class CondicionanteDescricaoVM
	{
		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
				});
			}
		}

		public List<SelectListItem> Setores { get; set; }

		public CondicionanteDescricaoVM() 
		{

		}
	}
}