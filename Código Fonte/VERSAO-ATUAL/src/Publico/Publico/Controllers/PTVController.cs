using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Publico.ViewModels;
using Tecnomapas.EtramiteX.Publico.ViewModels.VMPTV;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.EtramiteX.Publico.Model.ModuloPTV.Business;

namespace Tecnomapas.EtramiteX.Publico.Controllers
{
	public class PTVController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();

		private PTVBus _busPTV = new PTVBus();

		#endregion

		public ActionResult Index()
		{
			PTVListarVM vm = new PTVListarVM();
			return View(vm);
		}

		public ActionResult Filtrar(PTVListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<PTVListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(new ListaBus().QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<PTVListarResultado> resultados = _busPTV.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeGerarPDF = true;

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new
			{
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm)
			}, JsonRequestBehavior.AllowGet);
		}


		public ActionResult GerarPdfInterno(int id)
		{
			try
			{
				Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV.Pdf.PdfEmissaoPTV pdf = new Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV.Pdf.PdfEmissaoPTV();
				Tecnomapas.EtramiteX.Interno.Model.ModuloPTV.Business.PTVBus PTVBus = new Tecnomapas.EtramiteX.Interno.Model.ModuloPTV.Business.PTVBus();
				PTV PTV = PTVBus.Obter(id, simplificado: true);

				int situacaoId = PTV.Situacao;
				string situacaoTexto = PTV.SituacaoTexto;
				return ViewModelHelper.GerarArquivoPdf(pdf.Gerar(id, PTV.Tid, PTV.Situacao, PTV.SituacaoTexto), "PTV");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", "PTV", Validacao.QueryParamSerializer());
			}
		}

		public ActionResult GerarPdfCredenciado(int id)
		{
			try
			{
				Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV.Pdf.PdfEmissaoPTV pdf = new Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV.Pdf.PdfEmissaoPTV();
				Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business.PTVBus PTVBus = new Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business.PTVBus();
				PTV PTV = PTVBus.Obter(id, simplificado: true);

				int situacaoId = PTV.Situacao;
				string situacaoTexto = PTV.SituacaoTexto;
				return ViewModelHelper.GerarArquivoPdf(pdf.Gerar(id, situacaoId, situacaoTexto, PTV), "EPTV");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", "PTV", Validacao.QueryParamSerializer());
			}
		}
	}
}