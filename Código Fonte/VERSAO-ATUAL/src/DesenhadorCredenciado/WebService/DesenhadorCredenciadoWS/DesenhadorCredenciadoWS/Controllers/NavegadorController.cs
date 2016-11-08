using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.DesenhadorWS.Models.Business;
using Tecnomapas.DesenhadorWS.Models.Entities;
using System.Runtime.Serialization;

namespace Tecnomapas.DesenhadorWS.Controllers
{
    public class NavegadorController : Controller
    {
       
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public JsonResult Buscar(int idNavegador, int idProjeto)
        {

            NavegadorBusiness navBus = new NavegadorBusiness();

            Navegador navegador = navBus.Buscar(idNavegador,idProjeto);

            return Json(navegador, JsonRequestBehavior.AllowGet);
        }

		/*public JsonResult Teste(int idNavegador, int idProjeto)
		{

			NavegadorBusiness navBus = new NavegadorBusiness();

			Navegador navegador = navBus.Buscar(idNavegador, idProjeto);

			return Json(navegador, JsonRequestBehavior.AllowGet);
		}

        [HttpPost]
        public JsonResult Teste(List<Navegador> ids)
        {
            Navegador navegador = null;
            //if (ids != null && ids.Length > 0)
            {

                NavegadorBusiness navBus = new NavegadorBusiness();

                //navegador = navBus.Buscar(ids.Id);

                
            }
            return Json(navegador, JsonRequestBehavior.AllowGet);
        }

		public JsonResult Teste(int idNavegador, int idProjeto)
		{
			NavegadorBusiness navBus = new NavegadorBusiness();

			Navegador navegador = navBus.Buscar(idNavegador, idProjeto);

			return Json(navegador, JsonRequestBehavior.AllowGet);
		}*/
    }
}
