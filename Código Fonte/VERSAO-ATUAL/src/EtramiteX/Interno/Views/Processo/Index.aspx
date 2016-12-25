<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProcesso" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Processos</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Processo/listar.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
	<script>
		$(function () {
			ProcessoListar.urlExcluirConfirm = '<%= Url.Action("ExcluirConfirm", "Processo") %>';
			ProcessoListar.urlExcluir = '<%= Url.Action("Excluir", "Processo") %>';
			ProcessoListar.urlConsultarInformacoes = '<%= Url.Action("ConsultarInformacoes", "Processo") %>';

			ProcessoListar.urlEditar = '<%= Url.Action("Editar", "Processo") %>';
			ProcessoListar.urlValidarEditar = '<%= Url.Action("ValidarEditar", "Processo") %>';

			ProcessoListar.urlExisteProcessoAtividade = '<%= Url.Action("ExisteProcessoAtividade", "Processo") %>';
			ProcessoListar.urlAtividadesSolicitadas = '<%= Url.Action("AtividadesSolicitadas", "Processo") %>';

			ProcessoListar.urlEditarApensadosJuntados = '<%= Url.Action("EditarApensadosJuntados") %>';
			ProcessoListar.urlValidarEditarApensadosJuntados = '<%= Url.Action("ValidarEditarApensadosJuntados", "Processo") %>';
			ProcessoListar.load($('#central'));

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])){%>
				ContainerAcoes.load($(".containerAcoes"), {
					urls:{
						urlGerarPdf: '<%= Url.Action("GerarPdfDocRegistroRecebimento", "Processo", new {id = Request.Params["acaoId"].ToString() }) %>'
					}
				});
			<%}%>
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros"); %>
	</div>
</asp:Content>