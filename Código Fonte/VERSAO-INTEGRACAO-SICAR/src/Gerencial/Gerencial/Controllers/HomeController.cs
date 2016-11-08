using System;
using System.Web.Mvc;

namespace Tecnomapas.EtramiteX.Gerencial.Controllers
{
	public class HomeController : DefaultController
	{
		public ActionResult Index()
		{
			if (User == null || !User.Identity.IsAuthenticated)
			{
				return RedirectToAction("LogOn", "Autenticacao");
			}

			return View();
		}

		public ActionResult SistemaMensagem()
		{
			return View();
		}

		public ActionResult Teste()
		{
			return View();
		}

		public string ValoresCache()
		{
			return String.Empty;
		}
	}
}