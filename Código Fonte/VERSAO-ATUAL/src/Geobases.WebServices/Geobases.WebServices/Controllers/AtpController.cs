using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Geobases.WebServices.Models.Business;
using Tecnomapas.Geobases.WebServices.Models.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Geobases.WebServices.Controllers
{
    public class MunicipioController : Controller
    {
		[HttpGet]
		public JsonResult LocalizarMunicipio(String municipio)
		{
			List<Municipio> result = new List<Municipio>();
			try
			{
				MunicipioBus bus = new MunicipioBus();
				result = bus.listarMunicipios(municipio);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Json(new { @Data = result, Erros = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult ObterMunicipio(Decimal easting, Decimal northing)
		{
			EstadoBus busEstado = new EstadoBus();
			MunicipioBus bus = new MunicipioBus();
			object objRetorno = null;
			
			try
			{
				objRetorno = new
				{ 
					Municipio = bus.ObterMunicipio(easting, northing),
					EstaNoEstado = busEstado.EstaNoEstado(easting, northing)
				};
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Json(new { @Data = objRetorno, Erros = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}
    }
}
