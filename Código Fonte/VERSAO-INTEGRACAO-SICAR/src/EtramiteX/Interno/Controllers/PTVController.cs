﻿using System;
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
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.Destinatario;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPTV.Business;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class PTVController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		PTVValidar _validar = new PTVValidar();
		CredenciadoIntBus _busCredenciadoInterno = new CredenciadoIntBus();

		private DestinatarioPTVBus _bus = new DestinatarioPTVBus();
		private PTVBus _busPTV = new PTVBus();

		#endregion

		#region PTV

		[Permite(RoleArray = new Object[] { ePermissao.PTVListar })]
		public ActionResult Index()
		{
			PTVListarVM vm = new PTVListarVM(_busLista.PTVSituacao);
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
			vm.SetListItens(new ListaBus().QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<PTVListarResultado> resultados = _busPTV.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeVisualizar = User.IsInRole(ePermissao.PTVVisualizar.ToString());
			vm.PodeEditar = User.IsInRole(ePermissao.PTVEditar.ToString());
			vm.PodeExcluir = User.IsInRole(ePermissao.PTVExcluir.ToString());
			vm.PodeGerarPDF = User.IsInRole(ePermissao.PTVListar.ToString());
			vm.PodeAtivar = User.IsInRole(ePermissao.PTVAtivar.ToString());
			vm.PodeCancelar = User.IsInRole(ePermissao.PTVCancelar.ToString());


			EtramiteIdentity func = User.Identity as EtramiteIdentity ?? new EtramiteIdentity("", "", "", null, "", 0, 0, "", "", 0, 0);
			_busPTV.ObterResponsavelTecnico(func.UsuarioId).ForEach(x => { vm.RT = x.Id; });

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
		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar })]
		public ActionResult Criar()
		{
			if (!_validar.FuncionarioHabilitadoValido())
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			List<TratamentoFitossanitario> lsFitossanitario = null;
			EtramiteIdentity func = User.Identity as EtramiteIdentity;
			List<ListaValor> LsSetor = _busPTV.ObterLocalEmissao(func.UsuarioId);

			PTV ptv = new PTV();
			ptv.NumeroTipo = (int)eDocumentoFitossanitarioTipoNumero.Digital;
			ptv.PartidaLacradaOrigem = (int)ePartidaLacradaOrigem.Nao;
			ptv.Destinatario.PessoaTipo = (int)ePessoaTipo.Fisica;
			ptv.RotaTransitoDefinida = (int)eRotaTransitoDefinida.Sim;
			ptv.NotaFiscalApresentacao = (int)eApresentacaoNotaFiscal.Sim;
			ptv.Situacao = (int)ePTVSituacao.EmElaboracao;

			_busPTV.ObterResponsavelTecnico(func.UsuarioId).ForEach(x => { ptv.ResponsavelTecnicoId = x.Id; ptv.ResponsavelTecnicoNome = x.Texto; });

			PTVVM vm = new PTVVM(
				ptv,
				_busLista.PTVSituacao.Where(x => Convert.ToInt32(x.Id) != (int)eDocumentoFitossanitarioSituacao.Cancelado).ToList(),
				new List<ListaValor>(),
				_busLista.DocumentosFitossanitario,
				lsFitossanitario,
				new List<LaudoLaboratorial>(),
				_busPTV.ObterCultura(),
				_busLista.TipoTransporte,
				_busLista.Municipios(8), LsSetor);

			vm.LstUnidades = ViewModelHelper.CriarSelectList(_busLista.PTVUnidadeMedida);

			return View("Criar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVEditar })]
		public ActionResult Editar(int id)
		{
			if (!_validar.FuncionarioHabilitadoValido())
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			PTV ptv = _busPTV.Obter(id);

			if (!_validar.ValidarSituacao(ptv))
			{
				return RedirectToAction("Index", "PTV", Validacao.QueryParamSerializer());
			}

			List<TratamentoFitossanitario> lsFitossanitario = _busPTV.TratamentoFitossanitário(ptv.Produtos);
			List<LaudoLaboratorial> lstLaboratorio = _busPTV.ObterLaudoLaboratorial(ptv.Produtos);

			PTVVM vm = new PTVVM(
				ptv,
				_busLista.PTVSituacao,
				_busPTV.ObterResponsaveisEmpreendimento(ptv.Empreendimento, ptv.Produtos),
				_busLista.DocumentosFitossanitario,
				lsFitossanitario,
				lstLaboratorio,
				_busPTV.ObterCultura(),
				_busLista.TipoTransporte,
				_busLista.Municipios(8),
				new List<ListaValor>());

			DestinatarioPTVBus _destinatarioBus = new DestinatarioPTVBus();
			vm.PTV.Destinatario = _destinatarioBus.Obter(ptv.DestinatarioID);
			vm.LstUnidades = ViewModelHelper.CriarSelectList(_busLista.PTVUnidadeMedida);

			return View("Editar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVVisualizar })]
		public ActionResult Visualizar(int id)
		{
			EtramiteIdentity func = User.Identity as EtramiteIdentity ?? new EtramiteIdentity("", "", "", null, "", 0, 0, "", "", 0, 0);

			PTV ptv = _busPTV.Obter(id);

			List<TratamentoFitossanitario> lsFitossanitario = _busPTV.TratamentoFitossanitário(ptv.Produtos);
			List<LaudoLaboratorial> lstLaboratorio = _busPTV.ObterLaudoLaboratorial(ptv.Produtos);

			PTVVM vm = new PTVVM(
				ptv,
				_busLista.PTVSituacao,
				_busPTV.ObterResponsaveisEmpreendimento(ptv.Empreendimento, ptv.Produtos),
				_busLista.DocumentosFitossanitario,
				lsFitossanitario,
				lstLaboratorio,
				_busPTV.ObterCultura(),
				_busLista.TipoTransporte,
				_busLista.Municipios(8),
				new List<ListaValor>());

			DestinatarioPTVBus _destinatarioBus = new DestinatarioPTVBus();
			vm.PTV.Destinatario = _destinatarioBus.Obter(ptv.DestinatarioID);
			vm.LstUnidades = ViewModelHelper.CriarSelectList(_busLista.PTVUnidadeMedida);
			vm.IsVisualizar = true;
			return View("Visualizar", vm);
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

		[HttpGet]
		[Permite(RoleArray = new object[] { ePermissao.PTVExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			if (!_validar.FuncionarioHabilitadoValido())
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			PTV ptv = _busPTV.Obter(id, true);

			ConfirmarVM vm = new ConfirmarVM();
			vm.Id = ptv.Id;
			vm.Mensagem = Mensagem.PTV.MensagemExcluir(ptv.Numero.ToString());
			vm.Titulo = "Excluir PTV";

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
		public ActionResult VerificarDocumentoOrigem(int origemTipo, string origemTipoTexto, long numero)
		{
			Dictionary<string, object> dadosDocumentoOrigem = _busPTV.VerificarDocumentoOrigem((eDocumentoFitossanitarioTipo)origemTipo, origemTipoTexto, numero);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Cultura = (List<Lista>)dadosDocumentoOrigem["listaCulturas"],
				@OrigemID = (int)dadosDocumentoOrigem["id"],
				@EmpreendimentoID = (int)dadosDocumentoOrigem["empreendimento_id"],
				@EmpreendimentoDenominador = dadosDocumentoOrigem["empreendimento_denominador"].ToString(),
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
			//int destinatarioID = _validar.ValidarDocumento(pessoaTipo, CpfCnpj);
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

		#region Ativar/Cancelar PTV

		[HttpGet]
		[Permite(RoleArray = new object[] { ePermissao.PTVAtivar })]
		public ActionResult Ativar(int id)
		{
			if (!_validar.FuncionarioHabilitadoValido())
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			PTV ptv = _busPTV.Obter(id, true);
			ptv.DataAtivacao.Data = DateTime.Today;

			return View("Ativar", ptv);
		}

		[HttpGet]
		[Permite(RoleArray = new object[] { ePermissao.PTVAtivar })]
		public ActionResult AtivarConfirm(int id, bool tela = false)
		{
			if (!_validar.FuncionarioHabilitadoValido())
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			PTV ptv = _busPTV.Obter(id, true);

			return PartialView("AtivarPartial", ptv);
		}

		[HttpPost]
		[Permite(RoleArray = new object[] { ePermissao.PTVAtivar })]
		public ActionResult Ativar(PTV ptv)
		{
			_busPTV.Ativar(ptv);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Url = Url.Action("Index", "PTV") + "?Msg=" + Validacao.QueryParam() + "&acaoGerarPdfId=" + ptv.Id.ToString(),
				@Id = ptv.Id
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new object[] { ePermissao.PTVCancelar })]
		public ActionResult CancelarConfirm(int id)
		{
			if (!_validar.FuncionarioHabilitadoValido())
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			PTV ptv = _busPTV.Obter(id, true);
			return PartialView("CancelarPartial", ptv);
		}

		[HttpPost]
		[Permite(RoleArray = new object[] { ePermissao.PTVCancelar })]
		public ActionResult PTVCancelar(PTV ptv)
		{
			_busPTV.PTVCancelar(ptv);
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Url = Url.Action("Index", "PTV") + "?Msg=" + Validacao.QueryParam()
			}, JsonRequestBehavior.AllowGet);
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

					return ViewModelHelper.GerarArquivoPdf(pdf.Gerar(id, PTV.Tid, PTV.Situacao, PTV.SituacaoTexto), "PTV", dataHoraControleAcesso: true);
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

		#endregion

		#region Destinatario

		[Permite(RoleArray = new Object[] { ePermissao.DestinatarioPTVListar })]
		public ActionResult DestinatarioIndex()
		{
			DestinatarioPTVListarVM vm = new DestinatarioPTVListarVM(new ListaBus().QuantPaginacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return View("Destinatario/Index", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DestinatarioPTVListar })]
		public ActionResult DestinatarioFiltrar(DestinatarioPTVListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<DestinatarioPTVListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(new ListaBus().QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<DestinatarioListarResultado> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeEditar = User.IsInRole(ePermissao.DestinatarioPTVEditar.ToString());
			vm.PodeExcluir = User.IsInRole(ePermissao.DestinatarioPTVExcluir.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new
			{
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "Destinatario/ListarResultados", vm)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DestinatarioPTVCriar })]
		public ActionResult DestinatarioCriar()
		{
			DestinatarioPTVVM vm = new DestinatarioPTVVM(new DestinatarioPTV(), _busLista.Estados, null);
			vm.Destinatario.PessoaTipo = PessoaTipo.FISICA;

			return View("Destinatario/Criar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DestinatarioPTVCriar })]
		public ActionResult DestinatarioModal()
		{
			DestinatarioPTVVM vm = new DestinatarioPTVVM(new DestinatarioPTV(), _busLista.Estados, null);
			vm.Destinatario.PessoaTipo = PessoaTipo.FISICA;

			return View("Destinatario/DestinatarioModal", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DestinatarioPTVEditar })]
		public ActionResult DestinatarioEditar(int id)
		{
			DestinatarioPTV destinatario = _bus.Obter(id);
			DestinatarioPTVVM vm = new DestinatarioPTVVM(destinatario, _busLista.Estados, _busLista.Municipios(destinatario.EstadoID));

			return View("Destinatario/Editar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DestinatarioPTVCriar, ePermissao.DestinatarioPTVEditar })]
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

		[Permite(RoleArray = new Object[] { ePermissao.DestinatarioPTVVisualizar })]
		public ActionResult DestinatarioVisualizar(int id)
		{
			DestinatarioPTV destinatario = _bus.Obter(id);

			DestinatarioPTVVM vm = new DestinatarioPTVVM(destinatario, _busLista.Estados, _busLista.Municipios(destinatario.EstadoID), true);

			return View("Destinatario/Visualizar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DestinatarioPTVExcluir })]
		public ActionResult DestinatarioExcluirConfirm(int id)
		{
			ConfirmarVM vm = new ConfirmarVM();

			DestinatarioPTV destinatario = _bus.Obter(id);
			vm.Id = destinatario.ID;
			vm.Mensagem = Mensagem.DestinatarioPTV.MensagemExcluir(destinatario.NomeRazaoSocial);
			vm.Titulo = "Excluir Destinatário";

			return PartialView("Confirmar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DestinatarioPTVExcluir })]
		public ActionResult DestinatarioExcluir(int id)
		{
			_bus.Excluir(id);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.DestinatarioPTVCriar, ePermissao.DestinatarioPTVEditar })]
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

		#region EPTV

		[Permite(RoleArray = new Object[] { ePermissao.PTVListar })]
		public ActionResult EPTVListar()
		{
			PTVListarVM vm = new PTVListarVM(_busLista.PTVSolicitacaoSituacao);
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PTVListar })]
		public ActionResult EPTVFiltrar(PTVListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<PTVListarVM>(vm.UltimaBusca).Filtros;
			}
			
			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(new ListaBus().QuantPaginacao, vm.Paginacao.QuantPaginacao);

			EtramiteIdentity func = User.Identity as EtramiteIdentity;
			vm.Filtros.FuncionarioId = func.FuncionarioId;

			Resultados<PTVListarResultado> resultados = _busPTV.FiltrarEPTV(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeVisualizar = User.IsInRole(ePermissao.PTVVisualizar.ToString());
			vm.PodeGerarPDF = User.IsInRole(ePermissao.PTVListar.ToString());
			vm.PodeAnalisar = User.IsInRole(ePermissao.PTVVisualizar.ToString());


			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new
			{
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "EPTVListarResultados", vm)
			}, JsonRequestBehavior.AllowGet);
		}


		[Permite(RoleArray = new Object[] { ePermissao.PTVEditar })]
		public ActionResult EPTVAnalisar(int id)
		{
			Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business.PTVBus ptvBusCredenciado = new Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business.PTVBus();
			PTV ptv = ptvBusCredenciado.Obter(id);

			if (!_validar.ValidarAcessoAnalisar(ptv))
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			List<Setor> locaisVistorias = _busLista.SetoresComSiglaAtuais;
			List<TratamentoFitossanitario> lsFitossanitario = _busPTV.TratamentoFitossanitário(ptv.Produtos);
			List<LaudoLaboratorial> lstLaboratorio = _busPTV.ObterLaudoLaboratorial(ptv.Produtos);

			EtramiteIdentity func = User.Identity as EtramiteIdentity;
			_busPTV.ObterResponsavelTecnico(func.UsuarioId).ForEach(x => { ptv.ResponsavelTecnicoId = x.Id; ptv.ResponsavelTecnicoNome = x.Texto; });

			PTVVM vm = new PTVVM(
				ptv,
				_busLista.PTVSituacao,
				_busPTV.ObterResponsaveisEmpreendimento(ptv.Empreendimento, ptv.Produtos),
				_busLista.DocumentosFitossanitario,
				lsFitossanitario,
				lstLaboratorio,
				_busPTV.ObterCultura(),
				_busLista.TipoTransporte,
				_busLista.Municipios(8),
				locaisVistorias,
				false,
				_busPTV.DiasHorasVistoria(ptv.LocalVistoriaId));

			foreach (var item in _busLista.PTVSolicitacaoSituacao)
			{
				int situacao = Convert.ToInt32(item.Id);

				if (situacao == (int)eSolicitarPTVSituacao.Aprovado ||
					situacao == (int)eSolicitarPTVSituacao.Rejeitado ||
					situacao == (int)eSolicitarPTVSituacao.AgendarFiscalizacao ||
					situacao == (int)eSolicitarPTVSituacao.Bloqueado)
				{
					vm.AcoesAlterar.Add(new Acao() { Id = situacao, Texto = item.Texto, IsAtivo = item.IsAtivo, Mostrar = true });
				}
			}
			vm.SetarAcoesTela(vm.AcoesAlterar);

			DestinatarioPTVBus _destinatarioBus = new DestinatarioPTVBus();
			vm.PTV.Destinatario = _destinatarioBus.Obter(ptv.DestinatarioID);
			vm.LstUnidades = ViewModelHelper.CriarSelectList(_busLista.PTVUnidadeMedida);

			return View("EPTVAnalisar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PTVCriar, ePermissao.PTVEditar })]
		public ActionResult EPTVAnalisar(PTV eptv)
		{
			_busPTV.Analisar(eptv);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("EPTVListar", "PTV") + "?Msg=" + Validacao.QueryParam()
			});
		}


		[Permite(RoleArray = new Object[] { ePermissao.PTVEditar })]
		public ActionResult EPTVVisualizar(int id)
		{
			Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business.PTVBus ptvBusCredenciado = new Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business.PTVBus();

			PTV ptv = ptvBusCredenciado.Obter(id);

			List<Setor> locaisVistorias = _busLista.SetoresComSiglaAtuais;
			List<TratamentoFitossanitario> lsFitossanitario = _busPTV.TratamentoFitossanitário(ptv.Produtos);
			List<LaudoLaboratorial> lstLaboratorio = _busPTV.ObterLaudoLaboratorial(ptv.Produtos);

			PTVVM vm = new PTVVM(
				ptv,
				_busLista.PTVSituacao,
				_busPTV.ObterResponsaveisEmpreendimento(ptv.Empreendimento, ptv.Produtos),
				_busLista.DocumentosFitossanitario,
				lsFitossanitario,
				lstLaboratorio,
				_busPTV.ObterCultura(),
				_busLista.TipoTransporte,
				_busLista.Municipios(8),
				locaisVistorias,
				false,
				_busPTV.DiasHorasVistoria(ptv.LocalVistoriaId));

			foreach (var item in _busLista.PTVSolicitacaoSituacao)
			{
				int situacao = Convert.ToInt32(item.Id);

				if (situacao == (int)eSolicitarPTVSituacao.Aprovado ||
					situacao == (int)eSolicitarPTVSituacao.Rejeitado ||
					situacao == (int)eSolicitarPTVSituacao.AgendarFiscalizacao ||
					situacao == (int)eSolicitarPTVSituacao.Bloqueado)
				{
					vm.AcoesAlterar.Add(new Acao() { Id = situacao, Texto = item.Texto, IsAtivo = item.IsAtivo, Mostrar = true });
				}
			}
			vm.SetarAcoesTela(vm.AcoesAlterar);

			DestinatarioPTVBus _destinatarioBus = new DestinatarioPTVBus();
			vm.PTV.Destinatario = _destinatarioBus.Obter(ptv.DestinatarioID);
			vm.LstUnidades = ViewModelHelper.CriarSelectList(_busLista.PTVUnidadeMedida);
			vm.IsVisualizar = true;
			return View("EPTVVisualizar", vm);
		}

		#endregion EPTV

		#region PTVComunicador

		[Permite(RoleArray = new Object[] { ePermissao.PTVComunicador })]
		public ActionResult ValidarAcessoComunicador(int id)
		{
			_validar.ValidarAcessoComunicadorPTV(id);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
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
				@Url = Url.Action("ComunicadorPTV", "PTV") + "/" + Comunicador.PTVId.ToString(),
				@Local = Comunicador
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}