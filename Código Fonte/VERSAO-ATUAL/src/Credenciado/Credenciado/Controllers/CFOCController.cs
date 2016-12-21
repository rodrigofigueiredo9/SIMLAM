using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC.Lote;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCFOCFOC.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFOC.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloVegetal.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCFOCFOC.Pdf;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCFOCFOC.CFOC;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class CFOCController : DefaultController
	{
		#region Propriedades

		EmissaoCFOCBus _bus = new EmissaoCFOCBus();
		CredenciadoBus _credenciadoBus = new CredenciadoBus();
		EmissaoCFOCValidar _validar = new EmissaoCFOCValidar();

		LoteBus _loteBus = new LoteBus();
		LoteValidar _loteValidar = new LoteValidar();

		#endregion

		#region Listar

		[Permite(RoleArray = new Object[] { ePermissao.CFOCListar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(ListaCredenciadoBus.QuantPaginacao, ListaCredenciadoBus.DocFitossanitarioSituacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);

			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.CFOCListar })]
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

			Resultados<EmissaoCFOC> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeEditar = User.IsInRole(ePermissao.CFOCEditar.ToString());
			vm.PodeVisualizar = User.IsInRole(ePermissao.CFOCVisualizar.ToString());
			vm.PodeExcluir = User.IsInRole(ePermissao.CFOCExcluir.ToString());
			vm.PodeGerarPDF = User.IsInRole(ePermissao.CFOCListar.ToString());
			vm.PodeAtivar = User.IsInRole(ePermissao.CFOCAlterarSituacao.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { Msg = Validacao.Erros, Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.CFOCEmitir })]
		public ActionResult Criar()
		{
			if (!_validar.VerificarCredenciadoHabilitado())
			{
				return RedirectToAction("Index", "CFOC", Validacao.QueryParamSerializer());
			}

			EmissaoCFOC entidade = new EmissaoCFOC();

			entidade.SituacaoId = (int)eDocumentoFitossanitarioSituacao.EmElaboracao;
			entidade.DataEmissao.DataTexto = DateTime.Today.ToShortDateString();
			entidade.EstadoEmissaoId = ViewModelHelper.EstadoDefaultId();

			CFOCVM vm = new CFOCVM(entidade, ListaCredenciadoBus.Estados, new List<Municipio>(), _bus.ObterEmpreendimentosLista(),
				new List<Lista>(), ListaCredenciadoBus.CFOCLoteEspecificacao, ListaCredenciadoBus.DocFitossanitarioSituacao, ListaCredenciadoBus.Municipios(entidade.EstadoEmissaoId));

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CFOCEmitir })]
		public ActionResult Criar(EmissaoCFOC entidade)
		{
            entidade.PossuiLaudoLaboratorial = false;
            entidade.PossuiTratamentoFinsQuarentenario = false;

			_bus.Salvar(entidade);

			string UrlRedirecionar = Url.Action("Criar", "CFOC") + "?Msg=" + Validacao.QueryParam() + "&acaoId=" + entidade.Id.ToString();
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @UrlRedirecionar = UrlRedirecionar }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.CFOCEditar })]
		public ActionResult Editar(int id)
		{
			if (!_validar.VerificarCredenciadoHabilitado())
			{
				return RedirectToAction("Index", "CFOC", Validacao.QueryParamSerializer());
			}

			EmissaoCFOC entidade = _bus.Obter(id);

			if (!_validar.Editar(entidade))
			{
				return RedirectToAction("Index", "CFOC", Validacao.QueryParamSerializer());
			}

			List<int> culturas = new List<int>();
			entidade.Produtos.Select(x => x.CulturaId).ToList().ForEach(cultura =>
			{
				culturas.Add(cultura);
			});

			CFOCVM vm = new CFOCVM(entidade, ListaCredenciadoBus.Estados, ListaCredenciadoBus.Municipios(entidade.EstadoId), _bus.ObterEmpreendimentosLista(),
				_bus.ObterPragasLista(entidade.Produtos), ListaCredenciadoBus.CFOCLoteEspecificacao, ListaCredenciadoBus.DocFitossanitarioSituacao, ListaCredenciadoBus.Municipios(entidade.EstadoEmissaoId));

			CulturaInternoBus culturaBus = new CulturaInternoBus();
			List<Cultivar> cultivares = culturaBus.ObterCultivares(entidade.Produtos.Select(x => x.CulturaId).ToList(), entidade.Produtos.Select(z => z.LoteId).ToList()) ?? new List<Cultivar>();
			List<string> declaracoesAdicionais = cultivares
				.Where(x => entidade.Produtos.Select(y => y.CultivarId).ToList().Any(y => y == x.Id))
				.SelectMany(x => x.LsCultivarConfiguracao.Where(y => entidade.Produtos.Count(z => z.CultivarId == y.Cultivar /*&& y.TipoProducaoId == (int)ValidacoesGenericasBus.ObterTipoProducao(z.UnidadeMedidaId)*/) > 0))
				.Where(x => entidade.Pragas.Any(y => y.Id == x.PragaId))
				.Select(x => x.DeclaracaoAdicionalTextoHtml)
				.Distinct().ToList();

			vm.CFOC.DeclaracaoAdicionalHtml = String.Join(" ", declaracoesAdicionais);

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CFOCEditar })]
		public ActionResult Editar(EmissaoCFOC entidade)
		{
			_bus.Salvar(entidade);

			string UrlRedirecionar = Url.Action("Index", "CFOC") + "?Msg=" + Validacao.QueryParam() + "&acaoId=" + entidade.Id.ToString();
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @UrlRedirecionar = UrlRedirecionar }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.CFOCVisualizar })]
		public ActionResult Visualizar(int id)
		{
			EmissaoCFOC entidade = _bus.Obter(id);

			CFOCVM vm = new CFOCVM(entidade, ListaCredenciadoBus.Estados, ListaCredenciadoBus.Municipios(entidade.EstadoId), _bus.ObterEmpreendimentosLista(),
				new List<Lista>(), ListaCredenciadoBus.CFOCLoteEspecificacao, ListaCredenciadoBus.DocFitossanitarioSituacao, ListaCredenciadoBus.Municipios(entidade.EstadoEmissaoId));

			vm.IsVisualizar = true;
			return View(vm);
		}

		#endregion

		#region Excluir

		[Permite(RoleArray = new Object[] { ePermissao.CFOCExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ConfirmarVM vm = new ConfirmarVM();

			vm.Id = id;
			vm.Mensagem = Mensagem.EmissaoCFOC.ExcluirMensagem;
			vm.Titulo = "Excluir Certificado Fitossanitário de Origem Consolidado";

			return PartialView("Confirmar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CFOCExcluir })]
		public ActionResult Excluir(int id)
		{
			_bus.Excluir(id);
			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Ativar

		[Permite(RoleArray = new Object[] { ePermissao.CFOCAlterarSituacao })]
		public ActionResult AtivarConfirm(int id)
		{
			EmissaoCFOC entidade = _bus.Obter(id, simplificado: true);
			entidade.DataAtivacao.Data = DateTime.Today;

			return PartialView("AtivarPartial", entidade);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CFOCAlterarSituacao })]
		public ActionResult Ativar(EmissaoCFOC entidade)
		{
			_bus.Ativar(entidade);

			string UrlRedirecionar = Url.Action("Index", "CFOC") + "?Msg=" + Validacao.QueryParam() + "&acaoGerarPdfId=" + entidade.Id.ToString();
			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, @UrlRedirecionar = UrlRedirecionar, @Id = entidade.Id }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region PDF

		[Permite(RoleArray = new Object[] { ePermissao.CFOCListar })]
		public ActionResult GerarPdf(int id)
		{
			try
			{
				if (HttpContext.User != null && HttpContext.User.Identity.IsAuthenticated)
				{
					EtramiteIdentity credenciado = User.Identity as EtramiteIdentity;

					PdfCFOC pdf = new PdfCFOC();
					return ViewModelHelper.GerarArquivoPdf(pdf.Gerar(id, credenciado.FuncionarioId), "CFOC");
				}

				Validacao.Add(Mensagem.Funcionario.SemPermissao);
				return Redirect(FormsAuthentication.LoginUrl);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", "CFOC", Validacao.QueryParamSerializer());
			}
		}

		#endregion

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.CFOCEmitir, ePermissao.CFOCEditar })]
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

		[Permite(RoleArray = new Object[] { ePermissao.CFOCEmitir, ePermissao.CFOCEditar })]
		public ActionResult ObterPragas(List<IdentificacaoProduto> produtos)
		{
			if (produtos == null || produtos.Count <= 0)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Lista = new List<Lista>() });
			}

			List<Lista> lista = _bus.ObterPragasLista(produtos);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Lista = lista });
		}

		[Permite(RoleArray = new Object[] { ePermissao.CFOCEmitir, ePermissao.CFOCEditar })]
		public ActionResult ValidarIdentificacaoProduto(int cfoc, int empreendimento, IdentificacaoProduto item, List<IdentificacaoProduto> lista)
		{
			_validar.ValidarProduto(cfoc, empreendimento, item, lista);

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

		[Permite(RoleArray = new Object[] { ePermissao.CFOCEmitir, ePermissao.CFOCEditar })]
		public ActionResult ValidarPraga(Praga item, List<Praga> lista)
		{
			_validar.ValidarPraga(item, lista);

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

		[Permite(RoleArray = new Object[] { ePermissao.CFOCEmitir, ePermissao.CFOCEditar })]
		public ActionResult ValidarTratamentoFitossanitario(TratamentoFitossanitario item, List<TratamentoFitossanitario> lista)
		{
			_validar.ValidarTratamento(item, lista);

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

		[Permite(RoleArray = new Object[] { ePermissao.CFOCEmitir })]
		public ActionResult VerificarCredenciadoHabilitado(EmissaoCFOC entidade)
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
			List<Cultivar> cultivares = culturaBus.ObterCultivares(produtos.Select(x => x.CulturaId).ToList(), produtos.Select(y => y.LoteId).ToList()) ?? new List<Cultivar>();

			List<string> declaracoesAdicionais = cultivares
				.Where(x => produtos.Select(y => y.CultivarId).ToList().Any(y => y == x.Id))
				.SelectMany(x => x.LsCultivarConfiguracao.Where(y => produtos.Count(z => z.CultivarId == y.Cultivar && y.TipoProducaoId == (int)ValidacoesGenericasBus.ObterTipoProducao(z.UnidadeMedidaId)) > 0))
				.Where(x => pragas.Any(y => y.Id == x.PragaId))
				.Select(x => x.DeclaracaoAdicionalTextoHtml)
				.Distinct().ToList();

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @DeclaracoesAdicionais = String.Join(" ", declaracoesAdicionais) });
		}

		#endregion Auxiliares

		#region Lote

		#region Listar

		[Permite(RoleArray = new Object[] { ePermissao.LoteListar })]
		public ActionResult LoteIndex()
		{
			LoteListarVM vm = new LoteListarVM(ListaCredenciadoBus.QuantPaginacao, ListaCredenciadoBus.LoteSituacoes);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);

			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.LoteListar })]
		[HttpPost]
		public ActionResult LoteFiltrar(LoteListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<LoteListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(ListaCredenciadoBus.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			if (vm.PodeAssociar)
			{
				vm.Filtros.SituacaoId = (int)eLoteSituacao.NaoUtilizado;
			}

			Resultados<Lote> resultados = _loteBus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			if (!vm.PodeAssociar)
			{
				vm.PodeEditar = User.IsInRole(ePermissao.LoteEditar.ToString());
				vm.PodeExcluir = User.IsInRole(ePermissao.LoteExcluir.ToString());
			}

			vm.PodeVisualizar = User.IsInRole(ePermissao.LoteVisualizar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { Msg = Validacao.Erros, Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "LoteListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.LoteListar })]
		public ActionResult LoteAssociar(int empreendimento = 0)
		{
			LoteListarVM vm = new LoteListarVM(ListaCredenciadoBus.QuantPaginacao, ListaCredenciadoBus.LoteSituacoes.Where(x => Convert.ToInt32(x.Id) == (int)eLoteSituacao.NaoUtilizado).ToList());
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.PodeAssociar = true;
			vm.Filtros.EmpreendimentoId = empreendimento;

			return PartialView("LoteListarFiltros", vm);
		}

		#endregion

		#region Criar/Editar

		[Permite(RoleArray = new Object[] { ePermissao.LoteCriar })]
		public ActionResult LoteCriar()
		{
			Lote lote = new Lote();
			lote.DataCriacao.DataTexto = DateTime.Today.ToShortDateString();
			lote.Ano = Convert.ToInt16(DateTime.Today.Year.ToString().Substring(2));

			List<Lista> origemTipo = new List<Lista> {
				new Lista{ IsAtivo=true, Id="5", Texto= "CF/CFR"},
				new Lista{ IsAtivo=true, Id="1", Texto= "CFO"},
				new Lista{ IsAtivo=true, Id="2", Texto= "CFOC"},
				new Lista{ IsAtivo=true, Id="4", Texto= "PTV de outro Estado"},
				new Lista{ IsAtivo=true, Id="6", Texto= "TF"}
			};

			//List<Lista> origemTipo = ListaCredenciadoBus.DocumentosFitossanitario.Where(x =>
			//	x.Id != ((int)eDocumentoFitossanitarioTipo.PTV).ToString()).ToList();
			
			if (!_loteValidar.AcessarTela(lote))
			{
				return RedirectToAction("LoteIndex", "CFOC", Validacao.QueryParamSerializer());
			}

			LoteVM vm = new LoteVM(_loteBus.ObterEmpreendimentosResponsaveis(), origemTipo, lote);
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.LoteEditar })]
		public ActionResult LoteEditar(int id)
		{
			Lote lote = _loteBus.Obter(id);
			List<Lista> origemTipo = new List<Lista> {
				new Lista{ IsAtivo=true, Id="5", Texto= "CF/CFR"},
				new Lista{ IsAtivo=true, Id="1", Texto= "CFO"},
				new Lista{ IsAtivo=true, Id="2", Texto= "CFOC"},
				new Lista{ IsAtivo=true, Id="4", Texto= "PTV de outro Estado"},
				new Lista{ IsAtivo=true, Id="6", Texto= "TF"}
			};

			//List<Lista> origemTipo = ListaCredenciadoBus.DocumentosFitossanitario.Where(x =>
			//	x.Id != ((int)eDocumentoFitossanitarioTipo.PTV).ToString()).ToList();

			if (!_loteValidar.AcessarTela(lote) || !_loteValidar.Editar(lote))
			{
				return RedirectToAction("LoteIndex", "CFOC", Validacao.QueryParamSerializer());
			}

			LoteVM vm = new LoteVM(_loteBus.ObterEmpreendimentosResponsaveis(), origemTipo, lote);
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.LoteCriar, ePermissao.LoteEditar })]
		public ActionResult LoteSalvar(Lote lote)
		{
			bool editar = lote.Id > 0;

			_loteBus.Salvar(lote);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Url = (editar ? Url.Action("LoteIndex", "CFOC") : Url.Action("LoteCriar", "CFOC")) + "?Msg=" + Validacao.QueryParam()
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion Criar/Editar

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.LoteCriar, ePermissao.LoteEditar })]
		public ActionResult ObterCodigoCaracterizacaoUC(int empreendimentoID)
		{
			return Json(new
			{
				@Msg = Validacao.Erros,
				@EhValido = Validacao.EhValido,
				@Caracterizacao = _loteBus.ObterCodigoUC(empreendimentoID)
			});
		}

		[Permite(RoleArray = new Object[] { ePermissao.LoteCriar, ePermissao.LoteEditar })]
		public ActionResult LoteVerificarNumero(int origemTipo, string origemNumero)
		{
			int origemID = 0;
			List<IdentificacaoProduto> produtos = _loteValidar.OrigemNumero(origemNumero, origemTipo, out origemID);
			List<Lista> culturas = new List<Lista>();
			string selecionado = "0";

			if (produtos != null)
			{
				produtos.ForEach(produto =>
				{
					if (!culturas.Any(x => x.Id == produto.CulturaId.ToString()))
					{
						culturas.Add(new Lista() { Id = produto.CulturaId.ToString(), Texto = produto.CulturaTexto });
					}
				});
			}

			if (culturas.Count == 1)
			{
				selecionado = culturas.First().Id;
			}

			return Json(new
			{
				@Msg = Validacao.Erros,
				@EhValido = Validacao.EhValido,
				@OrigemID = origemID,
				@Culturas = culturas
			});
		}

		[Permite(RoleArray = new Object[] { ePermissao.LoteCriar, ePermissao.LoteEditar })]
		public ActionResult ObterCultivar(int origemTipo, int origemID, int culturaID)
		{
			return Json(new
			{
				@Msg = Validacao.Erros,
				@EhValido = Validacao.EhValido,
				@Cultivar = _loteBus.ObterCultivar(origemTipo, origemID, culturaID)
			});
		}

		[Permite(RoleArray = new Object[] { ePermissao.LoteCriar, ePermissao.LoteEditar })]
		public ActionResult ObterUnidadeMedida(int origemTipo, int origemID, int culturaID, int cultivarID)
		{
			return Json(new
			{
				@Msg = Validacao.Erros,
				@EhValido = Validacao.EhValido,
				@Lista = _loteBus.ObterUnidadeMedida(origemTipo, origemID, culturaID, cultivarID)
			});
		}

		[Permite(RoleArray = new Object[] { ePermissao.LoteCriar, ePermissao.LoteEditar })]
		public ActionResult LoteValidarItem(LoteItem item, DateTecno dataCriacao, int empreendimentoID, List<LoteItem> lista, int loteID)
		{
			_loteValidar.Lote(item, dataCriacao, empreendimentoID, lista, loteID);
			return Json(new
			{
				@Msg = Validacao.Erros,
				@EhValido = Validacao.EhValido
			});
		}

		#endregion Auxiliares

		#region Excluir

		[Permite(RoleArray = new Object[] { ePermissao.LoteExcluir })]
		public ActionResult LoteExcluirConfirm(int id)
		{
			ConfirmarVM vm = new ConfirmarVM();

			vm.Id = id;
			vm.Mensagem = Mensagem.Lote.ExcluirMensagem;
			vm.Titulo = "Excluir Lote";

			return PartialView("Confirmar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.LoteExcluir })]
		public ActionResult LoteExcluir(int id)
		{
			_loteBus.Excluir(id);
			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar Lote

		[Permite(RoleArray = new Object[] { ePermissao.LoteVisualizar })]
		public ActionResult LoteVisualizar(int id)
		{
			Lote lote = _loteBus.Obter(id);
			List<Lista> origemTipo = ListaCredenciadoBus.DocumentosFitossanitario.Where(x => x.Id == ((int)eDocumentoFitossanitarioTipo.CFO).ToString() || x.Id == ((int)eDocumentoFitossanitarioTipo.CFOC).ToString()).ToList();

			if (!_loteValidar.AcessarTela(lote))
			{
				return RedirectToAction("LoteIndexx", "CFOC");
			}

			LoteVM vm = new LoteVM(_loteBus.ObterEmpreendimentosResponsaveis(), origemTipo, lote);
			vm.IsVisualizar = true;

			if (Request.IsAjaxRequest())
			{
				return PartialView("LotePartial", vm);
			}
			else
			{
				return View(vm);
			}
		}

		#endregion

		#endregion Lote
	}
}