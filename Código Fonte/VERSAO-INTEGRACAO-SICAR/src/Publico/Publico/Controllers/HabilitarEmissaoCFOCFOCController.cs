using System;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Publico.Model.ModuloHabilitarEmissaoCFOCFOC.Business;
using Tecnomapas.EtramiteX.Publico.ViewModels;
using Tecnomapas.EtramiteX.Publico.ViewModels.VMHabilitarEmissaoCFOCFOC;

namespace Tecnomapas.EtramiteX.Publico.Controllers
{
	public class HabilitarEmissaoCFOCFOCController : DefaultController
	{
		#region Propriedades

		HabilitarEmissaoCFOCFOCBus _bus = new HabilitarEmissaoCFOCFOCBus();

		ListaBus _busLista = new ListaBus();

		#endregion

		public ActionResult IndexHabilitarEmissaoCFOCFOC()
		{
			ListarVM vm = new ListarVM(_busLista.QuantPaginacao, _busLista.CredenciadoTipos, _busLista.CredenciadoSituacoes);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return View("IndexHabilitarEmissaoCFOCFOC", vm);
		}

		public ActionResult FiltrarHabilitarEmissaoCFOCFOC(ListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(_busLista.QuantPaginacao, _busLista.CredenciadoTipos, _busLista.CredenciadoSituacoes, vm.Paginacao.QuantPaginacao);

			var resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);

			if (resultados == null)
			{
				return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;
			vm.PodeVisualizar = true;
			
			return Json(new { Msg = Validacao.Erros, Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultadosHabilitarEmissaoCFOCFOC", vm) }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult VisualizarHabilitarEmissaoCFOCFOC(int id)
		{
			HabilitarEmissaoCFOCFOCVM viewModel = new HabilitarEmissaoCFOCFOCVM(_busLista.HabilitacaoCFOSituacoes, _busLista.Estados, _busLista.HabilitacaoCFOMotivos);
			viewModel.HabilitarEmissao = _bus.Obter(id);
			viewModel.IsVisualizar = true;			
			viewModel.IsAjaxRequest = false;

			return PartialView("HabilitarEmissaoCFOCFOCVisualizar", viewModel);
		}
	}
}