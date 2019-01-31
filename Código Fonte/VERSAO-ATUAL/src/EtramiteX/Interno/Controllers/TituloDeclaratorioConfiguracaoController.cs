using System;
using System.Web.Mvc;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTItuloDeclaratorioConfiguracao;


namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class TituloDeclaratorioConfiguracaoController : DefaultController
	{
		TituloDeclaratorioConfiguracaoBus _bus = new TituloDeclaratorioConfiguracaoBus();

		[Permite(RoleArray = new Object[] { ePermissao.ConfigDocumentoFitossanitario })]
		public ActionResult Configurar()
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
	}
}