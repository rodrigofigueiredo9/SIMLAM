using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.Destinatario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTVOutro;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPTV.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPTVOutro.Business;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMPTVOutro;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class PTVOutroController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		PTVOutroValidar _validar = new PTVOutroValidar();
		private PTVOutroBus _bus = new PTVOutroBus();

		string[] ListTipoOrigem = new[] 
		{
			Convert.ToString((int)eDocumentoFitossanitarioTipo.PTVOutroEstado),
			Convert.ToString((int)eDocumentoFitossanitarioTipo.CFCFR),
			Convert.ToString((int)eDocumentoFitossanitarioTipo.TF)
		};

		#endregion

		[Permite(RoleArray = new Object[] { ePermissao.PTVOutroListar })]
		public ActionResult Index()
		{
			PTVOutroListarVM vm = new PTVOutroListarVM(_busLista.PTVSituacao);
			return View("Index", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVOutroListar })]
		public ActionResult Filtrar(PTVOutroListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<PTVOutroListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(new ListaBus().QuantPaginacao, vm.Paginacao.QuantPaginacao);

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
				_busLista.PTVSituacao,
				_busLista.DocumentosFitossanitario.Where(x => ListTipoOrigem.Contains(x.Id)).ToList(),
				_busLista.Estados,
				new List<Municipio>(),
				_busLista.Estados,
				_busLista.Municipios(ptv.InteressadoMunicipioId));

			vm.LstUnidades = ViewModelHelper.CriarSelectList(_busLista.PTVUnidadeMedida);

			return View("Criar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PTVOutroCriar })]
		public ActionResult Salvar(PTVOutro ptv)
		{
			return Json(new
			{
				@EhValido = _bus.Salvar(ptv),
				@Erros = Validacao.Erros,
				@Url = Url.Action("Criar", "PTVOutro") + "?Msg=" + Validacao.QueryParam() + "&acaoId=" + ptv.Id.ToString()
			});
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVOutroVisualizar })]
		public ActionResult Visualizar(int id)
		{
			PTVOutro ptv = _bus.Obter(id);

			PTVOutroVM vm = new PTVOutroVM(
				ptv,
				_busLista.PTVSituacao,
				_busLista.DocumentosFitossanitario.Where(x => ListTipoOrigem.Contains(x.Id)).ToList(),
				_busLista.Estados,
				_busLista.Municipios(ptv.Estado),
				_busLista.Estados,
				_busLista.Municipios(ptv.InteressadoEstadoId));

			DestinatarioPTVBus _destinatarioBus = new DestinatarioPTVBus();
			vm.PTV.Destinatario = _destinatarioBus.Obter(ptv.DestinatarioID);
			vm.LstUnidades = ViewModelHelper.CriarSelectList(_busLista.PTVUnidadeMedida);
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
				@Municipios = _busLista.Municipios(estado)
			});
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVOutroCriar })]
		public ActionResult ValidarIdentificacaoProduto(PTVOutroProduto item, List<PTVOutroProduto> lista)
		{
			_validar.ValidarProduto(item, lista);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
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
	}
}