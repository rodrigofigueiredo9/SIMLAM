using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemRoteiro;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloChecagemRoteiro.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloChecagemRoteiro.Pdf.RoteiroPdf;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMChecagemRoteiro;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMRoteiro;
using moduloRoteiro = Tecnomapas.EtramiteX.Interno.Model.ModuloRoteiro;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class ChecagemRoteiroController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		ChecagemRoteiroBus _busCheck = new ChecagemRoteiroBus();
        ChecagemValidar _validar = new ChecagemValidar();

		private string QuantidadePorPagina
		{
			get { return (Request.Cookies.Get("QuantidadePorPagina") != null) ? Request.Cookies.Get("QuantidadePorPagina").Value : "5"; }
		}

		#endregion

		#region Listar

		[Permite(RoleArray = new Object[] { ePermissao.ChecagemItemRoteiroListar })]
		public ActionResult Index()
		{
			ListarCheckListRoteiroVM vm = new ListarCheckListRoteiroVM(_busLista.QuantPaginacao, _busLista.SituacaoChecagemRoteiro);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(QuantidadePorPagina);
			return PartialView(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ChecagemItemRoteiroListar })]
		public ActionResult Filtrar(ListarCheckListRoteiroVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ListarCheckListRoteiroVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(_busLista.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<ChecagemRoteiro> resultados = _busCheck.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			if (!vm.PodeAssociar)
			{
				vm.PodeExcluir = User.IsInRole(ePermissao.ChecagemItemRoteiroExcluir.ToString());
			}
			vm.PodeVisualizar = User.IsInRole(ePermissao.ChecagemItemRoteiroVisualizar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ChecagemRoteiroListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.ChecagemItemRoteiroCriar })]
		public ActionResult ChecagemRoteiroCriar()
		{
			SalvarCheckListRoteiroVM viewModel = new SalvarCheckListRoteiroVM();

			if (!string.IsNullOrEmpty(Request.QueryString["idPdf"]))
			{
				viewModel.IdPdf = Convert.ToInt32(Request.QueryString["idPdf"]);
			}
			return View(viewModel);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ChecagemItemRoteiroCriar })]
		public ActionResult ChecagemRoteiroCriar(SalvarCheckListRoteiroVM vm)
		{
			SalvarCheckListRoteiroVM view = new SalvarCheckListRoteiroVM();
			try
			{
				GerarItensRoteiro(vm);
				if (_busCheck.Salvar(vm.ChecagemRoteiro))
				{
					view.ChecagemRoteiro.Interessado = string.Empty;
					return RedirectToAction("ChecagemRoteiroCriar", Validacao.QueryParamSerializer(new {acaoId = "requerimento"}));
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return View("ChecagemRoteiroCriar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ChecagemItemRoteiroCriar })]
		public ActionResult ObterItensRoteiro(int idRoteiro)
		{
			List<Item> itens = _busCheck.ObterItensRoteiro(idRoteiro);
			return Json(new { Itens = itens, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ChecagemItemRoteiroCriar })]
		public ActionResult ChecagemRoteiroPDF(int id)
		{
			try
			{
				return ViewModelHelper.GerarArquivoPdf(PdfCheckListRoteiro.GerarCheckListRoteiroPdf(id), "Pendencias Checagem de Itens");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ChecagemItemRoteiroCriar })]
		public ActionResult ChecagemRoteiroPdfObj(string paramsJson)
		{
			try
			{
				ChecagemRoteiro checkListRoteiro = ViewModelHelper.JsSerializer.Deserialize<ChecagemRoteiro>(paramsJson);
				if (_busCheck.ValidarCheckListRoteiroPdf(checkListRoteiro))
				{
					return ViewModelHelper.GerarArquivoPdf(_busCheck.GerarPdf(checkListRoteiro), "Pendencias Checagem de Itens");
				}
				else
				{
					return RedirectToAction("", Validacao.QueryParamSerializer());
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("", Validacao.QueryParamSerializer());
			}
		}

		public ActionResult ChecagemRoteiroPdfObjValidar(ChecagemRoteiro checkListRoteiro)
		{
			_busCheck.ValidarCheckListRoteiroPdf(checkListRoteiro);
			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		private void GerarItensRoteiro(SalvarCheckListRoteiroVM vm)
		{
			if (vm.RoteirosJson != null)
			{
				foreach (string roteiroJson in vm.RoteirosJson)
				{
					Roteiro roteiro = ViewModelHelper.JsSerializer.Deserialize<Roteiro>(roteiroJson);
					vm.ChecagemRoteiro.Roteiros.Add(roteiro);
				}
			}

			if (vm.ItensJson != null)
			{
				foreach (string itemJson in vm.ItensJson)
				{
					Item item = ViewModelHelper.JsSerializer.Deserialize<Item>(itemJson);
					vm.ChecagemRoteiro.Itens.Add(item);
				}
			}
		}

		#endregion
		
		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.ChecagemItemRoteiroVisualizar })]
		public ActionResult ChecagemRoteiroVisualizar(int id)
		{
			SalvarCheckListRoteiroVM vm = new SalvarCheckListRoteiroVM();
			
			ChecagemRoteiro checkListRoteiro = _busCheck.Obter(id);

			if (!Validacao.EhValido)
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			if (checkListRoteiro != null && checkListRoteiro.Id>0)
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

		#endregion

		#region Excluir

		[Permite(RoleArray = new Object[] { ePermissao.ChecagemItemRoteiroExcluir })]
		public ActionResult ChecagemRoteiroExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();

			ChecagemRoteiro checagem = new ChecagemRoteiro() { Id = id };
			vm.Id = id;
			vm.Mensagem = Mensagem.ChecagemRoteiro.MensagemExcluirConfirm(checagem.Id.ToString());//numero
			vm.Titulo = "Excluir Checagem de Itens de Roteiro";
			return PartialView("Excluir", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ChecagemItemRoteiroExcluir })]
		public ActionResult ChecagemRoteiroExcluir(int id)
		{
			_busCheck.Excluir(id);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Validar

		[Permite(RoleArray = new Object[] { ePermissao.ChecagemItemRoteiroCriar })]
		public ActionResult ValidarAssociarRoteiro(int roteiroAssociado, List<int> roteirosAssociados)
		{
			_busCheck.ValidarAssociarRoteiro(roteiroAssociado, roteirosAssociados);
			return Json(new
			{
				@Msg = Validacao.Erros,
				@EhValido = Validacao.EhValido
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion
		
		#region Adicionar

		[Permite(RoleArray = new Object[] { ePermissao.ChecagemItemRoteiroCriar })]
		public ActionResult ItemAdicionar()
		{
			return PartialView(new ItemRoteiroVM());
		}

		[Permite(RoleArray = new Object[] { ePermissao.ChecagemItemRoteiroCriar })]
		public ActionResult EditarItemRoteiro(string itemJson)
		{
			return PartialView("ItemAdicionar", new ItemRoteiroVM(ViewModelHelper.JsSerializer.Deserialize<Item>(itemJson)));
		}

		#endregion

		#region  Associar

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoListar })]
		public ActionResult AssociarCheckList()
		{
			ListarCheckListRoteiroVM vm = new ListarCheckListRoteiroVM(_busLista.QuantPaginacao, _busLista.SituacaoChecagemRoteiro);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(QuantidadePorPagina);
			return PartialView("ChecagemRoteiroListarFiltros", vm);
		}

		#endregion
	}
}