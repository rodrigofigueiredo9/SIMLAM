using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IdafNavEmpreendimentoWS.Models;

namespace IdafNavEmpreendimentoWS.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public JsonResult ListarEmpreendimentos(String empreendimento, String pessoa, String processo, String segmento, String municipio, String atividade)
        {
            PontoEmpreendimentoModel pontoempreendimento = new PontoEmpreendimentoModel();
            List<PontoEmpreendimento> result = pontoempreendimento.listarEmpreendimentos(empreendimento, pessoa, processo, segmento, municipio, atividade);
            
            return Json(result,JsonRequestBehavior.AllowGet);
        }

        public JsonResult IdentificarEmpreendimentos(List<String> empreendimentos)
        {
            PontoEmpreendimentoModel pontoempreendimento = new PontoEmpreendimentoModel();
            List<PontoEmpreendimento> result = pontoempreendimento.listarEmpreendimentos(empreendimentos);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterListas()
        {
            ListasModel listas = new ListasModel();
            Listas result = listas.GetListas();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
