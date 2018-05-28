using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProcesso.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloAnalise.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloProtocolo.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloTitulo.Data;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloTramitacao.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMProcesso;
using AtividadeVM = Tecnomapas.EtramiteX.Interno.ViewModels.VMAtividade;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class ProcessoController : DefaultController
	{
		#region Propriedade

		ListaBus _busLista;
		ProcessoBus _bus;
		ProcessoValidar _validar;
		JuntarApensarValidar _validarJuntarApensar;
		JuntarApensarBus _busJuntarApensar;
		DocumentoBus _busDoc;
		RequerimentoBus _busRequerimento;
		FiscalizacaoValidar _validarFiscalizacao;
		FuncionarioBus _busFuncionario;
		TramitacaoBus _busTramitacao;
		AtividadeBus _busAtividade;
		ProtocoloBus _busProtocolo;
		FiscalizacaoBus _busFiscalizacao;

		#endregion

		public ProcessoController()
		{
			_busProtocolo = new ProtocoloBus();
			_busLista = new ListaBus();
			_bus = new ProcessoBus();
			_validarJuntarApensar = new JuntarApensarValidar();
			_validar = new ProcessoValidar();
			_busJuntarApensar = new JuntarApensarBus();
			_busDoc = new DocumentoBus();
			_busRequerimento = new RequerimentoBus(new RequerimentoValidar());
			_busFuncionario = new FuncionarioBus();
			_busTramitacao = new TramitacaoBus();
			_busAtividade = new AtividadeBus();
			_validarFiscalizacao = new FiscalizacaoValidar();
			_busFiscalizacao = new FiscalizacaoBus();
		}

		#region Ações

		#region Listar

		[Permite(RoleArray = new Object[] { ePermissao.ProcessoListar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(
				_busLista.QuantPaginacao,
				_busLista.AtividadesSolicitadaAtivasDesativas,
				_busLista.SituacoesProcessoAtividade,
				_busLista.TiposProcesso,
				_busLista.Municipios(_busLista.EstadoDefault)
			);

			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProcessoListar })]
		public ActionResult Associar()
		{
			ListarVM vm = new ListarVM(
				_busLista.QuantPaginacao,
				_busLista.AtividadesSolicitada,
				_busLista.SituacoesProcessoAtividade,
				_busLista.TiposProcesso,
				_busLista.Municipios(_busLista.EstadoDefault)
			);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return PartialView("ListarFiltros", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProcessoListar })]
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

			Resultados<Protocolo> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			if (!vm.PodeAssociar)
			{
				vm.PodeEditar = User.IsInRole(ePermissao.ProcessoEditar.ToString());
				vm.PodeExcluir = User.IsInRole(ePermissao.ProcessoExcluir.ToString());
			}

			vm.PodeVisualizar = User.IsInRole(ePermissao.ProcessoVisualizar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.ProcessoCriar })]
		public ActionResult Criar()
		{
			SalvarVM vm = new SalvarVM(_busLista.TiposProcesso);
			vm.CarregarSetores(_busFuncionario.ObterSetoresFuncionario(ProcessoBus.User.FuncionarioId));
			vm.PodeAutuar = User.IsInRole(ePermissao.ProcessoAutuar.ToString());
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProcessoCriar })]
		public ActionResult Criar(Processo processo)
		{
			string urlRedirecionar = urlRedirecionar = Url.Action("Criar", "Processo");

			if (_bus.Salvar(processo))
			{
				urlRedirecionar += "?Msg=" + Validacao.QueryParam() + "&acaoId=" + processo.Id.ToString();
				return Json(new { @IsProcessoSalvo = Validacao.EhValido, @UrlRedireciona = urlRedirecionar, @Msg = Validacao.Erros, @urlRetorno = urlRedirecionar });
			}
			else
			{
				return Json(new { @IsProcessoSalvo = false, @Msg = Validacao.Erros });
			}
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.ProcessoEditar })]
		public ActionResult Editar(int id)
		{
			ActionResult retorno = Redirecionar(id, true);

			if (retorno != null)
			{
				return retorno;
			}

			Processo processo = _bus.Obter(id);

			if (!Validacao.EhValido)
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			SalvarVM vm = new SalvarVM(_busLista.TiposProcesso, processo.Tipo.Id);
			vm.SetProcesso(processo, _busLista.ResponsavelFuncoes);
			vm.IsEditar = true;
			vm.RequerimentoVM.IsEditar = true;
			vm.PodeAutuar = User.IsInRole(ePermissao.ProcessoAutuar.ToString());

			if (Request.IsAjaxRequest())
			{
				return Json(new { @EhValido = Validacao.EhValido, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ProcessoPartial", vm) }, JsonRequestBehavior.AllowGet);
			}
			else
			{
				return View(vm);
			}
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProcessoEditar })]
		public ActionResult Editar(Processo processo)
		{
			string urlRedirecionar = Url.Action("Index", "Processo");

			if (_bus.Salvar(processo))
			{

				urlRedirecionar += "?Msg=" + Validacao.QueryParam() + "&acaoId=" + processo.Id.ToString();
				return Json(new { @IsProcessoSalvo = Validacao.EhValido, @UrlRedireciona = urlRedirecionar, @Msg = Validacao.Erros, @urlRetorno = urlRedirecionar });
			}
			else
			{
				return Json(new { @IsProcessoSalvo = false, @Msg = Validacao.Erros });
			}
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProcessoEditar })]
		public ActionResult Autuar(Processo processo)
		{
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Processo = _bus.Autuar(processo) });
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.ProcessoVisualizar })]
		public ActionResult Visualizar(int id)
		{
			Processo processo = _bus.Obter(id);

			if (!Validacao.EhValido)
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			SalvarVM vm = new SalvarVM(_busLista.TiposProcesso, processo.Tipo.Id);
			vm.RequerimentoVM.IsVisualizar = true;
			vm.RequerimentoVM.IsRequerimentoProcesso = true;
			vm.SetProcesso(processo, _busLista.ResponsavelFuncoes);

			vm.IsEditar = false;
			vm.RequerimentoVM.IsEditar = false;

			if (Request.IsAjaxRequest())
			{
				return PartialView("VisualizarModal", vm);
			}
			else
			{
				return View(vm);
			}
		}

		#endregion

		#region Excluir

		[Permite(RoleArray = new Object[] { ePermissao.ProcessoExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			Processo processo = _bus.Obter(id);
			vm.Id = id;
			vm.Mensagem = Mensagem.Processo.MensagemExcluir(processo.Numero);
			vm.Titulo = "Excluir Processo";
			return PartialView("Excluir", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProcessoExcluir })]
		public ActionResult Excluir(int id)
		{
			bool excluido = _bus.Excluir(id);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Excluido = excluido }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#endregion

		#region Consultar Informações

		[Permite(RoleArray = new Object[] { ePermissao.ProcessoListar })]
		public ActionResult ConsultarInformacoes(int id)
		{
			try
			{
				ListarTramitacaoFiltro filtro = new ListarTramitacaoFiltro();
				filtro.Protocolo.Id = id;
				filtro.ProtocoloTipo = (int)eTipoProtocolo.Processo;

				ConsultarInformacaoVM vm = new ConsultarInformacaoVM();
				vm.Id = id;
				vm.MostrarInformacao(0, (_busTramitacao.FiltrarHistorico(filtro).Itens.Count > 0) || (_busProtocolo.FiltrarHistoricoAssociados(new ListarProtocoloFiltro() { Id = id }).Itens.Count > 0));
				vm.MostrarInformacao(1, Convert.ToBoolean(new PdfAnalise().Existe(id)));
				vm.MostrarInformacao(2, new PdfProtocoloAssociado().Existe(id) > 0);
				vm.MostrarInformacao(3, new PdfTramitacaoArquivamento().Existe(id) > 0);

				int valor = new RelatorioEntregaDa().ExisteEntregaProtocolo(id);
				vm.MostrarInformacao(4, valor > 0, valor);
				vm.MostrarInformacao(5, true, id);

				Processo processo = _bus.ObterSimplificado(id);
				vm.ProcessoNumero = processo.Numero;
				vm.ProcessoTipo = processo.Tipo.Texto;

				ProtocoloLocalizacao loc = _bus.ObterLocalizacao(id);
				if (loc.Localizacao == eLocalizacaoProtocolo.OrgaoExterno)
				{
					vm.ProcessoLocalizacao = loc.OrgaoExternoNome;
				}
				else if (loc.Localizacao == eLocalizacaoProtocolo.Arquivado)
				{
					vm.ProcessoLocalizacao = loc.ArquivoNome;
				}
				else if (loc.Localizacao == eLocalizacaoProtocolo.EnviadoParaSetor || loc.Localizacao == eLocalizacaoProtocolo.EnviadoParaFuncionario)
				{
					vm.ProcessoLocalizacao = "Em tramitação";
				}
				else if (loc.ProcessoPaiId > 0 || loc.Localizacao == eLocalizacaoProtocolo.PosseFuncionario)
				{
					vm.ProcessoLocalizacao = loc.SetorDestinatarioNome;
					vm.ProcessoEnviadoPor = loc.FuncionarioDestinatarioNome;
					vm.LabelEnviadoPor = "Em posse de";
				}
				if (Request.IsAjaxRequest())
				{
					return PartialView("ConsultarInformacoes", vm);
				}
				else
				{
					return View("ConsultarInformacoes", vm);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		public ActionResult GerarPdfDocJuntadoProcApensado(int id)
		{
			try
			{
				return ViewModelHelper.GerarArquivoPdf(new PdfProtocoloAssociado().GerarPdfProtocoloAssociado(id), "Documentos Juntados/Processos Apensados");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		public ActionResult GerarPdfDocRegistroRecebimento(int id)
		{
			try
			{
				return ViewModelHelper.GerarArquivoPdf(new PdfProtocoloRecebimento().Gerar(id), "Registro de Recebimento");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		public ActionResult GerarPdfDocComunicacaoInterna(int id)
		{
			try
			{
				return ViewModelHelper.GerarArquivoPdf(new PdfComunicacaoInterna().Gerar(id), "Comunicação Interna");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		public ActionResult GerarPdfDocOficioAdministrativo(int id)
		{
			try
			{
				return ViewModelHelper.GerarArquivoPdf(new PdfOficioAdministrativo().Gerar(id), "Ofício (Administrativo)");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		#endregion

		#region Editar Apensados/Juntados (Requerimento Padrão)

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoApensarJuntar })]
		public ActionResult EditarApensadosJuntados(int id)
		{
			ActionResult retorno = ValidarEditarApensadosJuntados(id);

			if (retorno != null)
			{
				return retorno;
			}

			EditarApensadosJuntadosVM vm = new EditarApensadosJuntadosVM();
			vm.Processo = _busJuntarApensar.ObterProcessosDocumentos(id);
			vm.CarregarRequerimentoVM(vm.Processo.Processos, vm.Processo.Documentos);

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoApensarJuntar })]
		public ActionResult EditarApensadosJuntados(Processo processo)
		{
			_busJuntarApensar.EditarApensadosJuntados(processo);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoApensarJuntar })]
		public ActionResult CriarAtividadesSolicitadasDeRequerimento(int requerimentoId, bool isProcesso, int excetoId = 0)
		{
			if (!_validar.RequerimentoFinalizado(requerimentoId, excetoId, isProcesso) || !_validarJuntarApensar.RequerimentoAssociadoTitulo(excetoId, isProcesso))
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}
			Requerimento requerimento = _busRequerimento.Obter(requerimentoId);

			RequerimentoVM vm = new RequerimentoVM(requerimento);
			vm.SetSituacaoAtividadeCadastro(_busLista.SituacoesProcessoAtividade.SingleOrDefault(x => Equals(x.Id, 1)).Texto);
			vm.ResetIdRelacionamento();

			vm.IsEditar = true;

			return PartialView("RequerimentoAtividadesSolicitadas", vm);
		}

		#endregion

		#region Juntar/Apensar

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoApensarJuntar })]
		public ActionResult JuntarApensar(int? id)
		{
			JuntarApensarVM vm = new JuntarApensarVM();
			if (id != null && id.HasValue)
			{
				if (_busJuntarApensar.VerificarJuntarApensar(id))
				{
					vm.Processo = _busJuntarApensar.ObterJuntadosApensados(id.Value);
					vm.Processo.Id = id.Value;
				}
			}

			return View("JuntarApensar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoApensarJuntar })]
		public ActionResult JuntarApensarVerificar(string numero)
		{
			JuntarApensarVM vm = new JuntarApensarVM();
			if (_busJuntarApensar.VerificarJuntarApensar(numero))
			{
				vm.Processo = _bus.Obter(numero);
				Processo procApensadosJuntados = _busJuntarApensar.ObterJuntadosApensados(vm.Processo.Id.Value);
				vm.Processo.Processos = procApensadosJuntados.Processos;
				vm.Processo.Documentos = procApensadosJuntados.Documentos;
			}

			return Json(new
			{
				Msg = Validacao.Erros,
				@Html = ((vm.Processo != null && vm.Processo.Id > 0) ? ViewModelHelper.RenderPartialViewToString(ControllerContext, "JuntarApensarPartial", vm) : "")
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoApensarJuntar })]
		public ActionResult JuntarDocumentoVerificar(string numero, int procId)
		{
			Documento documento = null;
			if (_busJuntarApensar.VerificarJuntarDocumento(numero, procId))
			{
				documento = _busDoc.Obter(numero);
			}

			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, doc = documento }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoApensarJuntar })]
		public ActionResult ApensarProcessoVerificar(string numero, int procId)
		{
			Processo processo = null;
			if (_busJuntarApensar.VerificarApensarProcesso(numero, procId))
			{
				processo = _bus.Obter(numero);
			}

			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, proc = processo }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoApensarJuntar })]
		public ActionResult JuntarApensarSalvar(JuntarApensarVM vm)
		{
			string urlRedireciona = "";
			if (_busJuntarApensar.JuntarApensar(vm.Processo))
			{
				Validacao.Add(Mensagem.Processo.JuntadoApensadoSucesso);
				urlRedireciona = Url.Action("Index", "Tramitacao", Validacao.QueryParamSerializer());
			}

			return Json(new { @Msg = Validacao.Erros, @UrlRedireciona = urlRedireciona });
		}

		#endregion

		#region Métodos Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.AtividadeEncerrar, ePermissao.ProcessoVisualizar, ePermissao.ProcessoListar })]
		public ActionResult AtividadesSolicitadas(int id, bool isProcesso)
		{
			AtividadeVM.ListarAtividadesSolicitadasVM vm = new AtividadeVM.ListarAtividadesSolicitadasVM();
			vm = new AtividadeVM.ListarAtividadesSolicitadasVM(_busLista.TiposProcesso, _busLista.TiposDocumento, _busAtividade.ObterProtocoloAtividadesSolicitadas(id), vm.Protocolo.Tipo.Id);

			vm.IsEncerrar = User.IsInRole(ePermissao.AtividadeEncerrar.ToString());
			vm.IsProcesso = isProcesso;

			return PartialView("~/Views/Atividade/AtividadesSolicitadas.aspx", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProcessoEditar })]
		public ActionResult ValidarChecagemTemTituloPendencia(int processoId = 0)
		{
			return Json(new { @EhValido = _bus.VerificarChecagemTemTituloPendencia(processoId), @Msg = Validacao.Erros });
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProcessoCriar, ePermissao.ProcessoEditar })]
		public ActionResult ValidarAssociarResponsavel(int id)
		{
			_bus.ValidarAssociarResponsavelTecnico(id);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProcessoCriar, ePermissao.ProcessoEditar })]
		public ActionResult ObterRequerimento(int id, int excetoId = 0)
		{
			if (!_validar.RequerimentoFinalizado(id, excetoId, local: " do requerimento") || !_validarJuntarApensar.RequerimentoAssociadoTitulo(excetoId, true))
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			RequerimentoVM vm = new RequerimentoVM(_busRequerimento.ObterFinalizar(id));
			vm.SetSituacaoAtividadeCadastro(_busLista.SituacoesProcessoAtividade.SingleOrDefault(x => Equals(x.Id, 1)).Texto);
			vm.ResetIdRelacionamento();

			vm.CarregarListas(_busLista.ResponsavelFuncoes);

			return Json(new { @EhValido = Validacao.EhValido, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "Requerimento", vm) }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProcessoCriar, ePermissao.ProcessoEditar })]
		public ActionResult ValidarAssociacaoChecagem(int id, int? processoId)
		{
			_bus.ValidarCheckList(id, processoId.GetValueOrDefault());
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProcessoEditar })]
		public ActionResult ValidarEditar(int processoId)
		{
			_validar.VerificarPossuiAtividadesNaoEncerrada(processoId);

			if (Validacao.EhValido)
			{
				_validar.EmPosse(processoId, 2);
			}

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProcessoEditar })]
		public ActionResult ValidarProcessoPosse(int processoId)
		{
			_validar.EmPosse(processoId, 2);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProcessoListar })]
		public ActionResult ObterProcesso(int processoId)
		{
			Processo processo = _bus.ObterSimplificado(processoId);

			return Json(new { @Processo = processo, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProcessoEditar, ePermissao.ProcessoVisualizar })]
		public FileResult Baixar(int id)
		{
			return ViewModelHelper.BaixarArquivo(id);
		}

		private ActionResult Redirecionar(int id, bool isEditar = false)
		{
			if (Request.IsAjaxRequest())
			{
				_validarJuntarApensar.Apensado(id);

				if (Validacao.EhValido && isEditar)
				{
					_validar.VerificarPossuiAtividadesNaoEncerrada(id);
				}

				if (Validacao.EhValido)
				{
					_validar.EmPosse(id, 2);
				}

				if (!isEditar)
				{
					Validacao.Erros.RemoveAll(x => x.Tipo == eTipoMensagem.Advertencia);
				}
				else if (Validacao.EhValido)
				{
					return null;
				}

				Processo processo = _bus.Obter(id);

				SalvarVM vm = new SalvarVM(_busLista.TiposProcesso, processo.Tipo.Id);
				vm.RequerimentoVM.IsVisualizar = true;
				vm.RequerimentoVM.IsRequerimentoProcesso = true;
				vm.SetProcesso(processo, _busLista.ResponsavelFuncoes);

				vm.IsEditar = false;
				vm.RequerimentoVM.IsEditar = false;

				return Json(new { @EhValido = Validacao.EhValido, @SetarHtml = true, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "VisualizarPartial", vm), @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}
			else
			{
				_validarJuntarApensar.Apensado(id);

				if (Validacao.EhValido && isEditar)
				{
					_validar.VerificarPossuiAtividadesNaoEncerrada(id);
				}

				if (Validacao.EhValido)
				{
					_validar.EmPosse(id, 1);
				}

				if (!Validacao.EhValido)
				{
					return RedirectToAction("", Validacao.QueryParamSerializer());
				}
			}

			return null;
		}

		public ActionResult ExisteProcessoAtividade(int id)
		{
			return Json(new { @EhValido = _bus.ExisteProcessoAtividade(id), @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult ValidarEditarApensadosJuntados(int processo)
		{
			_validarJuntarApensar.Apensado(processo);

			if (Validacao.EhValido)
			{
				_validar.EmPosse(processo, 1);
			}

			if (Validacao.EhValido)
			{
				_busProtocolo.ExisteRequerimento(processo, true);
			}

			if (Request.IsAjaxRequest())
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}
			else
			{
				if (!Validacao.EhValido)
				{
					return RedirectToAction("", Validacao.QueryParamSerializer());
				}

				return null;
			}
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProcessoEditar })]
		public ActionResult ValidarRequerimentoAtividades(int id, int processoId = 0)
		{
			if (!_validar.RequerimentoFinalizado(id, processoId) || !_validarJuntarApensar.RequerimentoAssociadoTitulo(processoId, true))
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			return Json(new { @EhValido = Validacao.EhValido }, JsonRequestBehavior.AllowGet);
		}

		#region Fiscalizacao

		[Permite(RoleArray = new Object[] { ePermissao.ProcessoCriar, ePermissao.ProcessoEditar })]
		public ActionResult ObterFiscalizacao(int fiscalizacaoId)
		{
			if (!_bus.ValidarAssociarFiscalizacao(fiscalizacaoId))
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			FiscalizacaoBus fisBus = new FiscalizacaoBus();
			Pessoa autuado = fisBus.ObterAutuado(fiscalizacaoId);

			return Json(new { @EhValido = Validacao.EhValido, Autuado = autuado }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProcessoCriar, ePermissao.ProcessoEditar })]
		public ActionResult ValidarDesassociarFiscalizacao(int fiscalizacaoId)
		{
			_bus.ValidarDesassociarFiscalizacao(fiscalizacaoId);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#endregion
	}
}