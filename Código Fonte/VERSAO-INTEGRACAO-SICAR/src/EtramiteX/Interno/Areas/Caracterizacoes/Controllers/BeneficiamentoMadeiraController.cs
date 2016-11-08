using System;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBeneficiamentoMadeira;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloBeneficiamentoMadeira.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class BeneficiamentoMadeiraController : DefaultController
	{
		#region Propriedades

		ListaBus _listaBus = new ListaBus();
		BeneficiamentoMadeiraBus _bus = new BeneficiamentoMadeiraBus();
		BeneficiamentoMadeiraValidar _validar = new BeneficiamentoMadeiraValidar();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.BeneficiamentoMadeiraCriar })]
		public ActionResult Criar(int id)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			BeneficiamentoMadeira caracterizacao = new BeneficiamentoMadeira();
			caracterizacao.EmpreendimentoId = id;

			if (!_validar.Acessar(caracterizacao.EmpreendimentoId))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			caracterizacao.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(id, eCaracterizacao.BeneficiamentoMadeira, eCaracterizacaoDependenciaTipo.Caracterizacao);

			BeneficiamentoMadeiraVM vm = new BeneficiamentoMadeiraVM(caracterizacao);
			vm.BeneficiamentoMadeiraBeneficiamentosTemplateVM = new BeneficiamentoMadeiraBeneficiamentoVM(_listaBus.AtividadesSolicitada, _listaBus.BeneficiamentoMadeiraMateriaPrimaConsumida, _listaBus.CaracterizacaoGeometriaTipo, _listaBus.CaracterizacaoUnidadeMedida);

			foreach (BeneficiamentoMadeiraBeneficiamento beneficiamento in caracterizacao.Beneficiamentos)
			{
				beneficiamento.Identificador = Guid.NewGuid().ToString();
				BeneficiamentoMadeiraBeneficiamentoVM viewModelAux = new BeneficiamentoMadeiraBeneficiamentoVM(beneficiamento, _listaBus.AtividadesSolicitada, _listaBus.BeneficiamentoMadeiraMateriaPrimaConsumida, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.BeneficiamentoMadeira, (eTipoGeometria)beneficiamento.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo, _listaBus.CaracterizacaoUnidadeMedida);
				vm.BeneficiamentoMadeiraBeneficiamentosVM.Add(viewModelAux);
			}


			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.BeneficiamentoMadeiraCriar })]
		public ActionResult Criar(BeneficiamentoMadeira caracterizacao)
		{
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.BeneficiamentoMadeira,
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

		[Permite(RoleArray = new Object[] { ePermissao.BeneficiamentoMadeiraEditar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.beneficiamentomadeira)]
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

			BeneficiamentoMadeira caracterizacao = _bus.ObterPorEmpreendimento(id);
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.BeneficiamentoMadeira,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao = _bus.MergiarGeo(caracterizacao);
			}

			BeneficiamentoMadeiraVM vm = new BeneficiamentoMadeiraVM(caracterizacao, isVisualizar: false, isEditar: true);

			foreach (BeneficiamentoMadeiraBeneficiamento beneficiamento in caracterizacao.Beneficiamentos)
			{
				beneficiamento.Identificador = Guid.NewGuid().ToString();
				BeneficiamentoMadeiraBeneficiamentoVM viewModelAux = new BeneficiamentoMadeiraBeneficiamentoVM(beneficiamento, _listaBus.AtividadesSolicitada, _listaBus.BeneficiamentoMadeiraMateriaPrimaConsumida, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.BeneficiamentoMadeira, (eTipoGeometria)beneficiamento.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo, _listaBus.CaracterizacaoUnidadeMedida, isVisualizar: false, isEditar: true);
				vm.BeneficiamentoMadeiraBeneficiamentosVM.Add(viewModelAux);
			}

			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.BeneficiamentoMadeiraEditar })]
		public ActionResult Editar(BeneficiamentoMadeira caracterizacao)
		{
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.BeneficiamentoMadeira,
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

		[Permite(RoleArray = new Object[] { ePermissao.BeneficiamentoMadeiraVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.beneficiamentomadeira)]
		public ActionResult Visualizar(int id)
		{
			if (!_validar.Acessar(id))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			BeneficiamentoMadeira caracterizacao = _bus.ObterPorEmpreendimento(id);
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.BeneficiamentoMadeira,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias, true);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao = _bus.MergiarGeo(caracterizacao);
			}

			BeneficiamentoMadeiraVM vm = new BeneficiamentoMadeiraVM(caracterizacao, true);

			foreach (BeneficiamentoMadeiraBeneficiamento beneficiamento in caracterizacao.Beneficiamentos)
			{
				beneficiamento.Identificador = Guid.NewGuid().ToString();
				BeneficiamentoMadeiraBeneficiamentoVM viewModelAux = new BeneficiamentoMadeiraBeneficiamentoVM(beneficiamento, _listaBus.AtividadesSolicitada, _listaBus.BeneficiamentoMadeiraMateriaPrimaConsumida, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.BeneficiamentoMadeira, (eTipoGeometria)beneficiamento.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo, _listaBus.CaracterizacaoUnidadeMedida, true);
				vm.BeneficiamentoMadeiraBeneficiamentosVM.Add(viewModelAux);
			}

			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			return View(vm);
		}

		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.BeneficiamentoMadeiraExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			vm.Id = id;
			vm.Mensagem = Mensagem.BeneficiamentoMadeira.ExcluirMensagem;
			vm.Titulo = "Excluir Beneficiamento e tratamento de madeira";

			return PartialView("Excluir", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.BeneficiamentoMadeiraExcluir })]
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

		[Permite(RoleArray = new Object[] { ePermissao.BeneficiamentoMadeiraCriar, ePermissao.BeneficiamentoMadeiraEditar, ePermissao.BeneficiamentoMadeiraVisualizar })]
		public ActionResult ObterDadosTipoGeometria(int empreendimentoId, int caracterizacaoTipo)
		{
			return Json(new
			{
				@TiposGeometricos = _caracterizacaoBus.ObterTipoGeometria(empreendimentoId, caracterizacaoTipo)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.BeneficiamentoMadeiraCriar, ePermissao.BeneficiamentoMadeiraEditar, ePermissao.BeneficiamentoMadeiraVisualizar })]
		public ActionResult ObterDadosCoordenadaAtividade(int empreendimentoId, int tipoGeometria)
		{
			return Json(new
			{
				@CoordenadaAtividade = _caracterizacaoBus.ObterCoordenadaAtividadeLst(empreendimentoId, eCaracterizacao.BeneficiamentoMadeira, (eTipoGeometria)tipoGeometria)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.BeneficiamentoMadeiraCriar, ePermissao.BeneficiamentoMadeiraEditar })]
		public ActionResult ObterTemplateBeneficiamento()
		{
			BeneficiamentoMadeiraBeneficiamentoVM vm = new BeneficiamentoMadeiraBeneficiamentoVM(_listaBus.AtividadesSolicitada, _listaBus.BeneficiamentoMadeiraMateriaPrimaConsumida, _listaBus.CaracterizacaoGeometriaTipo, _listaBus.CaracterizacaoUnidadeMedida);

			vm.Caracterizacao = new BeneficiamentoMadeiraBeneficiamento() { Identificador = Guid.NewGuid().ToString()};
			String html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Caracterizacoes/Views/BeneficiamentoMadeira/BeneficiamentoMadeiraBeneficiamento.ascx", vm);

			return Json(new
			{
				@html = html
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region GeoMergiar

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.BeneficiamentoMadeiraCriar, ePermissao.BeneficiamentoMadeiraEditar })]
		public ActionResult GeoMergiar(BeneficiamentoMadeira caracterizacao)
		{
			BeneficiamentoMadeira caracterizacaoMerge = _bus.MergiarGeo(caracterizacao);
			BeneficiamentoMadeiraVM vm = new BeneficiamentoMadeiraVM(caracterizacaoMerge);

			foreach (BeneficiamentoMadeiraBeneficiamento beneficiamento in caracterizacaoMerge.Beneficiamentos)
			{
				beneficiamento.Identificador = Guid.NewGuid().ToString();
				BeneficiamentoMadeiraBeneficiamentoVM viewModelAux = new BeneficiamentoMadeiraBeneficiamentoVM(beneficiamento, _listaBus.AtividadesSolicitada, _listaBus.BeneficiamentoMadeiraMateriaPrimaConsumida, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.BeneficiamentoMadeira, (eTipoGeometria)beneficiamento.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo, _listaBus.CaracterizacaoUnidadeMedida);
				vm.BeneficiamentoMadeiraBeneficiamentosVM.Add(viewModelAux);
			}

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "BeneficiamentoMadeira", vm),
				@Dependencias = ViewModelHelper.Json(vm.Caracterizacao.Dependencias)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}
