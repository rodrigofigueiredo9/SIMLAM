using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFO.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloVegetal.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCFOCFOC.Pdf;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCFOCFOC.CFO;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class CFOController : DefaultController
	{
		#region Propriedades

		EmissaoCFOBus _bus = new EmissaoCFOBus();
		CredenciadoBus _credenciadoBus = new CredenciadoBus();
		EmissaoCFOValidar _validar = new EmissaoCFOValidar();

		#endregion

		#region Listar

		[Permite(RoleArray = new Object[] { ePermissao.CFOListar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(ListaCredenciadoBus.QuantPaginacao, ListaCredenciadoBus.DocFitossanitarioSituacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);

			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.CFOListar })]
		public ActionResult Filtrar(ListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Filtros.CredenciadoId = (User.Identity as EtramiteIdentity).FuncionarioId;
			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(ListaCredenciadoBus.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<EmissaoCFO> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeEditar = User.IsInRole(ePermissao.CFOEditar.ToString());
			vm.PodeVisualizar = User.IsInRole(ePermissao.CFOVisualizar.ToString());
			vm.PodeExcluir = User.IsInRole(ePermissao.CFOExcluir.ToString());
			vm.PodeGerarPDF = User.IsInRole(ePermissao.CFOListar.ToString());
			vm.PodeAtivar = User.IsInRole(ePermissao.CFOAlterarSituacao.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { Msg = Validacao.Erros, Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.CFOEmitir })]
		public ActionResult Criar()
		{
			if (!_validar.VerificarCredenciadoHabilitado())
			{
				return RedirectToAction("Index", "CFO", Validacao.QueryParamSerializer());
			}

			EmissaoCFO cfo = new EmissaoCFO();

			cfo.SituacaoId = (int)eDocumentoFitossanitarioSituacao.EmElaboracao;
			cfo.DataEmissao.DataTexto = DateTime.Today.ToShortDateString();
			cfo.EstadoEmissaoId = ViewModelHelper.EstadoDefaultId();

			CFOVM vm = new CFOVM(cfo, _bus.ObterProdutoresLista(), ListaCredenciadoBus.Estados, new List<Municipio>(), new List<Lista>(),
				new List<Lista>(), ListaCredenciadoBus.CFOProdutoEspecificacao, ListaCredenciadoBus.DocFitossanitarioSituacao, new List<Lista>(), ListaCredenciadoBus.Municipios(cfo.EstadoEmissaoId));

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CFOEmitir })]
		public ActionResult Criar(EmissaoCFO entidade)
		{
            entidade.PossuiLaudoLaboratorial = false;
            entidade.PossuiTratamentoFinsQuarentenario = false;
			_bus.Salvar(entidade);

			string UrlRedirecionar = Url.Action("Criar", "CFO") + "?Msg=" + Validacao.QueryParam() + "&acaoId=" + entidade.Id.ToString();
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @UrlRedirecionar = UrlRedirecionar }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.CFOEditar })]
		public ActionResult Editar(int id)
		{
			if (!_validar.VerificarCredenciadoHabilitado())
			{
				return RedirectToAction("Index", "CFO", Validacao.QueryParamSerializer());
			}

			EmissaoCFO cfo = _bus.Obter(id);

			if (!_validar.Editar(cfo))
			{
				return RedirectToAction("Index", "CFO", Validacao.QueryParamSerializer());
			}

			List<int> culturas = new List<int>();
			cfo.Produtos.Select(x => x.CulturaId).ToList().ForEach(cultura =>
			{
				culturas.Add(cultura);
			});

			CFOVM vm = new CFOVM(cfo, _bus.ObterProdutoresLista(), ListaCredenciadoBus.Estados, ListaCredenciadoBus.Municipios(cfo.EstadoId), _bus.ObterEmpreendimentosLista(cfo.ProdutorId), _bus.ObterPragasLista(cfo.Produtos),
			ListaCredenciadoBus.CFOProdutoEspecificacao, ListaCredenciadoBus.DocFitossanitarioSituacao, _bus.ObterUnidadesProducaoLista(cfo.EmpreendimentoId, cfo.ProdutorId), ListaCredenciadoBus.Municipios(cfo.EstadoEmissaoId));

			CulturaInternoBus culturaBus = new CulturaInternoBus();
			List<Cultivar> cultivares = culturaBus.ObterCultivares(cfo.Produtos.Select(x => x.CulturaId).ToList(), cfo.Produtos.Select(x => x.LoteId).ToList()) ?? new List<Cultivar>();
			List<string> declaracoesAdicionais = cultivares
				.Where(x => cfo.Produtos.Select(y => y.CultivarId).ToList().Any(y => y == x.Id))
				.SelectMany(x => x.LsCultivarConfiguracao.Where(y => cfo.Produtos.Count(z => z.CultivarId == y.Cultivar && y.TipoProducaoId == (int)ValidacoesGenericasBus.ObterTipoProducao(z.UnidadeMedidaId)) > 0))
				.Where(x => cfo.Pragas.Any(y => y.Id == x.PragaId))
				.Select(x => x.DeclaracaoAdicionalTextoHtml)
				.Distinct().ToList();

			vm.CFO.DeclaracaoAdicionalHtml = String.Join(" ", declaracoesAdicionais);

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CFOEditar })]
		public ActionResult Editar(EmissaoCFO entidade)
		{
			_bus.Salvar(entidade);

			string UrlRedirecionar = Url.Action("Index", "CFO") + "?Msg=" + Validacao.QueryParam() + "&acaoId=" + entidade.Id.ToString();
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @UrlRedirecionar = UrlRedirecionar }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.CFOVisualizar })]
		public ActionResult Visualizar(int id)
		{
			EmissaoCFO cfo = _bus.Obter(id);

			CFOVM vm = new CFOVM(cfo, _bus.ObterProdutoresLista(), ListaCredenciadoBus.Estados, ListaCredenciadoBus.Municipios(cfo.EstadoId), _bus.ObterEmpreendimentosLista(cfo.ProdutorId),
				new List<Lista>(), ListaCredenciadoBus.CFOProdutoEspecificacao, ListaCredenciadoBus.DocFitossanitarioSituacao, new List<Lista>(), ListaCredenciadoBus.Municipios(cfo.EstadoEmissaoId));

			vm.IsVisualizar = true;
			return View(vm);
		}

		#endregion

		#region Excluir

		[Permite(RoleArray = new Object[] { ePermissao.CFOExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ConfirmarVM vm = new ConfirmarVM();

			vm.Id = id;
			vm.Mensagem = Mensagem.EmissaoCFO.ExcluirMensagem;
			vm.Titulo = "Excluir Certificado Fitossanitário de Origem";

			return PartialView("Confirmar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CFOExcluir })]
		public ActionResult Excluir(int id)
		{
			_bus.Excluir(id);
			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Ativar

		[Permite(RoleArray = new Object[] { ePermissao.CFOAlterarSituacao })]
		public ActionResult AtivarConfirm(int id)
		{
			EmissaoCFO cfo = _bus.Obter(id, simplificado: true);
			cfo.DataAtivacao.Data = DateTime.Today;

			return PartialView("AtivarPartial", cfo);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CFOAlterarSituacao })]
		public ActionResult Ativar(EmissaoCFO entidade)
		{
			_bus.Ativar(entidade);

			string UrlRedirecionar = Url.Action("Index", "CFO") + "?Msg=" + Validacao.QueryParam() + "&acaoGerarPdfId=" + entidade.Id.ToString();
			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, @UrlRedirecionar = UrlRedirecionar, @Id = entidade.Id }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region PDF

		[Permite(RoleArray = new Object[] { ePermissao.CFOListar })]
		public ActionResult GerarPdf(int id)
		{
			try
			{
				if (HttpContext.User != null && HttpContext.User.Identity.IsAuthenticated)
				{
					EtramiteIdentity credenciado = User.Identity as EtramiteIdentity;

					PdfCFO pdf = new PdfCFO();
					return ViewModelHelper.GerarArquivoPdf(pdf.Gerar(id, credenciado.FuncionarioId), "CFO");
				}

				Validacao.Add(Mensagem.Funcionario.SemPermissao);
				return Redirect(FormsAuthentication.LoginUrl);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", "CFO", Validacao.QueryParamSerializer());
			}
		}

		#endregion

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.CFOEmitir, ePermissao.CFOEditar })]
		public ActionResult VerificarNumero(string numero, int tipoNumero)
		{
			numero = _bus.VerificarNumero(numero, tipoNumero);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Numero = numero
			});
		}

		[Permite(RoleArray = new Object[] { ePermissao.CFOEmitir, ePermissao.CFOEditar })]
		public ActionResult ObterEmpreendimentos(int produtorId)
		{
			List<Lista> lista = _bus.ObterEmpreendimentosLista(produtorId);

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Lista = lista });
		}

		[Permite(RoleArray = new Object[] { ePermissao.CFOEmitir, ePermissao.CFOEditar })]
		public ActionResult ObterUPs(int empreendimentoId, int produtorId)
		{
			List<Lista> listaUP = _bus.ObterUnidadesProducaoLista(empreendimentoId, produtorId);

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Lista = listaUP });
		}

		[Permite(RoleArray = new Object[] { ePermissao.CFOEmitir, ePermissao.CFOEditar })]
		public ActionResult ObterCulturaUP(int unidadeProducaoId)
		{
			Cultivar cultivar = _bus.ObterCulturaUP(unidadeProducaoId);

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Cultivar = cultivar });
		}

		[Permite(RoleArray = new Object[] { ePermissao.CFOEmitir, ePermissao.CFOEditar })]
		public ActionResult ObterPragas(List<IdentificacaoProduto> produtos)
		{
			if (produtos == null || produtos.Count <= 0)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Lista = new List<Lista>() });
			}

			List<Lista> lista = _bus.ObterPragasLista(produtos);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Lista = lista });
		}

		[Permite(RoleArray = new Object[] { ePermissao.CFOEmitir, ePermissao.CFOEditar })]
		public ActionResult ValidarIdentificacaoProduto(int cfo, int empreendimento, IdentificacaoProduto item, List<IdentificacaoProduto> lista)
		{
			_validar.ValidarProduto(cfo, empreendimento, item, lista);

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

		[Permite(RoleArray = new Object[] { ePermissao.CFOEmitir, ePermissao.CFOEditar })]
		public ActionResult ValidarPraga(Praga item, List<Praga> lista)
		{
			_validar.ValidarPraga(item, lista);

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

		[Permite(RoleArray = new Object[] { ePermissao.CFOEmitir, ePermissao.CFOEditar })]
		public ActionResult ValidarTratamentoFitossanitario(TratamentoFitossanitario item, List<TratamentoFitossanitario> lista)
		{
			_validar.ValidarTratamento(item, lista);

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

		[Permite(RoleArray = new Object[] { ePermissao.CFOEmitir })]
		public ActionResult VerificarCredenciadoHabilitado(EmissaoCFO entidade)
		{
			_validar.ValidarAssociarResponsavelTecnicoHabilitado(entidade);

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.CFOEmitir, ePermissao.CFOEditar })]
		public ActionResult ObterDeclaracaoAdicional(List<IdentificacaoProduto> produtos, List<Praga> pragas)
		{
			if (produtos == null || produtos.Count <= 0 || pragas == null || pragas.Count <= 0)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @DeclaracoesAdicionais = string.Empty });
			}

			CulturaInternoBus culturaBus = new CulturaInternoBus();
			List<Cultivar> cultivares = culturaBus.ObterCultivares(produtos.Select(x => x.CulturaId).ToList()) ?? new List<Cultivar>();

			List<string> declaracoesAdicionais = cultivares
			.Where(x => produtos.Select(y => y.CultivarId).ToList().Any(y => y == x.Id))
			.SelectMany(x => x.LsCultivarConfiguracao.Where(y => produtos.Count(z => z.CultivarId == y.Cultivar && y.TipoProducaoId == (int)ValidacoesGenericasBus.ObterTipoProducao(z.UnidadeMedidaId)) > 0))
			.Where(x => pragas.Any(y => y.Id == x.PragaId))
			.Select(x => x.DeclaracaoAdicionalTextoHtml)
			.Distinct().ToList();

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @DeclaracoesAdicionais = String.Join(" ", declaracoesAdicionais) });
		}

		#endregion Auxiliares
	}
}