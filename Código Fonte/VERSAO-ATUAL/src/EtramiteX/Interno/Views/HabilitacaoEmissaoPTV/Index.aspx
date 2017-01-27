<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMHabilitacaoEmissaoPTV" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Habilitações de emissão de PTV</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/HabilitacaoEmissaoPTV/listar.js") %>" ></script>
	<script>
		$(function () {
			HabilitacaoEmissaoPTVListar.load($('#central'));
			HabilitacaoEmissaoPTVListar.urlVisualizar = '<%= Url.Action("Visualizar", "HabilitacaoEmissaoPTV") %>';
			HabilitacaoEmissaoPTVListar.urlEditar = '<%= Url.Action("Editar", "HabilitacaoEmissaoPTV") %>';
			HabilitacaoEmissaoPTVListar.urlAlterarSituacao = '<%= Url.Action("AlterarSituacao", "HabilitacaoEmissaoPTV") %>';
			HabilitacaoEmissaoPTVListar.urlGerarPDF = '<%= Url.Action("GerarPDF", "HabilitacaoEmissaoPTV") %>';
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central"><% Html.RenderPartial("ListarFiltros", Model); %></div>
</asp:Content>