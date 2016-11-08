using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloMotosserra;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloMotosserra.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMMotosserra;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class MotosserraController : DefaultController
	{
		#region Propriedades

		MotosserraBus _bus = new MotosserraBus();
		ListaBus _busLista = new ListaBus();

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.MotosserraCriar })]
		public ActionResult Criar()
		{
			MotosserraVM vm = new MotosserraVM();
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.MotosserraCriar })]
		public ActionResult Criar(Motosserra motosserra)
		{
			string urlRedirecionar = urlRedirecionar = Url.Action("Criar", "Motosserra");

			

			if (_bus.Salvar(motosserra))
			{
				urlRedirecionar += "?Msg=" + Validacao.QueryParam();
				return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, UrlRedirecionar = urlRedirecionar });
			}

			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros });
		}

		#endregion

		#region Filtrar/Associar

		[Permite(RoleArray = new Object[] { ePermissao.MotosserraListar })]
		public ActionResult Index()
		{
			MotosserraListarVM vm = new MotosserraListarVM(_busLista.QuantPaginacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.MotosserraListar })]
		public ActionResult Associar()
		{
			MotosserraListarVM vm = new MotosserraListarVM(_busLista.QuantPaginacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return PartialView("ListarFiltros", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.MotosserraListar })]
		public ActionResult Filtrar(MotosserraListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<MotosserraListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(_busLista.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<Motosserra> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			if (!vm.PodeAssociar)
			{
				vm.PodeEditar = User.IsInRole(ePermissao.MotosserraEditar.ToString());
				vm.PodeExcluir = User.IsInRole(ePermissao.MotosserraExcluir.ToString());
				vm.PodeAlterarSituacao = User.IsInRole(ePermissao.MotosserraAlterarSituacao.ToString());
			}

			vm.PodeVisualizar = User.IsInRole(ePermissao.MotosserraVisualizar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.MotosserraEditar })]
		public ActionResult Editar(int id)
		{
			Motosserra motosserra = _bus.Obter(id);

			if (motosserra.SituacaoId == (int)eMotosserraSituacao.Desativo)
			{
				Validacao.Add(Mensagem.Motosserra.SituacaoNaoPodeEditar("desativado"));
				return RedirectToAction("Index", "Motosserra", Validacao.QueryParamSerializer());
			}

			MotosserraVM vm = new MotosserraVM(_bus.Obter(id));
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.MotosserraEditar })]
		public ActionResult Editar(Motosserra motosserra)
		{
			string urlRedirecionar = urlRedirecionar = Url.Action("Index", "Motosserra");

			if (_bus.Salvar(motosserra))
			{
				urlRedirecionar += "?Msg=" + Validacao.QueryParam();
				return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, UrlRedirecionar = urlRedirecionar });
			}

			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros });
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.MotosserraVisualizar })]
		public ActionResult Visualizar(int id)
		{
			MotosserraVM vm = new MotosserraVM(_bus.Obter(id));
			vm.IsVisualizar = true;

			if (Request.IsAjaxRequest())
			{
				return PartialView("VisualizarMotosserraModal", vm);
			}
			else
			{
				return View(vm);
			}
		}

		#endregion

		#region Excluir

		[Permite(RoleArray = new Object[] { ePermissao.MotosserraExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			Motosserra motosserra = _bus.Obter(id);

			vm.Id = id;
			vm.Mensagem = Mensagem.Motosserra.MensagemExcluir(motosserra.RegistroNumero.ToString());
			vm.Titulo = "Excluir Motosserra";

			return PartialView("Excluir", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.MotosserraExcluir })]
		public ActionResult Excluir(int id)
		{
			_bus.Excluir(id);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.MotosserraCriar })]
		public ActionResult Verificar(String numero)
		{
			bool podeCriarNovo;
			List<Motosserra> motosserras = _bus.Verificar(numero, out podeCriarNovo);
			MotosserraListarVM vm = new MotosserraListarVM();
			vm.Resultados = motosserras;

			String partialResultados = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarMotosserras", vm);

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Html = partialResultados, @PodeCriarNovo = podeCriarNovo }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.MotosserraCriar })]
		public ActionResult ObterPartialCriar()
		{
			MotosserraVM vm = new MotosserraVM();
			String partial = ViewModelHelper.RenderPartialViewToString(ControllerContext, "MotosserraPartial", vm);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Html = partial }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.MotosserraEditar })]
		public ActionResult ValidarEditar(int motosserraId)
		{
			Motosserra motosserra = _bus.Obter(motosserraId);

			if (motosserra.SituacaoId == (int)eMotosserraSituacao.Desativo)
			{
				Validacao.Add(Mensagem.Motosserra.SituacaoNaoPodeEditar("desativado"));
			}

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros}, JsonRequestBehavior.AllowGet);
		}


		#endregion

		#region Alterar Situacao

		[Permite(RoleArray = new Object[] { ePermissao.MotosserraAlterarSituacao})]
		public ActionResult Desativar(int motosserraId)
		{
			_bus.AlterarSituacao(new Motosserra() { Id = motosserraId, SituacaoId = (int)eMotosserraSituacao.Desativo });
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}