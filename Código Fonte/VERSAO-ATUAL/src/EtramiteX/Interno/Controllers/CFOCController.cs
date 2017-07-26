using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFOC.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCFOCFOC.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMCFOCFOC.CFOC;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC.Lote;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCFOCFOC.Business;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class CFOCController : DefaultController
	{
		#region Propriedades

		EmissaoCFOCBus _bus = new EmissaoCFOCBus();

		#endregion

		#region Listar

		[Permite(RoleArray = new Object[] { ePermissao.CFOCListar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(ListaCredenciadoBus.QuantPaginacao, ListaCredenciadoBus.DocFitossanitarioSituacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);

			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.CFOCListar })]
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

			Resultados<EmissaoCFOC> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeVisualizar = User.IsInRole(ePermissao.CFOCVisualizar.ToString());
			vm.PodeGerarPDF = User.IsInRole(ePermissao.CFOCListar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens.Where(x => x.SituacaoId != (int)eDocumentoFitossanitarioSituacao.EmElaboracao).ToList();

			return Json(new
			{
				Msg = Validacao.Erros,
				Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.CFOCVisualizar })]
		public ActionResult Visualizar(int id)
		{
			EmissaoCFOC entidade = _bus.Obter(id);

			CFOCVM vm = new CFOCVM(entidade, ListaCredenciadoBus.Estados, ListaCredenciadoBus.Municipios(entidade.EstadoId), _bus.ObterEmpreendimentosLista(entidade.CredenciadoId),
				new List<Lista>(), ListaCredenciadoBus.CFOCLoteEspecificacao, ListaCredenciadoBus.DocFitossanitarioSituacao, ListaCredenciadoBus.Municipios(entidade.EstadoEmissaoId));

			vm.IsVisualizar = true;
			return View(vm);
		}

		#endregion

		#region PDF

		[Permite(RoleArray = new Object[] { ePermissao.CFOCListar })]
		public ActionResult GerarPdf(int id)
		{
			try
			{
				EmissaoCFOC entidade = _bus.Obter(id, simplificado: true);

				PdfCFOC pdf = new PdfCFOC();
				return ViewModelHelper.GerarArquivoPdf(pdf.Gerar(id, entidade.CredenciadoId), "CFOC");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", "CFOC", Validacao.QueryParamSerializer());
			}
		}

		#endregion

		#region Lote

		#region Visualizar Lote

		[Permite(RoleArray = new Object[] { ePermissao.CFOCVisualizar })]
		public ActionResult LoteVisualizar(int id)
		{
			LoteBus loteBus = new LoteBus();
			Lote lote = loteBus.Obter(id);
			List<Lista> origemTipo = ListaCredenciadoBus.DocumentosFitossanitario.Where(x => x.Id == ((int)eDocumentoFitossanitarioTipo.CFO).ToString() || x.Id == ((int)eDocumentoFitossanitarioTipo.CFOC).ToString()).ToList();

			LoteVM vm = new LoteVM(loteBus.ObterEmpreendimentosResponsaveis(), origemTipo, lote);
			vm.IsVisualizar = true;

			if (Request.IsAjaxRequest())
			{
				return PartialView("LotePartial", vm);
			}
			else
			{
				return View(vm);
			}
		}

		#endregion Visualizar Lote

		#endregion Lote
	}
}