using System;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloSetor;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloSetor.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMSetor;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class SetorController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		SetorLocalizacaoBus _bus = new SetorLocalizacaoBus(new SetorLocalizacaoValidar());
		SetorLocalizacaoValidar _validar = new SetorLocalizacaoValidar();

		#endregion

		#region Filtrar

		[Permite(RoleArray = new Object[] { ePermissao.SetorListar, ePermissao.SetorEditar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(_busLista.QuantPaginacao, _busLista.AgrupadoresSetor, _busLista.Setores, _busLista.Municipios("ES"));
			vm.PodeEditar = User.IsInRole(ePermissao.SetorEditar.ToString());
			return View(vm);
		}


		[Permite(RoleArray = new Object[] { ePermissao.SetorListar, ePermissao.SetorEditar })]
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

			Resultados<SetorLocalizacao> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeVisualizar = User.IsInRole(ePermissao.RoteiroVisualizar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;
			vm.PodeEditar = User.IsInRole(ePermissao.SetorEditar.ToString());

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Editar 

		[Permite(RoleArray = new Object[] { ePermissao.SetorEditar })]
		public ActionResult Editar(int id) 
		{
			SetorLocalizacao setor = _bus.Obter(id);
			SalvarVM vm = new SalvarVM(setor, _busLista.AgrupadoresSetor, _busLista.Setores, _busLista.Estados, _busLista.Municipios(setor.Endereco.EstadoId));
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.SetorEditar })]
		public ActionResult Editar(SetorLocalizacao setor)
		{
			_bus.Salvar(setor);

			string urlRetorno = Url.Action("Index", "Setor");
			urlRetorno += "?Msg=" + Validacao.QueryParam();

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @urlRetorno = urlRetorno }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar


		[Permite(RoleArray = new Object[] { ePermissao.SetorListar, ePermissao.SetorVisualizar })]
		public ActionResult Visualizar(int id)
		{
			SalvarVM vm = new SalvarVM(_bus.Obter(id), _busLista.AgrupadoresSetor, _busLista.Setores, _busLista.Estados, _busLista.Municipios("ES"));
			return View(vm);
		}

		#endregion

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.SetorEditar, ePermissao.SetorListar, ePermissao.SetorVisualizar })]
		public ActionResult ObterDadosMunicipios(int estado)
		{
			return Json(new
			{
				@Municipios = _busLista.Municipios(estado)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}
