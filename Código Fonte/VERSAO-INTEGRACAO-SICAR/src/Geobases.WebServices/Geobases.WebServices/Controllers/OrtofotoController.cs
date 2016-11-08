using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Geobases.WebServices.Models.Business;
using Tecnomapas.Geobases.WebServices.Models.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.Geobases.WebServices.Models.Security;

namespace Tecnomapas.Geobases.WebServices.Controllers
{
	public class OrtofotoController : Controller
	{
		[HttpPost]
		[Permite()]
		public JsonResult ObterOrtofoto(string wkt)
		{
			List<Ortofoto> lstOrtofotos = new List<Ortofoto>();

			try
			{
				OrtofotoBus ortofotoBus = new OrtofotoBus();
				lstOrtofotos = ortofotoBus.ObterArquivosOrtofoto(wkt);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Json(new { @Data = lstOrtofotos, Erros = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}
	}
}