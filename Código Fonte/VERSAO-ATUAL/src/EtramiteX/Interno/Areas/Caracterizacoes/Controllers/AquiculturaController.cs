using System;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloAquicultura;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloAquicultura.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class AquiculturaController : DefaultController
	{
		#region Propriedades

		ListaBus _listaBus = new ListaBus();
		AquiculturaBus _bus = new AquiculturaBus();
		AquiculturaValidar _validar = new AquiculturaValidar();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.AquiculturaCriar })]
		public ActionResult Criar(int id)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			Aquicultura caracterizacao = new Aquicultura();
			caracterizacao.EmpreendimentoId = id;

			if (!_validar.Acessar(caracterizacao.EmpreendimentoId))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			caracterizacao.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(id, eCaracterizacao.Aquicultura, eCaracterizacaoDependenciaTipo.Caracterizacao);

			AquiculturaVM vm = new AquiculturaVM(caracterizacao);
			vm.AquiculturaAquicultTemplateVM = new AquiculturaAquicultVM(new AquiculturaAquicult(){Identificador = Guid.NewGuid().ToString()}, _listaBus.AtividadesSolicitada, _listaBus.CaracterizacaoGeometriaTipo);

			foreach (AquiculturaAquicult aquicultura in caracterizacao.AquiculturasAquicult)
			{
				aquicultura.Identificador = Guid.NewGuid().ToString();
				AquiculturaAquicultVM viewModelAux = new AquiculturaAquicultVM(aquicultura, _listaBus.AtividadesSolicitada, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.Aquicultura, (eTipoGeometria)aquicultura.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo);
				vm.AquiculturaAquicultVM.Add(viewModelAux);
			}


			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.AquiculturaCriar })]
		public ActionResult Criar(Aquicultura caracterizacao)
		{
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.Aquicultura,
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

		[Permite(RoleArray = new Object[] { ePermissao.AquiculturaEditar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.aquicultura)]
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

			Aquicultura caracterizacao = _bus.ObterPorEmpreendimento(id);
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.Aquicultura,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao = _bus.MergiarGeo(caracterizacao);
			}

			AquiculturaVM vm = new AquiculturaVM(caracterizacao, isVisualizar: false, isEditar: true);

			foreach (AquiculturaAquicult aquicultura in caracterizacao.AquiculturasAquicult)
			{
				aquicultura.Identificador = Guid.NewGuid().ToString();
				AquiculturaAquicultVM viewModelAux = new AquiculturaAquicultVM(aquicultura, _listaBus.AtividadesSolicitada, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.Aquicultura, (eTipoGeometria)aquicultura.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo, isEditar: true);
				vm.AquiculturaAquicultVM.Add(viewModelAux);
			}

			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.AquiculturaEditar })]
		public ActionResult Editar(Aquicultura caracterizacao)
		{
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.Aquicultura,
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

		[Permite(RoleArray = new Object[] { ePermissao.AquiculturaVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.aquicultura)]
		public ActionResult Visualizar(int id)
		{
			if (!_validar.Acessar(id))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			Aquicultura caracterizacao = _bus.ObterPorEmpreendimento(id);

			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.Aquicultura,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias, true);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao = _bus.MergiarGeo(caracterizacao);
			}

			AquiculturaVM vm = new AquiculturaVM(caracterizacao, true);

			foreach (AquiculturaAquicult aquicultura in caracterizacao.AquiculturasAquicult)
			{
				aquicultura.Identificador = Guid.NewGuid().ToString();
				AquiculturaAquicultVM viewModelAux = new AquiculturaAquicultVM(aquicultura, _listaBus.AtividadesSolicitada, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.Aquicultura, (eTipoGeometria)aquicultura.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo, isVisualizar: true);
				vm.AquiculturaAquicultVM.Add(viewModelAux);
			}

			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			return View(vm);
		}

		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.AquiculturaExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			vm.Id = id;
			vm.Mensagem = Mensagem.Aquicultura.ExcluirMensagem;
			vm.Titulo = "Excluir Aquicultura";

			return PartialView("Excluir", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.AquiculturaExcluir })]
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

		[Permite(RoleArray = new Object[] { ePermissao.AquiculturaCriar, ePermissao.AquiculturaEditar, ePermissao.AquiculturaVisualizar })]
		public ActionResult ObterDadosTipoGeometria(int empreendimentoId, int caracterizacaoTipo)
		{
			return Json(new
			{
				@TiposGeometricos = _caracterizacaoBus.ObterTipoGeometria(empreendimentoId, caracterizacaoTipo)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.AquiculturaCriar, ePermissao.AquiculturaEditar, ePermissao.AquiculturaVisualizar })]
		public ActionResult ObterDadosCoordenadaAtividade(int empreendimentoId, int tipoGeometria)
		{
			return Json(new
			{
				@CoordenadaAtividade = _caracterizacaoBus.ObterCoordenadaAtividadeLst(empreendimentoId, eCaracterizacao.Aquicultura, (eTipoGeometria)tipoGeometria)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.AquiculturaCriar, ePermissao.AquiculturaEditar })]
		public ActionResult ObterTemplateBeneficiamento()
		{
			AquiculturaAquicultVM vm = new AquiculturaAquicultVM(new AquiculturaAquicult() { Identificador = Guid.NewGuid().ToString() }, _listaBus.AtividadesSolicitada, _listaBus.CaracterizacaoGeometriaTipo);

			String html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Caracterizacoes/Views/Aquicultura/AquiculturaAquicult.ascx", vm);

			return Json(new
			{
				@html = html
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region GeoMergiar

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.AquiculturaCriar, ePermissao.AquiculturaEditar })]
		public ActionResult GeoMergiar(Aquicultura caracterizacao)
		{
			Aquicultura caracterizacaoMerge = _bus.MergiarGeo(caracterizacao);
			AquiculturaVM vm = new AquiculturaVM(caracterizacaoMerge);

			foreach (AquiculturaAquicult aquicultura in caracterizacaoMerge.AquiculturasAquicult)
			{
				aquicultura.Identificador = Guid.NewGuid().ToString();
				AquiculturaAquicultVM viewModelAux = new AquiculturaAquicultVM(aquicultura, _listaBus.AtividadesSolicitada, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.Aquicultura, (eTipoGeometria)aquicultura.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo);
				vm.AquiculturaAquicultVM.Add(viewModelAux);
			}

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "Aquicultura", vm),
				@Dependencias = ViewModelHelper.Json(vm.Caracterizacao.Dependencias)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}
