using System;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
using Tecnomapas.Blocos.Entities.Interno.ModuloAgrotoxico;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMAgrotoxicos;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloAgrotoxico.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class AgrotoxicosController : DefaultController
	{
		#region Propriedades

		AgrotoxicoBus _bus = new AgrotoxicoBus();

		#endregion

		[Permite(RoleArray = new Object[] { ePermissao.ConsultarAgrotoxicos })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(ListaCredenciadoBus.QuantPaginacao, _bus.AgrotoxicoClasseUso(), _bus.AgrotoxicoModalidadeAplicacao(), 
				_bus.AgrotoxicoGrupoQuimico(), _bus.AgrotoxicoClassificacaoToxicologica(), _bus.AgrotoxicoSituacao());

			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);

			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ConsultarAgrotoxicos })]		
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

		[Permite(RoleArray = new Object[] { ePermissao.ConsultarAgrotoxicos })]
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