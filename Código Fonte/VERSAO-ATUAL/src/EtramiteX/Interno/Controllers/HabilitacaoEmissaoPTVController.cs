using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.HabilitacaoEmissao;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPTV.Business;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMHabilitacaoEmissaoPTV;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class HabilitacaoEmissaoPTVController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		HabilitacaoEmissaoPTVBus _bus = new HabilitacaoEmissaoPTVBus();
		FuncionarioBus _busFuncionario = new FuncionarioBus();

		#endregion

		#region Listar

		[Permite(RoleArray = new Object[] { ePermissao.HabilitacaoEmissaoPTVListar })]
		public ActionResult Index() 
		{
			ListarVM vm = new ListarVM(_busLista.QuantPaginacao, _busLista.Setores);

			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.HabilitacaoEmissaoPTVListar })]
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

			Resultados<HabilitacaoEmissaoPTVFiltro> resultados = _bus.Filtrar(vm.Filtros, paginacao);
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

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.HabilitacaoEmissaoPTVCriar, ePermissao.HabilitacaoEmissaoPTVEditar })]
		public ActionResult VerificarCPF(string cpf)
		{
			Funcionario funcionario = null;

			if (_bus.VerificarCPF(cpf))
			{
				funcionario = _busFuncionario.Obter(cpf);

				if (funcionario == null)
				{
					Validacao.Add(Mensagem.Funcionario.Inexistente);
				}
			}

			return Json(new { @Msg = Validacao.Erros, @EhValido = Validacao.EhValido, @Funcionario = funcionario });
		}

		[Permite(RoleArray = new Object[] { ePermissao.HabilitacaoEmissaoPTVCriar })]
		public ActionResult Criar() 
		{
			HabilitacaoEmissaoPTVVM vm = new HabilitacaoEmissaoPTVVM(
				new HabilitacaoEmissaoPTV(), 
				_busLista.Estados,
				new List<Municipio>()
				, _busLista.OrgaosClasse, false);

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.HabilitacaoEmissaoPTVCriar })]
		public ActionResult Criar(HabilitacaoEmissaoPTV habilitacao)
		{
			_bus.Salvar(habilitacao);
			return Json(new { @EhValido = Validacao.EhValido, @Erros = Validacao.Erros, @Url=Url.Action("Index", "HabilitacaoEmissaoPTV") +"?Msg=" + Validacao.QueryParam()});
		}

		#endregion Criar

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.HabilitacaoEmissaoPTVEditar})]
		public ActionResult Editar(int id)
		{
			HabilitacaoEmissaoPTV habilitacao = _bus.Obter(id);

			HabilitacaoEmissaoPTVVM vm = new HabilitacaoEmissaoPTVVM(
				habilitacao,
				_busLista.Estados,
				_busLista.Municipios(habilitacao.Endereco.EstadoId), 
				_busLista.OrgaosClasse, false);

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.HabilitacaoEmissaoPTVEditar })]
		public ActionResult Editar(HabilitacaoEmissaoPTV habilitacao)
		{
			_bus.Salvar(habilitacao);
			return Json(new { @EhValido = Validacao.EhValido, @Erros = Validacao.Erros, @Url = Url.Action("Index", "HabilitacaoEmissaoPTV") + "?Msg=" + Validacao.QueryParam() });
		}

		#endregion Editar

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.HabilitacaoEmissaoPTVListar})]
		public ActionResult Visualizar(int id)
		{
			HabilitacaoEmissaoPTV habilitacao = _bus.Obter(id);

			HabilitacaoEmissaoPTVVM vm = new HabilitacaoEmissaoPTVVM(
				habilitacao,
				_busLista.Estados,
				_busLista.Municipios(habilitacao.Endereco.EstadoId),
				_busLista.OrgaosClasse, true);

			return View(vm);
		}

		#endregion Visualizar

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.HabilitacaoEmissaoPTVCriar, ePermissao.HabilitacaoEmissaoPTVEditar })]
		public ActionResult ExisteOperador(int id)
		{
			return Json(new 
			{ 
				@Erros = Validacao.Erros, 
				@EhValido = Validacao.EhValido,
				@Operador = _bus.ExisteOperador(id)
			});
		}

		[Permite(RoleArray = new Object[] { ePermissao.HabilitacaoEmissaoPTVCriar, ePermissao.HabilitacaoEmissaoPTVEditar })]
		public ActionResult BuscarMunicipios(int estado)
		{
			return Json(new { @Erros = Validacao.Erros, @EhValido = Validacao.EhValido, @Municipios = _busLista.Municipios(estado) });
		}

		[Permite(RoleArray = new Object[] { ePermissao.HabilitacaoEmissaoPTVListar })]
		public ActionResult AlterarSituacao(int id, int status)
		{
			_bus.AlterarSituacao(id, status);

			return Json(new { @Msg = Validacao.Erros, @EhValido = Validacao.EhValido });
		}

		#endregion Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.HabilitacaoEmissaoPTVListar })]
		public ActionResult GerarPDF(int id)
		{
			return ViewModelHelper.GerarArquivoPdf(new PdfTermoHabilitacaoEmissaoPTV().Gerar(id), "Termo de habilitacao do funcionario para emissao de PTV");
		}
	}
}