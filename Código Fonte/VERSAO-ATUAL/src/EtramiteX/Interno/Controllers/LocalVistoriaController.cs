using System;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloLocalVistoria;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLocalVistoria.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMLocalVistoria;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class LocalVistoriaController : Controller
	{
		#region Propriedades

		//GrupoQuimicoBus _grupoQuimicoBus = new GrupoQuimicoBus();
		//ClasseUsoBus _classeUsoBus = new ClasseUsoBus();
		//PericulosidadeAmbientalBus _periculosidadeAmbiental = new PericulosidadeAmbientalBus();
		//ClassificacaoToxicologicaBus _classificacaoToxicologicaBus = new ClassificacaoToxicologicaBus();
		//FormaApresentacaoBus _formasApresentacao = new FormaApresentacaoBus();
		//CulturaBus _culturaBus = new CulturaBus();
		//PragaBus _pragaBus = new PragaBus();
		//ListaBus _busLista = new ListaBus();
		//CulturaValidar _validar = new CulturaValidar();

		//ModalidadeAplicacaoBus _modalidadeAplicacaoBus = new ModalidadeAplicacaoBus();

		LocalVistoriaBus _localVistoriaBus = new LocalVistoriaBus();
		ListaBus _busLista = new ListaBus();

		#endregion

		public ActionResult Index()
		{
			return LocalVistoriaListar();
		}

		#region CadastrarLocalVistoria

		[Permite(RoleArray = new Object[] { ePermissao.LocalVistoriaOperar })]
		public ActionResult CadastrarLocalVistoria()
		{
			LocalVistoriaVM vm = new LocalVistoriaVM(_localVistoriaBus.SetoresAgrupadorTecnico(), 0, _busLista.DiasSemana);
			vm.IsEdicao = false;
			vm.IsVisualizar = false;
			return View("LocalVistoria", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.LocalVistoriaOperar })]
		public ActionResult OperarLocalVistoria(int idSetor)
		{
			LocalVistoriaVM vm = new LocalVistoriaVM(_localVistoriaBus.SetoresAgrupadorTecnico(), idSetor, _busLista.DiasSemana);

			LocalVistoria buscandoLocal = new LocalVistoria();
			buscandoLocal = _localVistoriaBus.Obter(idSetor, null);
			vm.DiasHorasVistoria = buscandoLocal.DiasHorasVistoria;
            vm.ListBloqueios = buscandoLocal.Bloqueios;
			vm.IsEdicao = true;
			vm.IsVisualizar = false;
			return View("LocalVistoria", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.LocalVistoriaVisualizar })]
		public ActionResult VisualizarLocalVistoria(int idSetor)
		{
			LocalVistoriaVM vm = new LocalVistoriaVM(_localVistoriaBus.SetoresAgrupadorTecnico(), idSetor, _busLista.DiasSemana);

			LocalVistoria buscandoLocal = new LocalVistoria();
			buscandoLocal = _localVistoriaBus.Obter(idSetor, null);
			vm.DiasHorasVistoria = buscandoLocal.DiasHorasVistoria;
            vm.ListBloqueios = buscandoLocal.Bloqueios;
			vm.IsEdicao = false;
			vm.IsVisualizar = true;
			return View("LocalVistoriaVisualizar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.LocalVistoriaOperar })]
		public ActionResult EscolherSetorLocalVistoria(LocalVistoria local)
		{
			LocalVistoria buscandoLocal = new LocalVistoria();
			buscandoLocal = _localVistoriaBus.Obter(local.SetorID, null);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Local = buscandoLocal
			}, JsonRequestBehavior.AllowGet);

		}

		[Permite(RoleArray = new Object[] { ePermissao.LocalVistoriaOperar })]
		public ActionResult PodeExcluir(DiaHoraVistoria DiaHora)
		{
			bool verificaPode = _localVistoriaBus.VerificaPodeExcluir(DiaHora, true);
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros
			}, JsonRequestBehavior.AllowGet);

		}

        [Permite(RoleArray = new Object[] { ePermissao.LocalVistoriaOperar })]
        public ActionResult PodeAcrescentarBloqueio(string datInicial, string datFinal, int setorId)
        {

            if (!string.IsNullOrEmpty(datInicial) && !string.IsNullOrEmpty(datFinal) && setorId != 0)
                  _localVistoriaBus.VerificaPodeIncluirBloqueio(datInicial, datFinal, setorId);

          
            return Json(new
            {
                @EhValido = Validacao.EhValido,
                @Msg = Validacao.Erros
            }, JsonRequestBehavior.AllowGet);

        }

		[Permite(RoleArray = new Object[] { ePermissao.LocalVistoriaOperar })]
		public ActionResult SalvarLocalVistoria(LocalVistoria local)
		{

			_localVistoriaBus.Salvar(local);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Url = Url.Action("LocalVistoriaListar", "LocalVistoria") + "?Msg=" + Validacao.QueryParam(),
				@Local = local
			}, JsonRequestBehavior.AllowGet);

		}

		#endregion

		#region Listar

		[Permite(RoleArray = new Object[] { ePermissao.LocalVistoriaListar })]
		public ActionResult LocalVistoriaListar()
		{
			LocalVistoriaListarVM vm = new LocalVistoriaListarVM(_busLista.QuantPaginacao, _busLista.DiasSemana);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.PodeVisualizar = User.IsInRole(ePermissao.LocalVistoriaVisualizar.ToString());
			vm.PodeEditar = User.IsInRole(ePermissao.LocalVistoriaOperar.ToString());


			return View("Listar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.LocalVistoriaListar })]
		public ActionResult FiltrarLocalVistoria(LocalVistoriaListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<LocalVistoriaListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(_busLista.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			LocalVistoriaBus localVitoriaBus = new LocalVistoriaBus();
			Resultados<LocalVistoriaListar> resultados = localVitoriaBus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeVisualizar = User.IsInRole(ePermissao.LocalVistoriaVisualizar.ToString());
			vm.PodeEditar = User.IsInRole(ePermissao.LocalVistoriaOperar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new
			{
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}