using System.Web.Mvc;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class EspecificidadesAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "Especificidades";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"Especificidades_default",
				"Especificidades/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}