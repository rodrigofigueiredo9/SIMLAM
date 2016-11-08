using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao
{
	public class EstanteVM
	{
		private List<SelectListItem> _modos = new List<SelectListItem>();
		public List<SelectListItem> Modos
		{
			get { return _modos; }
			set { _modos = value; }
		}

		public int Id { get; set; }
		public String Texto { get; set; }
		
		private List<Prateleira> _prateleiras = new List<Prateleira>();
		public List<Prateleira> Prateleiras
		{
			get { return _prateleiras; }
			set { _prateleiras = value; }
		}

		public bool IsVisualizar { get; set; }

		public void Carregar(Estante estante)
		{
			this.Id = estante.Id;
			this.Texto= estante.Texto;
			this.Prateleiras = estante.Prateleiras;
		}
	}
}