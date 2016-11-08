using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Business;
using Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Entities;

namespace Tecnomapas.EtramiteX.Publico.Controllers
{
	public class HomeController : DefaultController
	{
		public ActionResult Index()
		{
			return RedirectToAction("Criar", "Credenciado");
			//return RedirectToAction("verificar", "simuladorGeo", new {area="GeoProcessamento"});
		}

		public ActionResult ListarEmpreendimentos(string municipio, string segmento, string processo, string empreendimento, string atividade, string pessoa)
		{
			PontoEmpreendimentoBus bus = new PontoEmpreendimentoBus();

			List<PontoEmpreendimento> pontos = bus.listarEmpreendimentos(empreendimento,pessoa,processo,segmento,municipio,atividade);

			return Json (pontos, JsonRequestBehavior.AllowGet);
		}

		public ActionResult ObterListas()
		{
			ListasBus bus = new ListasBus();

			Listas lista = bus.GetListas();

			return Json(lista, JsonRequestBehavior.AllowGet);
		}
	}
}