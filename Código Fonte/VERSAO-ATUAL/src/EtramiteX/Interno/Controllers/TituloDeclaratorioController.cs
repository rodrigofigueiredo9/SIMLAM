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
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
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
using Tecnomapas.EtramiteX.Interno.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class TituloDeclaratorioController : DefaultController
	{
		#region Propriedades

		TituloDeclaratorioBus _bus = new TituloDeclaratorioBus(new TituloDeclaratorioValidar());
		TituloDeclaratorioValidar _validar = new TituloDeclaratorioValidar();

		ListaBus _busLista = new ListaBus();
		TituloModeloBus _busModelo = new TituloModeloBus(new TituloModeloValidacao());
		EspecificidadeBusBase _busEspBase = new EspecificidadeBusBase();

		private string QuantidadePorPagina
		{
			get { return ViewModelHelper.CookieQuantidadePorPagina; }
		}

		#endregion

		#region Filtrar/Associar

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioListar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(_busLista.QuantPaginacao, _busModelo.ObterModelosDeclaratorios(), _busLista.TituloDeclaratorioSituacoes, _busLista.Setores, _busLista.SistemaOrigem);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(QuantidadePorPagina);
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioListar })]
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
				vm.PodeEditar = User.IsInRole(ePermissao.TituloDeclaratorioEditar.ToString());
				vm.PodeExcluir = User.IsInRole(ePermissao.TituloDeclaratorioExcluir.ToString());
				vm.PodeAlterarSituacao = User.IsInRole(ePermissao.TituloDeclaratorioAlterarSituacao.ToString());
			}
			vm.PodeVisualizar = User.IsInRole(ePermissao.TituloDeclaratorioVisualizar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "TituloListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Criar/Editar

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioCriar })]
		public ActionResult Criar()
		{
			SalvarVM vm = new SalvarVM(new List<Setor>(), _busModelo.ObterModelosDeclaratorios(), _bus.ObterLocais());
			vm.Titulo.DataCriacao.Data = DateTime.Now;
			vm.Titulo.Autor.Nome = _bus.User.Name;
			vm.Titulo.Situacao = _busLista.TituloDeclaratorioSituacoes.Single(x => x.Id == (int)eTituloSituacao.EmCadastro);

			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioEditar })]
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

			vm = new SalvarVM(lstSetores, _busModelo.ObterModelosDeclaratorios(), _bus.ObterLocais(), titulo.Setor.Id, titulo.Modelo.Id, titulo.LocalEmissao.Id);
			vm.SetoresEditar = lstSetores.Count > 1;
			vm.Titulo = titulo;
			vm.Modelo = _busModelo.Obter(titulo.Modelo.Id);
			vm.Titulo.Modelo = vm.Modelo;
			vm.AtividadeEspecificidadeCaracterizacaoJSON = ViewModelHelper.Json(_busEspBase.GetConfigAtivEspCaracterizacao(vm.Modelo.Codigo.Value));
			vm.TemEmpreendimento = titulo.EmpreendimentoId.HasValue;
			vm.IsEditar = true;

			if (!vm.Modelo.Regra(eRegra.PdfGeradoSistema))
			{
				ArquivoBus arqBus = new ArquivoBus(eExecutorTipo.Interno);
				titulo.ArquivoPdf = arqBus.ObterDados(titulo.ArquivoPdf.Id.GetValueOrDefault());

				vm.ArquivoId = titulo.ArquivoPdf.Id;
				vm.ArquivoTexto = titulo.ArquivoPdf.Nome;
				vm.ArquivoJSon = ViewModelHelper.JsSerializer.Serialize(titulo.ArquivoPdf);
			}

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

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioCriar, ePermissao.TituloDeclaratorioEditar })]
		public ActionResult Salvar(Titulo titulo)
		{
			String urlSucesso = String.Empty;
			String acao = (titulo.Id > 0) ? "Index" : "Criar";

			_bus.Salvar(titulo);

			urlSucesso = Url.Action(acao, "TituloDeclaratorio", Validacao.QueryParamSerializer(new { acaoId = titulo.Id, modelo = titulo.Modelo.Codigo }));

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @UrlSucesso = urlSucesso });
		}

		#endregion

		#region Excluir Titulo

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();

			Titulo titulo = _bus.ObterSimplificado(id);
			vm.Id = id;
			vm.Mensagem = Mensagem.Titulo.MensagemExcluir(titulo.Numero.Texto, titulo.Modelo.Nome);
			vm.Titulo = "Excluir Título Declaratório";
			return PartialView("Excluir", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioExcluir })]
		public ActionResult Excluir(int id)
		{
			_bus.Excluir(new Titulo() { Id = id });

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioVisualizar })]
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
			vm.LabelTipoPrazo = vm.Titulo.PrazoUnidade;

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

		#region Alterar Situação

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioAlterarSituacao })]
		public ActionResult AlterarSituacao(int id)
		{
			Titulo titulo = _bus.ObterSimplificado(id);
			List<Situacao> situacoes = new List<Situacao>();

			switch ((eTituloSituacao)titulo.Situacao.Id)
			{
				case eTituloSituacao.EmCadastro:
					{
						if (titulo.Modelo.Codigo != (int)eTituloModeloCodigo.OutrosInformacaoCorte)
							situacoes = _busLista.TituloDeclaratorioSituacoes.Where(x => x.Id == Convert.ToInt32(eTituloSituacao.Valido)).ToList();
					}
					break;
				case eTituloSituacao.Valido:
					{
						if(titulo.Modelo.Codigo == (int)eTituloModeloCodigo.OutrosInformacaoCorte)
							situacoes = _busLista.TituloDeclaratorioSituacoes.Where(x => x.Id == Convert.ToInt32(eTituloSituacao.SuspensoDeclaratorio) || x.Id == Convert.ToInt32(eTituloSituacao.EncerradoDeclaratorio) || x.Id == Convert.ToInt32(eTituloSituacao.ProrrogadoDeclaratorio)).ToList();
						else
							situacoes = _busLista.TituloDeclaratorioSituacoes.Where(x => x.Id == Convert.ToInt32(eTituloSituacao.SuspensoDeclaratorio) || x.Id == Convert.ToInt32(eTituloSituacao.EncerradoDeclaratorio)).ToList();
					}
					break;
				case eTituloSituacao.ProrrogadoDeclaratorio:
					{
						if (titulo.Modelo.Codigo == (int)eTituloModeloCodigo.OutrosInformacaoCorte)
							situacoes = _busLista.TituloDeclaratorioSituacoes.Where(x => x.Id == Convert.ToInt32(eTituloSituacao.SuspensoDeclaratorio) || x.Id == Convert.ToInt32(eTituloSituacao.EncerradoDeclaratorio)).ToList();
					}
					break;
				case eTituloSituacao.SuspensoDeclaratorio:
					situacoes = _busLista.TituloDeclaratorioSituacoes.Where(x => x.Id == Convert.ToInt32(eTituloSituacao.Valido) || x.Id == Convert.ToInt32(eTituloSituacao.EncerradoDeclaratorio)).ToList();
					break;
				case eTituloSituacao.AguardandoPagamento:
					situacoes = _busLista.TituloDeclaratorioSituacoes.Where(x => x.Id == Convert.ToInt32(eTituloSituacao.EncerradoDeclaratorio)).ToList();
					break;
				default:
					break;
			}

			AlterarSituacaoVM vm = new AlterarSituacaoVM(_busLista.DeclaratorioMotivosEncerramento, titulo, situacoes);

			if (Request.IsAjaxRequest())
			{
				return PartialView("AlterarSituacaoPartial", vm);
			}

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioAlterarSituacao })]
		public ActionResult AlterarSituacao(Titulo titulo)
		{
			Titulo tituloAtual = _bus.Obter(titulo.Id);
			tituloAtual.Modelo = _busModelo.Obter(tituloAtual.Modelo.Id);
			tituloAtual.Situacao.Id = titulo.Situacao.Id;
			tituloAtual.MotivoEncerramentoId = titulo.MotivoEncerramentoId;
			tituloAtual.MotivoSuspensao = titulo.MotivoSuspensao;
			tituloAtual.DiasProrrogados = titulo.DiasProrrogados;

			_bus.AlterarSituacao(tituloAtual);

			if (Validacao.EhValido)
			{
				return Json(new { @EhValido = true, @Msg = Validacao.QueryParam(), @AcaoId = titulo.Id, @modelo = tituloAtual.Modelo.Codigo });
			}
			else
			{
				return Json(new { @EhValido = false, @Msg = Validacao.Erros });
			}
		}

		#endregion

		#region Relatorio
		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioRelatorio })]
		public ActionResult Relatorio()
		{
			RelatorioVM vm = new RelatorioVM(
				_busModelo.ObterModelosDeclaratorios().Where(x => x.Id == 72).ToList(),
				_busLista.Municipios("ES"));
			
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioRelatorio })]
		public ActionResult GerarRelatorio(string paramsJson)
		{
			TituloRelatorioFiltro filtro = ViewModelHelper.JsSerializer.Deserialize<TituloRelatorioFiltro>(paramsJson);
			var relatorio = _bus.GerarRelatorio(filtro);
			
			return ViewModelHelper.GerarArquivo("Relatorio.csv", relatorio, "application/vnd.ms-excel");
		}

		#endregion

		#region Auxiliar

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioCriar, ePermissao.TituloDeclaratorioEditar })]
		public ActionResult TituloCamposModelo(int modeloId)
		{
			SalvarVM vm = new SalvarVM();
			vm.Modelo = _busModelo.Obter(modeloId);

			var lista = vm.Modelo.Assinantes.Select(x => new ListaValor { Id = x.SetorId, Texto = x.SetorTexto }).OrderBy(x => x.Texto).ToList();

			vm.AssinantesVM.Setores = ViewModelHelper.CriarSelectList(lista);
			vm.AssinantesVM.Cargos = ViewModelHelper.CriarSelectList(new List<ListaValor>());
			vm.AssinantesVM.Funcionarios = ViewModelHelper.CriarSelectList(new List<ListaValor>());

			string htmlCampos = ViewModelHelper.RenderPartialViewToString(ControllerContext, "TituloCamposModelo", vm);

			return Json(new { Msg = Validacao.Erros, @Html = htmlCampos }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioCriar, ePermissao.TituloDeclaratorioEditar })]
		public ActionResult ObterAssinanteCargos(int id, int modeloId)
		{
			var lista = _busModelo.ObterAssinanteCargos(modeloId, id);

			return Json(lista, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioCriar, ePermissao.TituloDeclaratorioEditar })]
		public ActionResult ObterAssinanteFuncionarios(int id, int modeloId, int setorId)
		{
			var lista = _busModelo.ObterAssinanteFuncionarios(modeloId, setorId, id);

			return Json(lista, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioCriar, ePermissao.TituloDeclaratorioEditar, ePermissao.TituloDeclaratorioVisualizar })]
		public ActionResult ValidarAssociarRequerimento(Requerimento requerimento, int modeloId)
		{
			RequerimentoBus requerimentoBus = new RequerimentoBus();
			var req = requerimentoBus.ObterSimplificado(requerimento.Id);

			_validar.AssociarRequerimento(req, modeloId);

			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioCriar, ePermissao.TituloDeclaratorioEditar, ePermissao.TituloDeclaratorioVisualizar })]
		public ActionResult ObterDadosRequerimeto(int requerimentoId)
		{
			RequerimentoBus requerimentoBus = new RequerimentoBus();
			var req = requerimentoBus.ObterSimplificado(requerimentoId);

			AtividadeBus atividadeBus = new AtividadeBus();
			var listAtividades = atividadeBus.ObterAtividadesListaReq(requerimentoId);

			return Json(new { Atividades = listAtividades, Interessado = req.Interessado }, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}