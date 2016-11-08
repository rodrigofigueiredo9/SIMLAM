using System;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAgrotoxico;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAgrotoxico.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMAgrotoxico;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class AgrotoxicoController : DefaultController
	{
		#region Propriedades

		AgrotoxicoBus _bus = new AgrotoxicoBus();
		ListaBus _busLista = new ListaBus();
		EmpreendimentoBus _busEmpreendimento = new EmpreendimentoBus();
		ClassificacaoToxicologicaBus _busClassToxicologica = new ClassificacaoToxicologicaBus();
		FormaApresentacaoBus _busFormaApresentacao = new FormaApresentacaoBus();
		GrupoQuimicoBus _busGrupoQuimico = new GrupoQuimicoBus();
		PericulosidadeAmbientalBus _busPericulosidadeAmbiental = new PericulosidadeAmbientalBus();
		ClasseUsoBus _busClasseUso = new ClasseUsoBus();
		ModalidadeAplicacaoBus _busModalidadeAplicacao = new ModalidadeAplicacaoBus();

		#endregion

		#region Index

		[Permite(RoleArray = new Object[] { ePermissao.AgrotoxicoListar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(
				_busLista.QuantPaginacao, 
				_busClasseUso.ObterLista(), 
				_busModalidadeAplicacao.ObterLista(),
				_busGrupoQuimico.ObterLista(), 
				_busClassToxicologica.ObterLista(), 
				_busLista.AtivoLista);

			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.AgrotoxicoListar })]
		public ActionResult Filtrar(ListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(_busLista.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<AgrotoxicoFiltro> resultados = _bus.Filtrar(vm.Filtros, paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.AgrotoxicoCriar })]
		public ActionResult Criar()
		{
			AgrotoxicoVM vm = new AgrotoxicoVM(
				new Agrotoxico() { CadastroAtivo = true },
				_busLista.AgrotoxicoIngredienteAtivoUnidadeMedida, 
				_busClasseUso.Listar(),
				_busFormaApresentacao.ObterLista(),
				_busGrupoQuimico.ObterLista(),
				_busPericulosidadeAmbiental.ObterLista(),
				_busClassToxicologica.ObterLista());

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.AgrotoxicoCriar })]
		public ActionResult Criar(Agrotoxico agrotoxico)
		{
			_bus.Salvar(agrotoxico);

			return Json(new
			{
				@Valido = Validacao.EhValido,
				@Erros = Validacao.Erros,
				@Url = Url.Action("Criar", "Agrotoxico", new { Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.AgrotoxicoEditar })]
		public ActionResult Editar(int id)
		{
			AgrotoxicoVM vm = new AgrotoxicoVM(
				_bus.Obter(id), 
				_busLista.AgrotoxicoIngredienteAtivoUnidadeMedida, 
				_busClasseUso.Listar(), 
				_busFormaApresentacao.ObterLista(), 
				_busGrupoQuimico.ObterLista(), 
				_busPericulosidadeAmbiental.ObterLista(), 
				_busClassToxicologica.ObterLista());

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.AgrotoxicoEditar })]
		public ActionResult Editar(Agrotoxico agrotoxico)
		{
			_bus.Salvar(agrotoxico);

			return Json(new
			{
				@Valido = Validacao.EhValido,
				@Erros = Validacao.Erros,
				@Url = Url.Action("Index", "Agrotoxico", new { Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.AgrotoxicoVisualizar })]
		public ActionResult Visualizar(int id)
		{
			AgrotoxicoVM vm = new AgrotoxicoVM(
				_bus.Obter(id),
				_busLista.AgrotoxicoIngredienteAtivoUnidadeMedida, 
				_busClasseUso.Listar(), 
				_busFormaApresentacao.ObterLista(), 
				_busGrupoQuimico.ObterLista(), 
				_busPericulosidadeAmbiental.ObterLista(), 
				_busClassToxicologica.ObterLista(), 
				true);

			return View(vm);
		}

		#endregion

		#region Excluir

		[Permite(RoleArray = new Object[] { ePermissao.AgrotoxicoExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ConfirmarVM vm = new ConfirmarVM();

			Agrotoxico agrotoxico = _bus.Obter(id);
			vm.Id = agrotoxico.Id;
			vm.Mensagem = Mensagem.Agrotoxico.MensagemExcluir(agrotoxico.NumeroCadastro.ToString());
			vm.Titulo = "Excluir Agrotóxico";

			return PartialView("Confirmar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.AgrotoxicoExcluir })]
		public ActionResult Excluir(int id)
		{
			_bus.Excluir(id);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Cultura

		[Permite(RoleArray = new Object[] { ePermissao.AgrotoxicoCriar })]
		public ActionResult CriarCultura(AgrotoxicoCultura agrotoxicoCultura)
		{
			AgrotoxicoCulturaVM vm = new AgrotoxicoCulturaVM(agrotoxicoCultura, _busModalidadeAplicacao.Listar());
			return PartialView("AgrotoxicoCulturaPartial", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.AgrotoxicoEditar })]
		public ActionResult EditarCultura(AgrotoxicoCultura agrotoxicoCultura)
		{
			AgrotoxicoCulturaVM vm = new AgrotoxicoCulturaVM(agrotoxicoCultura, _busModalidadeAplicacao.Listar());
			return PartialView("AgrotoxicoCulturaPartial", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.AgrotoxicoVisualizar })]
		public ActionResult VisualizarCultura(AgrotoxicoCultura agrotoxicoCultura)
		{
			AgrotoxicoCulturaVM vm = new AgrotoxicoCulturaVM(agrotoxicoCultura, _busModalidadeAplicacao.Listar(), true);
			return PartialView("AgrotoxicoCulturaPartial", vm);
		}

		#endregion

		#region Auxiliares

		public ActionResult ObterMensagemAgrotoxicoDesativado(Agrotoxico agrotoxico)
		{
			_bus.CarregarMotivoAgrotoxicoDesativado(agrotoxico);

			return Json(new
			{
				@Valido = Validacao.EhValido,
				@Erros = Validacao.Erros,
				@MotivoId = agrotoxico.MotivoId,
				@MotivoTexto = agrotoxico.MotivoTexto,
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}