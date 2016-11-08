using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using Tecnomapas.DesenhadorWS.Models.Business;
using Tecnomapas.DesenhadorWS.Models.Entities;


namespace Tecnomapas.DesenhadorWS.Controllers
{
    public class LayerFeicaoController : Controller
    {
        LayerFeicaoBusiness feicaoBus = null;
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ListarCategoria(int idNavegador, int idProjeto)
        {

            feicaoBus = new LayerFeicaoBusiness();
            List<CategoriaLayerFeicao> categorias = feicaoBus.ListarCategoria(idNavegador, idProjeto);

            if (categorias == null)
                categorias = new List<CategoriaLayerFeicao>();

            return Json(categorias.ToArray(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ListarQuantidadeLayerFeicao(int idNavegador, int idProjeto)
        {

            feicaoBus = new LayerFeicaoBusiness();
            List<LayerFeicaoQuantidade> layersQtde = feicaoBus.ListarQuantidadeFeicoes(idNavegador, idProjeto); ;

            if (layersQtde == null)
                layersQtde = new List<LayerFeicaoQuantidade>();

            return Json(layersQtde.ToArray(), JsonRequestBehavior.AllowGet);
        }
    }
}
