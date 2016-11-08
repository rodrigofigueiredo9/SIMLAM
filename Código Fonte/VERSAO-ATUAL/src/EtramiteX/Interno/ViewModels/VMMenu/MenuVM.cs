using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Perfil;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMMenu
{
	public class MenuVM
	{
		public List<Menu> Menus { get; set; }

		public MenuVM(List<Menu> menus)
		{
			Menus = menus;
		}
	}
}