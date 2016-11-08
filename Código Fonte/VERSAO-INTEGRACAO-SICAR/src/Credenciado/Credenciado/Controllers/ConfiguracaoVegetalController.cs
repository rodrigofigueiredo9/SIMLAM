using System;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
using Tecnomapas.Blocos.Entities.Interno.ModuloAgrotoxico;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMAgrotoxicos;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloAgrotoxico.Business;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMConfiguracaoVegetal;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloVegetal.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class ConfiguracaoVegetalController : DefaultController
	{

        #region Propriedades

        CulturaInternoBus _culturaBus = new CulturaInternoBus();

        #endregion

        #region Associar Cultura

        public ActionResult AssociarCultura(bool straggCultivar = true)
        {
            CulturaListarVM vm = new CulturaListarVM(ListaCredenciadoBus.QuantPaginacao);
            vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
            vm.Associar = true;
            vm.StraggCultivar = straggCultivar;
            return PartialView("Cultura/ListarFiltros", vm);
        }

        public ActionResult FiltrarCultura(CulturaListarVM vm, Paginacao paginacao)
        {
            if (!String.IsNullOrEmpty(vm.UltimaBusca))
            {
                vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<CulturaListarVM>(vm.UltimaBusca).Filtros;
            }

            vm.Paginacao = paginacao;
            vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
            vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
            vm.SetListItens(ListaCredenciadoBus.QuantPaginacao, vm.Paginacao.QuantPaginacao);
            vm.Filtros.StraggCultivar = vm.StraggCultivar;

            Resultados<CulturaListarResultado> resultados = _culturaBus.Filtrar(vm.Filtros, vm.Paginacao);
            if (resultados == null)
            {
                return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
            }

            vm.PodeEditar = !vm.Associar;

            vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
            vm.Paginacao.EfetuarPaginacao();
            vm.Resultados = resultados.Itens;

            return Json(new
            {
                @Msg = Validacao.Erros,
                @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "Cultura/ListarResultados", vm)
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion
        		
	}
}