using System.Web.Mvc;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class CaracterizacoesAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "Caracterizacoes";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"Caracterizacoes_default",
				"Caracterizacoes/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}