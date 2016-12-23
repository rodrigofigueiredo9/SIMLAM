using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.Destinatario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTVOutro;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTVOutro.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPTV;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPTVOutro;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloVegetal.Business;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class PTVOutroController : DefaultController
	{
		#region Propriedades

		PTVOutroValidar _validar = new PTVOutroValidar();
		private PTVOutroBus _bus = new PTVOutroBus();

		string[] ListTipoOrigem = new[] 
		{
			Convert.ToString((int)eDocumentoFitossanitarioTipo.CFO),
            Convert.ToString((int)eDocumentoFitossanitarioTipo.CFOC),
            Convert.ToString((int)eDocumentoFitossanitarioTipo.PTV),
			Convert.ToString((int)eDocumentoFitossanitarioTipo.CFCFR),
			Convert.ToString((int)eDocumentoFitossanitarioTipo.TF)
		};

		#endregion

		[Permite(RoleArray = new Object[] { ePermissao.PTVOutroListar })]
		public ActionResult Index()
		{
			PTVOutroListarVM vm = new PTVOutroListarVM(ListaCredenciadoBus.PTVSituacao);
			return View("Index", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVOutroListar })]
		public ActionResult Filtrar(PTVOutroListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<PTVOutroListarVM>(vm.UltimaBusca).Filtros;
			}
			vm.Filtros.CredenciadoID = _bus.User.FuncionarioId;

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(ListaCredenciadoBus.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<PTVOutroListarResultado> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeVisualizar = User.IsInRole(ePermissao.PTVOutroVisualizar.ToString());
			vm.PodeCancelar = User.IsInRole(ePermissao.PTVOutroCancelar.ToString());

			EtramiteIdentity func = User.Identity as EtramiteIdentity ?? new EtramiteIdentity("", "", "", null, "", 0, 0, "", "", 0, 0);
			_bus.ObterResponsavelTecnico(func.UsuarioId).ForEach(x => { vm.RT = x.Id; });

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new
			{
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm)
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.PTVOutroCriar })]
		public ActionResult Criar()
		{
			PTVOutro ptv = new PTVOutro();
			ptv.Situacao = (int)ePTVOutroSituacao.EmElaboracao;

			PTVOutroVM vm = new PTVOutroVM(
				ptv,
				ListaCredenciadoBus.PTVSituacao,
				ListaCredenciadoBus.DocumentosFitossanitario.Where(x => ListTipoOrigem.Contains(x.Id)).ToList(),
				ListaCredenciadoBus.Estados,
				new List<Municipio>(),
				ListaCredenciadoBus.Estados,
				ListaCredenciadoBus.Municipios(ptv.InteressadoMunicipioId),
                new List<Lista>());

			vm.LstUnidades = ViewModelHelper.CriarSelectList(ListaCredenciadoBus.PTVUnidadeMedida);

			return View("Criar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PTVOutroCriar })]
		public ActionResult Salvar(PTVOutro ptv)
		{
            new DestinatarioPTVBus().Salvar(ptv.Destinatario);

		    if (!Validacao.EhValido)
		    {
                return Json(new { @Valido = Validacao.EhValido, @Erros = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		    }

		    bool valido = _bus.Salvar(ptv);

			return Json(new
			{
                @EhValido = valido,
				@Erros = Validacao.Erros,
				@Url = Url.Action("Criar", "PTVOutro") + "?Msg=" + Validacao.QueryParam() + "&acaoId=" + ptv.Id
			});
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVOutroVisualizar })]
		public ActionResult Visualizar(int id)
		{
			PTVOutro ptv = _bus.Obter(id);

			PTVOutroVM vm = new PTVOutroVM(
				ptv,
				ListaCredenciadoBus.PTVSituacao,
				ListaCredenciadoBus.DocumentosFitossanitario.Where(x => ListTipoOrigem.Contains(x.Id)).ToList(),
				ListaCredenciadoBus.Estados,
				ListaCredenciadoBus.Municipios(ptv.Estado),
				ListaCredenciadoBus.Estados,
				ListaCredenciadoBus.Municipios(ptv.InteressadoEstadoId),
                _bus.ObterPragasLista(ptv.Produtos));

			DestinatarioPTVBus _destinatarioBus = new DestinatarioPTVBus();
			vm.PTV.Destinatario = _destinatarioBus.Obter(ptv.DestinatarioID);
			vm.LstUnidades = ViewModelHelper.CriarSelectList(ListaCredenciadoBus.PTVUnidadeMedida);
			vm.IsVisualizar = true;

			return View("Visualizar", vm);
		}

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.PTVOutroCriar })]
		public ActionResult VerificarNumeroPTV(string numero)
		{
			_bus.VerificarNumeroPTV(numero);
			return Json(new
			{
				@Valido = Validacao.EhValido,
				@Msg = Validacao.Erros
			},
			JsonRequestBehavior.AllowGet);
		}

        [Permite(RoleArray = new Object[] { ePermissao.PTVOutroCriar })]
        public ActionResult ObterPragas(List<PTVOutroProduto> produtos)
        {
            if (produtos == null || produtos.Count <= 0)
            {
                return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Lista = new List<Lista>() });
            }

            List<Lista> lista = _bus.ObterPragasLista(produtos);
            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Lista = lista });
        }

        [Permite(RoleArray = new Object[] { ePermissao.PTVOutroCriar })]
        public ActionResult ObterDeclaracaoAdicional(List<PTVOutroProduto> produtos, List<Praga> pragas)
        {
            if (produtos == null || produtos.Count <= 0 || pragas == null || pragas.Count <= 0)
            {
                return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @DeclaracoesAdicionais = string.Empty });
            }

            CulturaInternoBus culturaBus = new CulturaInternoBus();
            List<Cultivar> cultivares = culturaBus.ObterCultivares(produtos.Select(x => x.Cultura).ToList(), null, 1 ) ?? new List<Cultivar>();

            List<string> declaracoesAdicionais = cultivares
            .Where(x => produtos.Select(y => y.Cultivar).ToList().Any(y => y == x.Id))
            .SelectMany(x => x.LsCultivarConfiguracao.Where(y => produtos.Count(z => z.Cultivar == y.Cultivar && y.TipoProducaoId == (int)ValidacoesGenericasBus.ObterTipoProducao(z.UnidadeMedida)) > 0))
            .Where(x => pragas.Any(y => y.Id == x.PragaId))
            .Select(x => x.DeclaracaoAdicionalTextoHtml)
            .Distinct().ToList();

            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @DeclaracoesAdicionais = String.Join(" ", declaracoesAdicionais) });
        }

		[Permite(RoleArray = new Object[] { ePermissao.PTVOutroCriar })]
		public ActionResult ObterCultivar(eDocumentoFitossanitarioTipo origemTipo, int culturaID = 0)
		{
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Erros = Validacao.Erros,
				@Cultivar = _bus.ObterCultivar(origemTipo, culturaID)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVOutroCriar })]
		public ActionResult ObterMunicipio(int estado)
		{
			return Json(new
			{
				@Valido = Validacao.EhValido,
				@Erros = Validacao.Erros,
				@Municipios = ListaCredenciadoBus.Municipios(estado)
			});
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVOutroCriar })]
		public ActionResult ValidarIdentificacaoProduto(PTVOutroProduto item, List<PTVOutroProduto> lista)
		{
			_validar.ValidarProduto(item, lista);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

        [Permite(RoleArray = new Object[] { ePermissao.PTVOutroCriar })]
        public ActionResult ValidarPraga(Praga item, List<Praga> lista)
        {
            //_validar.ValidarPraga(item, lista);

            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
        }

        [Permite(RoleArray = new Object[] { ePermissao.PTVOutroCriar })]
        public ActionResult ObterDeclaracoes(int pragaId )
        {
            return Json(new
            {
                @Valido = Validacao.EhValido,
                @Msg = Validacao.Erros,
                @Declaracoes = _bus.ObterListaDeclaracao(pragaId)
            },
            JsonRequestBehavior.AllowGet);
        }

		[Permite(RoleArray = new Object[] { ePermissao.PTVOutroCriar })]
		public ActionResult ObterDestinatario(int destinatarioId)
		{
			DestinatarioPTV destinatario = _bus.ObterDestinatario(destinatarioId);
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Destinatario = destinatario
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVOutroCriar })]
		public ActionResult ValidarDocDestinatario(int pessoaTipo, string CpfCnpj)
		{
			DestinatarioPTV destinatario = _validar.ValidarDocumento(pessoaTipo, CpfCnpj);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@NovoDestinatario = _validar.NovoDestinatario,
				@Destinatario = destinatario
			}, JsonRequestBehavior.AllowGet);
		}

        
        [Permite(RoleArray = new Object[] { ePermissao.PTVOutroCriar })]
        public ActionResult ObterDestinarioCodigoUc(decimal codigoUc)
        {
            DestinatarioPTV destinatario = _validar.ObterDestinarioCodigoUc(codigoUc);

            return Json(new
            {
                @EhValido = Validacao.EhValido,
                @Msg = Validacao.Erros,
                @Destinatario = destinatario
            }, JsonRequestBehavior.AllowGet);
        }

		#endregion

		#region Cancelar PTV

		[HttpGet]
		[Permite(RoleArray = new object[] { ePermissao.PTVOutroCancelar })]
		public ActionResult Cancelar(int id)
		{
			PTVOutro ptv = _bus.Obter(id, true);

			return View("Cancelar", ptv);
		}

		[HttpGet]
		[Permite(RoleArray = new object[] { ePermissao.PTVOutroCancelar })]
		public ActionResult CancelarConfirm(int id)
		{
			PTVOutro ptv = _bus.Obter(id, true);
			return PartialView("CancelarPartial", ptv);
		}

		[HttpPost]
		[Permite(RoleArray = new object[] { ePermissao.PTVOutroCancelar })]
		public ActionResult PTVCancelar(PTVOutro ptv)
		{
			_bus.PTVCancelar(ptv);
			return Json(new
			{
				@Url = Url.Action("Index", "PTVOutro") + "?Msg=" + Validacao.QueryParam(),
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Destinatario

		[Permite(RoleArray = new Object[] { ePermissao.PTVOutroCriar })]
		public ActionResult DestinatarioModal()
		{
			DestinatarioPTVVM vm = new DestinatarioPTVVM(new DestinatarioPTV(), ListaCredenciadoBus.Estados, new List<Municipio>());
			vm.Destinatario.PessoaTipo = PessoaTipo.FISICA;

			return PartialView("Destinatario/DestinatarioModal", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVOutroCriar })]
		public ActionResult DestinatarioSalvar(DestinatarioPTV destinatario)
		{
			DestinatarioPTVBus destinatarioPTVBus = new DestinatarioPTVBus();
			destinatarioPTVBus.Salvar(destinatario);

			return Json(new
			{
				@Valido = Validacao.EhValido,
				@Erros = Validacao.Erros,
				@Url = Url.Action("DestinatarioIndex", "PTV", new { Msg = Validacao.QueryParam() }),
				@ID = destinatario.ID
			}, JsonRequestBehavior.AllowGet);
		}

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.PTVOutroCriar })]
		public ActionResult VerificarDestinatarioCPFCNPJ(int pessoaTipo, string CPFCNPJ)
		{
			DestinatarioPTVValidar validar = new DestinatarioPTVValidar();
			validar.VerificarCPFCNPJ(pessoaTipo, CPFCNPJ);

			if (Validacao.EhValido)
			{
				Validacao.Add(pessoaTipo == PessoaTipo.FISICA ? Mensagem.DestinatarioPTV.CPFNaoAssociado : Mensagem.DestinatarioPTV.CNPJNaoAssociado);
			}

			return Json(new
			{
				@Valido = Validacao.EhValido,
				@Erros = Validacao.Erros
			});
		}

		#endregion

		#endregion
	}
}