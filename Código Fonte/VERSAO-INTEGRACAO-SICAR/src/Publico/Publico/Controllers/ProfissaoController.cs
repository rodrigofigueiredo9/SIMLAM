using System;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloProfissao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProfissao.Business;
using Tecnomapas.EtramiteX.Publico.ViewModels;
using Tecnomapas.EtramiteX.Publico.ViewModels.VMProfissao;

namespace Tecnomapas.EtramiteX.Publico.Controllers
{
	public class ProfissaoController : DefaultController
	{
		#region Propriedades

		public ProfissaoCredenciadoBus _bus = new ProfissaoCredenciadoBus();

		#endregion

		#region Filtrar

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Associar()
		{
			ProfissaoListarVM vm = new ProfissaoListarVM(ListaCredenciadoBus.QuantPaginacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.IsAssociar = true;

			return PartialView("ListarFiltros", vm);
		}

		[HttpPost]
		public ActionResult Filtrar(ProfissaoListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ProfissaoListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(ListaCredenciadoBus.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<Profissao> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}