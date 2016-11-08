using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.DesenhadorWS.Models.Business;
using Tecnomapas.DesenhadorWS.Models.Entities;

namespace Tecnomapas.DesenhadorWS.Controllers
{
    public class FeicaoController : Controller
    {       
        public ActionResult Index()
        {
            return View();
        }

		public string cultura()
		{
			FeicaoBusiness busGeo = new FeicaoBusiness();
			return busGeo.ObterCultura();
		}

         [HttpPost]
         public JsonResult Cadastrar(FeicaoObjeto feicao)
         {
             FeicaoBusiness busGeo = new FeicaoBusiness();
             Retorno retorno = null;
             if (feicao != null)
             {
                 LayerFeicaoBusiness.LimparCacheLayers(feicao.IdProjeto);
                 ProjetoBusiness.InvalidarProcessamento(feicao.IdProjeto);
                 retorno = busGeo.Cadastrar(feicao);
             }
             if(retorno == null)
                retorno = new Retorno();

             return Json(retorno, JsonRequestBehavior.AllowGet);
         }

         [HttpPost]
         public JsonResult CadastrarGeometrias(List<FeicaoObjeto> feicoes)
         {
             FeicaoBusiness busGeo = new FeicaoBusiness();

             List<Retorno> resposta = new List<Retorno>();
             Retorno retorno;
             if (feicoes != null && feicoes.Count > 0)
             {
                 LayerFeicaoBusiness.LimparCacheLayers(feicoes[0].IdProjeto);
                 ProjetoBusiness.InvalidarProcessamento(feicoes[0].IdProjeto);
                 for (int n = 0; n < feicoes.Count; n++)
                 {
                     retorno = new Retorno();
                     retorno = busGeo.Cadastrar(feicoes[n]);
                     resposta.Add(retorno);
                 }
             }

             return Json(resposta, JsonRequestBehavior.AllowGet);
         }

         [HttpPost]
         public JsonResult AtualizarGeometria(FeicaoObjeto feicao)
         {
             FeicaoBusiness busGeo = new FeicaoBusiness();
             Retorno retorno = null;
             if (feicao != null)
             {
                  LayerFeicaoBusiness.LimparCacheLayers(feicao.IdProjeto);
                  ProjetoBusiness.InvalidarProcessamento(feicao.IdProjeto);
                  retorno = busGeo.AtualizarGeometria(feicao);
             }
             if (retorno == null)
                 retorno = new Retorno();

             return Json(retorno, JsonRequestBehavior.AllowGet);
         }

         [HttpPost]
         public JsonResult AtualizarGeometrias(List<FeicaoObjeto> feicoes)
         {
             FeicaoBusiness busGeo = new FeicaoBusiness();

             List<Retorno> resposta = new List<Retorno>();
             Retorno retorno;
             if (feicoes != null && feicoes.Count > 0)
             {
                 LayerFeicaoBusiness.LimparCacheLayers(feicoes[0].IdProjeto);
                 ProjetoBusiness.InvalidarProcessamento(feicoes[0].IdProjeto);
                 for (int n = 0; n < feicoes.Count; n++)
                 {
                     retorno = new Retorno();
                     retorno = busGeo.AtualizarGeometria(feicoes[n]);
                     resposta.Add(retorno);
                 }
             }
             return Json(resposta, JsonRequestBehavior.AllowGet);
         }

         [HttpPost]
         public JsonResult Excluir(int idLayerFeicao, int objectId, int idProjeto)
         {
             FeicaoBusiness busGeo = new FeicaoBusiness();

             Retorno retorno = null;
           
             LayerFeicaoBusiness.LimparCacheLayers(idProjeto);
             ProjetoBusiness.InvalidarProcessamento(idProjeto);
             retorno = busGeo.Excluir(idLayerFeicao, objectId, idProjeto);
             
             if (retorno == null)
                 retorno = new Retorno();

             return Json(retorno, JsonRequestBehavior.AllowGet);
         }

         [HttpPost]
         public JsonResult ExcluirFeicoes(List<FeicaoObjeto> feicoes)
         {
             FeicaoBusiness busGeo = new FeicaoBusiness();
             Retorno retorno = null;
             List<Retorno> resposta = new List<Retorno>();

             if (feicoes != null && feicoes.Count >0)
             {
                 LayerFeicaoBusiness.LimparCacheLayers(feicoes[0].IdProjeto);
                 ProjetoBusiness.InvalidarProcessamento(feicoes[0].IdProjeto);
                 for (int n = 0; n < feicoes.Count; n++)
                 {
                     retorno = new Retorno();
                     retorno = busGeo.Excluir(feicoes[n].IdLayerFeicao, feicoes[n].ObjectId,  feicoes[n].IdProjeto);
                     resposta.Add(retorno);
                 }
             }
             if (resposta == null)
                 resposta = new List<Retorno>();

             return Json(resposta, JsonRequestBehavior.AllowGet);
         }

		 [HttpPost]
		 public JsonResult AtualizarAtributos(FeicaoObjeto feicao)
		 {
			 FeicaoBusiness busGeo = new FeicaoBusiness();
			 Retorno retorno = null;
			 if (feicao != null && feicao.Atributos != null)
			 {
				 ProjetoBusiness.InvalidarProcessamento(feicao.IdProjeto);
				 retorno = busGeo.AtualizarAtributos(feicao.Atributos, feicao.IdLayerFeicao, feicao.ObjectId, feicao.IdProjeto);
			 }
			 if (retorno == null)
				 retorno = new Retorno();

			 return Json(retorno, JsonRequestBehavior.AllowGet);
		 }

         [HttpPost]
         public JsonResult ImportarFeicoes(int idNavegador, int idProjeto, bool isFinalizadas)
         {
             FeicaoBusiness busGeo = new FeicaoBusiness();

             LayerFeicaoBusiness.LimparCacheLayers(idProjeto);
             ProjetoBusiness.InvalidarProcessamento(idProjeto);
             Retorno retorno = busGeo.ImportarFeicoes(idNavegador, idProjeto, isFinalizadas);

             if (retorno == null)
                 retorno = new Retorno();

             return Json(retorno, JsonRequestBehavior.AllowGet);
         }
    }
}
