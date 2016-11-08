using System;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Publico.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Publico.ViewModels;
using Tecnomapas.EtramiteX.Publico.ViewModels.VMProtocolo;
using AtividadeVM = Tecnomapas.EtramiteX.Publico.ViewModels.VMAtividade;

namespace Tecnomapas.EtramiteX.Publico.Controllers
{
	public class ProtocoloController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		ProtocoloBus _bus = new ProtocoloBus();
		AtividadeBus _busAtividade = new AtividadeBus();

		#endregion

		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(
				_busLista.QuantPaginacao,
				_busLista.AtividadesSolicitada,
				_busLista.SituacoesProcessoAtividade,
				_busLista.TiposProcesso,
				_busLista.Municipios(_busLista.EstadoDefault)
			);

			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return View(vm);
		}

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

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Visualizar(int id)
		{
			return View("ProcessoVisualizar", new ProcessoVM());

			/*IProtocolo protocolo = _bus.Obter(id);

			if (!Validacao.EhValido)
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			if (protocolo.IsProcesso)
			{
				Processo processo = protocolo as Processo;
				ProcessoVM vm = new ProcessoVM(_busLista.TiposProcesso, processo.Tipo.Id);
				vm.RequerimentoVM.IsVisualizar = true;
				vm.RequerimentoVM.IsRequerimentoProcesso = true;
				vm.SetProcesso(processo, _busLista.ResponsavelFuncoes);

				vm.IsEditar = false;
				vm.RequerimentoVM.IsEditar = false;

				return View("ProcessoVisualizar", vm);
			}
			else
			{
				Documento documento = protocolo as Documento;
				DocumentoVM vm = new DocumentoVM(_busLista.TiposDocumento, documento.Tipo.Id);
				vm.SetDocumento(documento, _busLista.ResponsavelFuncoes);
				vm.RequerimentoVM.IsVisualizar = true;

				return View("DocumentoVisualizar", vm);
			}*/
		}

		public ActionResult AtividadesSolicitadas(int id, bool isProcesso)
		{
			AtividadeVM.ListarAtividadesSolicitadasVM vm = new AtividadeVM.ListarAtividadesSolicitadasVM();
			vm = new AtividadeVM.ListarAtividadesSolicitadasVM(_busLista.TiposProcesso, _busLista.TiposDocumento, _busAtividade.ObterProtocoloAtividadesSolicitadas(id), vm.Protocolo.Tipo.Id);

			vm.IsProcesso = isProcesso;

			return PartialView("~/Views/Atividade/AtividadesSolicitadas.aspx", vm);
		}

		public ActionResult ExisteProtocoloAtividade(int id)
		{
			return Json(new { @EhValido = _bus.ExisteProtocoloAtividade(id), @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}
	}
}