using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;

using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCadastroAmbientalRural.Pdf;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCARSolicitacao;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMDUA;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloDUA.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class DUAController : DefaultController
	{
		#region Propriedades

		DuaBus _bus = new DuaBus();
		TituloDeclaratorioBus _tituloBus = new TituloDeclaratorioBus();

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
				_tituloBus.AlterarSituacao(id, eTituloSituacao.AguardandoPagamento);
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