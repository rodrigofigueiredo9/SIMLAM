using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.DesenhadorWS.Models.Entities;
using Tecnomapas.DesenhadorWS.Models.Business;

namespace Tecnomapas.DesenhadorWS.Controllers
{
    public class ProjetoController : Controller
    {
        ProjetoBusiness busProjeto = null;

		public JsonResult BuscarDadosProjeto()
		{
			busProjeto = new ProjetoBusiness();

			Projeto projeto = busProjeto.BuscarDadosProjeto(167, 5);

			return Json(projeto, JsonRequestBehavior.AllowGet);
		}

        [HttpPost]
        public JsonResult BuscarDadosProjeto(int idProjeto, int idFilaTipo)
        {
            busProjeto = new ProjetoBusiness();

            Projeto projeto = busProjeto.BuscarDadosProjeto(idProjeto, idFilaTipo);

            return Json(projeto, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ListarQuadroAreasDominialidade(int idProjeto)
        {

            busProjeto = new ProjetoBusiness();
            List<CategoriaQuadroDeArea> layersQtde = busProjeto.ListarQuadroAreas(1, idProjeto);

            if (layersQtde == null)
                layersQtde = new List<CategoriaQuadroDeArea>();

            return Json(layersQtde.ToArray(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ListarQuadroAreas(int idNavegador, int idProjeto)
        {

            busProjeto = new ProjetoBusiness();
            List<CategoriaQuadroDeArea> layersQtde =null;

            layersQtde = busProjeto.ListarQuadroAreas(idNavegador, idProjeto);
          
            if (layersQtde == null)
                layersQtde = new List<CategoriaQuadroDeArea>();

            return Json(layersQtde.ToArray(), JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public JsonResult SalvarAreaAbrangenciaProjeto(FeicaoAreaAbrangencia feicao)
        {
            busProjeto = new ProjetoBusiness();

            Retorno retorno = busProjeto.SalvarAreaAbrangencia(feicao);

            if (retorno == null)
                retorno = new Retorno();

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }
    }
}
