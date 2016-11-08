using System;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProtocolo;
using AtividadeVM = Tecnomapas.EtramiteX.Credenciado.ViewModels.VMAtividade;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class ProtocoloController : DefaultController
	{
		#region Propriedades

		ProtocoloCredenciadoBus _bus = new ProtocoloCredenciadoBus();
		AtividadeInternoBus _busAtividade = new AtividadeInternoBus();

		#endregion

		[Permite(RoleArray = new Object[] { ePermissao.ProtocoloListar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(
				ListaCredenciadoBus.QuantPaginacao,
				ListaCredenciadoBus.AtividadesSolicitada,
				ListaCredenciadoBus.SituacoesProcessoAtividade,
				ListaCredenciadoBus.TiposProcesso,
				ListaCredenciadoBus.Municipios(ListaCredenciadoBus.EstadoDefault)
			);

			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProtocoloListar })]
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

			Resultados<Protocolo> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProtocoloVisualizar })]
		public ActionResult Visualizar(int id)
		{
			IProtocolo protocolo = _bus.Obter(id, validarPosse: false);

			if (!Validacao.EhValido)
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			if (protocolo.IsProcesso)
			{
				Processo processo = protocolo as Processo;
				ProcessoVM vm = new ProcessoVM(ListaCredenciadoBus.TiposProcesso, processo.Tipo.Id);
				vm.RequerimentoVM.IsVisualizar = true;
				vm.RequerimentoVM.IsRequerimentoProcesso = true;
				vm.SetProcesso(processo, ListaCredenciadoBus.ResponsavelFuncoes);

				vm.IsEditar = false;
				vm.RequerimentoVM.IsEditar = false;

				return View("ProcessoVisualizar", vm);
			}
			else
			{
				Documento documento = protocolo as Documento;
				DocumentoVM vm = new DocumentoVM(ListaCredenciadoBus.TiposDocumento, documento.Tipo.Id);
				vm.SetDocumento(documento, ListaCredenciadoBus.ResponsavelFuncoes);
				vm.RequerimentoVM.IsVisualizar = true;

				return View("DocumentoVisualizar", vm);
			}
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProtocoloVisualizar })]
		public ActionResult VisualizarProcesso(int id)
		{
			IProtocolo protocolo = _bus.Obter(id, validarPosse: false);

			if (!Validacao.EhValido)
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			Processo processo = protocolo as Processo;
			ProcessoVM vm = new ProcessoVM(ListaCredenciadoBus.TiposProcesso, processo.Tipo.Id);
			vm.RequerimentoVM.IsVisualizar = true;
			vm.RequerimentoVM.IsRequerimentoProcesso = true;
			vm.SetProcesso(processo, ListaCredenciadoBus.ResponsavelFuncoes);

			vm.IsEditar = false;
			vm.RequerimentoVM.IsEditar = false;

			return View("ProcessoVisualizarModal", vm);

		}

		[Permite(RoleArray = new Object[] { ePermissao.ProtocoloVisualizar })]
		public ActionResult AtividadesSolicitadas(int id, bool isProcesso)
		{
            AtividadeVM.ListarAtividadesSolicitadasVM vm = new AtividadeVM.ListarAtividadesSolicitadasVM();
			vm = new AtividadeVM.ListarAtividadesSolicitadasVM(ListaCredenciadoBus.TiposProcesso, ListaCredenciadoBus.TiposDocumento, _busAtividade.ObterProtocoloAtividadesSolicitadas(id), vm.Protocolo.Tipo.Id);

			vm.IsProcesso = isProcesso;

			return PartialView("~/Views/Atividade/AtividadesSolicitadas.aspx", vm);
		}

		public ActionResult ExisteProtocoloAtividade(int id)
		{
			return Json(new { @EhValido = _bus.ExisteProtocoloAtividade(id), @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProtocoloVisualizar })]
		public FileResult Baixar(int id)
		{
			return ViewModelHelper.BaixarArquivo(id);
		}

		#region Notificacao de Pendencia

		[Permite(RoleArray = new Object[] { ePermissao.ProtocoloListar })]
		public ActionResult NotificacaoPendencia(int id, int tipo)
		{
			ListarNotificacaoPendenciaVM vm = new ListarNotificacaoPendenciaVM();
			Resultados<NotificacaoPendencia> resultados = _bus.FiltrarNotificacaoPendencia(id);

			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.Resultados = resultados.Itens;
			vm.PodeVisualizar = User.IsInRole(ePermissao.ProtocoloVisualizar.ToString());

			return PartialView("NotificacoesPendencia", vm);
		}

		#endregion
	}
}