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
using Tecnomapas.Blocos.Entities.Interno.ModuloLocalVistoria;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.Destinatario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV.Pdf;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPTV;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class PTVController : DefaultController
	{
		#region Propriedades

		PTVValidar _validar = new PTVValidar();
		CredenciadoIntBus _busCredenciadoInterno = new CredenciadoIntBus();

		private DestinatarioPTVBus _bus = new DestinatarioPTVBus();
		private PTVBus _busPTV = new PTVBus();

		#endregion

		#region EPTV

		[Permite(RoleArray = new Object[] { ePermissao.PTVListar })]
		public ActionResult Index()
		{
			PTVListarVM vm = new PTVListarVM(ListaCredenciadoBus.PTVSolicitacaoSituacao, ListaCredenciadoBus.DocumentosFitossanitario);
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVListar })]
		public ActionResult Filtrar(PTVListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<PTVListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(ListaCredenciadoBus.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<PTVListarResultado> resultados = _busPTV.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeVisualizar = User.IsInRole(ePermissao.PTVVisualizar.ToString());
			vm.PodeEditar = User.IsInRole(ePermissao.PTVEditar.ToString());
			vm.PodeExcluir = User.IsInRole(ePermissao.PTVExcluir.ToString());
			vm.PodeGerarPDF = User.IsInRole(ePermissao.PTVListar.ToString());
			vm.PodeEnviar = User.IsInRole(ePermissao.PTVEnviar.ToString());
			vm.PodeSolicitarDesbloqueio = User.IsInRole(ePermissao.PTVSolicitarDesbloqueio.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new
			{
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm)
			}, JsonRequestBehavior.AllowGet);
		}

		#region Criar/Editar

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar })]
		public ActionResult Criar()
		{
			List<TratamentoFitossanitario> lsFitossanitario = null;
			EtramiteIdentity func = User.Identity as EtramiteIdentity;

			PTV ptv = new PTV();
			ptv.NumeroTipo = (int)eDocumentoFitossanitarioTipoNumero.Digital;
			ptv.PartidaLacradaOrigem = (int)ePartidaLacradaOrigem.Nao;
			ptv.Destinatario.PessoaTipo = (int)ePessoaTipo.Fisica;
			ptv.RotaTransitoDefinida = (int)eRotaTransitoDefinida.Sim;
			ptv.NotaFiscalApresentacao = (int)eApresentacaoNotaFiscal.Sim;
			ptv.Situacao = (int)eSolicitarPTVSituacao.Cadastrado;

			List<Setor> setores = _busPTV.SetoresLocalVistoria();

			List<Lista> _listaFitossanitario = ListaCredenciadoBus.DocumentosFitossanitario;

			PTVVM vm = new PTVVM(
				ptv,
				ListaCredenciadoBus.PTVSolicitacaoSituacao,
				new List<ListaValor>(),
				_listaFitossanitario,
				lsFitossanitario,
				new List<LaudoLaboratorial>(),
				_busPTV.ObterCultura(),
				ListaCredenciadoBus.TipoTransporte,
				ListaCredenciadoBus.Municipios(8), setores, false, new List<ListaValor>());

			vm.LstUnidades = ViewModelHelper.CriarSelectList(ListaCredenciadoBus.PTVUnidadeMedida);

			return View("Criar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVEditar })]
		public ActionResult Editar(int id)
		{
			PTV ptv = _busPTV.Obter(id);

			if (!_validar.ValidarSituacao(ptv))
			{
				return RedirectToAction("Index", "PTV", Validacao.QueryParamSerializer());
			}

			List<Setor> locaisVistorias = _busPTV.SetoresLocalVistoria();
			List<LaudoLaboratorial> lstLaboratorio = _busPTV.ObterLaudoLaboratorial(ptv.Produtos);
			List<TratamentoFitossanitario> lsFitossanitario = _busPTV.TratamentoFitossanitário(ptv.Produtos);

			if (lsFitossanitario != null && lsFitossanitario.Count > 5)
			{
				lsFitossanitario.RemoveAt(5);
				lsFitossanitario.RemoveAt(0);
			}

			PTVVM vm = new PTVVM(
				ptv,
				ListaCredenciadoBus.PTVSolicitacaoSituacao,
				_busPTV.ObterResponsaveisEmpreendimento(ptv.Empreendimento, ptv.Produtos),
				ListaCredenciadoBus.DocumentosFitossanitario,
				lsFitossanitario,
				lstLaboratorio,
				_busPTV.ObterCultura(),
				ListaCredenciadoBus.TipoTransporte,
				ListaCredenciadoBus.Municipios(8),
				locaisVistorias, false, _busPTV.DiasHorasVistoria(ptv.LocalVistoriaId, ptv.DataVistoria.AddDays(-1)));

			DestinatarioPTVBus _destinatarioBus = new DestinatarioPTVBus();
			vm.PTV.Destinatario = _destinatarioBus.Obter(ptv.DestinatarioID);
			vm.LstUnidades = ViewModelHelper.CriarSelectList(ListaCredenciadoBus.PTVUnidadeMedida);

			return View("Editar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
		public ActionResult Salvar(PTV ptv)
		{
			String acao = (ptv.Id > 0) ? "Index" : "Criar";

			return Json(new
			{
				@EhValido = _busPTV.Salvar(ptv),
				@Erros = Validacao.Erros,
				@Url = Url.Action(acao, "PTV") + "?Msg=" + Validacao.QueryParam() + "&acaoId=" + ptv.Id.ToString()
			});
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVListar })]
		public ActionResult Visualizar(int id)
		{
			PTV ptv = _busPTV.Obter(id);

			List<Setor> locaisVistorias = _busPTV.SetoresLocalVistoria();
			List<LaudoLaboratorial> lstLaboratorio = _busPTV.ObterLaudoLaboratorial(ptv.Produtos);
			List<TratamentoFitossanitario> lsFitossanitario = _busPTV.TratamentoFitossanitário(ptv.Produtos);

			if (lsFitossanitario != null && lsFitossanitario.Count > 5)
			{
				lsFitossanitario.RemoveAt(5);
				lsFitossanitario.RemoveAt(0);
			}

			PTVVM vm = new PTVVM(
				ptv,
				ListaCredenciadoBus.PTVSolicitacaoSituacao,
				_busPTV.ObterResponsaveisEmpreendimento(ptv.Empreendimento, ptv.Produtos),
				ListaCredenciadoBus.DocumentosFitossanitario,
				lsFitossanitario,
				lstLaboratorio,
				_busPTV.ObterCultura(),
				ListaCredenciadoBus.TipoTransporte,
				ListaCredenciadoBus.Municipios(8),
				locaisVistorias, true, _busPTV.DiasHorasVistoria(ptv.LocalVistoriaId, ptv.DataVistoria.AddDays(-1)));

			DestinatarioPTVBus _destinatarioBus = new DestinatarioPTVBus();
			vm.PTV.Destinatario = _destinatarioBus.Obter(ptv.DestinatarioID);
			vm.LstUnidades = ViewModelHelper.CriarSelectList(ListaCredenciadoBus.PTVUnidadeMedida);

			return View("Visualizar", vm);
		}

		#endregion Criar/Editar

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new object[] { ePermissao.PTVExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			PTV ptv = _busPTV.Obter(id, true);

			ConfirmarVM vm = new ConfirmarVM();
			vm.Id = ptv.Id;
			vm.Mensagem = Mensagem.PTV.MensagemExcluir(ptv.Numero.ToString());
			vm.Titulo = "Excluir EPTV";

			return PartialView("Confirmar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PTVExcluir })]
		public ActionResult Excluir(int id)
		{
			return Json(new
			{
				@EhValido = _busPTV.Excluir(id),
				@Msg = Validacao.Erros
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion Excluir

		#region Enviar

		[HttpGet]
		[Permite(RoleArray = new object[] { ePermissao.PTVEnviar })]
		public ActionResult EnviarConfirm(int id)
		{
			PTV ptv = _busPTV.Obter(id, true);

			ConfirmarVM vm = new ConfirmarVM();
			vm.Id = ptv.Id;
			vm.Mensagem = Mensagem.PTV.MensagemEnviar;
			vm.Titulo = "Enviar PTV";

			return PartialView("Confirmar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new object[] { ePermissao.PTVEnviar })]
		public ActionResult Enviar(PTV ptv)
		{
			_busPTV.Enviar(ptv.Id);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Historico

		[Permite(RoleArray = new Object[] { ePermissao.PTVListar })]
		public ActionResult Historico(int id)
		{
			PTVHistoricoVM vm = new PTVHistoricoVM(_busPTV.ObterHistoricoAnalise(id), ListaCredenciadoBus.PTVSolicitacaoSituacao);

			return PartialView("PTVHistoricoPartial", vm);
		}

		#endregion

		#region GerarPDF PTV

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.PTVListar })]
		public ActionResult GerarPdf(int id)
		{
			try
			{
				if (HttpContext.User != null && HttpContext.User.Identity.IsAuthenticated)
				{
					PdfEmissaoPTV pdf = new PdfEmissaoPTV();
					PTV PTV = _busPTV.Obter(id, simplificado: true);

					int situacaoId = PTV.Situacao;
					string situacaoTexto = PTV.SituacaoTexto;
					return ViewModelHelper.GerarArquivoPdf(pdf.Gerar(id, situacaoId, situacaoTexto), "PTV", dataHoraControleAcesso: true);
				}

				Validacao.Add(Mensagem.Funcionario.SemPermissao);
				return Redirect(FormsAuthentication.LoginUrl);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", "PTV", Validacao.QueryParamSerializer());
			}
		}

		#endregion

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar })]
		public ActionResult VerificarNumeroPTV(string numero, int tipoNumero)
		{
			_busPTV.VerificarNumeroPTV(numero, tipoNumero);
			return Json(new
			{
				@Valido = Validacao.EhValido,
				@Msg = Validacao.Erros
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
        public ActionResult VerificarDocumentoOrigem(int origemTipo, string origemTipoTexto, long numero, string serieNumeral)
		{
			Dictionary<string, object> dadosDocumentoOrigem = _busPTV.VerificarDocumentoOrigem((eDocumentoFitossanitarioTipo)origemTipo, origemTipoTexto, numero, serieNumeral);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Cultura = (List<Lista>)dadosDocumentoOrigem["listaCulturas"],
				@OrigemID = (int)dadosDocumentoOrigem["id"],
				@EmpreendimentoID = (int)dadosDocumentoOrigem["empreendimento_id"],
				@EmpreendimentoDenominador = dadosDocumentoOrigem["empreendimento_denominador"].ToString(),
                @DeclaracaoAdicional = dadosDocumentoOrigem["declaracao_adicional"].ToString(),
				@Msg = Validacao.Erros
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
		public ActionResult ObterResponsaveisEmpreendimento(int empreendimentoID, List<PTVProduto> produtos)
		{
			return Json(new
			{
				@Valido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Responsaveis = _busPTV.ObterResponsaveisEmpreendimento(empreendimentoID, produtos)
			},
			JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
		public ActionResult ObterNumeroOrigem(eDocumentoFitossanitarioTipo origemTipo, int empreendimentoID)
		{
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Erros = Validacao.Erros,
				@NumeroOrigem = _busPTV.ObterNumeroOrigem(origemTipo, empreendimentoID)
			},
			JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
		public ActionResult ObterCultura(int origemTipo, Int64 numeroOrigem)
		{
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Erros = Validacao.Erros,
				@Cultura = _busPTV.ObterCultura(origemTipo, numeroOrigem)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
		public ActionResult ObterCultivar(eDocumentoFitossanitarioTipo origemTipo, int origemID, int culturaID = 0)
		{
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Erros = Validacao.Erros,
				@Cultivar = _busPTV.ObterCultivar(origemTipo, origemID, culturaID)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
		public ActionResult ObterUnidadeMedida(eDocumentoFitossanitarioTipo origemTipo, int origemID, int culturaID, int cultivarID)
		{
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Erros = Validacao.Erros,
				@UnidadeMedida = _busPTV.ObterUnidadeMedida(origemTipo, origemID, culturaID, cultivarID)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar })]
		public ActionResult ValidarIdentificacaoProduto(PTVProduto item, DateTecno dataEmissao, List<PTVProduto> lista, int ptvID)
		{
			_validar.ValidarProduto(item, dataEmissao, lista, ptvID);

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar })]
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

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
		public ActionResult ObterDestinatario(int destinatarioId)
		{
			DestinatarioPTV destinatario = _busPTV.ObterDestinatario(destinatarioId);
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Destinatario = destinatario
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
		public ActionResult ObterLaboratorio(List<PTVProduto> lista)
		{
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Lista = _busPTV.ObterLaudoLaboratorial(lista)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
		public ActionResult ObterTratamentoFitossanitario(List<PTVProduto> lista)
		{
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@TratamentoFitossa = _busPTV.TratamentoFitossanitário(lista)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
		public ActionResult ObterItinerario(int destinatarioId)
		{
			DestinatarioPTV destinatario = _busPTV.ObterDestinatario(destinatarioId);
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Destinatario = destinatario
			});
		}

		#endregion

		#endregion

		#region Destinatario

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
		public ActionResult DestinatarioIndex()
		{
			DestinatarioPTVListarVM vm = new DestinatarioPTVListarVM(ListaCredenciadoBus.QuantPaginacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return View("Destinatario/Index", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
		public ActionResult DestinatarioFiltrar(DestinatarioPTVListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<DestinatarioPTVListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(ListaCredenciadoBus.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<DestinatarioListarResultado> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			//vm.PodeEditar = User.IsInRole(ePermissao.DestinatarioPTVEditar.ToString());
			//vm.PodeExcluir = User.IsInRole(ePermissao.DestinatarioPTVExcluir.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new
			{
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "Destinatario/ListarResultados", vm)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
		public ActionResult DestinatarioCriar()
		{
			DestinatarioPTVVM vm = new DestinatarioPTVVM(new DestinatarioPTV(), ListaCredenciadoBus.Estados, ListaCredenciadoBus.Municipios(8));
			vm.Destinatario.PessoaTipo = PessoaTipo.FISICA;

			return View("Destinatario/Criar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
		public ActionResult DestinatarioModal()
		{
			DestinatarioPTVVM vm = new DestinatarioPTVVM(new DestinatarioPTV(), ListaCredenciadoBus.Estados, ListaCredenciadoBus.Municipios(8));
			vm.Destinatario.PessoaTipo = PessoaTipo.FISICA;

			return View("Destinatario/DestinatarioModal", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
		public ActionResult DestinatarioEditar(int id)
		{
			DestinatarioPTV destinatario = _bus.Obter(id);
			DestinatarioPTVVM vm = new DestinatarioPTVVM(destinatario, ListaCredenciadoBus.Estados, ListaCredenciadoBus.Municipios(destinatario.EstadoID));

			return View("Destinatario/Editar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
		public ActionResult DestinatarioSalvar(DestinatarioPTV destinatario)
		{
			_bus.Salvar(destinatario);

			return Json(new
			{
				@Valido = Validacao.EhValido,
				@Erros = Validacao.Erros,
				@Url = Url.Action("DestinatarioIndex", "PTV", new { Msg = Validacao.QueryParam() }),
				@ID = destinatario.ID
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
		public ActionResult DestinatarioVisualizar(int id)
		{
			DestinatarioPTV destinatario = _bus.Obter(id);

			DestinatarioPTVVM vm = new DestinatarioPTVVM(destinatario, ListaCredenciadoBus.Estados, ListaCredenciadoBus.Municipios(destinatario.EstadoID), true);

			return View("Destinatario/Visualizar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
		public ActionResult DestinatarioExcluirConfirm(int id)
		{
			ConfirmarVM vm = new ConfirmarVM();

			DestinatarioPTV destinatario = _bus.Obter(id);
			vm.Id = destinatario.ID;
			vm.Mensagem = Mensagem.DestinatarioPTV.MensagemExcluir(destinatario.NomeRazaoSocial);
			vm.Titulo = "Excluir Destinatário";

			return PartialView("Confirmar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
		public ActionResult DestinatarioExcluir(int id)
		{
			_bus.Excluir(id);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
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

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
		public ActionResult ObterDiasHorasVistoria(int setor, DateTime? dataVistoria = null)
		{
			return Json(new
			{
				@Valido = Validacao.EhValido,
				@Erros = Validacao.Erros,
				@DiasHorasVistoria = _busPTV.DiasHorasVistoria(setor, dataVistoria)

			});
		}

		#endregion

		#endregion

		#region DUA

		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
		public ActionResult GravarVerificacaoDUA(string numero, string cpfCnpj, string tipo, int ptvId)
		{
			var filaID = _busPTV.GravarConsultaDUA(numero, cpfCnpj, tipo);

			return Json(new
			{
				@Valido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@FilaID = filaID
			}, JsonRequestBehavior.AllowGet);
		}


		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
		public ActionResult VerificarConsultaDUA(int filaID, string numero, string cpfCnpj, string tipo, int ptvId)
		{
			cpfCnpj = cpfCnpj.Replace(".", "").Replace("-", "").Replace("/", "");

            if (!_busPTV.VerificarSeDUAConsultada(filaID))
                return Json(new
                {
                    @Valido = Validacao.EhValido,
                    @Msg = Validacao.Erros,
                    @Consultado = false
                }, JsonRequestBehavior.AllowGet);

            _busPTV.VerificarDUA(filaID, numero, cpfCnpj, tipo, ptvId);

			return Json(new
			{
				@Valido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Consultado = true
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region PTVComunicador

		[Permite(RoleArray = new Object[] { ePermissao.PTVComunicador })]
		public ActionResult ValidarAcessoComunicador(int id)
		{
			_validar.ValidarAcessoComunicadorPTV(id);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVComunicador })]
		public ActionResult SolicitarDesbloqueio(int id)
		{
			PTVComunicadorVW vm = new PTVComunicadorVW();
			vm.Comunicador = _busPTV.ObterComunicador(id);
			vm.IsDesbloqueio = true;
			return PartialView("ComunicadorPTVPartial", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVComunicador })]
		public ActionResult ComunicadorPTV(int id)
		{
			PTVComunicadorVW vm = new PTVComunicadorVW();
			vm.Comunicador = _busPTV.ObterComunicador(id);
			return PartialView("ComunicadorPTVPartial", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVComunicador })]
		public ActionResult EnviarComunicadorPTV(PTVComunicador Comunicador)
		{
			_busPTV.SalvarConversa(Comunicador);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Url = Url.Action("ComunicadorPTV", "PTV") + "/" + Comunicador.PTVId,
				@Local = Comunicador
			}, JsonRequestBehavior.AllowGet);

		}
		#endregion
	}
}