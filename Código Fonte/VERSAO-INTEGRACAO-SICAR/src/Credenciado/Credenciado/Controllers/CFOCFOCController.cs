using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloAgrotoxico.Business;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCFOCFOC;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.Blocos.Entities.Interno.ModuloLiberacaoCFOCFOC;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCFOCFOC.Business;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC.Lote;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class CFOCFOCController : DefaultController
	{
		#region Propriedades

		CFOCFOCInternoBus _internoBus = new CFOCFOCInternoBus();
		CredenciadoBus _credenciadoBus = new CredenciadoBus();

		#endregion

		#region Consultar

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult ConsultarNumeroCFOCFOCLiberado()
		{
			ConsultarNumeroCFOCFOCLiberadoVM vm = new ConsultarNumeroCFOCFOCLiberadoVM();

			return View(vm);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult FiltrarConsulta(ConsultaFiltro filtro)
		{
			List<NumeroCFOCFOC> retorno = _internoBus.FiltrarConsulta(filtro);
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@lstRetorno = retorno,
				@QtdCFO = retorno.Where(x => x.TipoNumero == filtro.TipoNumero && x.Tipo == (int)eCFOCFOCTipo.CFO).Count(),
				@QtdCFOC = retorno.Where(x => x.TipoNumero == filtro.TipoNumero && x.Tipo == (int)eCFOCFOCTipo.CFOC).Count(),
				@QtdCFOUtilizado = retorno.Where(x => x.TipoNumero == filtro.TipoNumero && x.Tipo == (int)eCFOCFOCTipo.CFO && x.Utilizado).Count(),
				@QtdCFOCUtilizado = retorno.Where(x => x.TipoNumero == filtro.TipoNumero && x.Tipo == (int)eCFOCFOCTipo.CFOC && x.Utilizado).Count()
			});
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult VisualizarMotivoInvalidacao(MotivoInvalidacaoVM vm)
		{
			return View("MotivoInvalidacaoNumero", vm);
		}

		#endregion
	}
}