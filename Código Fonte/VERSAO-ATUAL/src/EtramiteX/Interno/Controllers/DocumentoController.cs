using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
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
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloAnalise.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloTitulo.Data;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloTramitacao.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMDocumento;
using AtividadeVM = Tecnomapas.EtramiteX.Interno.ViewModels.VMAtividade;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class DocumentoController : DefaultController
	{
		#region Propriedade

		ListaBus _busLista = new ListaBus();
		DocumentoBus _bus = new DocumentoBus();
		DocumentoValidar _validar = new DocumentoValidar();
		RequerimentoBus _busRequerimento = new RequerimentoBus(new RequerimentoValidar());
		FuncionarioBus _busFuncionario = new FuncionarioBus();
		ProtocoloBus _busProtocolo = new ProtocoloBus();
		TramitacaoBus _busTramitacao = new TramitacaoBus();
		JuntarApensarValidar _validarJuntarApensar = new JuntarApensarValidar();
		AtividadeBus _busAtividade = new AtividadeBus();
		FiscalizacaoBus _busFiscalizacao = new FiscalizacaoBus();
		TituloBus _busTitulo = new TituloBus(new TituloValidar());

		#endregion

		#region Ações

		#region Listar

		[Permite(RoleArray = new Object[] { ePermissao.DocumentoListar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(
				_busLista.QuantPaginacao,
				_busLista.AtividadesSolicitadaAtivasDesativas, 
				_busLista.SituacoesProcessoAtividade, 
				_busLista.TiposDocumento,
				_busLista.Municipios(_busLista.EstadoDefault));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DocumentoListar })]
		public ActionResult Associar()
		{
			ListarVM vm = new ListarVM(
				_busLista.QuantPaginacao,
				_busLista.AtividadesSolicitada,
				_busLista.SituacoesProcessoAtividade,
				_busLista.TiposDocumento,
				_busLista.Municipios(_busLista.EstadoDefault));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return PartialView("ListarFiltros", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DocumentoListar })]
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
				return Json(new { @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			if (!vm.PodeAssociar)
			{
				vm.PodeEditar = User.IsInRole(ePermissao.DocumentoEditar.ToString());
				vm.PodeExcluir = User.IsInRole(ePermissao.DocumentoExcluir.ToString());
			}

			vm.PodeVisualizar = User.IsInRole(ePermissao.DocumentoVisualizar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.DocumentoCriar })]
		public ActionResult Criar()
		{
			SalvarVM vm = new SalvarVM(_busLista.TiposDocumento, _busLista.SetoresAtuais);
			vm.CarregarSetores(_busFuncionario.ObterSetoresFuncionario(DocumentoBus.User.FuncionarioId));

			#region Assinantes

			vm.AssinantesVM.Setores = ViewModelHelper.CriarSelectList(_busLista.SetoresAtuais);
			vm.AssinantesVM.Cargos = ViewModelHelper.CriarSelectList(new List<ListaValor>());
			vm.AssinantesVM.Funcionarios = ViewModelHelper.CriarSelectList(new List<ListaValor>());

			#endregion

			//Força a seleção do item no Dropdowm
			if (!vm.MostrarSetor)
			{
				vm.Documento.SetorId = _busFuncionario.ObterSetoresFuncionario(DocumentoBus.User.FuncionarioId).FirstOrDefault().Id;
			}
			vm.Documento.ProtocoloAssociado.IsProcesso = true;

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.DocumentoCriar })]
		public ActionResult Criar(Documento documento)
		{
			string urlRedirecionar = string.Empty;

			urlRedirecionar = Url.Action("Criar", "Documento");

			if (_bus.Salvar(documento))
			{
				urlRedirecionar += "?Msg=" + Validacao.QueryParam() + "&acaoId=" + documento.Id.ToString();
				return Json(new { @IsDocumentoSalvo = Validacao.EhValido, @UrlRedireciona = urlRedirecionar, @Msg = Validacao.Erros, @urlRetorno = urlRedirecionar });
			}
			else
			{
				return Json(new { @IsDocumentoSalvo = false, @Msg = Validacao.Erros });
			}
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.DocumentoEditar })]
		public ActionResult Editar(int id)
		{
			ActionResult retorno = Redirecionar(id, true);

			if (retorno != null)
			{
				return retorno;
			}

			Documento documento = _bus.Obter(id);

			if (!Validacao.EhValido)
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			SalvarVM vm = new SalvarVM(_busLista.TiposDocumento, _busLista.SetoresAtuais, documento.Tipo.Id);
			if (documento != null)
			{
				vm.AssinantesVM.MergeAssinantesCargos(documento.Assinantes);
				vm.Tipo = _busLista.TiposDocumento.First(x => x.Id == documento.Tipo.Id);
				if (documento.DestinatarioSetor.Id > 0)
				{
					var setor = vm.SetoresDestinatario.First(x => x.Value == documento.DestinatarioSetor.Id.ToString());
					if (setor != null)
						setor.Selected = true;
				}

				if (documento.DestinatarioSetor.Id > 0)
					vm.DestinatarioFuncionarios = ViewModelHelper.CriarSelectList(_busTramitacao.ObterFuncionariosSetor(documento.DestinatarioSetor.Id), true, selecionado: documento.Destinatario.Id.ToString());
			}

			if (vm.AssinantesVM.Assinantes != null && vm.AssinantesVM.Assinantes.Count > 0)
				vm.AssinantesVM.Assinantes = _busTitulo.ObterAssinantesCargos(vm.AssinantesVM.Assinantes).Where(x => x.Selecionado).ToList();

			vm.SetDocumento(documento, _busLista.ResponsavelFuncoes);

			vm.IsEditar = true;
			vm.RequerimentoVM.IsEditar = true;

			if (Request.IsAjaxRequest())
			{
				return Json(new { @EhValido = Validacao.EhValido, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "DocumentoPartial", vm) }, JsonRequestBehavior.AllowGet);
			}
			else
			{
				return View(vm);
			}
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.DocumentoEditar })]
		public ActionResult Editar(Documento documento)
		{
			string urlRedirecionar = string.Empty;

			urlRedirecionar = Url.Action("Index", "Documento");

			if (_bus.Salvar(documento))
			{
				urlRedirecionar += "?Msg=" + Validacao.QueryParam() + "&acaoId=" + documento.Id.ToString();
				return Json(new { @IsDocumentoSalvo = Validacao.EhValido, @UrlRedireciona = urlRedirecionar, @Msg = Validacao.Erros, @urlRetorno = urlRedirecionar });
			}
			else
			{
				return Json(new { @IsDocumentoSalvo = false, @Msg = Validacao.Erros });
			}
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.DocumentoVisualizar })]
		public ActionResult Visualizar(int id)
		{
			Documento documento = _bus.Obter(id);

			if (!Validacao.EhValido)
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			SalvarVM vm = new SalvarVM(_busLista.TiposDocumento, _busLista.SetoresAtuais, documento.Tipo.Id);
			if (documento != null)
			{
				vm.AssinantesVM.MergeAssinantesCargos(documento.Assinantes);
				vm.Tipo = _busLista.TiposDocumento.First(x => x.Id == documento.Tipo.Id);
				if (documento.DestinatarioSetor.Id > 0)
				{
					var setor = vm.SetoresDestinatario.First(x => x.Value == documento.DestinatarioSetor.Id.ToString());
					if (setor != null)
						setor.Selected = true;
				}

				if (documento.DestinatarioSetor.Id > 0)
					vm.DestinatarioFuncionarios = ViewModelHelper.CriarSelectList(_busTramitacao.ObterFuncionariosSetor(documento.DestinatarioSetor.Id), true, selecionado: documento.Destinatario.Id.ToString());
			}

			if (vm.AssinantesVM.Assinantes != null && vm.AssinantesVM.Assinantes.Count > 0)
				vm.AssinantesVM.Assinantes = _busTitulo.ObterAssinantesCargos(vm.AssinantesVM.Assinantes).Where(x => x.Selecionado).ToList();
			vm.AssinantesVM.IsVisualizar = true;
			vm.SetDocumento(documento, _busLista.ResponsavelFuncoes);
			vm.RequerimentoVM.IsVisualizar = true;
			vm.RequerimentoVM.IsRequerimentoDocumento = true;
			
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

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.DocumentoExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{			
			ExcluirVM vm = new ExcluirVM();
			Documento documento = _bus.ObterSimplificado(id);
			vm.Id = id;
			vm.Mensagem = Mensagem.Documento.MensagemExcluir(documento.Numero);
			vm.Titulo = "Excluir Documento";
			return PartialView("Excluir", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.DocumentoExcluir })]
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
				filtro.ProtocoloTipo = (int)eTipoProtocolo.Documento;

				ConsultarInformacaoVM vm = new ConsultarInformacaoVM();
				vm.Id = id;
				vm.MostrarInformacao(0, (_busTramitacao.FiltrarHistorico(filtro).Itens.Count > 0) || (_busProtocolo.FiltrarHistoricoAssociados(new ListarProtocoloFiltro() { Id = id }).Itens.Count > 0));
				vm.MostrarInformacao(1, Convert.ToBoolean(new PdfAnalise().Existe(id)));
				vm.MostrarInformacao(2, new PdfTramitacaoArquivamento().Existe(id) > 0);
				
				int valor = new RelatorioEntregaDa().ExisteEntregaProtocolo(id);
				vm.MostrarInformacao(3, valor > 0, valor);

				vm.MostrarInformacao(4, true, id);

				Documento doc = _bus.ObterSimplificado(id);
				vm.MostrarInformacao(5, doc.Tipo.Texto.Contains("CI - Comunic"), id);

				vm.DocumentoNumero = doc.Numero;
				vm.DocumentoTipo = doc.Tipo.Texto;

				ProtocoloLocalizacao loc = _bus.ObterLocalizacao(doc);
				if (loc.Localizacao == eLocalizacaoProtocolo.OrgaoExterno)
				{
					vm.DocumentoLocalizacao = loc.OrgaoExternoNome;
				}
				else if (loc.Localizacao == eLocalizacaoProtocolo.Arquivado)
				{
					vm.DocumentoLocalizacao = loc.ArquivoNome;
				}
				else if (loc.Localizacao == eLocalizacaoProtocolo.EnviadoParaSetor || loc.Localizacao == eLocalizacaoProtocolo.EnviadoParaFuncionario)
				{
					vm.DocumentoLocalizacao = "Em tramitação";
				}
				else if (loc.ProcessoPaiId > 0 || loc.Localizacao == eLocalizacaoProtocolo.PosseFuncionario)
				{
					vm.DocumentoLocalizacao = loc.SetorDestinatarioNome;
					vm.DocumentoEnviadoPor = loc.FuncionarioDestinatarioNome;
					vm.LabelEnviadoPor = "Em posse";
				}

				return View("ConsultarInformacoes", vm);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		#endregion

		#region Métodos Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.DocumentoCriar, ePermissao.DocumentoEditar })]
		public ActionResult ObterAssinanteCargos(int id)
		{
			var lista = _bus.ObterAssinanteCargos(id);

			return Json(lista, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DocumentoCriar, ePermissao.DocumentoEditar })]
		public ActionResult ObterAssinanteFuncionarios(int id, int setorId)
		{
			var lista = _bus.ObterAssinanteFuncionarios(setorId, id);

			return Json(lista, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.AtividadeEncerrar, ePermissao.ProcessoVisualizar })]
		public ActionResult AtividadesSolicitadas(int id, bool isProcesso)
		{
			AtividadeVM.ListarAtividadesSolicitadasVM vm = new AtividadeVM.ListarAtividadesSolicitadasVM();
			
			vm = new AtividadeVM.ListarAtividadesSolicitadasVM(_busLista.TiposProcesso, _busLista.TiposDocumento, _busAtividade.ObterProtocoloAtividadesSolicitadas(id), vm.Protocolo.Tipo.Id);
			
			vm.IsEncerrar = User.IsInRole(ePermissao.AtividadeEncerrar.ToString());
			vm.IsProcesso = isProcesso;

			return View("~/Views/Atividade/AtividadesSolicitadas.aspx", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DocumentoEditar })]
		public ActionResult ValidarChecagemTemTituloPendencia(int documentoId = 0)
		{
			return Json(new { @EhValido = _bus.VerificarChecagemTemTituloPendencia(documentoId), @Msg = Validacao.Erros });
		}

		[Permite(RoleArray = new Object[] { ePermissao.DocumentoCriar, ePermissao.DocumentoEditar })]
		public ActionResult ValidarAssociarResponsavel(int id)
		{
			_bus.ValidarAssociarResponsavelTecnico(id);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DocumentoCriar, ePermissao.DocumentoEditar })]
		public ActionResult ObterRequerimento(int id, int excetoId = 0)
		{
			if (!_validar.RequerimentoFinalizado(id, excetoId, local: " do requerimento") || !_validarJuntarApensar.RequerimentoAssociadoTitulo(excetoId, false))
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			Requerimento requerimento = _busRequerimento.ObterFinalizar(id);

			RequerimentoVM vm = new RequerimentoVM(requerimento);
			vm.SetSituacaoAtividadeCadastro(_busLista.SituacoesProcessoAtividade.SingleOrDefault(x => Equals(x.Id, 1)).Texto);
			vm.ResetIdRelacionamento();

			vm.CarregarListas(_busLista.ResponsavelFuncoes);

			return Json(new { @EhValido = Validacao.EhValido, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "Requerimento", vm) }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.DocumentoEncerrarOficioPendencia })]
		public ActionResult ObterProtocolo(int id, bool isProcesso)
		{
			List<PessoaLst> lista = _busProtocolo.ObterInteressadoRepresentantes(id);
			IProtocolo protocolo = _busProtocolo.ObterSimplificado(id);
			if (lista.Count == 0)
			{
				lista.Add(new PessoaLst() { Id = protocolo.Interessado.Id });
			}
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Objeto = new
				{
					@Id = id,
					@Numero = protocolo.Numero,
					@EmpreendimentoId = protocolo.Empreendimento.Id,
					@EmpreendimentoNome = protocolo.Empreendimento.Denominador,
					@Interessado = protocolo.Interessado,
					@Representantes = lista
				}
			});
		}

		[Permite(RoleArray = new Object[] { ePermissao.DocumentoCriar, ePermissao.DocumentoEditar })]
		public ActionResult ValidarAssociacaoChecagem(int id, int? documentoId)
		{
			_bus.ValidarCheckList(id, documentoId.GetValueOrDefault());
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DocumentoCriar, ePermissao.DocumentoEditar })]
		public ActionResult ValidarPossuiRequerimentoAtividades(int id)
		{
			Documento doc = _bus.ObterSimplificado(id);

			_validar.ValidarPossuiRequerimentoAtividades(doc);

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

		[Permite(RoleArray = new Object[] { ePermissao.DocumentoEditar })]
		public ActionResult ValidarRequerimentoAtividades(int id, int documentoId = 0)
		{
			if (!_validar.RequerimentoFinalizado(id, documentoId) || !_validarJuntarApensar.RequerimentoAssociadoTitulo(documentoId, false))
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			return Json(new { @EhValido = Validacao.EhValido }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DocumentoCriar, ePermissao.DocumentoEditar })]
		public ActionResult ValidarAssociacaoChecagemPendencia(int id)
		{
			_validar.ChecagemPendenciaJaAssociada(id);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DocumentoCriar, ePermissao.DocumentoEditar })]
		public ActionResult ValidarRoteirosChecagemRequerimento(int checagemId, int requerimentoId)
		{
			_validar.RoteirosChecagemRequerimento(checagemId, requerimentoId);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.DocumentoEditar })]
		public ActionResult ValidarEditar(int documentoId)
		{			
			_validar.VerificarPossuiAtividadesNaoEncerrada(documentoId);
			
			if (Validacao.EhValido)
			{
				_validar.EmPosse(documentoId, 2);
			}

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DocumentoCriar, ePermissao.DocumentoEditar })]
		public ActionResult ValidarDocumentoPosse(int documentoId)
		{
			_validar.EmPosse(documentoId, true);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

		[Permite(RoleArray = new Object[] { ePermissao.DocumentoVisualizar })]
		private ActionResult Redirecionar(int id, bool isEditar = false)
		{
			if (Request.IsAjaxRequest())
			{
				_validar.Juntado(id, isEditar);

				if (Validacao.EhValido && isEditar && _bus.ExisteAtividade(id))
				{
					_validar.VerificarPossuiAtividadesNaoEncerrada(id);
				}

				if (Validacao.EhValido)
				{
					_validar.VerificarPossuiPosse(id);
				}

				if (!isEditar)
				{
					Validacao.Erros.RemoveAll(x => x.Tipo == eTipoMensagem.Advertencia);
				}
				else if (Validacao.EhValido)
				{
					return null;
				}

				Documento documento = _bus.Obter(id);

				SalvarVM vm = new SalvarVM(_busLista.TiposDocumento, _busLista.SetoresAtuais, documento.Tipo.Id);
				if (vm.AssinantesVM.Assinantes != null && vm.AssinantesVM.Assinantes.Count > 0)
					vm.AssinantesVM.Assinantes = _busTitulo.ObterAssinantesCargos(vm.AssinantesVM.Assinantes).Where(x => x.Selecionado).ToList();

				vm.SetDocumento(documento, _busLista.ResponsavelFuncoes);
				vm.RequerimentoVM.IsVisualizar = true;
				vm.RequerimentoVM.IsRequerimentoDocumento = true;
				
				return Json(new { @EhValido = Validacao.EhValido, @SetarHtml = true, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "VisualizarPartial", vm), @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}
			else
			{
				_validar.Juntado(id, true);

				if (Validacao.EhValido && isEditar && _bus.ExisteAtividade(id))
				{
					_validar.VerificarPossuiAtividadesNaoEncerrada(id);
				}

				if (Validacao.EhValido)
				{
					_validar.VerificarPossuiPosse(id);
				}

				if (!Validacao.EhValido)
				{
					return RedirectToAction("", Validacao.QueryParamSerializer());
				}
			}

			return null;
		}

		[Permite(RoleArray = new Object[] { ePermissao.DocumentoEditar, ePermissao.DocumentoVisualizar })]
		public FileResult Baixar(int id)
		{
			return ViewModelHelper.BaixarArquivo(id);
		}

		#region Fiscalizacao

		[Permite(RoleArray = new Object[] { ePermissao.DocumentoCriar, ePermissao.DocumentoEditar })]
		public ActionResult ObterFiscalizacao(int fiscalizacaoId)
		{
			if (!_busFiscalizacao.ValidarAssociar(fiscalizacaoId))
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			FiscalizacaoBus fisBus = new FiscalizacaoBus();
			Pessoa autuado = fisBus.ObterAutuado(fiscalizacaoId);

			return Json(new { @EhValido = Validacao.EhValido, Autuado = autuado }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DocumentoCriar, ePermissao.DocumentoEditar })]
		public ActionResult ValidarDesassociarFiscalizacao(int fiscalizacaoId)
		{
			_busFiscalizacao.ValidarDesassociar(fiscalizacaoId);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#endregion

		#region Converter Documento

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoConverterDocumento })]
		public ActionResult ConverterDocumento()
		{
			ConverterDocumentoVM vm = new ConverterDocumentoVM();

			return View("ConverterDocumento", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoConverterDocumento })]
		public ActionResult ConverterDocumento(ConverterDocumento convertDoc)
		{
			DocumentoBus _busDoc = new DocumentoBus();
			_busDoc.ConverterDocumento(convertDoc);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoConverterDocumento })]
		public ActionResult ObterDocumentoDados(string strDocumentoNumero)
		{
			ConverterDocumentoVM vm = null;

			DocumentoBus _busDoc = new DocumentoBus();
			Documento doc = _busDoc.ObterDocumentoParaConversao(strDocumentoNumero);

			if (Validacao.EhValido && Validacao.Erros.Count == 0)
			{
				vm = new ConverterDocumentoVM(_busLista.TiposProcesso, 3);
				vm.RequerimentoVM.IsVisualizar = true;
				vm.RequerimentoVM.IsRequerimentoProcesso = true;
				vm.SetProcesso(doc, _busLista.ResponsavelFuncoes);
				vm.IsEditar = false;
				vm.RequerimentoVM.IsEditar = false;
			}
			else
			{
				return Json(new
				{
					@EhValido = false,
					@Msg = Validacao.Erros,
				}, JsonRequestBehavior.AllowGet);
			}

			return Json(new
			{
				@Id = doc.Id,
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ConverterDocumentoDadosPartial", vm),
				@VM = vm
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}