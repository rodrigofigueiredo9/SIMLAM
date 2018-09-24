using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMExploracaoFlorestal;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class ExploracaoFlorestalController : DefaultController
	{
		#region Propriedades

		ListaBus _listaBus = new ListaBus();
		ExploracaoFlorestalBus _bus = new ExploracaoFlorestalBus();
		ExploracaoFlorestalValidar _validar = new ExploracaoFlorestalValidar();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.ExploracaoFlorestalCriar })]
		public ActionResult Criar(int id)
		{
			if (!_caracterizacaoValidar.Basicas(id))
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());

			var exploracaoFlorestalList = _bus.ObterDadosGeo(id);

			if (!_validar.Acessar(id))
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });

			var vmList = new ExploracaoFlorestalListVM();
			vmList.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(id, eCaracterizacao.ExploracaoFlorestal, eCaracterizacaoDependenciaTipo.Caracterizacao);
			vmList.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;
			vmList.TextoAbrirModal = _validar.AbrirModalAcessar(id);

			foreach (var caracterizacao in exploracaoFlorestalList)
			{
				ExploracaoFlorestalVM vm = new ExploracaoFlorestalVM(caracterizacao, _listaBus.ExploracaoFlorestalFinalidadesExploracoes,
					_listaBus.ExploracaoFlorestalClassificacoesVegetais, _listaBus.ExploracaoFlorestalExploracoesTipos, _listaBus.CaracterizacaoProdutosExploracao,
					_listaBus.TipoExploracaoFlorestal);

				vmList.ExploracaoFlorestalVM.Add(vm);
			}

			return View(vmList);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ExploracaoFlorestalCriar })]
		public ActionResult Criar(List<ExploracaoFlorestal> caracterizacao)
		{
			int empreendimentoId = 0;
			foreach (var exploracao in caracterizacao)
			{
				string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
					exploracao.EmpreendimentoId,
					(int)eCaracterizacao.ExploracaoFlorestal,
					eCaracterizacaoDependenciaTipo.Caracterizacao,
					exploracao.Dependencias);

				if (!string.IsNullOrEmpty(textoMerge))
					return Json(new { @TextoMerge = textoMerge }, JsonRequestBehavior.AllowGet);

				_bus.Salvar(exploracao);
				empreendimentoId = exploracao.EmpreendimentoId;
			}

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = empreendimentoId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.ExploracaoFlorestalEditar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.exploracaoflorestal)]
		public ActionResult Editar(int id)
		{
			var exploracaoFlorestal = _bus.ObterPorId(id, false);
			if (!_caracterizacaoValidar.Basicas(exploracaoFlorestal.EmpreendimentoId))
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());

			if (!_validar.Acessar(exploracaoFlorestal.EmpreendimentoId))
				return RedirectToAction("", "Caracterizacao", new { id = exploracaoFlorestal.EmpreendimentoId, Msg = Validacao.QueryParam() });

			var vmList = new ExploracaoFlorestalListVM();
			vmList.Dependencias = exploracaoFlorestal.Dependencias;
			var textoMerge = _caracterizacaoValidar.DependenciasAlteradas(exploracaoFlorestal.EmpreendimentoId,
				(int)eCaracterizacao.ExploracaoFlorestal,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				vmList.Dependencias);
			vmList.TextoMerge = textoMerge;
			vmList.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;
			vmList.TextoAbrirModal = _validar.AbrirModalAcessar(exploracaoFlorestal.EmpreendimentoId);

			if (!string.IsNullOrEmpty(textoMerge))
				exploracaoFlorestal = _bus.MergiarGeo(exploracaoFlorestal);

			ExploracaoFlorestalVM vm = new ExploracaoFlorestalVM(exploracaoFlorestal, _listaBus.ExploracaoFlorestalFinalidadesExploracoes,
				_listaBus.ExploracaoFlorestalClassificacoesVegetais, _listaBus.ExploracaoFlorestalExploracoesTipos, _listaBus.CaracterizacaoProdutosExploracao,
				_listaBus.TipoExploracaoFlorestal);

			vmList.ExploracaoFlorestalVM.Add(vm);

			return View(vmList);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ExploracaoFlorestalEditar })]
		public ActionResult Editar(List<ExploracaoFlorestal> caracterizacao)
		{
			int empreendimentoId = 0;
			foreach (var exploracao in caracterizacao)
			{
				string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
					exploracao.EmpreendimentoId,
					(int)eCaracterizacao.ExploracaoFlorestal,
					eCaracterizacaoDependenciaTipo.Caracterizacao,
					exploracao.Dependencias);

				if (!string.IsNullOrEmpty(textoMerge))
					return Json(new { @TextoMerge = textoMerge }, JsonRequestBehavior.AllowGet);

				_bus.Salvar(exploracao);
				empreendimentoId = exploracao.EmpreendimentoId;
			}

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = empreendimentoId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.exploracaoflorestal)]
		[Permite(RoleArray = new Object[] { ePermissao.ExploracaoFlorestalVisualizar })]
		public ActionResult VisualizarExploracaoFlorestal(int id)
		{
			var exploracaoFlorestal = _bus.ObterPorId(id, false);
			if (!_validar.Acessar(exploracaoFlorestal.EmpreendimentoId))
				return RedirectToAction("", "Caracterizacao", new { id = exploracaoFlorestal.EmpreendimentoId, Msg = Validacao.QueryParam() });

			var vmList = new ExploracaoFlorestalListVM();
			vmList.Dependencias = exploracaoFlorestal.Dependencias;

			var textoMerge = _caracterizacaoValidar.DependenciasAlteradas(exploracaoFlorestal.EmpreendimentoId,
				(int)eCaracterizacao.ExploracaoFlorestal,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				vmList.Dependencias, true);
			vmList.TextoMerge = textoMerge;
			vmList.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;
			vmList.TextoAbrirModal = _validar.AbrirModalAcessar(exploracaoFlorestal.EmpreendimentoId);

			if (!string.IsNullOrEmpty(textoMerge))
				exploracaoFlorestal = _bus.MergiarGeo(exploracaoFlorestal);

			ExploracaoFlorestalVM vm = new ExploracaoFlorestalVM(exploracaoFlorestal, _listaBus.ExploracaoFlorestalFinalidadesExploracoes,
				_listaBus.ExploracaoFlorestalClassificacoesVegetais, _listaBus.ExploracaoFlorestalExploracoesTipos, _listaBus.CaracterizacaoProdutosExploracao,
				_listaBus.TipoExploracaoFlorestal, true);
			vmList.ExploracaoFlorestalVM.Add(vm);

			return View(vmList);
		}

		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.ExploracaoFlorestalExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			vm.Id = id;
			vm.Mensagem = Mensagem.ExploracaoFlorestal.ExcluirMensagem;
			vm.Titulo = "Excluir Exploração Florestal";

			return PartialView("Excluir", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ExploracaoFlorestalExcluir })]
		public ActionResult Excluir(int id)
		{
			string urlRedireciona = string.Empty;

			if (_bus.Excluir(id))
			{
				urlRedireciona = Url.Action("Index", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, urlRedireciona = urlRedireciona }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Merge

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ExploracaoFlorestalCriar, ePermissao.ExploracaoFlorestalEditar })]
		public ActionResult GeoMergiar(List<ExploracaoFlorestal> caracterizacao)
		{
			var vmList = new ExploracaoFlorestalListVM();
			var empreendimentoId = 0;
			foreach (var exploracao in caracterizacao)
			{
				ExploracaoFlorestalVM vm = new ExploracaoFlorestalVM(_bus.MergiarGeo(exploracao), _listaBus.ExploracaoFlorestalFinalidadesExploracoes,
					_listaBus.ExploracaoFlorestalClassificacoesVegetais, _listaBus.ExploracaoFlorestalExploracoesTipos, _listaBus.CaracterizacaoProdutosExploracao,
					_listaBus.TipoExploracaoFlorestal);
				empreendimentoId = exploracao.EmpreendimentoId;
			}
			vmList.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(empreendimentoId, eCaracterizacao.ExploracaoFlorestal, eCaracterizacaoDependenciaTipo.Caracterizacao);
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ExploracaoFlorestal", vmList),
				@Dependencias = ViewModelHelper.Json(vmList.Dependencias)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Filtrar

		[Permite(Tipo=ePermiteTipo.Logado)]
		public ActionResult Visualizar(int id)
		{
			ListarVM vm = new ListarVM(_listaBus.QuantPaginacao);
			vm.Filtros.EmpreendimentoId = id;
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return PartialView(vm);
		}

		[Permite(Tipo=ePermiteTipo.Logado)]
		public ActionResult Filtrar(ListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ListarVM>(vm.UltimaBusca).Filtros;

			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);

			var resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(_listaBus.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			if (resultados == null)
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;
			vm.PodeVisualizar = User.IsInRole(ePermissao.FiscalizacaoVisualizar.ToString());
			vm.PodeEditar = User.IsInRole(String.Format("{0}Editar", eCaracterizacao.ExploracaoFlorestal.ToString()));

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Caracterizacoes/Views/ExploracaoFlorestal/ListarResultados.ascx", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		public ActionResult GetCodigoExploracao(int tipoExploracao) => Json(new {
			@EhValido = Validacao.EhValido,
			@Msg = Validacao.Erros,
			@CodigoExploracao = _bus.ObterCodigoExploracao(tipoExploracao)
		}, JsonRequestBehavior.AllowGet);
	}
}