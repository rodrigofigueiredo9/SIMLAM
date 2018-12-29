using System;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMInformacaoCorte;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class InformacaoCorteController : DefaultController
	{
		ListaBus _listaBus = new ListaBus();
		CaracterizacaoBus _bus = new CaracterizacaoBus(new CaracterizacaoValidar());
		InformacaoCorteBus _informacaoCorteBus = new InformacaoCorteBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		InformacaoCorteValidar _validar = new InformacaoCorteValidar();

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteCriar })]
		public ActionResult Criar(int id)
		{
			if (!_caracterizacaoValidar.Basicas(id))
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());

			if (!_validar.Acessar(id))
				return RedirectToAction("Listar", "InformacaoCorte", new { id = id, Msg = Validacao.QueryParam() });

			var empreendimento = _bus.ObterEmpreendimentoSimplificado(id);
			var informacaoCorteVM = new InformacaoCorteVM(empreendimento, _listaBus.DestinacaoMaterial, _listaBus.CaracterizacaoProdutosExploracao,
				_listaBus.ListaEnumerado<eTipoCorte>(), _listaBus.ListaEnumerado<eEspecieInformada>());

			return View(informacaoCorteVM);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteEditar })]
		public ActionResult Editar(int id)
		{
			InformacaoCorte caracterizacao = _informacaoCorteBus.Obter(id);

			if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoId))
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());

			if (!_validar.Acessar(caracterizacao.EmpreendimentoId))
				return RedirectToAction("Listar", "InformacaoCorte", new { id = id, Msg = Validacao.QueryParam() });

			var empreendimento = _bus.ObterEmpreendimentoSimplificado(caracterizacao.EmpreendimentoId);
			var vm = new InformacaoCorteVM(empreendimento, _listaBus.DestinacaoMaterial, _listaBus.CaracterizacaoProdutosExploracao,
				_listaBus.ListaEnumerado<eTipoCorte>(), _listaBus.ListaEnumerado<eEspecieInformada>(), caracterizacao)
			{
				IsPodeExcluir = true
			};

			return View(vm);
		}

		#endregion

		#region Salvar

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteCriar, ePermissao.InformacaoCorteEditar })]
		public ActionResult Salvar(InformacaoCorte caracterizacao)
		{
			_informacaoCorteBus.Salvar(caracterizacao);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("Listar", "InformacaoCorte", new { id = caracterizacao.Empreendimento.Id, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteVisualizar })]
		public ActionResult Visualizar(int id)
		{
			InformacaoCorte caracterizacao = _informacaoCorteBus.Obter(id);

			if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoId))
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());

			if (!_validar.Acessar(caracterizacao.EmpreendimentoId))
				return RedirectToAction("Listar", "InformacaoCorte", new { id = id, Msg = Validacao.QueryParam() });

			var empreendimento = _bus.ObterEmpreendimentoSimplificado(caracterizacao.EmpreendimentoId);
			var vm = new InformacaoCorteVM(empreendimento, _listaBus.DestinacaoMaterial, _listaBus.CaracterizacaoProdutosExploracao,
				_listaBus.ListaEnumerado<eTipoCorte>(), _listaBus.ListaEnumerado<eEspecieInformada>(), caracterizacao)
			{
				IsVisualizar = true
			};

			return View(vm);
		}

		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ConfirmarVM vm = new ConfirmarVM();
			vm.Id = id;
			vm.Mensagem = Mensagem.InformacaoCorte.ExcluirMensagem;
			vm.Titulo = "Excluir Informação de Corte";

			return PartialView("Confirmar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteExcluir })]
		public ActionResult Excluir(int id)
		{
			string urlRedireciona = string.Empty;

			if (_informacaoCorteBus.Excluir(id))
				urlRedireciona = Url.Action("Listar", "InformacaoCorte", new { id = id, Msg = Validacao.QueryParam() });

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, urlRedireciona = urlRedireciona }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Filtrar

		[Permite(RoleArray = new Object[] { ePermissao.InformacaoCorteVisualizar })]
		public ActionResult Listar(int id)
		{
			var resultados = _informacaoCorteBus.FiltrarPorEmpreendimento(id);
			var empreendimento = _bus.ObterEmpreendimentoSimplificado(id);
			var last = resultados?.LastOrDefault();
			var vm = new ListarVM(empreendimento)
			{
				Resultados = resultados,
				PodeCriar = User.IsInRole(ePermissao.InformacaoCorteCriar.ToString()),
				PodeEditar = User.IsInRole(ePermissao.InformacaoCorteEditar.ToString()),
				PodeExcluir = User.IsInRole(ePermissao.InformacaoCorteExcluir.ToString()),
				PodeVisualizar = User.IsInRole(ePermissao.InformacaoCorteVisualizar.ToString()),
				AreaPlantada = last?.AreaFlorestaPlantada ?? 0
			};

			return View(vm);
		}

		#endregion
	}
}