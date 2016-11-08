using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemPendencia;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloChecagemPendencia;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.ModuloChecagemPendencia.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloChecagemPendencia.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMChecagemPendencia;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class ChecagemPendenciaController : DefaultController
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoTituloModelo> _configTituloModelo = new GerenciadorConfiguracao<ConfiguracaoTituloModelo>(new ConfiguracaoTituloModelo());
		ChecagemPendenciaBus _bus = new ChecagemPendenciaBus(new ChecagemPendenciaValidar());
		ListaBus _busLista = new ListaBus();

		public List<int> ModeloCodigosPendencia
		{
			get { return _configTituloModelo.Obter<List<int>>(ConfiguracaoTituloModelo.KeyModeloCodigoPendencia); }
		}

		#endregion

		#region Filtrar

		[Permite(RoleArray = new Object[] { ePermissao.ChecagemPendenciaListar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(_busLista.QuantPaginacao, _busLista.SituacaoChecagemPendencia);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return PartialView(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ChecagemPendenciaListar })]
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

			Resultados<ChecagemPendencia> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			if (!vm.PodeAssociar)
			{
				vm.PodeExcluir = User.IsInRole(ePermissao.ChecagemPendenciaExcluir.ToString());
			}

			vm.PodeVisualizar = User.IsInRole(ePermissao.ChecagemPendenciaVisualizar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Associar

		[Permite(RoleArray = new Object[] { ePermissao.ChecagemPendenciaListar })]
		public ActionResult Associar()
		{
			ListarVM vm = new ListarVM(_busLista.QuantPaginacao, _busLista.SituacaoChecagemPendencia);
			vm.PodeAssociar = true;
			return PartialView("ListarFiltros", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ChecagemPendenciaCriar })]
		public ActionResult AssociarTitulo(int tituloId, int checagemPendenciaId = 0)
		{
			ChecagemPendencia checagemPendencia = new ChecagemPendencia();
			if (_bus.VerificaTituloAssociado(tituloId, checagemPendenciaId))
			{
				checagemPendencia = _bus.ObterDeTituloDePendencia(tituloId);
				_bus.VerificaChecagemPendenciaItens(checagemPendencia);
			}

			string htmlItens = "";
			if (Validacao.EhValido)
			{
				htmlItens = ViewModelHelper.RenderPartialViewToString(
					ControllerContext,
					"~/Views/ChecagemPendencia/SalvarItens.ascx",
					new SalvarVM() { ChecagemPendencia = checagemPendencia }
				);
			}

			return Json(
				new
				{
					@EhValido = Validacao.EhValido,
					@Msg = Validacao.Erros,
					ChecagemPendencia = checagemPendencia,
					HtmlItens = htmlItens
				},
				JsonRequestBehavior.AllowGet
			);
		}

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.ChecagemPendenciaCriar })]
		public ActionResult Criar()
		{
			SalvarVM vm = new SalvarVM();
			vm.ModelosListarTitulo = String.Join("@", ModeloCodigosPendencia);
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ChecagemPendenciaCriar })]
		public ActionResult Salvar(ChecagemPendencia checagemPendencia)
		{
			string urlRedireciona = Url.Action("Criar", "ChecagemPendencia");

			if (_bus.Salvar(checagemPendencia))
			{
				urlRedireciona += "?Msg=" + Validacao.QueryParam() + "&acaoId=" + checagemPendencia.Id.ToString();
			}

			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, UrlRedireciona = urlRedireciona });
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.ChecagemPendenciaVisualizar })]
		public ActionResult Visualizar(int id)
		{
			ChecagemPendencia checagem = _bus.Obter(id);

			if (!Validacao.EhValido)
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			if (checagem == null || checagem.Id == 0)
			{
				Validacao.Add(Mensagem.ChecagemPendencia.NaoEncontrado);
			}

			SalvarVM vm = new SalvarVM(checagem);

			vm.IsVisualizar = true;

			if (Request.IsAjaxRequest())
			{
				return PartialView("VisualizarPartial", vm);
			}
			else
			{
				return View(vm);
			}

		}

		#endregion

		#region Excluir

		[Permite(RoleArray = new Object[] { ePermissao.ChecagemPendenciaExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();

			ChecagemPendencia checagem = _bus.Obter(id);
			vm.Id = id;
			vm.Mensagem = Mensagem.ChecagemPendencia.MensagemExcluir(checagem.Numero.ToString());
			vm.Titulo = "Excluir Checagem de Pendência";
			return PartialView("Excluir", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ChecagemPendenciaExcluir })]
		public ActionResult Excluir(int id)
		{
			_bus.Excluir(id);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Pdf

		public ActionResult ChecagemPendenciaPdfObj(string paramsJson)
		{
			try
			{
				ChecagemPendenciaRelatorioRelatorio checagem = ViewModelHelper.JsSerializer.Deserialize<ChecagemPendenciaRelatorioRelatorio>(paramsJson);
				return ViewModelHelper.GerarArquivoPdf(new PdfChecagemPendencia().ChecagemPendenciaPdf(checagem), "Relatorio de Pendencias");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("", Validacao.QueryParamSerializer());
			}
		}

		#endregion
	}
}