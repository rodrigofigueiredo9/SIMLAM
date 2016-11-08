

using System.Web.Mvc;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
   public class RelatoriosAreaRegistration : AreaRegistration
   {
      public override string AreaName
      {
         get
         {
            return "Relatorios";
         }
      }

      public override void RegisterArea(AreaRegistrationContext context)
      {
         context.MapRoute(
             "Relatorios_default",
             "Relatorios/{controller}/{action}/{id}",
             new { action = "Index", id = UrlParameter.Optional }
         );
      }
   }
}
