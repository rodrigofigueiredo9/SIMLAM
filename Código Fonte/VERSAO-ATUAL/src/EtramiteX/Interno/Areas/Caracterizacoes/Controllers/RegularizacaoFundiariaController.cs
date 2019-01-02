using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class RegularizacaoFundiariaController : DefaultController
	{
		#region Propriedades

		ListaBus _listaBus = new ListaBus();
		RegularizacaoFundiariaBus _bus = new RegularizacaoFundiariaBus();
		RegularizacaoFundiariaValidar _validar = new RegularizacaoFundiariaValidar();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.RegularizacaoFundiariaCriar })]
		public ActionResult Criar(int id)
		{

			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			RegularizacaoFundiaria caracterizacao = _bus.ObterDadosGeo(id);
			caracterizacao.EmpreendimentoId = id;

			if (!_validar.Acessar(caracterizacao))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			caracterizacao.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(id, eCaracterizacao.RegularizacaoFundiaria, eCaracterizacaoDependenciaTipo.Caracterizacao);

			RegularizacaoFundiariaVM vm = new RegularizacaoFundiariaVM(caracterizacao);
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RegularizacaoFundiariaCriar })]
		public ActionResult Criar(RegularizacaoFundiaria caracterizacao)
		{
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.RegularizacaoFundiaria,
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

		[Permite(RoleArray = new Object[] { ePermissao.RegularizacaoFundiariaEditar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.regularizacaofundiaria)]
		public ActionResult Editar(int id)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			RegularizacaoFundiaria caracterizacao = _bus.ObterPorEmpreendimento(id);
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.RegularizacaoFundiaria,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (string.IsNullOrEmpty(textoMerge) && _validar.EmpreendimentoZonaAlterada(id))
			{
				textoMerge = Mensagem.Caracterizacao.SegmentoEmpreendimentoAlterado.Texto;
			}

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao = _bus.MergiarGeo(caracterizacao);
			}

			if (!_validar.Acessar(caracterizacao))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			RegularizacaoFundiariaVM vm = new RegularizacaoFundiariaVM(
				caracterizacao,
				_listaBus.RegularizacaoFundiariaRelacaoTrabalho,
				_listaBus.RegularizacaoFundiariaTipoLimite,
				_listaBus.RegularizacaoFundiariaTipoRegularizacao,
				_listaBus.RegularizacaoFundiariaTipoUso,
				_listaBus.RegularizacaoFundiariaHomologacao,
				_listaBus.DominialidadeComprovacoes);

			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RegularizacaoFundiariaEditar })]
		public ActionResult Editar(RegularizacaoFundiaria caracterizacao)
		{
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.RegularizacaoFundiaria,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				return Json(new { @TextoMerge = textoMerge }, JsonRequestBehavior.AllowGet);
			}

			_bus.Salvar(caracterizacao);

			var msg = Validacao.Erros.Find(x => x.Texto == Mensagem.Caracterizacao.SegmentoEmpreendimentoAlterado.Texto);

			if (msg != null)
			{
				return Json(new { @TextoMerge = Mensagem.Caracterizacao.SegmentoEmpreendimentoAlterado.Texto }, JsonRequestBehavior.AllowGet);
			}

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.RegularizacaoFundiariaVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.regularizacaofundiaria)]
		public ActionResult Visualizar(int id)
		{
			RegularizacaoFundiaria caracterizacao = _bus.ObterPorEmpreendimento(id);
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.RegularizacaoFundiaria,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias, true);

			if (string.IsNullOrEmpty(textoMerge) && _validar.EmpreendimentoZonaAlterada(id))
			{
				textoMerge = Mensagem.Caracterizacao.SegmentoEmpreendimentoAlterado.Texto;
			}

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao = _bus.MergiarGeo(caracterizacao);
			}

			if (!_validar.Acessar(caracterizacao, true))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			RegularizacaoFundiariaVM vm = new RegularizacaoFundiariaVM(caracterizacao, true);
			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			return View(vm);
		}

		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.RegularizacaoFundiariaExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			vm.Id = id;
			vm.Mensagem = Mensagem.RegularizacaoFundiaria.ExcluirMensagem;
			vm.Titulo = "Excluir Regularização Fundiária";

			return PartialView("Excluir", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RegularizacaoFundiariaExcluir })]
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

		[Permite(RoleArray = new Object[] { ePermissao.RegularizacaoFundiariaCriar, ePermissao.RegularizacaoFundiariaEditar, ePermissao.RegularizacaoFundiariaVisualizar })]
		public ActionResult CriarPosse(int empreendimento, Posse posse, List<Dominio> matriculas)
		{
			RegularizacaoFundiaria caracterizacao = new RegularizacaoFundiaria();

			caracterizacao.Matriculas = matriculas ?? new List<Dominio>();
			caracterizacao.Posse = posse;

			RegularizacaoFundiariaVM vm = new RegularizacaoFundiariaVM(caracterizacao, _listaBus.RegularizacaoFundiariaRelacaoTrabalho, _listaBus.RegularizacaoFundiariaTipoLimite, _listaBus.RegularizacaoFundiariaTipoRegularizacao, _listaBus.RegularizacaoFundiariaTipoUso, _listaBus.RegularizacaoFundiariaHomologacao, _listaBus.DominialidadeComprovacoes);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "RegularizacaoFundiariaPartial", vm) }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RegularizacaoFundiariaCriar, ePermissao.RegularizacaoFundiariaEditar, ePermissao.RegularizacaoFundiariaVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.regularizacaofundiariaposse)]
		public ActionResult EditarPosse(int id, int empreendimento, Posse posse, List<Dominio> matriculas)
		{
			RegularizacaoFundiaria caracterizacao = new RegularizacaoFundiaria();

			caracterizacao.Matriculas = matriculas ?? new List<Dominio>();
			caracterizacao.Posse = posse;

			RegularizacaoFundiariaVM vm = new RegularizacaoFundiariaVM(caracterizacao, _listaBus.RegularizacaoFundiariaRelacaoTrabalho, _listaBus.RegularizacaoFundiariaTipoLimite, _listaBus.RegularizacaoFundiariaTipoRegularizacao, _listaBus.RegularizacaoFundiariaTipoUso, _listaBus.RegularizacaoFundiariaHomologacao, _listaBus.DominialidadeComprovacoes);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "RegularizacaoFundiariaPartial", vm) }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RegularizacaoFundiariaCriar, ePermissao.RegularizacaoFundiariaEditar, ePermissao.RegularizacaoFundiariaVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.regularizacaofundiariaposse)]
		public ActionResult VisualizarPosse(int id, int empreendimento, Posse posse, List<Dominio> matriculas)
		{
			RegularizacaoFundiaria caracterizacao = new RegularizacaoFundiaria();

			caracterizacao.Matriculas = matriculas ?? new List<Dominio>();
			caracterizacao.Posse = posse;

			RegularizacaoFundiariaVM vm = new RegularizacaoFundiariaVM(caracterizacao, _listaBus.RegularizacaoFundiariaRelacaoTrabalho, _listaBus.RegularizacaoFundiariaTipoLimite, _listaBus.RegularizacaoFundiariaTipoRegularizacao, _listaBus.RegularizacaoFundiariaTipoUso, _listaBus.RegularizacaoFundiariaHomologacao, _listaBus.DominialidadeComprovacoes, true);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "RegularizacaoFundiariaPartial", vm) }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RegularizacaoFundiariaCriar, ePermissao.RegularizacaoFundiariaEditar })]
		public ActionResult ValidarDominioAvulso(List<Dominio> dominioLista, Dominio dominio)
		{
			_validar.DominioAvulso(dominioLista, dominio);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RegularizacaoFundiariaCriar, ePermissao.RegularizacaoFundiariaEditar })]
		public ActionResult ValidarTransmitente(List<TransmitentePosse> transmitenteLista, TransmitentePosse transmitente)
		{
			_validar.Transmitente(transmitenteLista, transmitente);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RegularizacaoFundiariaCriar, ePermissao.RegularizacaoFundiariaEditar })]
		public ActionResult ValidarUsoAtualSolo(List<UsoAtualSolo> usoAtualSoloLista, UsoAtualSolo usoAtualSolo)
		{
			_validar.UsoAtualSolo(usoAtualSoloLista, usoAtualSolo);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RegularizacaoFundiariaCriar, ePermissao.RegularizacaoFundiariaEditar })]
		public ActionResult ValidarEdificacao(Edificacao edificacao)
		{
			_validar.Edificacao(edificacao);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RegularizacaoFundiariaCriar, ePermissao.RegularizacaoFundiariaEditar })]
		public ActionResult ValidarPosse(Posse posse, int empreendimentoId)
		{
			_validar.Posse(posse, empreendimentoId);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RegularizacaoFundiariaCriar, ePermissao.RegularizacaoFundiariaEditar })]
		public ActionResult ObterAreaTotalPosse(int dominio)
		{
			DominialidadeBus dominialidadeBus = new DominialidadeBus();
			Dominio aux = dominialidadeBus.ObterDominio(dominio);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, AreaTotalPosse = aux.AreaCroqui.ToStringTrunc(2) }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RegularizacaoFundiariaCriar, ePermissao.RegularizacaoFundiariaEditar })]
		public ActionResult ObterPerimetroPosse(int dominio)
		{
			DominialidadeBus dominialidadeBus = new DominialidadeBus();
			Dominio aux = dominialidadeBus.ObterDominio(dominio);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, PerimetroPosse = aux.Perimetro.ToStringTrunc(3) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Merge

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar, ePermissao.DominialidadeEditar })]
		public ActionResult GeoMergiar(RegularizacaoFundiaria caracterizacao)
		{
			RegularizacaoFundiariaVM vm = new RegularizacaoFundiariaVM(_bus.MergiarGeo(caracterizacao));

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "RegularizacaoFundiariaLista", vm),
				@Dependencias = ViewModelHelper.Json(vm.Caracterizacao.Dependencias)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}