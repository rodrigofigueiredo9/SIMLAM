﻿using System;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels;
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
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			ExploracaoFlorestal caracterizacao = _bus.ObterDadosGeo(id);
			caracterizacao.EmpreendimentoId = id;

			if (!_validar.Acessar(caracterizacao.EmpreendimentoId))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			caracterizacao.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(id, eCaracterizacao.ExploracaoFlorestal, eCaracterizacaoDependenciaTipo.Caracterizacao);
			ExploracaoFlorestalVM vm = new ExploracaoFlorestalVM(caracterizacao, _listaBus.ExploracaoFlorestalFinalidadesExploracoes, 
				_listaBus.ExploracaoFlorestalClassificacoesVegetais, _listaBus.ExploracaoFlorestalExploracoesTipos, _listaBus.CaracterizacaoProdutosExploracao);
			
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;
			vm.TextoAbrirModal = _validar.AbrirModalAcessar(caracterizacao);
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ExploracaoFlorestalCriar })]
		public ActionResult Criar(ExploracaoFlorestal caracterizacao)
		{
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.ExploracaoFlorestal,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				return Json(new { @TextoMerge = textoMerge }, JsonRequestBehavior.AllowGet);
			}

			_bus.Salvar(caracterizacao);
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.ExploracaoFlorestalEditar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.exploracaoflorestal)]
		public ActionResult Editar(int id)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			if (!_validar.Acessar(id))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			ExploracaoFlorestal caracterizacao = _bus.ObterPorEmpreendimento(id);
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.ExploracaoFlorestal,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao = _bus.MergiarGeo(caracterizacao);
			}

			ExploracaoFlorestalVM vm = new ExploracaoFlorestalVM(caracterizacao, _listaBus.ExploracaoFlorestalFinalidadesExploracoes,
				_listaBus.ExploracaoFlorestalClassificacoesVegetais, _listaBus.ExploracaoFlorestalExploracoesTipos, _listaBus.CaracterizacaoProdutosExploracao);

			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;
			vm.TextoAbrirModal = _validar.AbrirModalAcessar(caracterizacao);

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ExploracaoFlorestalEditar })]
		public ActionResult Editar(ExploracaoFlorestal caracterizacao)
		{
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.ExploracaoFlorestal,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				return Json(new { @TextoMerge = textoMerge }, JsonRequestBehavior.AllowGet);
			}

			_bus.Salvar(caracterizacao);
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.exploracaoflorestal)]
		[Permite(RoleArray = new Object[] { ePermissao.ExploracaoFlorestalVisualizar })]
		public ActionResult Visualizar(int id)
		{
			if (!_validar.Acessar(id))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			ExploracaoFlorestal caracterizacao = _bus.ObterPorEmpreendimento(id);
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.ExploracaoFlorestal,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias, true);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao = _bus.MergiarGeo(caracterizacao);
			}

			ExploracaoFlorestalVM vm = new ExploracaoFlorestalVM(caracterizacao, _listaBus.ExploracaoFlorestalFinalidadesExploracoes,
				_listaBus.ExploracaoFlorestalClassificacoesVegetais, _listaBus.ExploracaoFlorestalExploracoesTipos, _listaBus.CaracterizacaoProdutosExploracao, true);

			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;
			vm.TextoAbrirModal = _validar.AbrirModalAcessar(caracterizacao);

			return View(vm);
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
		public ActionResult GeoMergiar(ExploracaoFlorestal exploracao)
		{
			ExploracaoFlorestalVM vm = new ExploracaoFlorestalVM(_bus.MergiarGeo(exploracao), _listaBus.ExploracaoFlorestalFinalidadesExploracoes,
				_listaBus.ExploracaoFlorestalClassificacoesVegetais, _listaBus.ExploracaoFlorestalExploracoesTipos, _listaBus.CaracterizacaoProdutosExploracao);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ExploracaoFlorestal", vm),
				@Dependencias = ViewModelHelper.Json(vm.Caracterizacao.Dependencias)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}