using System;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.Blocos.Entities.Interno.ModuloAgrotoxico;
using Tecnomapas.EtramiteX.Publico.ViewModels.VMAgrotoxico;
using Tecnomapas.EtramiteX.Publico.Model.ModuloAgrotoxico.Business;
using Tecnomapas.EtramiteX.Publico.ViewModels;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;

namespace Tecnomapas.EtramiteX.Publico.Controllers
{
	public class AgrotoxicoController : DefaultController
	{
		#region Propriedades

		AgrotoxicoBus _bus = new AgrotoxicoBus();

		ListaBus _busLista = new ListaBus();

		#endregion
		
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(_busLista.QuantPaginacao, _bus.AgrotoxicoClasseUso(), _bus.AgrotoxicoModalidadeAplicacao(), 
				_bus.AgrotoxicoGrupoQuimico(), _bus.AgrotoxicoClassificacaoToxicologica(), _bus.AgrotoxicoSituacao());

			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);

			return View(vm);
		}
		
		[HttpPost]
		public ActionResult Filtrar(ListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(ListaCredenciadoBus.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<AgrotoxicoFiltro> resultados = _bus.Filtrar(vm.Filtros, paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}
		
		public ActionResult BaixarPdfAgrotoxico(int id)
		{
			try
			{
				var arquivo = _bus.ObterAgrotoxicoArquivo(id);
				if (arquivo != null)
				{
					return ViewModelHelper.GerarArquivo(arquivo);
				}
				else
				{
					return RedirectToAction("Index", Validacao.QueryParamSerializer());
				}				
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}
	}
}