using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloConfiguracaoDocumentoFitossanitario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoDocumentoFitossanitario;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class ConfiguracaoDocumentoFitossanitarioController : DefaultController
	{
		#region Propriedades

		ConfiguracaoDocumentoFitossanitarioBus _bus = new ConfiguracaoDocumentoFitossanitarioBus();
		ConfiguracaoDocumentoFitossanitarioValidar _validar = new ConfiguracaoDocumentoFitossanitarioValidar();
		ListaBus _listaBus = new ListaBus();

		#endregion

		[Permite(RoleArray = new Object[] { ePermissao.ConfigDocumentoFitossanitario })]
		public ActionResult Configurar()
		{
			ConfiguracaoDocumentoFitossanitarioVM vm = new ConfiguracaoDocumentoFitossanitarioVM(
				_bus.Obter(),
				_listaBus.DocumentosFitossanitario.Where(x => 
					Convert.ToInt32(x.Id) == (int)eDocumentoFitossanitarioTipo.CFO || 
					Convert.ToInt32(x.Id) == (int)eDocumentoFitossanitarioTipo.CFOC || 
					Convert.ToInt32(x.Id) == (int)eDocumentoFitossanitarioTipo.PTV).ToList());

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ConfigDocumentoFitossanitario })]
		public ActionResult Configurar(ConfiguracaoDocumentoFitossanitario configuracao)
		{
			_bus.Salvar(configuracao);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Url = Url.Action("Configurar", "ConfiguracaoDocumentoFitossanitario", new { Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ConfigDocumentoFitossanitario })]
		public ActionResult ValidarIntervalo(DocumentoFitossanitario intervalo, List<DocumentoFitossanitario> intervalos)
		{
			_validar.ValidarIntervalo(intervalo, intervalos);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
			}, JsonRequestBehavior.AllowGet);
		}

        [HttpPost]
        [Permite(RoleArray = new Object[] {ePermissao.ConfigDocumentoFitossanitario })]
        public ActionResult Editar()
        {
            int i = 2;

            //só pra testar!!!
            return Json(new
            {
                @EhValido = Validacao.EhValido,
                @Msg = Validacao.Erros,
            }, JsonRequestBehavior.AllowGet);
        }
	}
}