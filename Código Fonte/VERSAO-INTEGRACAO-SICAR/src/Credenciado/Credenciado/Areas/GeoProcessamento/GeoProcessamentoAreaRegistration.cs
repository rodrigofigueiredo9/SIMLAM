using System.Web.Mvc;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class GeoProcessamentoAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "GeoProcessamento";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"GeoProcessamento_default",
				"GeoProcessamento/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
