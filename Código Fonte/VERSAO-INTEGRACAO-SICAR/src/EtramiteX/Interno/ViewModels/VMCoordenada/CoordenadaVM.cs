using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using CoordenadaTipo = Tecnomapas.Blocos.Entities.Configuracao.Interno.CoordenadaTipo;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMCoordenada
{
	public class CoordenadaVM
	{
		public Coordenada Coordenada;
		public List<SelectListItem> TiposCoordenada { get; set; }
		public List<SelectListItem> Datuns { get; set; }

		public CoordenadaVM()
		{
			TiposCoordenada = new List<SelectListItem>();
			Datuns = new List<SelectListItem>();
			Coordenada = new Coordenada();
		}

		public CoordenadaVM(List<CoordenadaTipo> tiposCoordenada, List<Datum> datuns)
		{
			TiposCoordenada = ViewModelHelper.CriarSelectList(tiposCoordenada, true);
			Datuns = ViewModelHelper.CriarSelectList(datuns, true);
		}
	}
}