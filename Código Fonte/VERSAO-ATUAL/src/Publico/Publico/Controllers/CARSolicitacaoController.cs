using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCadastroAmbientalRural.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloCadastroAmbientalRural.Pdf;
using Tecnomapas.EtramiteX.Publico.Model.ModuloCadastroAmbientalRural.Business;
using Tecnomapas.EtramiteX.Publico.ViewModels;
using Tecnomapas.EtramiteX.Publico.ViewModels.VMCARSolicitacao;

namespace Tecnomapas.EtramiteX.Publico.Controllers
{
	public class CARSolicitacaoController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		CARSolicitacaoBus _bus = new CARSolicitacaoBus();
		TituloBus _busTitulo = new TituloBus();
		AtividadeBus _busAtividade = new AtividadeBus();
		ProtocoloBus _busProtocolo = new ProtocoloBus();

		public static EtramitePrincipal Usuario
		{
			get { return (System.Web.HttpContext.Current.User as EtramitePrincipal); }
		}

		#endregion

		#region Filtrar

		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(_busLista.QuantPaginacao, _busLista.Municipios(ViewModelHelper.EstadoDefaultId()));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
            vm.Filtros.IsSolicitacaoNumero = true;
            vm.Filtros.IsCPF = true;
            return View(vm);
		}

		public ActionResult Filtrar(ListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(_busLista.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<SolicitacaoListarResultados> resultados = _bus.Filtrar(vm.Filtros, paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		public ActionResult Visualizar(int id)
		{
			CARSolicitacao solicitacao = null;
			List<Protocolos> lstProcessosDocumentos = null;

			if (_bus.ExisteCredenciado(id))
			{
				solicitacao = _bus.ObterCredenciado(id);
			}
			else
			{
				solicitacao = _bus.ObterInterno(id);
				lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(solicitacao.Protocolo.Id.GetValueOrDefault(0));
			}

			CARSolicitacaoVM vm = new CARSolicitacaoVM(
				solicitacao,
				_busLista.CadastroAmbientalRuralSolicitacaoSituacao,
				lstProcessosDocumentos,
				_busLista.AtividadesSolicitada,
				_bus.ObterDeclarantesLst(solicitacao.Empreendimento.Id),
				isVisualizar: true);

			return View(vm);
		}

		#endregion

		#region GerarPdf

		public ActionResult GerarPdf(int id)
		{
			try
			{
				MemoryStream resultado = null;

				if (_bus.ExisteCredenciado(id))
					resultado = new PdfCARSolicitacaoCredenciado().Gerar(_bus.ObterCredenciado(id));
				else
					resultado = new PdfCARSolicitacaoInterno().Gerar(_bus.ObterInterno(id));

				if (!Validacao.EhValido)
					return RedirectToAction("Index", Validacao.QueryParamSerializer());

				return ViewModelHelper.GerarArquivo("Solicitacao Inscricao CAR.pdf", resultado, "application/pdf");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		public ActionResult GerarTituloPdf(int id)
		{
			try
			{
				Arquivo arquivo = _busTitulo.GerarPdf(id);
				arquivo.Nome = String.Concat(arquivo.Nome, " .pdf");

				DateTime dataAtual = DateTime.Now;
				String mensagemTarja = "Consultado em " + dataAtual.ToShortDateString() + " às " + dataAtual.ToString(@"HH\hmm\min");

				arquivo.Buffer = Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.PdfMetodosAuxiliares.TarjaVerde(arquivo.Buffer, mensagemTarja);

				if (arquivo != null && Validacao.EhValido)
				{
					return ViewModelHelper.GerarArquivo(arquivo);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return RedirectToAction("Index", Validacao.QueryParamSerializer());
		}

		public ActionResult BaixarDemonstrativoCar(int id, bool isTitulo)
		{
			var schemaSolicitacao = 0;

			if (!isTitulo)
				schemaSolicitacao = _bus.ExisteCredenciado(id) ? 2 : 1;

			var url = _bus.ObterUrlDemonstrativo(id, schemaSolicitacao, isTitulo);

			return Json(new { @UrlPdfDemonstrativo = url }, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}