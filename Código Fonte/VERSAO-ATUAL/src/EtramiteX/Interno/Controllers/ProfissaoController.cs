using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloProfissao;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProfissao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMProfissao;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class ProfissaoController : DefaultController
	{
		#region Propriedades

		ProfissaoBus _bus = new ProfissaoBus();
		ListaBus _busLista = new ListaBus();
		ProfissaoValidar _validar = new ProfissaoValidar();

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.ProfissaoCriar })]
		public ActionResult Criar()
		{
			ProfissaoVM vm = new ProfissaoVM();
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProfissaoCriar })]
		public ActionResult Criar(Profissao profissao)
		{
			if (!_bus.Salvar(profissao))
			{
				return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros });	
			}

			return Json(new
			{
				EhValido = Validacao.EhValido,
				Msg = Validacao.Erros,
				UrlRedirecionar = Url.Action("Criar", "Profissao", new { acaoId = profissao.Id, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.ProfissaoEditar })]
		public ActionResult Editar(int id)
		{
			Profissao profissao = _bus.Obter(id);

			if (!_validar.Editar(profissao))
			{
				if (Request.IsAjaxRequest())
				{
					return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
				}

				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			ProfissaoVM vm = new ProfissaoVM(profissao);
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProfissaoEditar })]
		public ActionResult Editar(Profissao profissao)
		{
			if (!_bus.Salvar(profissao))
			{
				return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros });
			}

			return Json(new
			{
				EhValido = Validacao.EhValido,
				Msg = Validacao.Erros,
				UrlRedirecionar = Url.Action("Index", "Profissao", new { Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Filtrar/Associar

		[Permite(RoleArray = new Object[] { ePermissao.ProfissaoListar })]
		public ActionResult Index()
		{
			ProfissaoListarVM vm = new ProfissaoListarVM(_busLista.QuantPaginacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProfissaoListar, ePermissao.PessoaCriar, ePermissao.PessoaEditar })]
		public ActionResult Associar()
		{
			ProfissaoListarVM vm = new ProfissaoListarVM(_busLista.QuantPaginacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.IsAssociar = true;

			return PartialView("ListarFiltros", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProfissaoListar ,ePermissao.PessoaCriar, ePermissao.PessoaEditar })]
		public ActionResult Filtrar(ProfissaoListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ProfissaoListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(_busLista.QuantPaginacao, vm.Paginacao.QuantPaginacao);

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