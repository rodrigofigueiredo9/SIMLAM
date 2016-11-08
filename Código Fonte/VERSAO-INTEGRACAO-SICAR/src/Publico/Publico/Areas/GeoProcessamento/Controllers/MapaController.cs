using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Business;
using Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Entities;

namespace Tecnomapas.EtramiteX.Publico.Controllers
{
    public class MapaController : DefaultController
    {
		public JsonResult ListarEmpreendimentos(String empreendimento, String pessoa, String processo, String segmento, String municipio, String atividade)
		{
			PontoEmpreendimentoBus pontoempreendimento = new PontoEmpreendimentoBus();
			List<PontoEmpreendimento> result = pontoempreendimento.listarEmpreendimentos(empreendimento, pessoa, processo, segmento, municipio, atividade);

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		public JsonResult IdentificarEmpreendimentos(List<String> empreendimentos)
		{
			PontoEmpreendimentoBus pontoempreendimento = new PontoEmpreendimentoBus();
			List<PontoEmpreendimento> result = pontoempreendimento.listarEmpreendimentos(empreendimentos);

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		public JsonResult ObterListas()
		{
			ListasBus listas = new ListasBus();
			Listas result = listas.GetListas();

			return Json(result, JsonRequestBehavior.AllowGet);
		}

    }
}
