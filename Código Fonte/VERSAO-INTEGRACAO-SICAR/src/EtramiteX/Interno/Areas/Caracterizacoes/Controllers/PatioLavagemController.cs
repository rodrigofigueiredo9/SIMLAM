using System;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloPatioLavagem;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloPatioLavagem.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class PatioLavagemController : DefaultController
	{
		#region Propriedades

		ListaBus _listaBus = new ListaBus();
		PatioLavagemBus _bus = new PatioLavagemBus();
		PatioLavagemValidar _validar = new PatioLavagemValidar();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		EmpreendimentoBus _empBus = new EmpreendimentoBus();

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.PatioLavagemCriar })]
		public ActionResult Criar(int id)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			PatioLavagem caracterizacao = new PatioLavagem();
			caracterizacao.EmpreendimentoId = id;

			if (!_validar.Acessar(caracterizacao.EmpreendimentoId))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			caracterizacao.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(id, eCaracterizacao.PatioLavagem, eCaracterizacaoDependenciaTipo.Caracterizacao);
			PatioLavagemVM vm = new PatioLavagemVM(caracterizacao, _listaBus.AtividadesSolicitada, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.PatioLavagem, (eTipoGeometria)caracterizacao.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo);

			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PatioLavagemCriar })]
		public ActionResult Criar(PatioLavagem caracterizacao)
		{
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.PatioLavagem,
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

		[Permite(RoleArray = new Object[] { ePermissao.PatioLavagemEditar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.patiolavagem)]
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

			PatioLavagem caracterizacao = _bus.ObterPorEmpreendimento(id);
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.PatioLavagem,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao = _bus.MergiarGeo(caracterizacao);
			}

			PatioLavagemVM vm = new PatioLavagemVM(caracterizacao, _listaBus.AtividadesSolicitada, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.PatioLavagem, (eTipoGeometria)caracterizacao.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo, isVisualizar: false, isEditar: true);

			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PatioLavagemEditar })]
		public ActionResult Editar(PatioLavagem caracterizacao)
		{
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.PatioLavagem,
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

		[Permite(RoleArray = new Object[] { ePermissao.PatioLavagemVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.patiolavagem)]
		public ActionResult Visualizar(int id)
		{

			if (!_validar.Acessar(id))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			PatioLavagem caracterizacao = _bus.ObterPorEmpreendimento(id);

			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.PatioLavagem,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias, true);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao = _bus.MergiarGeo(caracterizacao);
			}

			PatioLavagemVM vm = new PatioLavagemVM(caracterizacao, _listaBus.AtividadesSolicitada, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.PatioLavagem, (eTipoGeometria)caracterizacao.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo, true);

			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			return View(vm);

		}

		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.PatioLavagemExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			vm.Id = id;
			vm.Mensagem = Mensagem.PatioLavagem.ExcluirMensagem;
			vm.Titulo = "Excluir Pátio de Lavagem, Abastecimento e Descontaminação de Aeronave Agrícola";

			return PartialView("Excluir", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PatioLavagemExcluir })]
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

		[Permite(RoleArray = new Object[] { ePermissao.PatioLavagemCriar, ePermissao.PatioLavagemEditar, ePermissao.PatioLavagemVisualizar })]
		public ActionResult ObterDadosTipoGeometria(int empreendimentoId, int caracterizacaoTipo)
		{
			return Json(new
			{
				@TiposGeometricos = _caracterizacaoBus.ObterTipoGeometria(empreendimentoId, caracterizacaoTipo)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PatioLavagemCriar, ePermissao.PatioLavagemEditar, ePermissao.PatioLavagemVisualizar })]
		public ActionResult ObterDadosCoordenadaAtividade(int empreendimentoId, int tipoGeometria)
		{
			return Json(new
			{
				@CoordenadaAtividade = _caracterizacaoBus.ObterCoordenadaAtividadeLst(empreendimentoId, eCaracterizacao.PatioLavagem, (eTipoGeometria)tipoGeometria)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region GeoMergiar

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PatioLavagemCriar, ePermissao.PatioLavagemEditar })]
		public ActionResult GeoMergiar(PatioLavagem caracterizacao)
		{
			PatioLavagemVM vm = new PatioLavagemVM(_bus.MergiarGeo(caracterizacao), _listaBus.AtividadesSolicitada, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.PatioLavagem, (eTipoGeometria)caracterizacao.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "PatioLavagem", vm),
				@Dependencias = ViewModelHelper.Json(vm.Caracterizacao.Dependencias)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

	}
}
