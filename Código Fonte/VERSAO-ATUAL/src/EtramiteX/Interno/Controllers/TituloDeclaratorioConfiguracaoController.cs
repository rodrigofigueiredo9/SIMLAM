using System;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTItuloDeclaratorioConfiguracao;


namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class TituloDeclaratorioConfiguracaoController : DefaultController
	{
        [Permite(RoleArray = new Object[] { ePermissao.ConfigDocumentoFitossanitario })]
		public ActionResult Configurar()
		{
			ConfigurarVM vm = new ConfigurarVM();
			return View(vm);
		}
	}
}