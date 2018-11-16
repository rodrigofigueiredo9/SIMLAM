using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloRequerimento.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMRequerimento;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class RequerimentoController : DefaultController
	{
		#region Propriedades
		
		RequerimentoBus _bus = new RequerimentoBus(new RequerimentoValidar());
		//PessoaBus _busPessoa = new PessoaBus(new PessoaValidar());
		ListaBus _busLista = new ListaBus();
		FuncionarioBus _busFuncionario = new FuncionarioBus();

		#endregion

		#region Filtrar

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoListar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM();
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return PartialView(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoListar })]
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

			Resultados<Requerimento> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			if (!vm.PodeAssociar)
			{
				vm.PodeEditar = User.IsInRole(ePermissao.RequerimentoEditar.ToString());
				vm.PodeExcluir = User.IsInRole(ePermissao.RequerimentoExcluir.ToString());
				vm.PodeVisualizar = User.IsInRole(ePermissao.RequerimentoVisualizar.ToString());
			}

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult Criar()
		{
			RequerimentoVM  vm = new RequerimentoVM();
			vm.CarregarListas(_busLista.ResponsavelFuncoes, _busLista.AgendamentoVistoria, _busFuncionario.ObterSetoresFuncionario(RequerimentoBus.User.FuncionarioId));
			return View(vm);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult EditarValidar(int id)
		{
			Requerimento requerimento = _bus.Obter(id);

			_bus.ValidarEditar(requerimento);

			if (!Validacao.EhValido)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			RequerimentoVM vm = new RequerimentoVM(requerimento);
			vm.CarregarListas(_busLista.ResponsavelFuncoes, _busLista.AgendamentoVistoria, _busFuncionario.ObterSetoresFuncionario(RequerimentoBus.User.FuncionarioId));

			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros });
		}

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult Editar(int id)
		{
			Requerimento requerimento = _bus.Obter(id);

			_bus.ValidarRoteiroRemovido(requerimento);

			if (Validacao.Erros.Count <= 0)
			{
				_bus.ValidarSituacaoVersaoRoteiro(requerimento.Roteiros);
			}

			_bus.ValidarEditar(requerimento);

			if (!Validacao.EhValido)
			{
				return RedirectToAction("Index", "Requerimento", Validacao.QueryParamSerializer());
			}

			RequerimentoVM vm = new RequerimentoVM(requerimento);
			vm.CarregarListas(_busLista.ResponsavelFuncoes, _busLista.AgendamentoVistoria, _busFuncionario.ObterSetoresFuncionario(RequerimentoBus.User.FuncionarioId));

			return View(vm);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult Visualizar(int id)
		{
			Requerimento requerimento = _bus.Obter(id);

			if (requerimento == null)
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			RequerimentoVM vm = new RequerimentoVM(requerimento, true);
			vm.CarregarListas(_busLista.ResponsavelFuncoes, _busLista.AgendamentoVistoria, _busFuncionario.ObterSetoresFuncionario(RequerimentoBus.User.FuncionarioId));

			if (Request.IsAjaxRequest())
			{
				return PartialView("VisualizarPartial", vm);
			}
			else
			{
				return View(vm);
			}
		}

		#endregion

		#region Objetivo do Pedido

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult ObterObjetivoPedidoVisualizar(int id)
		{
			RequerimentoVM vm = new RequerimentoVM();
			
			if (id != 0)
			{
				Requerimento requerimento = _bus.Obter(id);

				if (requerimento != null)
				{
					vm.CarregarRequerimentoVM(requerimento);

					vm.CarregarListas(_busLista.ResponsavelFuncoes, _busLista.AgendamentoVistoria, _busFuncionario.ObterSetoresFuncionario(RequerimentoBus.User.FuncionarioId));
				}
			}

			return PartialView("ObjetivoPedidoVisualizar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult ObterObjetivoPedido(int id)
		{
			RequerimentoVM vm = new RequerimentoVM();
			if (id != 0)
			{
				Requerimento requerimento = _bus.Obter(id);

				if (requerimento != null)
				{
					vm.CarregarRequerimentoVM(requerimento);

					vm.CarregarListas(_busLista.ResponsavelFuncoes, _busLista.AgendamentoVistoria, _busFuncionario.ObterSetoresFuncionario(RequerimentoBus.User.FuncionarioId));
				}
			}

			return PartialView("ObjetivoPedido", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult CriarObjetivoPedido(RequerimentoVM vm)
		{
			Requerimento requerimento = null;
			if (vm.Id != 0)
			{
				requerimento = _bus.Obter(vm.Id);

				if (requerimento != null)
				{
					vm.DataCriacao = requerimento.DataCadastro.ToString();
					vm.Empreendimento = requerimento.Empreendimento;
					vm.Responsaveis = requerimento.Responsaveis;
					vm.Interessado = requerimento.Interessado;
				}
			}

			
			requerimento = GerarRequerimento(vm);
			requerimento.SituacaoId = 1;
			_bus.SalvarObjetivoPedido(requerimento);

			return Json(new { id = requerimento.Id, Msg = Validacao.Erros });
		}

		public ActionResult ObterTituloModelo(int atividadeId, int finalidade)
		{
			List<TituloModeloLst> modelos = _bus.ObterModelosAtividades(new List<AtividadeSolicitada>() { new AtividadeSolicitada() { Id = atividadeId } }, finalidade == 3);//3 Renovação

			return Json(new { @Lista = modelos, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult ObterTituloModeloAnterior(int titulo)
		{
			List<TituloModeloLst> tituloModelosFaseAnterior = _bus.ObterModelosAnteriores(titulo);

			List<TituloModeloLst> tituloModelosRenovacao = _bus.ObterModelosRenovacao(titulo);

			tituloModelosRenovacao.AddRange(tituloModelosFaseAnterior);

			TituloModelo modelo = _bus.VerficarTituloPassivelRenovação(titulo);

			TituloModelo modeloAtual = new TituloModeloBus().Obter(titulo);

			Boolean faseAnteriorObrigatoria = Convert.ToBoolean((modeloAtual.Resposta(eRegra.FaseAnterior, eResposta.TituloAnteriorObrigatorio) ?? new TituloModeloResposta()).Valor);

			return Json(new { @Lista = tituloModelosFaseAnterior, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @EhFaseAnterior = modelo.Regra(eRegra.FaseAnterior), @FaseAnteriorEhObrigatoria = faseAnteriorObrigatoria.ToString().ToLower(), @ListaRenovacao = tituloModelosRenovacao }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult ObterNumerosTitulos(string numero, int modeloId)
		{
			List<TituloModeloLst> titulos = _bus.ObterNumerosTitulos(numero, modeloId);

			return Json(new { @Lista = titulos, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult AlterarSituacao(int requerimentoId)
		{
			_bus.AlterarSituacao(new Requerimento() { Id = requerimentoId, SituacaoId = 1 });// Em andamento

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult ObterRoteirosAtividade(List<Atividade> AtividadesSolicitadas)
		{
			List<Roteiro> listaRoteiro = _bus.ObterRoteirosPorAtividades(AtividadesSolicitadas);

			return Json(new { @Lista =listaRoteiro, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult ValidarNumeroModeloAnterior(int tituloAnteriorId, int tituloAnteriorTipo)
		{
			_bus.ValidarModeloAnteriorNumero(tituloAnteriorId, tituloAnteriorTipo);
			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros });
		}

		#region  Atividade Solicitada

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult CriarAtividadeSolicitada(int id)
		{
			return View("AtividadeSolicitada", new AtividadeSolicitadaVM(_busLista.Finalidades));
		}

		#endregion

		#endregion

		#region Interessado

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult AssociarInteressado(int requerimentoId, int interessadoId)
		{
			Requerimento requerimento = new Requerimento(){ Id =requerimentoId };

			requerimento.Interessado.Id = interessadoId;

			_bus.AssociarInteressado(requerimento);

			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros });
		}

		#endregion

		#region Responsavel Tecnico

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult ObterResponsavelVisualizar(int id)
		{
			string view = "CriarRespTecnico";
			RequerimentoVM vm = new RequerimentoVM();
			if (id != 0)
			{
				Requerimento requerimento = _bus.Obter(id);
				if (requerimento != null)
				{
					vm.CarregarRequerimentoVM(requerimento);
					vm.CarregarListas(_busLista.ResponsavelFuncoes, _busLista.AgendamentoVistoria, _busFuncionario.ObterSetoresFuncionario(RequerimentoBus.User.FuncionarioId));

					view = (requerimento.Responsaveis.Count > 0) ? "ResponsavelTecnicoVisualizar" : "CriarRespTecnico";
				}
			}

			return PartialView(view, vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoExcluir })]
		public ActionResult ExcluirResponsaveis(int id)
		{
			_bus.ExcluirResponsaveis(new Requerimento() { Id = id });
			return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros });
		}
		
		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult ObterResponsavel(int id)
		{
			RequerimentoVM vm = new RequerimentoVM();
			if (id != 0) 
			{
				Requerimento requerimento = _bus.Obter(id);
				if (requerimento != null)
				{
					vm.CarregarRequerimentoVM(requerimento);
					vm.CarregarListas(_busLista.ResponsavelFuncoes, _busLista.AgendamentoVistoria, _busFuncionario.ObterSetoresFuncionario(RequerimentoBus.User.FuncionarioId));
				}
			}
			return PartialView("CriarRespTecnico", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult CriarResponsavel(int id, List<ResponsavelTecnico> responsaveis)
		{
			Requerimento requerimento = _bus.Obter(id);

			if (requerimento != null)
			{
				requerimento.Responsaveis = responsaveis ?? new List<ResponsavelTecnico>();

				_bus.SalvarResponsavelTecnico(requerimento);
			}

			return Json(new { Msg = Validacao.Erros });
		}

		#endregion

		#region Empreendimento

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult AssociarEmpreendimento(int requerimentoId, int empreendimentoId)
		{
			Requerimento requerimento = new Requerimento() { Id = requerimentoId };
			requerimento.Empreendimento.Id = empreendimentoId;

			bool isAssociado = _bus.AssociarEmpreendimento(requerimento);

			return Json(new { @Msg = Validacao.Erros, @empAssociado = isAssociado });
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult IsAtividadeCorte(int requerimentoId)
		{
			Requerimento requerimento = new Requerimento() { Id = requerimentoId };

			bool isAssociado = _bus.IsRequerimentoAtividadeCorte(requerimentoId);
			
			return Json(new
			{
				reqAssociado = isAssociado,
				Msg = Validacao.Erros,
				RedirecionarListar = Validacao.Erros.Exists(x => x.Tipo == eTipoMensagem.Erro),
				urlRedirecionar = Url.Action("Index", "Requerimento", Validacao.QueryParamSerializer())
			});
		}

		#endregion

		#region Finalizar

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult ObterFinalizar(int id)
		{
			RequerimentoVM vm = new RequerimentoVM(_bus.ObterFinalizar(id));
			vm.IsAbaFinalizar = true;
			vm.CarregarListas(_busLista.ResponsavelFuncoes, _busLista.AgendamentoVistoria, _busFuncionario.ObterSetoresFuncionario(RequerimentoBus.User.FuncionarioId));

			return PartialView("Finalizar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult Finalizar(Requerimento requerimento)
		{
			_bus.Finalizar(requerimento);

			if (!Validacao.EhValido)
			{
				return Json(new { id = requerimento.Id, Msg = Validacao.Erros });
			}

			string urlGerarPdf = HttpUtility.UrlEncode(Url.Action("GerarPdf", "Requerimento", new { Id = requerimento.Id, acaoId = requerimento.Id }));
			string urlRedirect = Url.Action("Criar", "Requerimento", Validacao.QueryParamSerializer(new {redireciona= urlGerarPdf, acaoId = requerimento.Id}));

			urlRedirect = Url.Action("Criar", "Requerimento", Validacao.QueryParamSerializer(new { acaoId = requerimento.Id }));
			
			return Json(new { id = requerimento.Id, redirect = urlRedirect, Msg = Validacao.Erros });
		}

		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			vm.Id = id;
			vm.Titulo = "Excluir Requerimento";
			vm.Mensagem = Mensagem.Requerimento.ExcluirConfirmacao(id);
			return View("Excluir", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoExcluir })]
		public ActionResult Excluir(int id)
		{
			 _bus.Excluir(id);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido });
		}

		#endregion

		#region GerarPdf

		private Requerimento GerarRequerimento(RequerimentoVM vm)
		{
			Requerimento req = new Requerimento();
			req.Id = vm.Numero;
			req.DataCadastro = Convert.ToDateTime(vm.DataCriacao);
			req.Roteiros = vm.Roteiros;
			req.Atividades = vm.AtividadesSolicitadas;
			req.Responsaveis = vm.Responsaveis;
			req.Empreendimento = vm.Empreendimento;
			req.Interessado = vm.Interessado;
			req.AgendamentoVistoria = vm.AgendamentoVistoriaId;
			req.SetorId = vm.SetorId;
			req.Informacoes = vm.InformacaoComplementar;

			return req;
		}

		public ActionResult GerarPdf(int id)
		{
			try
			{
				Requerimento requerimento = _bus.ObterSimplificado(id) ?? new Requerimento();
				if (requerimento.IsRequerimentoDigital)
				{
					return ViewModelHelper.GerarArquivoPdf(new PdfRequerimentoDigital().Gerar(id), "Requerimento Digital");
				}

				return ViewModelHelper.GerarArquivoPdf(new PdfRequerimentoPadrao().Gerar(id), "Requerimento Padrao");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		#endregion

		#region Associar

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoListar })]
		public ActionResult Associar()
		{
			ListarVM vm = new ListarVM();
			return PartialView("ListarFiltros", vm);
		}

		#endregion

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult ObterReqInterEmp(int id)
		{
			Requerimento requerimento = _bus.ObterSimplificado(id);
			return Json(new { @Msg = Validacao.Erros, @Requerimento = new { requerimentoId = id, interessadoId = requerimento.Interessado.Id, empreendimentoId = requerimento.Empreendimento.Id } }, JsonRequestBehavior.AllowGet);
		}

		#endregion

	}
}