using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Antigo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels.VMInformacaoCorte;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class InformacaoCorteController : DefaultController
	{
		CaracterizacaoBus _bus = new CaracterizacaoBus(new CaracterizacaoValidar());
		InformacaoCorteBus _informacaoCorteBus = new InformacaoCorteBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		InformacaoCorteValidar _validar = new InformacaoCorteValidar();
		InformacaoCorteInternoBus _informacaoCorteInternoBus = new InformacaoCorteInternoBus();

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteCriar })]
		public ActionResult Criar(int id, int projetoDigitalId, bool visualizar = false)
		{
			if (!_caracterizacaoValidar.Basicas(id))
				return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = projetoDigitalId, area = "" }));

			if (!_informacaoCorteBus.ValidarCriar(id))
				return RedirectToAction("Listar", "InformacaoCorte", new { id = id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });

			var empreendimento = _bus.ObterEmpreendimentoSimplificado(id);
			var informacaoCorteVM = new InformacaoCorteVM(empreendimento, ListaCredenciadoBus.DestinacaoMaterial, ListaCredenciadoBus.Produto,
				ListaCredenciadoBus.ListaEnumerado<eTipoCorte>(), ListaCredenciadoBus.ListaEnumerado<eEspecieInformada>())
			{
				ProjetoDigitalId = projetoDigitalId,
				InformacaoCorteLicencaList = _informacaoCorteBus.ObterLicencas(empreendimento.InternoID)
			};

			return View(informacaoCorteVM);
		}
		
		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteEditar })]
		public ActionResult Editar(int id, int projetoDigitalId)
		{
			var caracterizacao = _informacaoCorteBus.Obter(id);
			var empreendimento = _bus.ObterEmpreendimentoSimplificado(caracterizacao.EmpreendimentoId);

			if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoId))
				return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = projetoDigitalId, area = "" }));

			if (!_informacaoCorteBus.ValidarEditar(id))
				return RedirectToAction("Listar", "InformacaoCorte", new { id = caracterizacao.EmpreendimentoId, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });

			var vm = new InformacaoCorteVM(empreendimento, ListaCredenciadoBus.DestinacaoMaterial, ListaCredenciadoBus.Produto,
				ListaCredenciadoBus.ListaEnumerado<eTipoCorte>(), ListaCredenciadoBus.ListaEnumerado<eEspecieInformada>(), caracterizacao)
			{
				ProjetoDigitalId = projetoDigitalId,
				IsPodeExcluir = true
			};

			return View(vm);
		}

		#endregion

		#region Salvar

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteCriar, ePermissao.InformacaoCorteEditar })]
		public ActionResult Salvar(InformacaoCorte caracterizacao, int projetoDigitalId)
		{
			_informacaoCorteBus.Salvar(caracterizacao, projetoDigitalId);
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("Listar", "InformacaoCorte", new { id = caracterizacao.Empreendimento.Id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteVisualizar })]
		public ActionResult Visualizar(int id, int projetoDigitalId, int? empreendimentoId = null)
		{
			var caracterizacao = empreendimentoId > 0 ? _informacaoCorteInternoBus.Obter(id) : _informacaoCorteBus.Obter(id);
			if (empreendimentoId > 0)
			{
				caracterizacao.EmpreendimentoId = empreendimentoId.GetValueOrDefault(0);
				caracterizacao.Empreendimento.Id = caracterizacao.EmpreendimentoId;
			}
			var empreendimento = _bus.ObterEmpreendimentoSimplificado(caracterizacao.EmpreendimentoId);

			if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoId))
				return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = projetoDigitalId, area = "" }));

			var vm = new InformacaoCorteVM(empreendimento, ListaCredenciadoBus.DestinacaoMaterial, ListaCredenciadoBus.Produto,
				ListaCredenciadoBus.ListaEnumerado<eTipoCorte>(), ListaCredenciadoBus.ListaEnumerado<eEspecieInformada>(), caracterizacao)
			{
				ProjetoDigitalId = projetoDigitalId,
				IsVisualizar = true
			};

			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteVisualizar })]
		public ActionResult VisualizarAntigo(int id, int empreendimento, int projetoDigitalId)
		{
			InformacaoCorteAntigo caracterizacao = _informacaoCorteBus.ObterAntigo(id);
			caracterizacao.EmpreendimentoId = empreendimento;
			InformacaoCorteAntigoVM vm = new InformacaoCorteAntigoVM(caracterizacao, true) { ProjetoDigitalId = projetoDigitalId };
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteVisualizar })]
		public ActionResult InformacaoCorteInformacaoVisualizar(int id)
		{
			InformacaoCorteInformacaoVM vm = new InformacaoCorteInformacaoVM(_informacaoCorteBus.ObterInformacaoItem(id), ListaCredenciadoBus.SilviculturaCulturasFlorestais, ListaCredenciadoBus.CaracterizacaoProdutosExploracao, ListaCredenciadoBus.DestinacaoMaterial, true);
			String html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "InformacaoCorteInformacao", vm);

			return Json(new
			{
				@Html = html,
				@Msg = Validacao.Erros
			}, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteExcluir })]
		public ActionResult ExcluirConfirm(int id, int projetoDigitalId)
		{
			ConfirmarVM vm = new ConfirmarVM();
			vm.Id = id;
			vm.AuxiliarID = projetoDigitalId;
			vm.Mensagem = Mensagem.InformacaoCorte.ExcluirMensagem;
			vm.Titulo = "Excluir Informação de Corte";

			return PartialView("Confirmar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteExcluir })]
		public ActionResult Excluir(int id, int projetoDigitalId)
		{
			string urlRedireciona = string.Empty;

			var caracterizacao = _informacaoCorteBus.Obter(id);
			if (_informacaoCorteBus.Excluir(id))
				urlRedireciona = Url.Action("Listar", "InformacaoCorte", new { id = caracterizacao.EmpreendimentoId, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, urlRedireciona = urlRedireciona }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Filtrar

		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteListar })]
		public ActionResult Listar(int id, int projetoDigitalId, bool visualizar = false)
		{
			var resultados = _informacaoCorteBus.FiltrarPorEmpreendimento(id);
			var empreendimento = _bus.ObterEmpreendimentoSimplificado(id);
			var last = resultados?.LastOrDefault();
			var vm = new ListarVM(empreendimento)
			{
				Resultados = resultados,
				IsVisualizar = visualizar,
				IsPodeExcluir = true,
				ProjetoDigitalId = projetoDigitalId,
				AreaPlantada = last?.AreaFlorestaPlantada ?? 0
			};

			return View(vm);
		}

		#endregion

		public ActionResult ObterProdutos(int destinacaoId) => Json(_informacaoCorteBus.ObterProdutos(destinacaoId), JsonRequestBehavior.AllowGet);
	}
}