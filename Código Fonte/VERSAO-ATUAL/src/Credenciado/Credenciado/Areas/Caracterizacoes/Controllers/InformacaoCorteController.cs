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

		#region Criar

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult Criar(int id, int projetoDigitalId, bool visualizar = false)
		{
			if (!_caracterizacaoValidar.Basicas(id))
				return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = projetoDigitalId, area = "" }));

			if (!_validar.Acessar(id, projetoDigitalId))
				return RedirectToAction("", "Caracterizacao", new { id = id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });

			var empreendimento = _bus.ObterEmpreendimentoSimplificado(id);
			var informacaoCorteVM = new InformacaoCorteVM(empreendimento, ListaCredenciadoBus.DestinacaoMaterial, ListaCredenciadoBus.Produto,
				ListaCredenciadoBus.ListaEnumerado<eTipoCorte>(), ListaCredenciadoBus.ListaEnumerado<eEspecieInformada>())
			{
				ProjetoDigitalId = projetoDigitalId
			};

			return View(informacaoCorteVM);
		}

		[HttpPost]
		public ActionResult Criar(InformacaoCorte caracterizacao, int projetoDigitalId = 0)
		{
			_informacaoCorteBus.Salvar(caracterizacao, projetoDigitalId);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoId, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoEditar })]
		public ActionResult Editar(int id, int projetoDigitalId)
		{
			if (!_caracterizacaoValidar.Basicas(id))
				return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = projetoDigitalId, area = "" }));

			if (!_validar.Acessar(id, projetoDigitalId))
				return RedirectToAction("", "Caracterizacao", new { id = id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });

			var caracterizacao = _informacaoCorteBus.Obter(id);
			var empreendimento = _bus.ObterEmpreendimentoSimplificado(id);
			var vm = new InformacaoCorteVM(empreendimento, ListaCredenciadoBus.DestinacaoMaterial, ListaCredenciadoBus.Produto,
				ListaCredenciadoBus.ListaEnumerado<eTipoCorte>(), ListaCredenciadoBus.ListaEnumerado<eEspecieInformada>(), caracterizacao)
			{
				ProjetoDigitalId = projetoDigitalId,
				IsPodeExcluir = true
			};

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoEditar })]
		public ActionResult Editar(InformacaoCorte caracterizacao, int projetoDigitalId)
		{
			_informacaoCorteBus.Salvar(caracterizacao, projetoDigitalId);
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.Empreendimento.Id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoEditar })]
		public ActionResult Visualizar(int id, int projetoDigitalId)
		{
			//if (!_caracterizacaoValidar.Basicas(id))
			//{
			//	return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = projetoDigitalId, area = "" }));
			//}

			//if (!_validar.Acessar(id, projetoDigitalId))
			//{
			//	return RedirectToAction("", "Caracterizacao", new { id = id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });
			//}

			var caracterizacao = _informacaoCorteBus.Obter(id);
			var empreendimento = _bus.ObterEmpreendimentoSimplificado(id);
			var vm = new InformacaoCorteVM(empreendimento, ListaCredenciadoBus.DestinacaoMaterial, ListaCredenciadoBus.Produto,
				ListaCredenciadoBus.ListaEnumerado<eTipoCorte>(), ListaCredenciadoBus.ListaEnumerado<eEspecieInformada>(), caracterizacao)
			{
				ProjetoDigitalId = projetoDigitalId,
				IsVisualizar = true
			};

			return View(vm);
		}

		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoExcluir })]
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
		[Permite(RoleArray = new Object[] { ePermissao.UnidadeProducaoExcluir })]
		public ActionResult Excluir(int id, int projetoDigitalId)
		{
			string urlRedireciona = string.Empty;

			if (_informacaoCorteBus.Excluir(id))
				urlRedireciona = Url.Action("Listar", "InformacaoCorte", new { id = id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, urlRedireciona = urlRedireciona }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Filtrar

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult Listar(int id, int projetoDigitalId, bool visualizar = false)
		{
			var resultados = _informacaoCorteBus.FiltrarPorEmpreendimento(id);
			var empreendimento = _bus.ObterEmpreendimentoSimplificado(id);
			var first = resultados?.FirstOrDefault();
			var vm = new ListarVM(empreendimento)
			{
				Resultados = resultados,
				IsVisualizar = visualizar,
				IsPodeExcluir = true,
				ProjetoDigitalId = projetoDigitalId,
				AreaPlantada = first?.AreaFlorestaPlantada ?? 0
			};

			return View(vm);
		}

		#endregion
	}
}