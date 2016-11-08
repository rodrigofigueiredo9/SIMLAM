using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPapel;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPapel.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMPapel;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class PapelController : DefaultController
	{
		#region Propriedades

		PapelBus _busPapel = new PapelBus();
		ListaBus _busLista = new ListaBus();

		private string QuantidadePorPagina
		{
			get { return (Request.Cookies.Get("QuantidadePorPagina") != null) ? Request.Cookies.Get("QuantidadePorPagina").Value : "5"; }
		}

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.PapelCriar })]
		public ActionResult Criar()
		{
			PapelVM papel = new PapelVM();

			papel.GrupoColecao = _busPapel.PermissaoGrupoColecao;

			return View(papel);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PapelCriar })]
		public ActionResult Criar(PapelVM papelVM)
		{
			Papel papel = new Papel();

			papel.Nome = papelVM.Nome;
			papel.Permissoes = new List<Permissao>();

			var perColecao = papelVM.GrupoColecao.SelectMany(c => c.PermissaoColecao.Where(p => p.IsPermitido));
			papel.Permissoes.AddRange(perColecao);

			if (_busPapel.Salvar(papel))
			{
				return RedirectToAction("Criar", Validacao.QueryParamSerializer());
			}

			PapelVM papelVMRetorno = new PapelVM();
			papelVMRetorno.Nome = papelVM.Nome;
			papelVMRetorno.GrupoColecao = _busPapel.PermissaoGrupoColecao;

			foreach (var grupo in papelVMRetorno.GrupoColecao)
			{
				foreach (var permissaoRet in grupo.PermissaoColecao)
				{
					foreach (var permissao in papel.Permissoes)
					{
						permissaoRet.IsPermitido = (permissao.IsPermitido && permissao.ID == permissaoRet.ID);
					}
				}
			}

			return View(papelVMRetorno);
		}

		#endregion

		#region Filtrar

		[Permite(RoleArray = new Object[] { ePermissao.PapelListar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(_busLista.QuantPaginacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(QuantidadePorPagina);
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PapelListar })]
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

			Resultados<Papel> resultados = _busPapel.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			if (!vm.PodeAssociar)
			{
				vm.PodeEditar = User.IsInRole(ePermissao.PapelEditar.ToString());
				vm.PodeExcluir = User.IsInRole(ePermissao.PapelExcluir.ToString());
			}

			vm.PodeVisualizar = User.IsInRole(ePermissao.PapelVisualizar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.PapelEditar })]
		public ActionResult Editar(int id)
		{
			PapelVM papelVM = new PapelVM();

			papelVM.ID = id;
			papelVM.GrupoColecao = _busPapel.ObterPermissaoGrupoColecao(id);
			papelVM.Nome = _busPapel.ObterNome(id);

			return View(papelVM);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PapelEditar })]
		public ActionResult Editar(PapelVM papelVM)
		{
			Papel papel = new Papel();

			papel.Id = papelVM.ID;
			papel.Nome = papelVM.Nome;
			papel.Permissoes = new List<Permissao>();

			var perColecao = papelVM.GrupoColecao.SelectMany(c => c.PermissaoColecao.Where(p => p.IsPermitido));
			papel.Permissoes.AddRange(perColecao);

			if (_busPapel.Salvar(papel))
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			papelVM = new PapelVM();

			papelVM.GrupoColecao = _busPapel.ObterPermissaoGrupoColecao(papel.Id);
			papelVM.Nome = _busPapel.ObterNome(papel.Id);

			return View(papelVM);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.PapelVisualizar })]
		public ActionResult Visualizar(int id)
		{
			PapelVM papelVM = new PapelVM();

			papelVM.GrupoColecao = _busPapel.ObterPermissaoGrupoColecao(id);
			papelVM.Nome = _busPapel.ObterNome(id);

			return View(papelVM);
		}

		#endregion

		#region Excluir

		[Permite(RoleArray = new Object[] { ePermissao.PapelExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();

			Papel papel = new Papel { Id = id, Nome = _busPapel.ObterNome(id) };
			vm.Id = id;
			vm.Mensagem = Mensagem.Papel.MensagemExcluir(papel.Nome);
			vm.Titulo = "Excluir Papel";
			return PartialView("Excluir", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PapelExcluir })]
		public ActionResult Excluir(int id)
		{
			_busPapel.Excluir(id);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}