using System;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFichaFundiaria;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFichaFundiaria.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFichaFundiaria.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMFichaFundiaria;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class FichaFundiariaController : DefaultController
	{
		#region Propriedades

		FichaFundiariaBus _bus = new FichaFundiariaBus();
		ListaBus _busLista = new ListaBus();

		#endregion

		#region Filtrar

		[Permite(RoleArray = new Object[] { ePermissao.FichaFundiariaListar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(_busLista.QuantPaginacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.FichaFundiariaListar })]
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

			Resultados<FichaFundiaria> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeVisualizar = User.IsInRole(ePermissao.FichaFundiariaVisualizar.ToString());
			vm.PodeEditar = User.IsInRole(ePermissao.FichaFundiariaEditar.ToString());
			vm.PodeExcluir = User.IsInRole(ePermissao.FichaFundiariaExcluir.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.FichaFundiariaCriar })]
		public ActionResult Criar()
		{
			FichaFundiariaVM vm = new FichaFundiariaVM();
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.FichaFundiariaCriar })]
		public ActionResult Criar(FichaFundiaria FichaFundiaria)
		{
			string urlRedirecionar = urlRedirecionar = Url.Action("Index");

			if (_bus.Salvar(FichaFundiaria))
			{
				urlRedirecionar += "?Msg=" + Validacao.QueryParam();
				return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, UrlRedirecionar = urlRedirecionar });
			}

			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros });
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.FichaFundiariaEditar })]
		public ActionResult Editar(int id)
		{
			FichaFundiariaVM vm = new FichaFundiariaVM(_bus.Obter(id));
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.FichaFundiariaEditar })]
		public ActionResult Editar(FichaFundiaria FichaFundiaria)
		{
			string urlRedirecionar = urlRedirecionar = Url.Action("Index", "FichaFundiaria");

			if (_bus.Salvar(FichaFundiaria))
			{
				urlRedirecionar += "?Msg=" + Validacao.QueryParam();
				return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, UrlRedirecionar = urlRedirecionar });
			}

			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros });
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.FichaFundiariaVisualizar })]
		public ActionResult Visualizar(int id)
		{
			FichaFundiariaVM vm = new FichaFundiariaVM(_bus.Obter(id), isVisualizar: true);
			vm.PodeEditar = User.IsInRole(ePermissao.FichaFundiariaEditar.ToString());
			
			return View(vm);
		}

		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.FichaFundiariaExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			vm.Id = id;
			vm.Titulo = "Excluir Ficha Fundiária";
			vm.Mensagem = Mensagem.FichaFundiaria.ExcluirConfirmacao;
			return View("Excluir", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.FichaFundiariaExcluir })]
		public ActionResult Excluir(int id)
		{
			_bus.Excluir(id);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido });
		}

		#endregion

		#region PDF

		[Permite(RoleArray = new Object[] { ePermissao.FichaFundiariaVisualizar })]
		public ActionResult GerarPdf(int id)
		{
			try
			{
				return ViewModelHelper.GerarArquivoPdf(new FichaFundiariaPDF().Gerar(id), "Ficha Fundiaria");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		#endregion
	}
}