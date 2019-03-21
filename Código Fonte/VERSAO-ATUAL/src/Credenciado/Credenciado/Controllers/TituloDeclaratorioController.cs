using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.SharedVM;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class TituloDeclaratorioController : DefaultController
	{
		#region Propriedades

		TituloDeclaratorioBus _bus = new TituloDeclaratorioBus(new TituloDeclaratorioValidar());
		TituloDeclaratorioValidar _validar = new TituloDeclaratorioValidar();

		TituloModeloInternoBus _busModelo = new TituloModeloInternoBus();
		EspecificidadeBusBase _busEspBase = new EspecificidadeBusBase();

		private string QuantidadePorPagina
		{
			get { return ViewModelHelper.CookieQuantidadePorPagina; }
		}

		#endregion

		#region Filtrar

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioListar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(ListaCredenciadoBus.QuantPaginacao, _busModelo.ObterModelosDeclaratorios(), ListaCredenciadoBus.TituloDeclaratorioSituacoes, ListaCredenciadoBus.Setores, ListaCredenciadoBus.SistemaOrigem);
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
			vm.SetListItens(ListaCredenciadoBus.QuantPaginacao, vm.Paginacao.QuantPaginacao);

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
			vm.Titulo.Situacao = ListaCredenciadoBus.TituloDeclaratorioSituacoes.Single(x => x.Id == (int)eTituloSituacao.EmCadastro);

			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioEditar })]
		public ActionResult Editar(int id)
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

			vm = new SalvarVM(new List<Setor>(), _busModelo.ObterModelosDeclaratorios(), _bus.ObterLocais(), titulo.Setor.Id, titulo.Modelo.Id, titulo.LocalEmissao.Id);
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

			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioCriar, ePermissao.TituloDeclaratorioEditar })]
		public ActionResult Salvar(Titulo titulo)
		{
			String urlSucesso = String.Empty;
			String acao = (titulo.Id > 0) ? "Index" : "Criar";

			_bus.Salvar(titulo);

			urlSucesso = Url.Action(acao, "TituloDeclaratorio", Validacao.QueryParamSerializer(new { acaoId = titulo.Id }));

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @UrlSucesso = urlSucesso });
		}

		#endregion

		#region Excluir

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ConfirmarVM vm = new ConfirmarVM();

			Titulo titulo = _bus.ObterSimplificado(id);
			vm.Id = id;
			vm.Mensagem = Mensagem.Titulo.MensagemExcluir(titulo.Numero.Texto, titulo.Modelo.Nome);
			vm.Titulo = "Excluir Título Declaratório";
			return PartialView("Confirmar", vm);
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

			vm = new SalvarVM(new List<Setor>(), _busModelo.ObterModelos(todos: true), _bus.ObterLocais(), titulo.Setor.Id, titulo.Modelo.Id, titulo.LocalEmissao.Id);
			vm.Titulo = titulo;
			vm.Modelo = _busModelo.Obter(titulo.Modelo.Id);
			vm.Titulo.Modelo = vm.Modelo;
			vm.AtividadeEspecificidadeCaracterizacaoJSON = ViewModelHelper.Json(_busEspBase.GetConfigAtivEspCaracterizacao(vm.Modelo.Codigo.Value));

			vm.IsVisualizar = true;

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
					situacoes = ListaCredenciadoBus.TituloDeclaratorioSituacoes.Where(x => x.Id == Convert.ToInt32(eTituloSituacao.Valido)).ToList();
					break;
				default:
					break;
			}

			AlterarSituacaoVM vm = new AlterarSituacaoVM(new List<Motivo>(), titulo, situacoes);

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

		#region Auxiliar

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioCriar, ePermissao.TituloDeclaratorioEditar })]
		public ActionResult TituloCamposModelo(int modeloId)
		{
			SalvarVM vm = new SalvarVM();
			//vm.Modelo = _busModelo.Obter(modeloId);

			string htmlCampos = string.Empty; //ViewModelHelper.RenderPartialViewToString(ControllerContext, "TituloCamposModelo", vm);

			return Json(new { Msg = Validacao.Erros, @Html = htmlCampos }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioCriar, ePermissao.TituloDeclaratorioEditar, ePermissao.TituloDeclaratorioVisualizar })]
		public ActionResult ValidarAssociarRequerimento(Requerimento requerimento, int modeloId)
		{
			_validar.AssociarRequerimento(requerimento, modeloId);

			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioCriar, ePermissao.TituloDeclaratorioEditar, ePermissao.TituloDeclaratorioVisualizar })]
		public ActionResult ObterDadosRequerimeto(int requerimentoId)
		{
			RequerimentoCredenciadoBus requerimentoBus = new RequerimentoCredenciadoBus();
			var req = requerimentoBus.ObterSimplificado(requerimentoId);

			AtividadeCredenciadoBus atividadeBus = new AtividadeCredenciadoBus();
			var listAtividades = atividadeBus.ObterAtividadesListaReq(requerimentoId);

			return Json(new { Atividades = listAtividades, Interessado = req.Interessado }, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}