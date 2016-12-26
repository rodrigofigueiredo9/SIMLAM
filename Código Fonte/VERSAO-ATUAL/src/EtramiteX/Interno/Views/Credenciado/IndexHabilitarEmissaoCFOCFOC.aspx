<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCredenciado" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Habilitações para Emissão de CFO e CFOC</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Credenciado/habilitarEmissaoCFOCFOCListar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Credenciado/habilitacaoCFOAlterarSituacao.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Credenciado/CredenciadoListar.js") %>"></script>

	<script>
		$(function () {
			HabilitarEmissaoCFOCFOCListar.Mensagens = <%= Model.Mensagens %>;
			HabilitarEmissaoCFOCFOCListar.visualizarLink = '<%= Url.Action("VisualizarHabilitarEmissaoCFOCFOC", "Credenciado") %>';
			HabilitarEmissaoCFOCFOCListar.alterarSituacaoLink = '<%= Url.Action("AlterarSituacaoHabilitacaoCFO", "Credenciado") %>';
			HabilitarEmissaoCFOCFOCListar.associar = '<%= Url.Action("Filtrar", "Credenciado") %>';
			HabilitarEmissaoCFOCFOCListar.urlEditar = '<%= Url.Action("EditarHabilitarEmissaoCFOCFOC", "Credenciado") %>';
			HabilitarEmissaoCFOCFOCListar.urlPdf = '<%= Url.Action("GerarPdfHabilitarEmissaoCFOCFOC", "Credenciado") %>';

			HabilitarEmissaoCFOCFOCListar.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltrosHabilitarEmissaoCFOCFOC", Model); %>
	</div>
</asp:Content>