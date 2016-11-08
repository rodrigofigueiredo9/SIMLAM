using System;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProducaoCarvaoVegetal;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProducaoCarvaoVegetal.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class ProducaoCarvaoVegetalController : DefaultController
	{
		#region Propriedades

		ListaBus _listaBus = new ListaBus();
		ProducaoCarvaoVegetalBus _bus = new ProducaoCarvaoVegetalBus();
		ProducaoCarvaoVegetalValidar _validar = new ProducaoCarvaoVegetalValidar();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.ProducaoCarvaoVegetalCriar })]
		public ActionResult Criar(int id)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			ProducaoCarvaoVegetal caracterizacao = new ProducaoCarvaoVegetal();
			caracterizacao.EmpreendimentoId = id;

			if (!_validar.Acessar(caracterizacao.EmpreendimentoId))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			caracterizacao.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(id, eCaracterizacao.ProducaoCarvaoVegetal, eCaracterizacaoDependenciaTipo.Caracterizacao);
			ProducaoCarvaoVegetalVM vm = new ProducaoCarvaoVegetalVM(caracterizacao, _listaBus.AtividadesSolicitada, _listaBus.ProducaoCarvaoVegetalMateriaPrimaConsumida, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.ProducaoCarvaoVegetal, (eTipoGeometria) caracterizacao.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo, _listaBus.CaracterizacaoUnidadeMedida);

			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProducaoCarvaoVegetalCriar })]
		public ActionResult Criar(ProducaoCarvaoVegetal caracterizacao)
		{
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.ProducaoCarvaoVegetal,
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

		[Permite(RoleArray = new Object[] { ePermissao.ProducaoCarvaoVegetalEditar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.producaocarvaovegetal)]
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

			ProducaoCarvaoVegetal caracterizacao = _bus.ObterPorEmpreendimento(id);
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.ProducaoCarvaoVegetal,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao = _bus.MergiarGeo(caracterizacao);
			}

			ProducaoCarvaoVegetalVM vm = new ProducaoCarvaoVegetalVM(caracterizacao, _listaBus.AtividadesSolicitada, _listaBus.ProducaoCarvaoVegetalMateriaPrimaConsumida, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.ProducaoCarvaoVegetal, (eTipoGeometria)caracterizacao.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo, _listaBus.CaracterizacaoUnidadeMedida, false, true);

			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProducaoCarvaoVegetalEditar })]
		public ActionResult Editar(ProducaoCarvaoVegetal caracterizacao)
		{
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.ProducaoCarvaoVegetal,
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

		[Permite(RoleArray = new Object[] { ePermissao.ProducaoCarvaoVegetalVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.producaocarvaovegetal)]
		public ActionResult Visualizar(int id)
		{
			if (!_validar.Acessar(id))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			ProducaoCarvaoVegetal caracterizacao = _bus.ObterPorEmpreendimento(id);
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.ProducaoCarvaoVegetal,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias, true);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao = _bus.MergiarGeo(caracterizacao);
			}

			ProducaoCarvaoVegetalVM vm = new ProducaoCarvaoVegetalVM(caracterizacao, _listaBus.AtividadesSolicitada, _listaBus.ProducaoCarvaoVegetalMateriaPrimaConsumida, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.ProducaoCarvaoVegetal, (eTipoGeometria) caracterizacao.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo, _listaBus.CaracterizacaoUnidadeMedida, true);

			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			return View(vm);
		}


		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.ProducaoCarvaoVegetalExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			vm.Id = id;
			vm.Mensagem = Mensagem.ProducaoCarvaoVegetal.ExcluirMensagem;
			vm.Titulo = "Excluir Produção de Carvão Vegetal";

			return PartialView("Excluir", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProducaoCarvaoVegetalExcluir })]
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

		[Permite(RoleArray = new Object[] { ePermissao.ProducaoCarvaoVegetalCriar, ePermissao.ProducaoCarvaoVegetalEditar, ePermissao.ProducaoCarvaoVegetalVisualizar })]
		public ActionResult ObterDadosTipoGeometria(int empreendimentoId, int caracterizacaoTipo)
		{
			return Json(new
			{
				@TiposGeometricos = _caracterizacaoBus.ObterTipoGeometria(empreendimentoId, caracterizacaoTipo)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProducaoCarvaoVegetalCriar, ePermissao.ProducaoCarvaoVegetalEditar, ePermissao.ProducaoCarvaoVegetalVisualizar })]
		public ActionResult ObterDadosCoordenadaAtividade(int empreendimentoId, int tipoGeometria)
		{
			return Json(new
			{
				@CoordenadaAtividade = _caracterizacaoBus.ObterCoordenadaAtividadeLst(empreendimentoId, eCaracterizacao.ProducaoCarvaoVegetal, (eTipoGeometria) tipoGeometria)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region GeoMergiar

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProducaoCarvaoVegetalCriar, ePermissao.ProducaoCarvaoVegetalEditar })]
		public ActionResult GeoMergiar(ProducaoCarvaoVegetal caracterizacao)
		{
			ProducaoCarvaoVegetalVM vm = new ProducaoCarvaoVegetalVM(_bus.MergiarGeo(caracterizacao), _listaBus.AtividadesSolicitada, _listaBus.ProducaoCarvaoVegetalMateriaPrimaConsumida, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.ProducaoCarvaoVegetal, (eTipoGeometria) caracterizacao.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo, _listaBus.CaracterizacaoUnidadeMedida);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ProducaoCarvaoVegetal", vm),
				@Dependencias = ViewModelHelper.Json(vm.Caracterizacao.Dependencias)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

	}
}
