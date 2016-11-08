using System;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSuinocultura;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSuinocultura.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class SuinoculturaController : DefaultController
	{

		#region Propriedades

		ListaBus _listaBus = new ListaBus();
		SuinoculturaBus _bus = new SuinoculturaBus();
		SuinoculturaValidar _validar = new SuinoculturaValidar();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.SuinoculturaCriar })]
		public ActionResult Criar(int id)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			Suinocultura caracterizacao = new Suinocultura();
			caracterizacao.EmpreendimentoId = id;

			if (!_validar.Acessar(caracterizacao.EmpreendimentoId))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			caracterizacao.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(id, eCaracterizacao.Suinocultura, eCaracterizacaoDependenciaTipo.Caracterizacao);
			SuinoculturaVM vm = new SuinoculturaVM(caracterizacao, _listaBus.AtividadesSolicitada, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.Suinocultura, (eTipoGeometria)caracterizacao.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo, _listaBus.SuinoculturaFases);

			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.SuinoculturaCriar })]
		public ActionResult Criar(Suinocultura caracterizacao)
		{
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.Suinocultura,
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

		[Permite(RoleArray = new Object[] { ePermissao.SuinoculturaEditar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.suinocultura)]
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

			Suinocultura caracterizacao = _bus.ObterPorEmpreendimento(id);
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.Suinocultura,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao = _bus.MergiarGeo(caracterizacao);
			}

			SuinoculturaVM vm = new SuinoculturaVM(caracterizacao, _listaBus.AtividadesSolicitada, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.Suinocultura, (eTipoGeometria)caracterizacao.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo, _listaBus.SuinoculturaFases, isVisualizar: false, isEditar: true);

			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.SuinoculturaEditar })]
		public ActionResult Editar(Suinocultura caracterizacao)
		{
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.Suinocultura,
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

		[Permite(RoleArray = new Object[] { ePermissao.SuinoculturaVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.suinocultura)]
		public ActionResult Visualizar(int id)
		{
			if (!_validar.Acessar(id))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			Suinocultura caracterizacao = _bus.ObterPorEmpreendimento(id);
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.Suinocultura,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias, true);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao = _bus.MergiarGeo(caracterizacao);
			}

			SuinoculturaVM vm = new SuinoculturaVM(caracterizacao, _listaBus.AtividadesSolicitada, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.Suinocultura, (eTipoGeometria)caracterizacao.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo, _listaBus.SuinoculturaFases, true);
			
			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			return View(vm);
		}

		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.SuinoculturaExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			vm.Id = id;
			vm.Mensagem = Mensagem.Suinocultura.ExcluirMensagem;
			vm.Titulo = "Excluir Suinocultura";

			return PartialView("Excluir", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.SuinoculturaExcluir })]
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

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.SuinoculturaCriar, ePermissao.SuinoculturaEditar, ePermissao.SuinoculturaVisualizar })]
		public ActionResult ObterDadosTipoGeometria(int empreendimentoId, int caracterizacaoTipo)
		{
			return Json(new
			{
				@TiposGeometricos = _caracterizacaoBus.ObterTipoGeometria(empreendimentoId, caracterizacaoTipo)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.SuinoculturaCriar, ePermissao.SuinoculturaEditar, ePermissao.SuinoculturaVisualizar })]
		public ActionResult ObterDadosCoordenadaAtividade(int empreendimentoId, int tipoGeometria)
		{
			return Json(new
			{
				@CoordenadaAtividade = _caracterizacaoBus.ObterCoordenadaAtividadeLst(empreendimentoId, eCaracterizacao.Suinocultura, (eTipoGeometria)tipoGeometria)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region GeoMergiar

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.SuinoculturaCriar, ePermissao.SuinoculturaEditar })]
		public ActionResult GeoMergiar(Suinocultura caracterizacao)
		{
			SuinoculturaVM vm = new SuinoculturaVM(_bus.MergiarGeo(caracterizacao), _listaBus.AtividadesSolicitada, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.Suinocultura, (eTipoGeometria)caracterizacao.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo, _listaBus.SuinoculturaFases);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "Suinocultura", vm),
				@Dependencias = ViewModelHelper.Json(vm.Caracterizacao.Dependencias)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

	}
}
