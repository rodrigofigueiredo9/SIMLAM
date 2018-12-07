using System;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class InformacaoCorteController : DefaultController
	{
		#region Propriedades

		ListaBus _listaBus = new ListaBus();
		InformacaoCorteBus _bus = new InformacaoCorteBus();
		InformacaoCorteValidar _validar = new InformacaoCorteValidar();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteCriar })]
		public ActionResult Criar(int id)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}


			InformacaoCorte caracterizacao = _bus.ObterPorEmpreendimento(id) ?? new InformacaoCorte();
			caracterizacao.EmpreendimentoId = id;
			caracterizacao.Emprendimento = _caracterizacaoBus.ObterEmpreendimentoSimplificado(id);

			if (caracterizacao.Id > 0) 
			{
				return RedirectToAction("Editar", new { id = caracterizacao.EmpreendimentoId });
			}

			if (!_validar.Acessar(caracterizacao.EmpreendimentoId))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			caracterizacao.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(id, eCaracterizacao.InformacaoCorte, eCaracterizacaoDependenciaTipo.Caracterizacao);
			InformacaoCorteVM vm = new InformacaoCorteVM(caracterizacao);

			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			return View(vm);

		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteEditar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.informacaocorte)]
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

			InformacaoCorte caracterizacao = _bus.ObterPorEmpreendimento(id);
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.InformacaoCorte,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao = _bus.MergiarGeo(caracterizacao);
			}

			InformacaoCorteVM vm = new InformacaoCorteVM(caracterizacao);

			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			return View(vm);
		}

		#endregion

		#region Salvar

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteCriar, ePermissao.InformacaoCorteEditar })]
		public ActionResult Salvar(InformacaoCorte caracterizacao)
		{
			caracterizacao.Id = _bus.Salvar(caracterizacao);
			caracterizacao = _bus.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId);
			caracterizacao.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(caracterizacao.EmpreendimentoId, eCaracterizacao.InformacaoCorte, eCaracterizacaoDependenciaTipo.Caracterizacao);

			InformacaoCorteVM vm = new InformacaoCorteVM(caracterizacao);
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			String html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "InformacaoCorte", vm);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@CaracterizacaoId = caracterizacao.Id,
				@Html = html,
				@Dependencias = ViewModelHelper.Json(caracterizacao.Dependencias),
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.informacaocorte)]
		public ActionResult Visualizar(int id)
		{
			if (!_validar.Acessar(id))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			InformacaoCorte caracterizacao = _bus.ObterPorEmpreendimento(id);
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.InformacaoCorte,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao = _bus.MergiarGeo(caracterizacao);
			}

			InformacaoCorteVM vm = new InformacaoCorteVM(caracterizacao, true);

			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			return View(vm);
		}

		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			vm.Id = id;
			vm.Mensagem = Mensagem.InformacaoCorte.ExcluirMensagem;
			vm.Titulo = "Excluir Informação de Corte";

			return PartialView("Excluir", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteExcluir })]
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

		#region Informacoes Corte Informacao

		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteCriar })]
		public ActionResult InformacaoCorteInformacaoCriar()
		{
			InformacaoCorteVM vm = new InformacaoCorteVM(new InformacaoCorte(), false);
			//InformacaoCorteInformacaoVM vm = new InformacaoCorteInformacaoVM(new InformacaoCorteInformacao(), _listaBus.SilviculturaCulturasFlorestais,_listaBus.CaracterizacaoProdutosExploracao, _listaBus.DestinacaoMaterial);
			//String html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "InformacaoCorteInformacao", vm);
			vm.Caracterizacao.Emprendimento = _caracterizacaoBus.ObterEmpreendimentoSimplificado(26190);

			return View("InformacaoCorteInformacao", vm);

			//if (Request.IsAjaxRequest())
			//{
			//	return PartialView("VisualizarPartial", vm);
			//}
			//else
			//{
			//	return View("Visualizar", vm);
			//}

			//return RedirectToAction("Index", Validacao.QueryParamSerializer());


			//return Json(new
			//{
			//	@Html = html,
			//	@Msg = Validacao.Erros
			//}, JsonRequestBehavior.AllowGet);

		}

		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteEditar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.informacaocorteinformacao)]
		public ActionResult InformacaoCorteInformacaoEditar(int id)
		{
			InformacaoCorteInformacaoVM vm = new InformacaoCorteInformacaoVM(_bus.ObterInformacaoItem(id), _listaBus.SilviculturaCulturasFlorestais, _listaBus.CaracterizacaoProdutosExploracao, _listaBus.DestinacaoMaterial);
			String html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "InformacaoCorteInformacao", vm);

			return Json(new
			{
				@Html = html,
				@Msg = Validacao.Erros
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.informacaocorteinformacao)]
		public ActionResult InformacaoCorteInformacaoVisualizar(int id)
		{
			InformacaoCorteInformacaoVM vm = new InformacaoCorteInformacaoVM(_bus.ObterInformacaoItem(id), _listaBus.SilviculturaCulturasFlorestais, _listaBus.CaracterizacaoProdutosExploracao, _listaBus.DestinacaoMaterial, true);
			String html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "InformacaoCorteInformacao", vm);

			return Json(new
			{
				@Html = html,
				@Msg = Validacao.Erros
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteExcluir })]
		public ActionResult InformacaoCorteInformacaoExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			vm.Id = id;
			vm.Mensagem = Mensagem.InformacaoCorte.ItemExcluirMensagem;
			vm.Titulo = "Excluir Informação de Corte";

			return PartialView("Excluir", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteExcluir })]
		public ActionResult InformacaoCorteInformacaoExcluir(int itemId, int empreendimentoId)
		{
			InformacaoCorte caracterizacao = _bus.ObterPorEmpreendimento(empreendimentoId);

			if (caracterizacao.InformacoesCortes.Count > 1)
			{
				_bus.ExcluirInformacao(itemId);
				caracterizacao = _bus.ObterPorEmpreendimento(empreendimentoId);
			}
			else 
			{
				Validacao.Add(Mensagem.InformacaoCorte.InformacaoCorteUltimoItemListaObrigatorio);
			}

			caracterizacao.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(empreendimentoId, eCaracterizacao.InformacaoCorte, eCaracterizacaoDependenciaTipo.Caracterizacao);

			InformacaoCorteVM vm = new InformacaoCorteVM(caracterizacao);
			String html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "InformacaoCorte", vm);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@CaracterizacaoId = caracterizacao.Id,
				@Html = html,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region GeoMergiar

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteCriar, ePermissao.InformacaoCorteEditar })]
		public ActionResult GeoMergiar(InformacaoCorte caracterizacao)
		{
			InformacaoCorteVM vm = new InformacaoCorteVM(_bus.MergiarGeo(caracterizacao));

			vm.Caracterizacao.InformacaoCorteInformacao = vm.Caracterizacao.InformacoesCortes.First();
			_bus.Salvar(vm.Caracterizacao);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "InformacaoCorte", vm),
				@Dependencias = ViewModelHelper.Json(vm.Caracterizacao.Dependencias)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}