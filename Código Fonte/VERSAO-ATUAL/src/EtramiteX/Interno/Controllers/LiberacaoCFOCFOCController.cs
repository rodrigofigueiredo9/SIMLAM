using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloLiberacaoCFOCFOC;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloOrgaoParceiroConveniado.Bussiness;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloLiberacaoNumeroCFOCFOC.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMLiberacaoCFOCFOC;


namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class LiberacaoCFOCFOCController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		CredenciadoIntBus _busCredenciadoInterno = new CredenciadoIntBus();
		HabilitarEmissaoCFOCFOCBus _busHabilitar = new HabilitarEmissaoCFOCFOCBus();
		LiberacaoNumeroCFOCFOCBus _bus = new LiberacaoNumeroCFOCFOCBus();
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		OrgaoParceiroConveniadoBus _busOrgaoParceiro = new OrgaoParceiroConveniadoBus();

        private Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business.PTVBus _PTVBusCred = new Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business.PTVBus();

		#endregion

		#region Filtrar

		[Permite(RoleArray = new Object[] { ePermissao.LiberacaoNumeroCFOCFOCListar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM();
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.LiberacaoNumeroCFOCFOCListar })]
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

			Resultados<ListarResultados> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;


			return Json(new { Msg = Validacao.Erros, Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.LiberacaoNumeroCFOCFOCCriar })]
		public ActionResult Obter(String CpfCnpj)
		{

			return View();
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.LiberacaoNumeroCFOCFOCListar })]
		public ActionResult Visualizar(int id)
		{
			LiberarNumeroCFOCFOCVM vm = new LiberarNumeroCFOCFOCVM() { isVisualizar = true };
			vm.Liberacao = _bus.Obter(id);
			if (vm.Liberacao.QuantidadeDigitalCFO >= 0)
			{
				vm.LstQuantidadeNumeroDigitalCFO.Single(x => x.Text == vm.Liberacao.QuantidadeDigitalCFO.ToString()).Selected = true;
			}

			if (vm.Liberacao.QuantidadeDigitalCFOC >= 0)
			{
				vm.LstQuantidadeNumeroDigitalCFOC.Single(x => x.Text == vm.Liberacao.QuantidadeDigitalCFOC.ToString()).Selected = true;
			}
			return View("LiberarNumeroCFOCFOC", vm);
		}

		#endregion

		#region Liberar Número de CFO / CFOC

		[Permite(RoleArray = new Object[] { ePermissao.LiberacaoNumeroCFOCFOCCriar })]
		public ActionResult LiberarNumeroCFOCFOC()
		{
			LiberarNumeroCFOCFOCVM vm = new LiberarNumeroCFOCFOCVM();
			return View(vm);
		}

        [Permite(RoleArray = new Object[] { ePermissao.LiberacaoNumeroCFOCFOCCriar })]
        public ActionResult VerificarConsultaDUA(int filaID, string NumeroDua, string cpf)
        { 
            cpf = cpf.Replace(".", "").Replace("-", "").Replace("/", "");

            if (!_PTVBusCred.VerificarSeDUAConsultada(filaID))
                return Json(new
                {
                    @Valido = Validacao.EhValido,
                    @Msg = Validacao.Erros,
                    @Consultado = false
                }, JsonRequestBehavior.AllowGet);

            _bus.VerificarDUA(filaID, NumeroDua, cpf);

            return Json(new
            {
                @Valido = Validacao.EhValido,
                @Msg = Validacao.Erros,
                @Consultado = true
            }, JsonRequestBehavior.AllowGet);
        }

        [Permite(RoleArray = new Object[] { ePermissao.LiberacaoNumeroCFOCFOCCriar })]
        public ActionResult GravarVerificacaoDUA(string NumeroDua, string cpf)
        {
            var filaID = _PTVBusCred.GravarConsultaDUA(NumeroDua, cpf, "1");

            return Json(new
            {
                @Valido = Validacao.EhValido,
                @Msg = Validacao.Erros,
                @FilaID = filaID
            }, JsonRequestBehavior.AllowGet);
        }

		[Permite(RoleArray = new Object[] { ePermissao.LiberacaoNumeroCFOCFOCCriar })]
        public ActionResult VerificarCPF(int filaID, string cpf, string NumeroDua )
		{
			CredenciadoPessoa credenciado = null;
			_bus.VerificarCPF(cpf);

			if (Validacao.EhValido)
			{
				credenciado = _busCredenciadoInterno.ObterPorCPF(cpf);
			}

            if (!_PTVBusCred.VerificarSeDUAConsultada(filaID))
                return Json(new
                {
                    @Valido = Validacao.EhValido,
                    @Msg = Validacao.Erros,
                    @Consultado = false
                }, JsonRequestBehavior.AllowGet);

            _bus.VerificarDUA(filaID, NumeroDua, cpf);


			return Json(new { @Msg = Validacao.Erros, @EhValido = Validacao.EhValido, @Credenciado = credenciado });
		}

		[Permite(RoleArray = new Object[] { ePermissao.LiberacaoNumeroCFOCFOCCriar })]
		public ActionResult SalvarLiberacao(LiberaracaoNumeroCFOCFOC liberacao)
		{
			_bus.Salvar(liberacao);

			string url = Url.Action("Index", "LiberacaoCFOCFOC") + "?Msg=" + Validacao.QueryParam() + "&acaoId=" + liberacao.Id;

			return Json(new { @Msg = Validacao.Erros, @EhValido = Validacao.EhValido, @Url = url });
		}

		#endregion

		#region Consultar

		[Permite(RoleArray = new Object[] { ePermissao.NumeroCFOCFOCConsultar })]
		public ActionResult ConsultarNumeroCFOCFOCLiberado()
		{
			ConsultarNumeroCFOCFOCLiberadoVM vm = new ConsultarNumeroCFOCFOCLiberadoVM();

			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.NumeroCFOCFOCConsultar })]
		public ActionResult VerificarCPFConsulta(string cpf)
		{
			CredenciadoPessoa credenciado = null;
			_bus.VerificarCPFConsulta(cpf);
			if (Validacao.EhValido)
			{
				credenciado = _busCredenciadoInterno.ObterPorCPF(cpf);
			}

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Credenciado = credenciado });
		}

		[Permite(RoleArray = new Object[] { ePermissao.NumeroCFOCFOCConsultar })]
		public ActionResult FiltrarConsulta(ConsultaFiltro filtro)
		{
			List<NumeroCFOCFOC> retorno = _bus.FiltrarConsulta(filtro);

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @lstRetorno = retorno });
		}

		[Permite(RoleArray = new Object[] { ePermissao.NumeroCFOCFOCConsultar })]
		public ActionResult MotivoCancelamento(MotivoCancelamentoVM vm)
		{
			return View("MotivoCancelamentoNumero", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.NumeroCFOCFOCConsultar })]
		public ActionResult Cancelar(NumeroCFOCFOC objeto)
		{
			_bus.Cancelar(objeto);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

		#endregion

		[Permite(RoleArray = new Object[] { ePermissao.LiberacaoNumeroCFOCFOCListar })]
		public ActionResult GerarPDF(int id)
		{
			return ViewModelHelper.GerarArquivoPdf(new PdfComprovanteLiberacaoNumeroCFOCFOC().Gerar(id), "Comprovante de Liberacao de Numero de CFO e CFOC");
		}
	}
}