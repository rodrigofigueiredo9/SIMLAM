using System;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloRoteiro.Pdf.RoteiroPdf;
using Tecnomapas.EtramiteX.Publico.Model.ModuloRoteiro.Business;
using Tecnomapas.EtramiteX.Publico.Model.ModuloRoteiro.Entities;
using Tecnomapas.EtramiteX.Publico.ViewModels;
using Tecnomapas.EtramiteX.Publico.ViewModels.VMRoteiro;

namespace Tecnomapas.EtramiteX.Publico.Controllers
{
	public class RoteiroController : DefaultController
	{
		ListaBus _busLista = new ListaBus();
		RoteiroBus _bus = new RoteiroBus();

		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(_busLista.Setores, _busLista.QuantPaginacao, _busLista.AtividadesSolicitada);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return View("Index", vm);
		}

		public ActionResult Filtrar(ListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(_busLista.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			vm.Filtros.Situacao = 1;
			Resultados<Roteiro> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.MostrarRelatorio = true;
			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult RelatorioRoteiro(int id, string tid = "")
		{
			try
			{
				return ViewModelHelper.GerarArquivoPdf((new PdfRoteiroOrientativo()).Gerar(id, tid), "Relatorio de Roteiro Orientativo");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}
	}
}