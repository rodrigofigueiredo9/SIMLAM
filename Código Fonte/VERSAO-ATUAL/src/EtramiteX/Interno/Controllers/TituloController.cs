using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloCadastroAmbientalRural.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloTitulo.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class TituloController : DefaultController
	{
		#region Propriedades

		TituloValidar _validar = new TituloValidar();
		TituloSituacaoValidar _tituloSituacaoValidar = new TituloSituacaoValidar();

		ListaBus _busLista = new ListaBus();
		TituloBus _bus = new TituloBus(new TituloValidar());
		TituloModeloBus _busModelo = new TituloModeloBus(new TituloModeloValidacao());
		EntregaBus _busEntrega = new EntregaBus(new EntregaValidar());
		TituloSituacaoBus _tituloSituacaoBus = new TituloSituacaoBus(new TituloSituacaoValidar());
		AtividadeBus _busAtividade = new AtividadeBus();
		CARSolicitacaoBus _busCar = new CARSolicitacaoBus();

		CondicionanteBus _busCondicionante = new CondicionanteBus(new CondicionanteValidar());

		GerenciadorConfiguracao<ConfiguracaoTituloModelo> _configModelo = new GerenciadorConfiguracao<ConfiguracaoTituloModelo>(new ConfiguracaoTituloModelo());

		EspecificidadeBusBase _busEspBase = new EspecificidadeBusBase();

		private string QuantidadePorPagina
		{
			get { return ViewModelHelper.CookieQuantidadePorPagina; }
		}

		#endregion

		#region Filtrar/Associar

		[Permite(RoleArray = new Object[] { ePermissao.TituloListar })]
		public ActionResult Index()
		{
			List<TituloModeloLst> modelos = _busModelo.ObterModelosSetorFunc();
			List<TituloModeloLst> modelosDeclaratorios = _busModelo.ObterModelosDeclaratorios();
			modelos = modelos.Where(x => !modelosDeclaratorios.Exists(y => y.Id == x.Id)).ToList();

			ListarVM vm = new ListarVM(_busLista.QuantPaginacao, modelos, _busLista.TituloSituacoes, _busLista.Setores);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(QuantidadePorPagina);
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloListar })]
		public ActionResult Associar(String modelosCodigos = null)
		{
			List<TituloModeloLst> modelos = _busModelo.ObterModelos();
			List<TituloModeloLst> modelosDeclaratorios = _busModelo.ObterModelosDeclaratorios();
			modelos = modelos.Where(x => !modelosDeclaratorios.Exists(y => y.Id == x.Id)).ToList();

			ListarVM vm = new ListarVM(_busLista.QuantPaginacao, modelos, _busLista.TituloSituacoes, _busLista.Setores);

			if (!string.IsNullOrEmpty(modelosCodigos))
			{
				vm.Filtros.Modelo = Convert.ToInt32(modelosCodigos.Split('@').GetValue(0));
				vm.Filtros.Modelo = modelos.SingleOrDefault(x => x.Codigo == vm.Filtros.Modelo).Id;
			}

			vm.Paginacao.QuantPaginacao = Convert.ToInt32(QuantidadePorPagina);

			return PartialView("TituloListarFiltros", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloListar })]
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

			Resultados<Titulo> resultados = _bus.Filtrar(vm.Filtros, paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			if (!vm.PodeAssociar)
			{
				vm.PodeEditar = User.IsInRole(ePermissao.TituloEditar.ToString());
				vm.PodeExcluir = User.IsInRole(ePermissao.TituloExcluir.ToString());
				vm.PodeAlterarSituacao = User.IsInRole(ePermissao.TituloProrrogar.ToString()) || User.IsInRole(ePermissao.TituloAssinar.ToString()) || User.IsInRole(ePermissao.TituloCancelarEmissao.ToString()) || User.IsInRole(ePermissao.TituloEmitir.ToString()) || User.IsInRole(ePermissao.TituloEncerrar.ToString());
				vm.PodeAlterarSituacaoCondicionante = User.IsInRole(ePermissao.CondicionanteAlterarSituacao.ToString());
			}
			vm.PodeVisualizar = User.IsInRole(ePermissao.TituloVisualizar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "TituloListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Criar/Editar

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar })]
		public ActionResult Criar()
		{
			List<Setor> lstSetores = _bus.ObterFuncionarioSetores();
			List<TituloModeloLst> modelos = lstSetores.Count > 1 ? new List<TituloModeloLst>() : _busModelo.ObterModelosSetorFunc();
			List<TituloModeloLst> modelosDeclaratorios = _busModelo.ObterModelosDeclaratorios();
			modelos = modelos.Where(x => !modelosDeclaratorios.Exists(y => y.Id == x.Id)).ToList();

			SalvarVM vm = new SalvarVM(lstSetores, modelos, _bus.ObterLocais());
			vm.Titulo.DataCriacao.Data = DateTime.Now;
			vm.SetoresEditar = lstSetores.Count <= 1;
			vm.Titulo.Autor.Nome = _bus.User.Name;
			vm.Titulo.Situacao = _busLista.TituloSituacoes.Single(x => x.Id == 1);

			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar })]
		public ActionResult CriarDeModelo(int modeloCodigo, int? protocoloId = null, bool isProcesso = false, string protocoloSelecionado = null)
		{
			List<TituloModeloLst> lstModelos = _busModelo.ObterModelos();
			SalvarVM vm = null;

			if (!lstModelos.Exists(x => x.Codigo == modeloCodigo))
			{
				Validacao.Add(Mensagem.Titulo.ModeloCodigoNaoEncontrado);
				return RedirectToAction("", Validacao.QueryParamSerializer());
			}

			int modeloId = lstModelos.SingleOrDefault(x => x.Codigo == modeloCodigo).Id;

			List<Setor> lstSetores = _bus.ObterFuncionarioSetores();
			List<Setor> lstSetoresModelo = _busModelo.ObterSetoresModelo(modeloId);
			List<Setor> setoresIntersect = _bus.SetoresModeloFuncionario(lstSetores, lstSetoresModelo);

			Protocolo protocolo = new Protocolo() { Id = protocoloId ?? 0, IsProcesso = isProcesso };
			_validar.Protocolo(new Titulo() { Protocolo = protocolo });

			if (!Validacao.EhValido)
			{
				return RedirectToAction("", Validacao.QueryParamSerializer());
			}

			List<TituloModeloLst> modelos = _busModelo.ObterModelosSetorFunc();
			List<TituloModeloLst> modelosDeclaratorios = _busModelo.ObterModelosDeclaratorios();
			modelos = modelos.Where(x => !modelosDeclaratorios.Exists(y => y.Id == x.Id)).ToList();

			if (setoresIntersect.Count >= 1)
			{
				vm = new SalvarVM(null, modelos, _bus.ObterLocais(), 0, modeloId);
				vm.Titulo.Modelo = vm.Modelo = _busModelo.Obter(modeloId);

				vm.Titulo.Protocolo.IsProcesso = (
					vm.Modelo.TipoProtocoloEnum == eTipoProtocolo.Processo ||
					vm.Modelo.TipoProtocoloEnum == eTipoProtocolo.Protocolo
				);

				vm.CarregarEspecificidade = vm.Titulo.Modelo.PossuiEspecificidade();
				vm.ProtocoloSelecionado = protocoloSelecionado;

				if ((protocoloId ?? 0) > 0)
				{
					_bus.ObterProtocolo(protocoloId.Value, isProcesso, vm.Titulo);
				}

				vm.SetaSetores(setoresIntersect, vm.Titulo.Protocolo.SetorId);
				vm.Titulo.Setor.Id = vm.Titulo.Protocolo.SetorId;
				vm.Titulo.EmpreendimentoId = vm.Titulo.Protocolo.Empreendimento.Id;
				vm.Titulo.EmpreendimentoTexto = vm.Titulo.Protocolo.Empreendimento.Denominador;
				vm.TemEmpreendimento = vm.Titulo.EmpreendimentoId.HasValue;

				List<DestinatarioEmail> destinatarios = new List<DestinatarioEmail>();
				if (vm.Modelo.Regra(eRegra.EnviarEmail))
				{
					destinatarios = _bus.ObterDestinatariosEmailProtocolo(protocolo.Id.Value);
				}

				vm.DestinatarioEmailVM.MergeDestinatario(destinatarios);

				#region Assinantes

				vm.AssinantesVM.Assinantes = _busModelo.ObterAssinantes(vm.Modelo);

				if (vm.Titulo != null)
				{
					vm.AssinantesVM.MergeAssinantesCargos(vm.Titulo.Assinantes);
				}

				List<ListaValor> lista = null;

				if (vm.AssinantesVM.Assinantes != null && vm.AssinantesVM.Assinantes.Count > 0)
				{
					vm.AssinantesVM.Assinantes = _bus.ObterAssinantesCargos(vm.AssinantesVM.Assinantes).Where(x => x.Selecionado).ToList();
					lista = vm.Modelo.Assinantes.Select(x => new ListaValor { Id = x.SetorId, Texto = x.SetorTexto }).OrderBy(x => x.Texto).ToList();

				}

				vm.AssinantesVM.Setores = ViewModelHelper.CriarSelectList(lista);
				vm.AssinantesVM.Cargos = ViewModelHelper.CriarSelectList(new List<ListaValor>());
				vm.AssinantesVM.Funcionarios = ViewModelHelper.CriarSelectList(new List<ListaValor>());

				#endregion
			}
			else
			{
				vm = new SalvarVM(lstSetores, modelos, _bus.ObterLocais());
			}

			_validar.CriarDeModelo(vm.Titulo);

			vm.Titulo.DataCriacao.Data = DateTime.Now;
			vm.SetoresEditar = lstSetores.Count <= 1;
			vm.Titulo.Autor.Nome = _bus.User.Name;
			vm.Titulo.Situacao = _busLista.TituloSituacoes.Single(x => x.Id == 1);

			return View("Criar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloEditar })]
		public ActionResult Editar(int id, bool trocarAutor = false, int? setorTrocado = null)
		{
			Titulo titulo = _bus.Obter(id);
			titulo.Modelo = _busModelo.Obter(titulo.Modelo.Id);
			SalvarVM vm = null;

			#region Nao Encontrada

			if (titulo == null || titulo.Id == 0)
			{
				Validacao.Add(Mensagem.Titulo.NaoEncontrado);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			if (!_validar.ListarEditar(titulo))
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			#endregion

			List<Setor> lstSetores = _bus.ObterFuncionarioSetores();

			if (trocarAutor)
			{
				titulo.Autor.Id = _bus.User.FuncionarioId;
				titulo.Autor.Nome = _bus.User.Name;
			}

			if (setorTrocado.GetValueOrDefault() > 0)
			{
				if (!_validar.ProtocoloSetorModelo(titulo, setorTrocado.Value))
				{
					return RedirectToAction("", Validacao.QueryParamSerializer());
				}

				titulo.Setor.Id = setorTrocado.Value;
			}

			List<TituloModeloLst> modelos = _busModelo.ObterModelosSetorFunc(titulo.Setor.Id);
			List<TituloModeloLst> modelosDeclaratorios = _busModelo.ObterModelosDeclaratorios();
			modelos = modelos.Where(x => !modelosDeclaratorios.Exists(y => y.Id == x.Id)).ToList();

			vm = new SalvarVM(lstSetores, modelos, _bus.ObterLocais(), titulo.Setor.Id, titulo.Modelo.Id, titulo.LocalEmissao.Id);
			vm.SetoresEditar = lstSetores.Count > 1;
			vm.Titulo = titulo;
			vm.Modelo = _busModelo.Obter(titulo.Modelo.Id);
			vm.Titulo.Modelo = vm.Modelo;
			vm.AtividadeEspecificidadeCaracterizacaoJSON = ViewModelHelper.Json(_busEspBase.GetConfigAtivEspCaracterizacao(vm.Modelo.Codigo.Value));

			Empreendimento emp = _bus.EmpreendimentoProcDocAlterado(titulo, true, eTipoMensagem.Informacao);
			if (emp != null)
			{
				titulo.EmpreendimentoId = emp.Id;
				titulo.EmpreendimentoTexto = emp.Denominador;
			}

			vm.TemEmpreendimento = titulo.EmpreendimentoId.HasValue;

			vm.IsEditar = true;
			vm.TituloCondicionanteVM.MostrarBotoes = true;

			if (!vm.Modelo.Regra(eRegra.PdfGeradoSistema))
			{
				ArquivoBus arqBus = new ArquivoBus(eExecutorTipo.Interno);
				titulo.ArquivoPdf = arqBus.ObterDados(titulo.ArquivoPdf.Id.GetValueOrDefault());

				vm.ArquivoId = titulo.ArquivoPdf.Id;
				vm.ArquivoTexto = titulo.ArquivoPdf.Nome;
				vm.ArquivoJSon = ViewModelHelper.JsSerializer.Serialize(titulo.ArquivoPdf);
			}

			if (vm.Modelo.Regra(eRegra.EnviarEmail))
			{
				vm.DestinatarioEmailVM.Destinatarios = _bus.ObterDestinatariosEmailProtocolo(titulo.Protocolo.Id.Value);
			}

			vm.DestinatarioEmailVM.MergeDestinatario(titulo.DestinatarioEmails);

			#region Assinantes

			vm.AssinantesVM.Assinantes = _busModelo.ObterAssinantes(vm.Modelo);

			if (titulo != null)
			{
				vm.AssinantesVM.MergeAssinantesCargos(titulo.Assinantes);
			}

			List<ListaValor> lista = null;

			if (vm.AssinantesVM.Assinantes != null && vm.AssinantesVM.Assinantes.Count > 0)
			{
				vm.AssinantesVM.Assinantes = _bus.ObterAssinantesCargos(vm.AssinantesVM.Assinantes).Where(x => x.Selecionado).ToList();
				lista = vm.Modelo.Assinantes.Select(x => new ListaValor { Id = x.SetorId, Texto = x.SetorTexto }).OrderBy(x => x.Texto).ToList();
			}

			vm.AssinantesVM.Setores = ViewModelHelper.CriarSelectList(lista);
			vm.AssinantesVM.Cargos = ViewModelHelper.CriarSelectList(new List<ListaValor>());
			vm.AssinantesVM.Funcionarios = ViewModelHelper.CriarSelectList(new List<ListaValor>());

			#endregion

			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar })]
		public ActionResult Salvar(Titulo titulo)
		{
			String urlSucesso = String.Empty;
			String acao = (titulo.Id > 0) ? "Index" : "Criar";
			if (titulo.CondicionantesJson != null)
			{
				foreach (String condicionanteJson in titulo.CondicionantesJson)
				{
					TituloCondicionante condicionante = ViewModelHelper.JsSerializer.Deserialize<TituloCondicionante>(condicionanteJson);
					if (condicionante != null)
					{
						titulo.Condicionantes.Add(condicionante);
					}
				}
			}

			_bus.Salvar(titulo);

			urlSucesso = Url.Action(acao, "Titulo", Validacao.QueryParamSerializer(new { acaoId = titulo.Id }));

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @UrlSucesso = urlSucesso });
		}

		#endregion

		#region Excluir Titulo

		[Permite(RoleArray = new Object[] { ePermissao.TituloExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();

			Titulo titulo = _bus.ObterSimplificado(id);
			vm.Id = id;
			vm.Mensagem = Mensagem.Titulo.MensagemExcluir(titulo.Numero.Texto, titulo.Modelo.Nome);
			vm.Titulo = "Excluir Título";
			return PartialView("Excluir", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloExcluir })]
		public ActionResult Excluir(int id)
		{
			_bus.Excluir(new Titulo() { Id = id });

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.TituloVisualizar })]
		public ActionResult Visualizar(int id)
		{
			Titulo titulo = _bus.Obter(id);
			SalvarVM vm = null;

			if (titulo == null)
			{
				vm = new SalvarVM(new List<Setor>(), new List<TituloModeloLst>(), new List<Municipio>());
				if (Request.IsAjaxRequest())
				{
					return PartialView("VisualizarPartial", vm);
				}

				return View(vm);
			}

			List<Setor> lstSetores = _bus.ObterFuncionarioSetores();

			vm = new SalvarVM(lstSetores, _busModelo.ObterModelos(todos: true), _bus.ObterLocais(), titulo.Setor.Id, titulo.Modelo.Id, titulo.LocalEmissao.Id);
			vm.SetoresEditar = false;
			vm.Titulo = titulo;
			vm.Modelo = _busModelo.Obter(titulo.Modelo.Id);
			vm.Titulo.Modelo = vm.Modelo;
			vm.AtividadeEspecificidadeCaracterizacaoJSON = ViewModelHelper.Json(_busEspBase.GetConfigAtivEspCaracterizacao(vm.Modelo.Codigo.Value));

			vm.IsVisualizar = true;
			vm.AssinantesVM.IsVisualizar = true;
			vm.DestinatarioEmailVM.IsVisualizar = true;
			vm.TituloCondicionanteVM.MostrarBotoes = false;

			vm.LabelTipoPrazo = vm.Titulo.PrazoUnidade;

			vm.DestinatarioEmailVM.Destinatarios = vm.Titulo.DestinatarioEmails;

			vm.AssinantesVM.Assinantes = _busModelo.ObterAssinantes(vm.Modelo);

			if (titulo != null)
			{
				List<TituloAssinante> assinantesDoTitulo = _bus.ObterAssinantes(id);
				vm.AssinantesVM.MergeAssinantesCargos(assinantesDoTitulo);
				vm.AssinantesVM.Assinantes = _bus.ObterAssinantesCargos(vm.AssinantesVM.Assinantes).Where(x => x.Selecionado).ToList();
			}

			if (!vm.Modelo.Regra(eRegra.PdfGeradoSistema))
			{
				ArquivoBus arqBus = new ArquivoBus(eExecutorTipo.Interno);
				titulo.ArquivoPdf = arqBus.ObterDados(titulo.ArquivoPdf.Id.GetValueOrDefault());

				vm.ArquivoId = titulo.ArquivoPdf.Id;
				vm.ArquivoTexto = titulo.ArquivoPdf.Nome;
				vm.ArquivoJSon = ViewModelHelper.JsSerializer.Serialize(titulo.ArquivoPdf);
			}

			if (Request.IsAjaxRequest())
			{
				return PartialView("VisualizarPartial", vm);
			}

			return View(vm);
		}

		#endregion

		#region Entregar

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloEntregar })]
		public ActionResult Entregar()
		{
			EntregaVM vm = new EntregaVM();

			if (Request.IsAjaxRequest())
			{
				return PartialView("EntregaPartial", vm);
			}
			else
			{
				return View("Entrega", vm);
			}
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TituloEntregar })]
		public ActionResult EntregarCriar(EntregaVM vm)
		{
			_busEntrega.Salvar(vm.Entrega);

			string urlRedirecionar = Url.Action("Entregar", "Titulo");
			urlRedirecionar += "?Msg=" + Validacao.QueryParam() + "&acaoId=" + vm.Entrega.Id.ToString();

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @urlRedirecionar = urlRedirecionar }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloEntregar })]
		public ActionResult ObterPessoaEntrega(string cpf)
		{
			Pessoa pessoa = _busEntrega.ObterPessoa(cpf);
			return Json(new { @Pessoa = pessoa, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloEntregar })]
		public ActionResult ObterTitulosProcesso(EntregaVM vm)
		{
			Entrega entrega = _busEntrega.ObterTituloEntrega(vm.Entrega.Protocolo);
			return Json(new { @Lista = entrega.TitulosEntrega, @ProtocoloId = entrega.Protocolo.Id, @ProcDocIsProcesso = entrega.Protocolo.IsProcesso, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloEntregar })]
		public ActionResult ObterDocumentoTipos()
		{
			return Json(new { @Lista = _busLista.TiposDocumento }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloEntregar })]
		public ActionResult ObterProcessoTipos()
		{
			return Json(new { @Lista = _busLista.TiposProcesso }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloEntregar })]
		public ActionResult ObterTitulosSitucaoAssinado(EntregaVM vm)
		{
			string numerosConcatenados = _busEntrega.ObterTitulosConcluirEntrega(vm.Entrega.Protocolo, vm.Entrega.Titulos);
			return Json(new { @NumerosConcatenados = numerosConcatenados, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloEntregar })]
		public ActionResult EntregaConfirm(string titulos)
		{
			ConfirmarVM vm = new ConfirmarVM();
			vm.Mensagem = Mensagem.Entrega.ConfirmModal(titulos);
			vm.Titulo = "Confirmar Entrega";
			return PartialView("Confirmar", vm);
		}

		#endregion

		#region Modelo de Titulo

		#region Modelo de Titulo - Editar

		[Permite(RoleArray = new Object[] { ePermissao.TituloModeloEditar })]
		public ActionResult TituloModeloEditar(int id)
		{
			TituloModeloVM vm = new TituloModeloVM();
			vm.ModeloId = id;
			vm.SetSetores(_busLista.SetoresAtuais);
			vm.SetListItens(_busLista.TituloModeloTipos,
				_busLista.TituloModeloProtocoloTipos,
				_busLista.TituloModeloPeriodosRenovacoes,
				_busLista.TituloModeloIniciosPrazos,
				_busLista.TituloModeloTiposPrazos,
				_busModelo.ObterModelos(id),
				_busLista.TituloModeloAssinantes,
				_busLista.TituloModeloTipoDocumento);

			vm.Modelo = _busModelo.Obter(id);

			vm.Modelo.Arquivo = vm.Modelo.Arquivo ?? new Arquivo();
			vm.ArquivoJSon = ViewModelHelper.JsSerializer.Serialize(vm.Modelo.Arquivo);

			return PartialView("TituloModeloEditar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TituloModeloEditar })]
		public ActionResult TituloModeloEditar(TituloModeloVM vm)
		{
			_busModelo.Salvar(vm.Modelo);

			return Json(new
			{
				@Msg = Validacao.Erros,
				@EhValido = Validacao.EhValido,
				UrlRedireciona = Url.Action("TituloModeloListar", "Titulo") + "?Msg=" + Validacao.QueryParam()
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloModeloEditar })]
		public ActionResult VerificarPublicoExternoAtividade(int id)
		{
			_busModelo.VerificarPublicoExternoAtividade(id);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloModeloEditar })]
		public ActionResult ObterUltimoNumeroGerado(int id, bool reiniciaPorAno)
		{
			return Json(new { @UltimoNumeroGerado = _busModelo.ObterUltimoNumeroGerado(id, reiniciaPorAno), @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloModeloEditar })]
		public ActionResult ObterTextoPadraoEmail(int id)
		{
			return Json(new { @TextoPadrao = _busLista.ModeloTextoEmailDefault, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Modelo de Titulo - Listar

		[Permite(RoleArray = new Object[] { ePermissao.TituloModeloListar })]
		public ActionResult TituloModeloListar()
		{
			TituloModeloListarVM vm = new TituloModeloListarVM(_busLista.QuantPaginacao, _busLista.SetoresAtuais, _busLista.TituloModeloTipos, _busModelo.ObterSituacoes());
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(QuantidadePorPagina);
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloModeloListar })]
		public ActionResult TituloModeloFiltrar(TituloModeloListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<TituloModeloListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(_busLista.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<TituloModelo> resultados = _busModelo.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeEditar = User.IsInRole(ePermissao.TituloModeloEditar.ToString());
			vm.PodeVisualizar = User.IsInRole(ePermissao.TituloModeloVisualizar.ToString());
			vm.PodeAlterarSituacao = User.IsInRole(ePermissao.TituloModeloAlterarSituacao.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "TituloModeloListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Modelo de Titulo - Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.TituloModeloVisualizar })]
		public ActionResult TituloModeloVisualizar(int id)
		{
			TituloModeloVM vm = new TituloModeloVM();
			vm.ModeloId = id;
			vm.SetSetores(_busLista.SetoresAtuais);
			vm.SetListItens(
				_busLista.TituloModeloTipos,
				_busLista.TituloModeloProtocoloTipos,
				_busLista.TituloModeloPeriodosRenovacoes,
				_busLista.TituloModeloIniciosPrazos,
				_busLista.TituloModeloTiposPrazos,
				_busModelo.ObterModelos(),
				_busLista.TituloModeloAssinantes,
				_busLista.TituloModeloTipoDocumento);

			vm.Modelo = _busModelo.Obter(id);
			vm.ArquivoJSon = ViewModelHelper.JsSerializer.Serialize(vm.Modelo.Arquivo);

			return PartialView("TituloModeloVisualizar", vm);
		}

		#endregion

		#region Modelo de Título - Desativar e Ativar

		[Permite(RoleArray = new Object[] { ePermissao.TituloModeloAlterarSituacao })]
		public ActionResult DesativarConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			TituloModelo modelo = _busModelo.Obter(id);
			vm.Id = id;
			vm.Mensagem = Mensagem.TituloModelo.DesativarModelo(modelo.Nome, modelo.Sigla);
			vm.Titulo = "Desativar Modelo de Título";
			return PartialView("Excluir", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloModeloAlterarSituacao })]
		public ActionResult Desativar(int id)
		{
			_busModelo.AlterarSituacaoModeloTitulo(new TituloModelo() { Id = id, SituacaoId = (int)eTituloModeloSituacao.Desativado });
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloModeloAlterarSituacao })]
		public ActionResult Ativar(int id)
		{
			_busModelo.AlterarSituacaoModeloTitulo(new TituloModelo() { Id = id, SituacaoId = (int)eTituloModeloSituacao.Ativo });
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#endregion

		#region Condicionante

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar })]
		public ActionResult CondicionanteCriar()
		{
			CondicionanteVM vm = new CondicionanteVM(_busLista.TituloCondicionantePeriodicidades);
			vm.Condicionante.DataCriacao.Data = DateTime.Now;
			vm.Condicionante.Situacao = _busLista.TituloCondicionanteSituacoes[0];

			return View("CondicionanteCriar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar })]
		public ActionResult CondicionanteCriarValidar(TituloCondicionante condicionante)
		{
			condicionante.DataCriacao.Data = DateTime.Now;

			_busCondicionante.SalvarValidar(condicionante);

			if (Validacao.EhValido)
			{
				condicionante.Situacao.Texto = _busLista.TituloCondicionanteSituacoes.First<TituloCondicionanteSituacao>().Texto;
			}
			return Json(new
			{
				@Msg = Validacao.Erros,
				@EhValido = Validacao.EhValido,
				@condicionante = condicionante,
				@condicionanteJson = ViewModelHelper.JsSerializer.Serialize(condicionante)
			});
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar })]
		public ActionResult CondicionanteEditar(string condicionanteJson)
		{
			CondicionanteVM vm = new CondicionanteVM(_busLista.TituloCondicionantePeriodicidades);
			vm.Condicionante = ViewModelHelper.JsSerializer.Deserialize<TituloCondicionante>(condicionanteJson);
			if (vm.Condicionante.Id <= 0)
			{
				vm.Condicionante.DataCriacao.Data = DateTime.Now;
			}
			return View("CondicionanteEditar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloListar })]
		public ActionResult CondicionanteVisualizar(int? condicionanteId = 0, string condicionanteJson = null)
		{
			CondicionanteVM vm = new CondicionanteVM(_busLista.TituloCondicionantePeriodicidades);

			if (!String.IsNullOrEmpty(condicionanteJson))
			{
				vm.Condicionante = ViewModelHelper.JsSerializer.Deserialize<TituloCondicionante>(condicionanteJson);
			}
			else
			{
				vm.Condicionante = _busCondicionante.Obter(condicionanteId.Value);
			}

			if (vm.Condicionante.Id <= 0)
			{
				vm.Condicionante.DataCriacao.Data = DateTime.Now;
				vm.Condicionante.Situacao = _busLista.TituloCondicionanteSituacoes[0];
			}

			return View("CondicionanteVisualizar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar })]
		public ActionResult CondicionanteAssociarTitulo(TituloCondicionante condicionante)
		{
			_busCondicionante.ValidarAssociarTitulo(condicionante);
			return Json(new { @Msg = Validacao.Erros, @EhValido = Validacao.EhValido, @CondicionantJson = condicionante });
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar })]
		public ActionResult CondicionanteEditarValidar(TituloCondicionante condicionante)
		{
			if (condicionante.Id <= 0)
			{
				condicionante.DataCriacao.Data = DateTime.Now;
				condicionante.Situacao = _busLista.TituloCondicionanteSituacoes[0];
			}
			else
			{
				condicionante.Situacao = _busLista.TituloCondicionanteSituacoes.Single(x => x.Id == condicionante.Situacao.Id);
			}
			_busCondicionante.SalvarValidar(condicionante);
			return Json(new
			{
				@Msg = Validacao.Erros,
				@EhValido = Validacao.EhValido,
				@condicionante = condicionante,
				@condicionanteJson = ViewModelHelper.JsSerializer.Serialize(condicionante)
			});
		}

		#region Descrição de Condicionante
		//Descrição-----------------------------------------------------------------------------
		[Permite(RoleArray = new Object[] { ePermissao.CondicionanteDescricaoListar })]
		public ActionResult CondicionanteDescricaoFiltrar()
		{
			CondicionanteDescricaoListarVM vm = new CondicionanteDescricaoListarVM(_busLista.QuantPaginacao);
			return PartialView("CondicionanteDescricaoFiltros", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CondicionanteDescricaoListar })]
		public ActionResult CondicionanteDescricaoFiltrar(CondicionanteDescricaoListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<CondicionanteDescricaoListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(QuantidadePorPagina);
			vm.Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(_busLista.QuantPaginacao, false, false, QuantidadePorPagina);

			Resultados<TituloCondicionante> resultados = _busCondicionante.FiltrarDescricao(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.Paginacao.QuantidadeRegistros = Convert.ToInt32(resultados.Quantidade);
			vm.Paginacao.EfetuarPaginacao();

			//deve ser setado apos a serializacao da ultimabusca
			vm.SetResultados(resultados.Itens);
			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "CondicionanteDescricaoResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.CondicionanteDescricaoCriar })]
		public ActionResult CondicionanteDescricaoCriar()
		{
			TituloCondicionanteDescricao condDesc = new TituloCondicionanteDescricao();
			return View("CondicionanteDescricaoCriar", condDesc);
		}

		[Permite(RoleArray = new Object[] { ePermissao.CondicionanteDescricaoCriar })]
		public ActionResult CondicionanteDescricaoEditar(int id)
		{
			TituloCondicionanteDescricao condDesc = new TituloCondicionanteDescricao() { Id = id };
			if (_busCondicionante.DescricaoValidarEditar(id))
			{
				condDesc = _busCondicionante.DescricaoObter(id);
			}
			return PartialView("CondicionanteDescricaoEditar", condDesc);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CondicionanteDescricaoCriar, ePermissao.CondicionanteDescricaoEditar })]
		public ActionResult CondicionanteDescricaoSalvar(TituloCondicionanteDescricao descricao)
		{
			_busCondicionante.DescricaoSalvar(descricao);
			return Json(new { @Msg = Validacao.Erros, @EhValido = Validacao.EhValido });
		}

		[Permite(RoleArray = new Object[] { ePermissao.CondicionanteDescricaoExcluir })]
		public ActionResult CondicionanteDescricaoExcluir(int id)
		{
			TituloCondicionanteDescricao cond = new TituloCondicionanteDescricao() { Id = id };
			_busCondicionante.DescricaoValidarExcluir(cond);
			return PartialView("CondicionanteDescricaoExcluir", cond);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CondicionanteDescricaoExcluir })]
		public ActionResult CondicionanteDescricaoExcluirSalvar(int id)
		{
			_busCondicionante.DescricaoExcluir(id);
			return Json(new { @Msg = Validacao.Erros, @EhValido = Validacao.EhValido });
		}

		//Descrição-fim-------------------------------------------------------------------------
		#endregion

		[Permite(RoleArray = new Object[] { ePermissao.CondicionanteAlterarSituacao })]
		public ActionResult CondicionanteSituacaoAlterar(int id)
		{
			List<TituloModeloLst> modelos = _busModelo.ObterModelos();
			List<TituloModeloLst> modelosDeclaratorios = _busModelo.ObterModelosDeclaratorios();
			modelos = modelos.Where(x => !modelosDeclaratorios.Exists(y => y.Id == x.Id)).ToList();

			CondicionanteSituacaoAlterarVM vm = new CondicionanteSituacaoAlterarVM(modelos);
			vm.Titulo = _bus.ObterSimplificado(id);
			vm.Titulo.Condicionantes = _bus.ObterCondicionantes(id);

			return PartialView("CondicionanteSituacaoAlterar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.CondicionanteAlterarSituacao })]
		public ActionResult CondicionanteProrrogar(int condicionanteId, int periodicidadeId = 0)
		{
			CondicionanteSituacaoProrrogarVM vm = new CondicionanteSituacaoProrrogarVM();
			vm.CondicionanteId = condicionanteId;
			vm.PeriodicidadeId = periodicidadeId;
			return PartialView("CondicionanteProrrogar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CondicionanteAlterarSituacao })]
		public ActionResult CondicionanteProrrogarSalvar(CondicionanteSituacaoProrrogarVM vm)
		{
			TituloCondicionante cond = _busCondicionante.Prorrogar(vm.CondicionanteId, vm.PeriodicidadeId, vm.Dias);
			TituloCondicionantePeriodicidade perio = cond.Periodicidades.SingleOrDefault(x => x.Id == vm.PeriodicidadeId);

			return Json(new { @Msg = Validacao.Erros, Condicionante = cond, Periodicidade = perio, @EhValido = Validacao.EhValido });
		}

		[Permite(RoleArray = new Object[] { ePermissao.CondicionanteAlterarSituacao })]
		public ActionResult CondicionanteAtender(int condicionanteId, int periodicidadeId = 0)
		{
			CondicionanteSituacaoAtenderVM vm = new CondicionanteSituacaoAtenderVM();
			vm.Condicionante = _busCondicionante.Obter(condicionanteId);
			vm.PeriodicidadeId = periodicidadeId;
			return PartialView("CondicionanteAtender", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CondicionanteAlterarSituacao })]
		public ActionResult CondicionanteAtenderSalvar(int condicionanteId, int periodicidadeId = 0)
		{
			TituloCondicionante cond = _busCondicionante.Atender(condicionanteId, periodicidadeId);
			TituloCondicionantePeriodicidade perio = cond.Periodicidades.SingleOrDefault(x => x.Id == periodicidadeId);

			return Json(new { @Msg = Validacao.Erros, Condicionante = cond, Periodicidade = perio, @EhValido = Validacao.EhValido });
		}

		#endregion

		#region Alterar Situação

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TituloEmitir, ePermissao.TituloCancelarEmissao, ePermissao.TituloAssinar, ePermissao.TituloProrrogar, ePermissao.TituloEncerrar })]
		public ActionResult ValidarAlterarSituacao(int id)
		{
			Titulo titulo = _bus.ObterSimplificado(id);
			_tituloSituacaoValidar.AlterarSituacaoAbrir(titulo);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloEmitir, ePermissao.TituloCancelarEmissao, ePermissao.TituloAssinar, ePermissao.TituloProrrogar, ePermissao.TituloEncerrar })]
		public ActionResult AlterarSituacao(int id)
		{
			Titulo titulo = _bus.ObterSimplificado(id);
			if (!_tituloSituacaoValidar.AlterarSituacaoAbrir(titulo))
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			titulo.Modelo = _busModelo.Obter(titulo.Modelo.Id);

			var busCar = new CARSolicitacaoBus();
			var codigoSicar = titulo.Modelo.Codigo == (int)eTituloModeloCodigo.AutorizacaoExploracaoFlorestal ? busCar.ObterCodigoSicarPorEmpreendimento(titulo.EmpreendimentoId.GetValueOrDefault(0)) : "";
			AlterarSituacaoVM vm = new AlterarSituacaoVM(_busLista.MotivosEncerramento, titulo, codigoSicar: codigoSicar);
			vm.AcoesAlterar = _busLista.TituloAlterarSituacaoAcoes;
			vm.AcoesAlterar = _tituloSituacaoBus.SetarAcoesTela(vm.AcoesAlterar, titulo);
			vm.MostrarPrazo = titulo.Modelo.Regra(eRegra.Prazo);
			vm.PrazoAutomatico = titulo.Modelo.Codigo == (int)eTituloModeloCodigo.CertificadoRegistroAtividadeFlorestal;

			_tituloSituacaoValidar.ValidarParaConcluir(titulo);

			if (vm.MostrarPrazo)
			{
				vm.LabelPrazo = (titulo.Modelo.Resposta(eRegra.Prazo, eResposta.TipoPrazo).Valor.ToString() == "1" ? "dias" : "anos");//dia
			}

			if (vm.MostrarPrazo && titulo.Modelo.Resposta(eRegra.Prazo, eResposta.InicioPrazo).Valor.ToString() == "2")//data da assinatura
			{
				vm.DataConclusao = titulo.DataAssinatura.DataTexto;
			}
			else
			{
				vm.DataConclusao = titulo.DataEmissao.DataTexto;
			}

			if (Request.IsAjaxRequest())
			{
				return PartialView("AlterarSituacaoPartial", vm);
			}

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TituloEmitir, ePermissao.TituloCancelarEmissao, ePermissao.TituloAssinar, ePermissao.TituloProrrogar, ePermissao.TituloEncerrar })]
		public ActionResult AlterarSituacao(Titulo titulo, int acao, bool gerouPdf = true)
		{
			Titulo tituloAtual = _bus.Obter(titulo.Id);
			tituloAtual.Modelo = _busModelo.Obter(tituloAtual.Modelo.Id);

			tituloAtual.Prazo = titulo.Prazo.HasValue ? titulo.Prazo : tituloAtual.Prazo;
			tituloAtual.DiasProrrogados = titulo.DiasProrrogados.HasValue ? titulo.DiasProrrogados : tituloAtual.DiasProrrogados;
			tituloAtual.MotivoEncerramentoId = titulo.MotivoEncerramentoId.HasValue ? titulo.MotivoEncerramentoId : tituloAtual.MotivoEncerramentoId;

			tituloAtual.DataAssinatura = (titulo.DataAssinatura.IsEmpty) ? tituloAtual.DataAssinatura : titulo.DataAssinatura;
			tituloAtual.DataEmissao = (titulo.DataEmissao.IsEmpty && acao == (int)eAlterarSituacaoAcao.Concluir) ? tituloAtual.DataEmissao : titulo.DataEmissao;
			tituloAtual.DataEncerramento = (titulo.DataEncerramento.IsEmpty && acao == (int)eAlterarSituacaoAcao.Encerrar) ? tituloAtual.DataEncerramento : titulo.DataEncerramento;

			_tituloSituacaoBus.AlterarSituacao(tituloAtual, acao, gerouPdf);

			if (Validacao.EhValido)
			{
				return Json(new { @EhValido = true, @Msg = Validacao.QueryParam(), @AcaoId = titulo.Id });
			}
			else
			{
				return Json(new { @EhValido = false, @Msg = Validacao.Erros });
			}
		}

		#endregion

		#region Alterar Autor Setor

		public ActionResult AlterarAutorSetor(int id, bool trocarAutor, bool trocarSetor)
		{
			Titulo titulo = _bus.ObterSimplificado(id);
			List<Setor> setoresIntersect = new List<Setor>();

			if (trocarSetor)
			{
				setoresIntersect = _bus.ObterSetoresFuncContidoModelo(id);
			}

			AlterarAutorSetorVM vm = new AlterarAutorSetorVM(setoresIntersect, titulo, _bus.User.Name);
			vm.TrocarAutor = trocarAutor;
			vm.TrocarSetor = trocarSetor;

			return PartialView("AlterarAutorSetor", vm);
		}

		#endregion

		#region Atividade Especificiade/Destinatarios Representante

		public ActionResult AtividadeEspecificidade(int tituloId)
		{


			return PartialView();
		}

		#endregion

		#region Auxiliar

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar })]
		public ActionResult TituloProtocolo(int modeloId)
		{
			SalvarVM vm = new SalvarVM();
			vm.Modelo = _busModelo.Obter(modeloId);
			vm.AtividadeEspecificidadeCaracterizacaoJSON = ViewModelHelper.Json(_busEspBase.GetConfigAtivEspCaracterizacao(vm.Modelo.Codigo.Value));

			//Obriga seleção do radio conforme configuracao do modelo!
			vm.Titulo.Protocolo.IsProcesso = (vm.Modelo.TipoProtocolo == 1 || vm.Modelo.TipoProtocolo == 3);

			//string htmlCampos = String.Empty;
			string htmlCampos = ViewModelHelper.RenderPartialViewToString(ControllerContext, "TituloProtocolo", vm);

			return Json(new { Msg = Validacao.Erros, @Html = htmlCampos }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar })]
		public ActionResult TituloCamposModelo(int modeloId, int? SetorId)
		{
			SalvarVM vm = new SalvarVM();
			vm.Modelo = _busModelo.Obter(modeloId);

			var lista = vm.Modelo.Assinantes.Select(x => new ListaValor { Id = x.SetorId, Texto = x.SetorTexto }).OrderBy(x => x.Texto).ToList();

			vm.AssinantesVM.Setores = ViewModelHelper.CriarSelectList(lista);
			vm.AssinantesVM.Cargos = ViewModelHelper.CriarSelectList(new List<ListaValor>());
			vm.AssinantesVM.Funcionarios = ViewModelHelper.CriarSelectList(new List<ListaValor>());

			vm.TituloCondicionanteVM.MostrarBotoes = true;
			string htmlCampos = ViewModelHelper.RenderPartialViewToString(ControllerContext, "TituloCamposModelo", vm);

			return Json(new { Msg = Validacao.Erros, @Html = htmlCampos }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar })]
		public ActionResult ObterModelosCadastradosSetor(int id)
		{
			List<TituloModeloLst> modelos = _busModelo.ObterModelosSetorFunc(id);
			List<TituloModeloLst> modelosDeclaratorios = _busModelo.ObterModelosDeclaratorios();
			modelos = modelos.Where(x => !modelosDeclaratorios.Exists(y => y.Id == x.Id)).ToList();

			return Json(modelos, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar })]
		public ActionResult ObterProtocolo(int id, bool isProcesso)
		{
			List<PessoaLst> lista = _bus.ObterInteressadoRepresentantes(new Protocolo() { Id = id, IsProcesso = isProcesso });
			IProtocolo protocolo = _bus.BusProtocolo.ObterSimplificado(id);
			if (lista.Count == 0)
			{
				lista.Add(new PessoaLst() { Id = protocolo.Interessado.Id });
			}
			return Json(new
			{
				@Objeto = new
				{
					@EmpreendimentoId = protocolo.Empreendimento.Id,
					@EmpreendimentoNome = protocolo.Empreendimento.Denominador,
					@Interessado = protocolo.Interessado,
					@Representantes = lista
				}
			});
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar })]
		public ActionResult ObterProcessosDocumentos(Protocolo protocolo)
		{
			return Json(new { Msg = Validacao.Erros, Valores = _bus.ObterProcessosDocumentos(protocolo.Id.Value) });
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar })]
		public ActionResult ObterAtividades(Protocolo protocolo)
		{
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();

			if (protocolo != null)
			{
				lstAtividades = _busAtividade.ObterAtividadesLista(protocolo);
			}

			return Json(lstAtividades, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar })]
		public ActionResult ValidarAssociarProtocolo(int modeloId, int setorId, int id, bool isProcesso)
		{
			Titulo titulo = new Titulo();
			titulo.Setor.Id = setorId;
			titulo.Modelo = _busModelo.Obter(modeloId);
			titulo.Protocolo = new Protocolo() { Id = id, IsProcesso = isProcesso };

			_validar.AssociarProtocolo(titulo);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloEditar })]
		public ActionResult ValidarAbrirAlterarAutorSetor(int id)
		{
			Titulo titulo = _bus.Obter(id);
			titulo.Modelo = _busModelo.Obter(titulo.Modelo.Id);

			_validar.ListarEditar(titulo);

			if (!Validacao.EhValido)
			{
				return Json(new { @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			object objeto = _bus.ValidarAbrirAlterarAutorSetor(id);

			if (Validacao.EhValido)
			{
				return Json(objeto, JsonRequestBehavior.AllowGet);
			}
			else
			{
				return Json(new { @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloEmitir, ePermissao.TituloCancelarEmissao, ePermissao.TituloAssinar, ePermissao.TituloProrrogar, ePermissao.TituloEncerrar })]
		public ActionResult ValidarObterSituacao(int id, int acao)
		{
			Titulo titulo = _bus.ObterSimplificado(id);
			titulo.Modelo = _busModelo.Obter(titulo.Modelo.Id);

			return Json(new { @Situacao = _tituloSituacaoBus.ObterNovaSituacao(titulo, acao), @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar })]
		public ActionResult ObterDestinatarioEmails(Protocolo protocolo, int modeloId)
		{
			List<DestinatarioEmail> destinatarios = new List<DestinatarioEmail>();
			TituloModelo modelo = _busModelo.Obter(modeloId);

			if (modelo.Regra(eRegra.EnviarEmail))
			{
				destinatarios = _bus.ObterDestinatariosEmailProtocolo(protocolo.Id.Value);
			}

			if (destinatarios.Count <= 0)
			{
				Validacao.Add(Mensagem.Titulo.DestinatarioEmailsInexistentes);
			}

			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, IsEnviarEmail = modelo.Regra(eRegra.EnviarEmail), Destinatarios = destinatarios });
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar })]
		public ActionResult ObterDestinatarioEspecificidade(Protocolo protocolo)
		{
			List<PessoaLst> destinatarios = _bus.ObterDestinatarios(protocolo.Id.Value);

			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, Destinatarios = destinatarios });
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar })]
		public ActionResult ValidarGerarTituloPendencia(Protocolo protocolo)
		{
			Titulo titulo = new Titulo();
			titulo.Protocolo = protocolo;

			_validar.GerarTituloPendencia(titulo);
			_validar.ModeloTituloTipoValidos(titulo);
			// TODO (prorrogado): validar se funcionário pertence ao setor configurado para emitir título de pendência 
			// esta validação já está sendo feita mais pra frente

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar })]
		public ActionResult ObterTituloAnteriorAtividade(Protocolo protocolo, int atividadeId, int modeloCodigo)
		{
			Finalidade finalidade = _bus.ObterTituloAnteriorAtividade(protocolo, atividadeId, modeloCodigo);

			if (finalidade == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
			}

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Objeto = new
				{
					@TituloAnteriorId = finalidade.TituloAnteriorId,
					@Numero = finalidade.TituloAnteriorNumero,
					@Modelo = finalidade.TituloModeloAnteriorTexto,
					@ModeloSigla = finalidade.TituloModeloAnteriorSigla,
					@OrgaoExpedidor = finalidade.OrgaoExpedidor
				}
			});
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar })]
		public ActionResult ValidarPossuiCondicionante(int id)
		{
			Titulo titulo = _bus.ObterSimplificado(id);
			titulo.Modelo = _busModelo.Obter(titulo.Modelo.Id);

			_validar.PossuiCondicionante(titulo);

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar })]
		public ActionResult ObterAssinanteCargos(int id, int modeloId)
		{
			var lista = _busModelo.ObterAssinanteCargos(modeloId, id);

			return Json(lista, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar })]
		public ActionResult ObterAssinanteFuncionarios(int id, int modeloId, int setorId)
		{
			var lista = _busModelo.ObterAssinanteFuncionarios(modeloId, setorId, id);

			return Json(lista, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region PDF

		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizarpdfentrega, Artefato = (int)eHistoricoArtefato.titulo)]
		public ActionResult GerarPdfEntrega(int id)
		{
			try
			{
				return ViewModelHelper.GerarArquivoPdf(new PdfEntrega().Gerar(id), "Entrega", dataHoraControleAcesso: true);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return RedirectToAction("Index", Validacao.QueryParamSerializer());
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloListar, ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar, ePermissao.TituloEmitir, ePermissao.TituloCancelarEmissao, ePermissao.TituloAssinar, ePermissao.TituloProrrogar, ePermissao.TituloEncerrar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizarpdf, Artefato = (int)eHistoricoArtefato.titulo)]
		public ActionResult GerarPdf(int id)
		{
			try
			{
				Arquivo arquivo = _bus.GerarPdf(id);
				arquivo.Nome = arquivo.Nome.RemoverAcentos() + ".pdf";

				Titulo titulo = _bus.ObterSimplificado(id);
				titulo.Modelo = _bus.ObterModelo(titulo.Modelo.Id);

				if (titulo.Modelo.Codigo == 19 || titulo.Modelo.Codigo == 20)
				{
					return GerarPDF(titulo.Modelo.Codigo.GetValueOrDefault(0), arquivo);
				}

				if (arquivo != null && Validacao.EhValido)
				{
					return ViewModelHelper.GerarArquivo(arquivo, dataHoraControleAcesso: true);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return RedirectToAction("Index", Validacao.QueryParamSerializer());
		}

		private FileResult GerarPDF(int tituloModeloCodigo, Arquivo arquivo)
		{
			try
			{
				if (arquivo.Buffer.CanSeek)
				{
					arquivo.Buffer.Seek(0, SeekOrigin.Begin);
					arquivo.Buffer = Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.PdfMetodosAuxiliares.AdicionarDataHoraControleAcesso(arquivo.Buffer, tituloModeloCodigo);

					FileStreamResult fileResult = new FileStreamResult(arquivo.Buffer, arquivo.ContentType);
					fileResult.FileDownloadName = arquivo.Nome;

					return fileResult;
				}
				else if (arquivo.Buffer is MemoryStream)
				{
					// Memorystreams criados por itext sao fechados, portanto use array de bytes
					MemoryStream stream = arquivo.Buffer as MemoryStream;
					using (MemoryStream msTemp = new MemoryStream(stream.ToArray()))
					{
						stream.Close();
						stream.Dispose();
						stream = Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.PdfMetodosAuxiliares.AdicionarDataHoraControleAcesso(msTemp, tituloModeloCodigo);
					}

					FileContentResult pdf = new FileContentResult(stream.ToArray(), arquivo.ContentType);
					pdf.FileDownloadName = arquivo.Nome;
					return pdf;
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public ActionResult BaixarDemonstrativoCar(int id, bool isTitulo)
		{
			var url = _busCar.ObterUrlDemonstrativo(id, 0, isTitulo);

			return Json(new { @UrlPdfDemonstrativo = url }, JsonRequestBehavior.AllowGet);
		}


		#endregion
	}
}