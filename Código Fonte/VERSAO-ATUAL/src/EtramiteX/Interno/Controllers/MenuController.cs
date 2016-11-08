using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Perfil;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMMenu;
using Tecnomapas.EtramiteX.Perfil.Business;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class MenuController : DefaultController
	{
		public ActionResult Menu()
		{
			if (User == null || !User.Identity.IsAuthenticated)
			{
				return PartialView("Menu", new MenuVM(new List<Menu>()));
			}

			BusMenu busMenu = new BusMenu();
			List<Menu> lstMenu = busMenu.ObterMenu();
			List<string> lstMenuAtivo = busMenu.ObterMenuAtivo();

			foreach (Menu item in lstMenu)
			{
				item.IsAtivo = lstMenuAtivo.Contains(item.Nome);
			}

			return PartialView("Menu", new MenuVM(lstMenu));
		}

		public ActionResult SubMenu()
		{
			if (User == null || !User.Identity.IsAuthenticated)
			{
				return PartialView("SubMenu", new MenuVM(new List<Menu>()));
			}

			BusMenu _busMenu = new BusMenu();
			List<Menu> lstMenu = _busMenu.ObterSubMenu();

			return PartialView("SubMenu", new MenuVM(lstMenu));
		}
	}
}