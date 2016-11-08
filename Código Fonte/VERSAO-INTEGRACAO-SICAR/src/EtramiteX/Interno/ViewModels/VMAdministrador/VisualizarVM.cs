using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloAdministrador;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMAdministrador
{
	public class VisualizarVM
	{
		public Administrador Administrador { get; set; }
		public string TextoPermissoes { get; set; }
		public List<PapeisVME> Papeis { get; set; }

		public VisualizarVM()
		{
			Administrador = new Administrador();
			Papeis = new List<PapeisVME>();
		}
	}
}