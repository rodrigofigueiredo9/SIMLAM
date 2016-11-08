using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Publico.ViewModels;
using Tecnomapas.EtramiteX.Publico.ViewModels.VMTitulo;
using TituloPublico = Tecnomapas.EtramiteX.Publico.Model.ModuloTitulo.Business;

namespace Tecnomapas.EtramiteX.Publico.Controllers
{
	public class TituloController : DefaultController
	{
		ListaBus _busLista = new ListaBus();
		TituloModeloBus _busModelo = new TituloModeloBus();
		TituloBus _bus = new TituloBus();
		TituloPublico.TituloBus _busPub = new TituloPublico.TituloBus();

		#region Filtrar

		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(_busLista.QuantPaginacao, _busModelo.ObterModelos(), _bus.ObterSituacoes(), _busLista.Setores);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
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

			vm.Filtros.SituacoesFiltrar.Add((int)eTituloSituacao.Concluido);
			vm.Filtros.SituacoesFiltrar.Add((int)eTituloSituacao.Prorrogado);
			Resultados<Titulo> resultados = _busPub.Filtrar(vm.Filtros, paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "TituloListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		public ActionResult Visualizar(int id)
		{
			return View(new SalvarVM());
			/*SalvarVM vm = null;

			Titulo titulo = _bus.Obter(id);

			if (titulo == null)
			{
				vm = new SalvarVM(new List<Setor>(), new List<TituloModeloLst>(), new List<Municipio>());
				if (Request.IsAjaxRequest())
				{
					return PartialView("VisualizarPartial", vm);
				}

				return View(vm);
			}

			List<Setor> lstSetores = _bus.ObterFuncionarioSetores();

			vm = new SalvarVM(lstSetores, _busModelo.ObterModelos(todos: true), _bus.ObterLocais(), titulo.Setor.Id, titulo.Modelo.Id, titulo.LocalEmissao.Id);
			vm.SetoresEditar = false;
			vm.Titulo = titulo;
			vm.Modelo = _busModelo.Obter(titulo.Modelo.Id);
			vm.Titulo.Modelo = vm.Modelo;

			vm.IsVisualizar = true;
			vm.AssinantesVM.IsVisualizar = true;
			vm.DestinatarioEmailVM.IsVisualizar = true;
			vm.TituloCondicionanteVM.MostrarBotoes = false;

			vm.LabelTipoPrazo = vm.Titulo.PrazoUnidade;

			vm.DestinatarioEmailVM.Destinatarios = vm.Titulo.DestinatarioEmails;

			vm.AssinantesVM.Assinantes = _busModelo.ObterAssinantes(vm.Modelo);

			if (titulo != null)
			{
				List<TituloAssinante> assinantesDoTitulo = _bus.ObterAssinantes(id);
				vm.AssinantesVM.MergeAssinantesCargos(assinantesDoTitulo);
				vm.AssinantesVM.Assinantes = _bus.ObterAssinantesCargos(vm.AssinantesVM.Assinantes);
			}

			if (!vm.Modelo.Regra(eRegra.PdfGeradoSistema))
			{
				ArquivoBus arqBus = new ArquivoBus(eExecutorTipo.Publico);
				titulo.ArquivoPdf = arqBus.ObterDados(titulo.ArquivoPdf.Id.GetValueOrDefault());

				vm.ArquivoId = titulo.ArquivoPdf.Id;
				vm.ArquivoTexto = titulo.ArquivoPdf.Nome;
				vm.ArquivoJSon = ViewModelHelper.JsSerializer.Serialize(titulo.ArquivoPdf);
			}

			if (Request.IsAjaxRequest())
			{
				return PartialView("VisualizarPartial", vm);
			}

			return View(vm);*/
		}

		#endregion

		#region Pdf

		public ActionResult GerarPdf(int id)
		{
			try
			{
				Arquivo arquivo = _bus.GerarPdf(id);

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

		#endregion
	}
}