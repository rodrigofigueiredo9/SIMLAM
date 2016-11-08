using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Geobases.WebServices.Models.Business;
using Tecnomapas.Geobases.WebServices.Models.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.Geobases.WebServices.Controllers
{
    public class TopologiaController : Controller
    {
		TopologiaBus _bus = new TopologiaBus();

		[HttpPost]
		public JsonResult Relacao(List<string> feicoes, string wkt)
		{
			List<FeicaoJson> lstFeicoes = new List<FeicaoJson>();
			try
			{
				lstFeicoes = _bus.ObterAtributosGeoRelacao(feicoes, wkt);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Json(new { @Data = lstFeicoes, Erros = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}
    }
}
