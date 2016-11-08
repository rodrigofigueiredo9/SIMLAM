using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class TituloController : DefaultController
	{
		#region Propriedades

		TituloCredenciadoBus _busTituloCredenciado = new TituloCredenciadoBus();
		TituloModeloInternoBus _busModelo = new TituloModeloInternoBus();
		CredenciadoBus _busCredenciado = new CredenciadoBus();

		private string QuantidadePorPagina
		{
			get { return ViewModelHelper.CookieQuantidadePorPagina; }
		}

		#endregion

		#region Filtrar/Associar

		[Permite(RoleArray = new Object[] { ePermissao.TituloListar })]
		public ActionResult Index()
		{
			List<TituloModeloLst> modelos = _busModelo.ObterModelos();
			List<TituloModeloLst> modelosDeclaratorios = _busModelo.ObterModelosDeclaratorios();
			modelos = modelos.Where(x => !modelosDeclaratorios.Exists(y => y.Id == x.Id)).ToList();


			ListarVM vm = new ListarVM(ListaCredenciadoBus.QuantPaginacao, modelos, _busTituloCredenciado.ObterSituacoes(), ListaCredenciadoBus.Setores);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(QuantidadePorPagina);
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloListar })]
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

			//2 - Emitido | 3 - Concluído | 4 - Assinado | 6 - Prorrogado
			vm.Filtros.SituacoesFiltrar = new List<int> { 2, 3, 4, 6 };

			Resultados<Titulo> resultados = _busTituloCredenciado.Filtrar(vm.Filtros, paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeVisualizar = User.IsInRole(ePermissao.TituloVisualizar.ToString());
 
			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "TituloListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region PDF

		[Permite(RoleArray = new Object[] { ePermissao.TituloVisualizar })]
		public ActionResult GerarPdf(int id)
		{
			try
			{
				TituloInternoBus tituloInternoBus = new TituloInternoBus();
				Arquivo arquivo = tituloInternoBus.GerarPdf(id);

				if (arquivo != null && Validacao.EhValido)
				{
					return ViewModelHelper.GerarArquivo(arquivo);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return RedirectToAction("Index", "Home", Validacao.QueryParamSerializer());
		}

		#endregion
	}
}