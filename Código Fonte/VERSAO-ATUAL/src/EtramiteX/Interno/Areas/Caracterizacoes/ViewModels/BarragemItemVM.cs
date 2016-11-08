using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragem;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class BarragemItemVM
	{
		public BarragemItem BarragemItem { set; get; }
		public List<SelectListItem> Finalidades { set; get; }
		public List<SelectListItem> Outorgas { set; get; }
		public CoordenadaAtividadeVM CoordenadaAtividadeVM { get; set; }

		public int FinalidadeOutrosId { get { return (int)eFinalidade.Outros;  } }
		public int FinalidadeReservacaoId { get { return (int)eFinalidade.Reservacao; } }

		public bool IsVisualizar { get; set; }
		public bool IsEditar { get; set; }

		public BarragemItemVM() 
		{
			this.BarragemItem = new BarragemItem();
			this.Finalidades = new List<SelectListItem>();
			this.Outorgas = new List<SelectListItem>();
			this.CoordenadaAtividadeVM = new CoordenadaAtividadeVM();		
		}
		
		public String GetJSON(object obj)
		{
			return ViewModelHelper.Json(obj);
		}	
	}
}