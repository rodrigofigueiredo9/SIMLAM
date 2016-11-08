using System;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMAtividade;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class AtividadeController : DefaultController
	{
		#region Propriedades

		AtividadeConfiguracaoInternoBus _bus = new AtividadeConfiguracaoInternoBus();
		AtividadeInternoBus _busAtividade = new AtividadeInternoBus();
		AtividadeEmpreendimentoInternoBus _busAtividadeEmp = new AtividadeEmpreendimentoInternoBus();

		#endregion

		#region Atividade do Empreendimento

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult AtividadeEmpListarFiltros()
		{
			ListarVM vm = new ListarVM(ListaCredenciadoBus.QuantPaginacao);
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
			vm.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(ListaCredenciadoBus.QuantPaginacao, false, false);
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

		#region Atividade Solicitadas

		//Atividade Solicitada
		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult AssociarAtividade()
		{
			ListarAtividadeVM vm = new ListarAtividadeVM(ListaCredenciadoBus.QuantPaginacao, ListaCredenciadoBus.Setores, ListaCredenciadoBus.AtividadesSolicitada, ListaCredenciadoBus.AgrupadoresAtividade);
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
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(ListaCredenciadoBus.QuantPaginacao, false, false, ViewModelHelper.CookieQuantidadePorPagina);

			vm.Filtros.ExibirCredenciado = true;

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
			vm.SiglaOrgao = ListaCredenciadoBus.OrgaoSigla;
			vm.AtividadeId = id;

			vm.SetLista(ListaCredenciadoBus.Finalidades);

			return PartialView("TituloFinalidadePartial", vm);
		}

		#endregion
	}
}