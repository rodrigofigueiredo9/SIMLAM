using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMDUA;
using Tecnomapas.EtramiteX.Interno.Controllers;
using Tecnomapas.EtramiteX.Interno.Model.ModuloDUA.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class DUAController : DefaultController
	{
		#region Propriedades

		DuaBus _bus = new DuaBus();

		public static EtramitePrincipal Usuario
		{
			get { return (System.Web.HttpContext.Current.User as EtramitePrincipal); }
		}
		#endregion

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioListar })]
		public ActionResult Listar(int id)
		{
			DUAVM vm = new DUAVM();
			vm.Titulo.Id = id;
			vm.DuaLst = _bus.Obter(id);

			if (vm.DuaLst.Count() == 0)
			{
				_bus.Emitir(id);
				vm.DuaLst = _bus.Obter(id);
			}

			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioListar })]
		public ActionResult ReemitirDua(string dua, int titulo)
		{
			_bus.Reemitir(dua);
			var lista = _bus.Obter(titulo);

			return Json(new { @lstDua = lista, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

	}
}