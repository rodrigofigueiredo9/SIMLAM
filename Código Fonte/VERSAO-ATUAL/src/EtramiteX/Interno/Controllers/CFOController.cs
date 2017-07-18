using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFO.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCFOCFOC.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMCFOCFOC.CFO;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class CFOController : DefaultController
	{
		#region Propriedades

		EmissaoCFOBus _bus = new EmissaoCFOBus();

		#endregion

		#region Listar

		[Permite(RoleArray = new Object[] { ePermissao.CFOListar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(ListaCredenciadoBus.QuantPaginacao, ListaCredenciadoBus.DocFitossanitarioSituacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);

			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.CFOListar })]
		public ActionResult Filtrar(ListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(ListaCredenciadoBus.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<EmissaoCFO> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeVisualizar = User.IsInRole(ePermissao.CFOVisualizar.ToString());
			vm.PodeGerarPDF = User.IsInRole(ePermissao.CFOListar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { Msg = Validacao.Erros, Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.CFOVisualizar })]
		public ActionResult Visualizar(int id)
		{
			EmissaoCFO entidade = _bus.ObterHistorico(id);

			CFOVM vm = new CFOVM(entidade, _bus.ObterProdutoresLista(entidade.CredenciadoId), ListaCredenciadoBus.Estados, ListaCredenciadoBus.Municipios(entidade.EstadoId), _bus.ObterEmpreendimentosListaEtramiteX(entidade.ProdutorId),
				new List<Lista>(), ListaCredenciadoBus.CFOProdutoEspecificacao, ListaCredenciadoBus.DocFitossanitarioSituacao, new List<Lista>(), ListaCredenciadoBus.Municipios(entidade.EstadoEmissaoId));

			vm.IsVisualizar = true;
			return View(vm);
		}

		#endregion

		#region PDF

		[Permite(RoleArray = new Object[] { ePermissao.CFOListar })]
		public ActionResult GerarPdf(int id)
		{
			try
			{
				EmissaoCFO cfo = _bus.Obter(id, simplificado: true);

				PdfCFO pdf = new PdfCFO();
				return ViewModelHelper.GerarArquivoPdf(pdf.Gerar(id, cfo.CredenciadoId), "CFO");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", "CFO", Validacao.QueryParamSerializer());
			}
		}

		#endregion
	}
}