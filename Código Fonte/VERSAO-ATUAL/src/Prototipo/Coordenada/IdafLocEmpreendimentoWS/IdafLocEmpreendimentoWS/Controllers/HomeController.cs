using System;
using System.Collections.Generic;
using System.Web.Mvc;
using IdafLocEmpreendimentoWS.Models;

namespace IdafLocEmpreendimentoWS.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult LocalizarMunicipio(String municipio)
        {
            MunicipioModel municipioModel = new MunicipioModel();
            List<Municipio> result = municipioModel.listarMunicipios(municipio);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
