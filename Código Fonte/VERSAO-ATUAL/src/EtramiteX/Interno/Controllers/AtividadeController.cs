using System;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMAtividade;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class AtividadeController : DefaultController
	{
		#region Propriedades

		AtividadeConfiguracaoBus _bus = new AtividadeConfiguracaoBus(new AtividadeConfiguracaoValidar());
		AtividadeBus _busAtividade = new AtividadeBus();
		ListaBus _busLista = new ListaBus();
		AtividadeEmpreendimentoBus _busAtividadeEmp = new AtividadeEmpreendimentoBus();

		private string QuantidadePorPagina
		{
			get { return (Request.Cookies.Get("QuantidadePorPagina") != null) ? Request.Cookies.Get("QuantidadePorPagina").Value : "5"; }
		}

		#endregion

		[Permite(RoleArray = new Object[] { ePermissao.AtividadeConfiguracaoListar })]
		public ActionResult Index()
		{
			ListarConfiguracaoVM vm = new ListarConfiguracaoVM(
				_busLista.QuantPaginacao, 
				_bus.ObterModelosSolicitadoExterno(), 
				_busLista.AtividadesSolicitada, 
				_busLista.Setores,
				_busLista.AgrupadoresAtividade);

			vm.Paginacao.QuantPaginacao = Convert.ToInt32(QuantidadePorPagina);
			return PartialView(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.AtividadeConfiguracaoListar })]
		public ActionResult Filtrar(ListarConfiguracaoVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ListarConfiguracaoVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(_busLista.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<AtividadeConfiguracao> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeEditar = User.IsInRole(ePermissao.AtividadeConfiguracaoEditar.ToString());
			vm.PodeExcluir = User.IsInRole(ePermissao.AtividadeConfiguracaoExcluir.ToString());
			vm.PodeVisualizar = User.IsInRole(ePermissao.AtividadeConfiguracaoVisualizar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "AtividadeConfiguracaoResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#region Atividade do Empreendimento

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult AtividadeEmpListarFiltros()
		{
			ListarVM vm = new ListarVM(_busLista.QuantPaginacao);
			return PartialView(vm);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult FiltrarAtividadeEmp(ListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.UltimaBusca = HttpUtility.HtmlDecode(vm.UltimaBusca);
				vm = ViewModelHelper.JsSerializer.Deserialize<ListarVM>(vm.UltimaBusca);
			}

			vm.Paginacao = paginacao;
			vm.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(_busLista.QuantPaginacao, false, false);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);

			Resultados<EmpreendimentoAtividade> resultados = _busAtividadeEmp.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm));

			vm.SetResultados(resultados.Itens);

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "AtividadeEmpListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Configuração Atividade Cadastrar e Editar

		[Permite(RoleArray = new Object[] { ePermissao.AtividadeConfiguracaoCriar })]
		public ActionResult ConfiguracaoCriar()
		{
			AtividadeConfiguracaoVM vm = new AtividadeConfiguracaoVM(_bus.ObterModelosSolicitadoExterno());
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.AtividadeConfiguracaoCriar })]
		public ActionResult ConfiguracaoCriar(AtividadeConfiguracao configuracao)
		{
			_bus.Salvar(configuracao);

			string urlRedireciona = Url.Action("ConfiguracaoCriar", "Atividade");
			urlRedireciona += "?Msg=" + Validacao.QueryParam();

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @urlRedireciona = urlRedireciona }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.AtividadeConfiguracaoEditar })]
		public ActionResult ConfiguracaoEditar(int id)
		{
			AtividadeConfiguracaoVM vm = new AtividadeConfiguracaoVM(_bus.ObterModelosSolicitadoExterno());
			vm.Configuracao = _bus.Obter(id);
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.AtividadeConfiguracaoEditar })]
		public ActionResult ConfiguracaoEditar(AtividadeConfiguracao configuracao)
		{
			_bus.Salvar(configuracao);

			string urlRedireciona = urlRedireciona = Url.Action("Index", "Atividade");
			urlRedireciona += "?Msg=" + Validacao.QueryParam();

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @urlRedireciona = urlRedireciona }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.AtividadeConfiguracaoCriar, ePermissao.AtividadeConfiguracaoEditar })]
		public ActionResult ValidarAtividadeConfigurada(int id)
		{
			_bus.ValidarAtividadeConfigurada(id);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Configuração Atividade Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.AtividadeConfiguracaoVisualizar })]
		public ActionResult ConfiguracaoVisualizar(int id)
		{
			AtividadeConfiguracaoVM vm = new AtividadeConfiguracaoVM();
			vm.Configuracao = _bus.Obter(id);
			return View(vm);
		}

		#endregion

		#region Configuração Atividade Excluir

		[Permite(RoleArray = new Object[] { ePermissao.AtividadeConfiguracaoExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			AtividadeConfiguracao configuracao = _bus.Obter(id);

			vm.Id = id;
			vm.Mensagem = Mensagem.AtividadeConfiguracao.MensagemExcluir(configuracao.NomeGrupo);
			vm.Titulo = "Excluir Configuração de Atividade Solicitada";
			return PartialView("Excluir", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.AtividadeConfiguracaoExcluir })]
		public ActionResult Excluir(int id)
		{
			_bus.Excluir(id);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Atividade Solicitadas

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult AssociarAtividade()
		{
			ListarAtividadeVM vm = new ListarAtividadeVM(_busLista.QuantPaginacao, _busLista.Setores, _busLista.AtividadesSolicitada, _busLista.AgrupadoresAtividade);
			return PartialView("AssociarAtividadeFiltros", vm);
		}

		[HttpPost]
		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult AssociarAtividade(ListarAtividadeVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ListarAtividadeVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(QuantidadePorPagina);
			vm.Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(_busLista.QuantPaginacao, false, false, QuantidadePorPagina);

			Resultados<AtividadeListarFiltro> resultados = _busAtividade.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.Paginacao.QuantidadeRegistros = Convert.ToInt32(resultados.Quantidade);
			vm.Paginacao.EfetuarPaginacao();

			//deve ser setado apos a serializacao da ultimabusca
			vm.SetResultados(resultados.Itens);
			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "AssociarAtividadeResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult ObterFinalidade(int id)
		{
			FinalidadeVM vm = new FinalidadeVM();
			vm.SiglaOrgao = _busLista.OrgaoSigla;
			vm.AtividadeId = id;

			vm.SetLista(_busLista.Finalidades);

			return PartialView("TituloFinalidadePartial", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.AtividadeEncerrar, ePermissao.ProcessoVisualizar })]
		public ActionResult EncerrarAtividade(Atividade atividade)
		{
			AtividadeValidar _validar = new AtividadeValidar();
			_validar.EncerrarAtividade(atividade.Protocolo.Id.Value, atividade.Protocolo.IsProcesso);
			_validar.EncerrarAtividadeManual(atividade);
			return Json(new { Msg = Validacao.Erros, IsAtividadePodeEncerrar = Validacao.EhValido });
		}

		[Permite(RoleArray = new Object[] { ePermissao.AtividadeEncerrar, ePermissao.ProcessoVisualizar })]
		public ActionResult MotivoEncerrarAtividade(Atividade atividade, int id, bool isProcesso)
		{
			return PartialView("MotivoEncerrarAtividade", new AtividadeVME(atividade));
		}

		[Permite(RoleArray = new Object[] { ePermissao.AtividadeEncerrar })]
		public ActionResult SalvarMotivoEncerrarAtividade(Atividade atividade, int id, bool isProcesso)
		{
			int SitucaoId = _busAtividade.EncerrarAtividadeMotivo(atividade, id, isProcesso);
			object situacao = new { Id = SitucaoId, Texto = _busAtividade.ObterSituacaoTexto(SitucaoId) };
			return Json(new { Msg = Validacao.Erros, IsAtividadeEncerrada = Validacao.EhValido, Situacao = situacao });
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProcessoVisualizar })]
		public ActionResult VisualizarMotivoEncerrarAtividade(string motivo)
		{
			return PartialView("MotivoEncerrarAtividade", new AtividadeVME() { IsVisualizar = true, Atividade = new Atividade { Motivo = motivo } });
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProcessoCriar, ePermissao.ProcessoEditar })]
		public ActionResult ValidarExcluirAtividadeFinalidade(int protocolo, bool isProcesso, int atividade, int modelo)
		{
			_busAtividade.ValidarExcluirAtividadeFinalidade(protocolo, isProcesso, atividade, modelo);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}