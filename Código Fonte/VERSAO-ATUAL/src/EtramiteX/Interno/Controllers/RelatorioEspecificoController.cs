using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMRelatorioEspecifico;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.Blocos.Entities.Interno.ModuloRelatorioEspecifico;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloRelatorioEspecifico.Business;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloRelatorioEspecifico.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloRelatorioEspecifico.Xlsx;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class RelatorioEspecificoController : Controller
	{

		#region Propriedades

//		LocalVistoriaBus _localVistoriaBus = new LocalVistoriaBus();
		ListaBus _busLista = new ListaBus();
		RelatorioMapaBus _validar = new RelatorioMapaBus();


		#endregion

		public ActionResult Index()
		{
			return RelatorioMapa();
		}

		#region RelatorioMapa

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioMapa })]
		public ActionResult RelatorioMapa()
		{
			RelatorioMapaVM vm = new RelatorioMapaVM(_busLista.TipoRelatorioMapa, -1);
			return View("RelatorioMapa", vm);
		}
		
		[Permite(RoleArray = new Object[] { ePermissao.RelatorioMapa })]
		public ActionResult PDFRelatorioMapa(string paramsJson)
		{
			try
			{
				RelatorioMapaFiltroeResultado relatorio = ViewModelHelper.JsSerializer.Deserialize<RelatorioMapaFiltroeResultado>(paramsJson);

				if (_validar.ValidarParametroRelatorio(relatorio))
				{
					EtramiteIdentity func = User.Identity as EtramiteIdentity;
					FuncionarioBus _busFuncionario = new FuncionarioBus();
					List<Setor> setores = _busFuncionario.ObterSetoresFuncionario(func.FuncionarioId);
					relatorio.IdSetor = setores[0].Id;
					relatorio.NomeFuncionario = func.Name;

					if (relatorio.tipoRelatorio == (int)eTipoRelatorioMapa.CFO_CFOC)
					{
						return ViewModelHelper.GerarArquivoPdf(new PdfRelatorioMapaCFOCFOC().Gerar(relatorio), "Relatorio MAPA - CFOCFOC");
					}
					return ViewModelHelper.GerarArquivoPdf(new PdfRelatorioMapaPTV().Gerar(relatorio), "Relatorio MAPA - PTV");
				}
				return RedirectToAction("", Validacao.QueryParamSerializer());
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("", Validacao.QueryParamSerializer());
			}
		}

		[Permite(RoleArray = new Object[] { ePermissao.RelatorioMapa })]
		public ActionResult XlsxRelatorioMapa(string paramsJson)
		{
			try
			{
				RelatorioMapaFiltroeResultado relatorio = ViewModelHelper.JsSerializer.Deserialize<RelatorioMapaFiltroeResultado>(paramsJson);

				if (_validar.ValidarParametroRelatorio(relatorio))
				{
					EtramiteIdentity func = User.Identity as EtramiteIdentity;
					FuncionarioBus _busFuncionario = new FuncionarioBus();
					List<Setor> setores = _busFuncionario.ObterSetoresFuncionario(func.FuncionarioId);
					relatorio.IdSetor = setores[0].Id;
					relatorio.NomeFuncionario = func.Name;

					if (relatorio.tipoRelatorio == (int)eTipoRelatorioMapa.CFO_CFOC)
					{
						return ViewModelHelper.GerarArquivo("Relatorio MAPA - CFOCFOC.xlsx",XlsxRelatorioMapaCFOCFOC.Gerar(relatorio),"application/vnd.ms-excel");
		
					}
					return ViewModelHelper.GerarArquivo("Relatorio MAPA - PTV.xlsx", XlsxRelatorioMapaPTV.Gerar(relatorio), "application/vnd.ms-excel");
				}
				return RedirectToAction("", Validacao.QueryParamSerializer()); 

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		#endregion
	}
}