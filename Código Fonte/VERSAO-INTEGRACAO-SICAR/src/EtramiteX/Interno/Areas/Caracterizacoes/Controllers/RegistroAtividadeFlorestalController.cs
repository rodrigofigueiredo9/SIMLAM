using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegistroAtividadeFlorestal;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloRegistroAtividadeFlorestal.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class RegistroAtividadeFlorestalController : DefaultController
	{
		#region Propriedades

		ListaBus _listaBus = new ListaBus();
		TituloModeloBus _modeloBus = new TituloModeloBus();
		RegistroAtividadeFlorestalBus _bus = new RegistroAtividadeFlorestalBus();
		RegistroAtividadeFlorestalValidar _validar = new RegistroAtividadeFlorestalValidar();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.RegistroAtividadeFlorestalCriar })]
		public ActionResult Criar(int id)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			RegistroAtividadeFlorestal caracterizacao = new RegistroAtividadeFlorestal();
			caracterizacao.EmpreendimentoId = id;

			RegistroAtividadeFlorestalVM vm = new RegistroAtividadeFlorestalVM(
				caracterizacao,
				_listaBus.AtividadesCategoria,
				_listaBus.RegistroAtividadeFlorestalFonte,
				_listaBus.RegistroAtividadeFlorestalUnidade,
				_bus.ObterModelosLista());

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RegistroAtividadeFlorestalCriar })]
		public ActionResult Criar(RegistroAtividadeFlorestal caracterizacao)
		{
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

		[Permite(RoleArray = new Object[] { ePermissao.RegistroAtividadeFlorestalEditar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.registroatividadeflorestal)]
		public ActionResult Editar(int id)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			RegistroAtividadeFlorestal caracterizacao = _bus.ObterPorEmpreendimento(id);

			RegistroAtividadeFlorestalVM vm = new RegistroAtividadeFlorestalVM(
				caracterizacao,
				_listaBus.AtividadesCategoria,
				_listaBus.RegistroAtividadeFlorestalFonte,
				_listaBus.RegistroAtividadeFlorestalUnidade,
				_bus.ObterModelosLista());

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RegistroAtividadeFlorestalEditar })]
		public ActionResult Editar(RegistroAtividadeFlorestal caracterizacao)
		{
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

		[Permite(RoleArray = new Object[] { ePermissao.RegistroAtividadeFlorestalVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.registroatividadeflorestal)]
		public ActionResult Visualizar(int id)
		{
			RegistroAtividadeFlorestal caracterizacao = _bus.ObterPorEmpreendimento(id, false, null, true);
			RegistroAtividadeFlorestalVM vm = new RegistroAtividadeFlorestalVM(
				caracterizacao,
				_listaBus.AtividadesCategoria,
				_listaBus.RegistroAtividadeFlorestalFonte,
				_listaBus.RegistroAtividadeFlorestalUnidade,
				_modeloBus.ObterModelos(),
				isVisualizar: true);

			return View(vm);
		}

		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.RegistroAtividadeFlorestalExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			vm.Id = id;
			vm.Mensagem = Mensagem.RegistroAtividadeFlorestal.ExcluirMensagem;
			vm.Titulo = "Excluir Registro Atividade Florestal ";

			return PartialView("Excluir", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RegistroAtividadeFlorestalExcluir })]
		public ActionResult Excluir(int id)
		{
			_bus.Excluir(id);
			string urlRedireciona = Url.Action("Index", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, urlRedireciona = urlRedireciona }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.RegistroAtividadeFlorestalCriar, ePermissao.RegistroAtividadeFlorestalEditar })]
		public ActionResult ValidarConsumo(List<Consumo> consumoLista)
		{
			_validar.Consumo(consumoLista);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RegistroAtividadeFlorestalCriar, ePermissao.RegistroAtividadeFlorestalEditar })]
		public ActionResult ValidarFonte(List<FonteEnergia> fonteLista, FonteEnergia fonte, int indice)
		{
			_validar.Fonte(fonteLista, fonte, indice);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar })]
		public ActionResult ObterTitulo(TituloFiltro filtros, int indice)
		{
			if (!_validar.ObterTitulo(filtros, indice))
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			TituloBus tituloBus = new TituloBus();
			List<Titulo> resultados = tituloBus.Obter(filtros);

			_validar.AposObterTitulo(filtros, resultados);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Titulo = resultados.FirstOrDefault() }, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}