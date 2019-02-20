using System;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTituloDeclaratorioConfiguracao;


namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class TituloDeclaratorioConfiguracaoController : DefaultController
	{
		#region Propriedades

		TituloDeclaratorioConfiguracaoBus _bus = new TituloDeclaratorioConfiguracaoBus();
		ListaBus _busLista = new ListaBus();

		private string QuantidadePorPagina
		{
			get { return ViewModelHelper.CookieQuantidadePorPagina; }
		}

		#endregion Propriedades

		#region Configuracao de Titulo Declaratorio

		[Permite(RoleArray = new Object[] { ePermissao.ConfigDocumentoFitossanitario })]
		public ActionResult Index()
		{
			ConfigurarVM vm = new ConfigurarVM()
			{
				Configuracao = _bus.Obter()
			};
			vm.Configuracao.BarragemSemAPP = vm.Configuracao.BarragemSemAPP ?? new Arquivo();
			vm.BarragemSemAPPJSon = ViewModelHelper.JsSerializer.Serialize(vm.Configuracao.BarragemSemAPP);

			vm.Configuracao.BarragemComAPP = vm.Configuracao.BarragemComAPP ?? new Arquivo();
			vm.BarragemComAPPJSon = ViewModelHelper.JsSerializer.Serialize(vm.Configuracao.BarragemComAPP);

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ConfigDocumentoFitossanitario })]
		public ActionResult Salvar(TituloDeclaratorioConfiguracao configuracao)
		{
			_bus.Salvar(configuracao);

			return Json(new
			{
				@Msg = Validacao.Erros,
				@EhValido = Validacao.EhValido,
				UrlRedireciona = Url.Action("Configurar", "TituloDeclaratorioConfiguracao") + "?Msg=" + Validacao.QueryParam()
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion Configuracao de Titulo Declaratorio

		#region Relatorio de Alteracao de Titulo Declaratorio

		[Permite(RoleArray = new Object[] { ePermissao.ConfigDocumentoFitossanitario })]
		public ActionResult RelatorioAlteracaoTitulo()
		{
			ListarVM vm = new ListarVM(_busLista.QuantPaginacao, _busLista.TituloDeclaratorioSituacoes);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(QuantidadePorPagina);

			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ConfigDocumentoFitossanitario })]
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

			Resultados<RelatorioTituloDecListarResultado> resultados = _bus.Filtrar(vm.Filtros, paginacao);
			if (resultados == null)
			{
				Validacao.Add(new Mensagem() { Texto = "funcionou", Tipo = eTipoMensagem.Informacao });
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "TituloListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion Relatorio de Alteracao de Titulo Declaratorio
	}
}