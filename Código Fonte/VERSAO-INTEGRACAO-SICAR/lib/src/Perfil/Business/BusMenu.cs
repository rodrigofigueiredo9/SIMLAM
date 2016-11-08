using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Perfil;

namespace Tecnomapas.EtramiteX.Perfil.Business
{
	public class BusMenu
	{
		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		private static string KeyMenu 
		{
			get { return User.Login + User.FuncionarioId + "Menu"; }
		}
		
		public static List<Menu> Menu
		{
			get 
			{
				return HttpContext.Current.Cache[KeyMenu] as List<Menu>; 
			}
			set
			{
				if (value == null)
				{
					HttpContext.Current.Cache.Remove(KeyMenu);
				}
				else
				{
					HttpContext.Current.Cache[KeyMenu] = value;
				}
			}
		}

		public List<Menu> ObterMenu()
		{
			if (Menu != null)
			{
				return Menu as List<Menu>;
			}

			List<Menu> lstMenu = new List<Menu>();

			foreach (SiteMapNode nodeItem in SiteMap.RootNode.ChildNodes)
			{
				if (nodeItem.Roles.Cast<String>().Any(x => HttpContext.Current.User.IsInRole(x)))
				{
					lstMenu.Add(new Menu(nodeItem.Title, nodeItem.Url, nodeItem.Description, false));
				}
			}

			Menu = lstMenu;
			return lstMenu;
		}

		public List<Menu> ObterSubMenu()
		{
			SiteMapNode node = SiteMap.CurrentNode;

			List<Menu> lstMenu = new List<Menu>();

			List<String> lstMenuAtivo = ObterMenuAtivo();

			if (node != null && node.Url == "/")
				return lstMenu;

			foreach (SiteMapNode nodeMenuVert in SiteMap.RootNode.ChildNodes)
			{
				if (lstMenuAtivo.Contains(nodeMenuVert.Title))
				{
					foreach (SiteMapNode nodeItem in nodeMenuVert.ChildNodes)
					{
						if (!String.IsNullOrEmpty(nodeItem.Title))
						{
							if (nodeItem.Roles.Cast<String>().Any(x => HttpContext.Current.User.IsInRole(x)))
							{
								lstMenu.Add(new Menu(nodeItem.Title, nodeItem.Url, nodeItem.Description, lstMenuAtivo.Contains(nodeItem.Title)));
							}
						}
					}
					break;
				}
			}

			return lstMenu;
		}

		public SiteMapNode CurrentNodeMvc()
		{
			SiteMapNode node = SiteMap.CurrentNode;
			if (node == null)
			{
				string url = HttpContext.Current.Request.RawUrl.Remove(HttpContext.Current.Request.RawUrl.LastIndexOf('/'));

				foreach (SiteMapNode item in SiteMap.RootNode.GetAllNodes())
				{
					if (item.Url.Equals(url, StringComparison.InvariantCultureIgnoreCase))
					{
						node = item;
					}
				}
			}

			return node;
		}

		public List<string> ObterMenuAtivo()
		{
			List<string> lstMapNode = new List<string>();

			SiteMapNode node = CurrentNodeMvc();

			if (node == null)
			{
				return lstMapNode;
			}

			lstMapNode.Add(node.Title);

			while (node.ParentNode != null && node.ParentNode.Url != "/")
			{
				lstMapNode.Add(node.ParentNode.Title);
				node = node.ParentNode;
			}

			return lstMapNode;
		}
	}
}