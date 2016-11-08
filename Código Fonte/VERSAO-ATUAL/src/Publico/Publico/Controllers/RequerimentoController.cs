using System;
using System.Web.Mvc;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloRequerimento.Pdf;
using Tecnomapas.EtramiteX.Publico.ViewModels;

namespace Tecnomapas.EtramiteX.Publico.Controllers
{
	public class RequerimentoController : DefaultController
	{
		public ActionResult GerarPdf(int id)
		{
			try
			{
				id = 0;
				return ViewModelHelper.GerarArquivoPdf(new PdfRequerimentoPadrao().Gerar(id), "Requerimento Padrao");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}
	}
}