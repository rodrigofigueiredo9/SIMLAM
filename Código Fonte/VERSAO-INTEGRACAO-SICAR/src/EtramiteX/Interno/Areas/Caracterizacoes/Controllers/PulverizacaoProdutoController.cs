using System;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloPulverizacaoProduto;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloPulverizacaoProduto.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class PulverizacaoProdutoController : DefaultController
	{
		#region Propriedades

		ListaBus _listaBus = new ListaBus();
		PulverizacaoProdutoBus _bus = new PulverizacaoProdutoBus();
		PulverizacaoProdutoValidar _validar = new PulverizacaoProdutoValidar();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		EmpreendimentoBus _empBus = new EmpreendimentoBus();

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.PulverizacaoProdutoCriar })]
		public ActionResult Criar(int id)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			PulverizacaoProduto caracterizacao = new PulverizacaoProduto();
			caracterizacao.EmpreendimentoId = id;

			if (!_validar.Acessar(caracterizacao.EmpreendimentoId))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			caracterizacao.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(id, eCaracterizacao.PulverizacaoProduto, eCaracterizacaoDependenciaTipo.Caracterizacao);
			PulverizacaoProdutoVM vm = new PulverizacaoProdutoVM(caracterizacao, _listaBus.AtividadesSolicitada, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.PulverizacaoProduto, (eTipoGeometria)caracterizacao.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo, _listaBus.PulverizacaoProdutoCulturas);

			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			var emp = _empBus.Obter(id);
			var enderecoEmp = emp.Enderecos.Find(x => x.ZonaLocalizacaoId == (int)eEmpreendimentoLocalizacao.ZonaRural);

			if (enderecoEmp != null)
			{
				vm.TemARL = _bus.TemARL(id);
				vm.TemARLDesconhecida = _bus.TemARLDesconhecida(id);
			}
			else
			{
				vm.TemARL = true;
				vm.TemARLDesconhecida = false;
			}

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PulverizacaoProdutoCriar })]
		public ActionResult Criar(PulverizacaoProduto caracterizacao)
		{
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.PulverizacaoProduto,
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

		[Permite(RoleArray = new Object[] { ePermissao.PulverizacaoProdutoEditar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.pulverizacaoproduto)]
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

			PulverizacaoProduto caracterizacao = _bus.ObterPorEmpreendimento(id);
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.PulverizacaoProduto,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao = _bus.MergiarGeo(caracterizacao);
			}

			PulverizacaoProdutoVM vm = new PulverizacaoProdutoVM(caracterizacao, _listaBus.AtividadesSolicitada, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.PulverizacaoProduto, (eTipoGeometria)caracterizacao.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo, _listaBus.PulverizacaoProdutoCulturas, isVisualizar: false, isEditar: true);

			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			var emp = _empBus.Obter(id);
			var enderecoEmp = emp.Enderecos.Find(x => x.ZonaLocalizacaoId == (int)eEmpreendimentoLocalizacao.ZonaRural);

			if (enderecoEmp != null)
			{
				vm.TemARL = _bus.TemARL(id);
				vm.TemARLDesconhecida = _bus.TemARLDesconhecida(id);
			}
			else
			{
				vm.TemARL = true;
				vm.TemARLDesconhecida = false;
			}

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PulverizacaoProdutoEditar })]
		public ActionResult Editar(PulverizacaoProduto caracterizacao)
		{
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.PulverizacaoProduto,
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

		[Permite(RoleArray = new Object[] { ePermissao.PulverizacaoProdutoVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.pulverizacaoproduto)]
		public ActionResult Visualizar(int id)
		{

			if (!_validar.Acessar(id))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			PulverizacaoProduto caracterizacao = _bus.ObterPorEmpreendimento(id);

			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.PulverizacaoProduto,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias, true);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao = _bus.MergiarGeo(caracterizacao);
			}

			PulverizacaoProdutoVM vm = new PulverizacaoProdutoVM(caracterizacao, _listaBus.AtividadesSolicitada, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.PulverizacaoProduto, (eTipoGeometria)caracterizacao.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo, _listaBus.PulverizacaoProdutoCulturas, true);

			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			vm.TemARL = true;
			vm.TemARLDesconhecida = false;

			return View(vm);

		}

		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.PulverizacaoProdutoExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			vm.Id = id;
			vm.Mensagem = Mensagem.PulverizacaoProduto.ExcluirMensagem;
			vm.Titulo = "Excluir Pulverização Aérea de Produtos Agrotóxicos";

			return PartialView("Excluir", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PulverizacaoProdutoExcluir })]
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

		[Permite(RoleArray = new Object[] { ePermissao.PulverizacaoProdutoCriar, ePermissao.PulverizacaoProdutoEditar, ePermissao.PulverizacaoProdutoVisualizar })]
		public ActionResult ObterDadosTipoGeometria(int empreendimentoId, int caracterizacaoTipo)
		{
			return Json(new
			{
				@TiposGeometricos = _caracterizacaoBus.ObterTipoGeometria(empreendimentoId, caracterizacaoTipo)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PulverizacaoProdutoCriar, ePermissao.PulverizacaoProdutoEditar, ePermissao.PulverizacaoProdutoVisualizar })]
		public ActionResult ObterDadosCoordenadaAtividade(int empreendimentoId, int tipoGeometria)
		{
			return Json(new
			{
				@CoordenadaAtividade = _caracterizacaoBus.ObterCoordenadaAtividadeLst(empreendimentoId, eCaracterizacao.PulverizacaoProduto, (eTipoGeometria)tipoGeometria)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region GeoMergiar

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PulverizacaoProdutoCriar, ePermissao.PulverizacaoProdutoEditar })]
		public ActionResult GeoMergiar(PulverizacaoProduto caracterizacao)
		{
			PulverizacaoProdutoVM vm = new PulverizacaoProdutoVM(_bus.MergiarGeo(caracterizacao), _listaBus.AtividadesSolicitada, _caracterizacaoBus.ObterCoordenadaAtividadeLst(caracterizacao.EmpreendimentoId, eCaracterizacao.PulverizacaoProduto, (eTipoGeometria)caracterizacao.CoordenadaAtividade.Tipo), _listaBus.CaracterizacaoGeometriaTipo, _listaBus.PulverizacaoProdutoCulturas);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "PulverizacaoProduto", vm),
				@Dependencias = ViewModelHelper.Json(vm.Caracterizacao.Dependencias)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

	}
}

