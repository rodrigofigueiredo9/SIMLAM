using System;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemRoteiro;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloChecagemRoteiro.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMChecagemRoteiro;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class ChecagemRoteiroController : DefaultController
	{
		private ChecagemRoteiroInternoBus _busCheck = new ChecagemRoteiroInternoBus();

		[Permite(RoleArray = new Object[] {ePermissao.ProtocoloVisualizar})]
		public ActionResult ChecagemRoteiroVisualizar(int id)
		{
			SalvarCheckListRoteiroVM vm = new SalvarCheckListRoteiroVM();

			ChecagemRoteiro checkListRoteiro = _busCheck.Obter(id);

			if (!Validacao.EhValido)
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			if (checkListRoteiro != null && checkListRoteiro.Id > 0)
			{
				vm = new SalvarCheckListRoteiroVM(checkListRoteiro);
			}

			if (Request.IsAjaxRequest())
			{
				return PartialView("ChecagemRoteiroVisualizarPartial", vm);
			}
			else
			{
				return View("ChecagemRoteiroVisualizar", vm);
			}
		}
	}
}