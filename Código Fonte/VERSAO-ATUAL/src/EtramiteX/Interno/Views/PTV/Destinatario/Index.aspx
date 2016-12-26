<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<DestinatarioPTVListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Destinatários PTV</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/PTV/destinatarioListar.js") %>"></script>

	<script>
		$(function () {
			DestinatarioPTVListar.load($('#central'));
			DestinatarioPTVListar.urlVisualizar = '<%= Url.Action("DestinatarioVisualizar", "PTV") %>';
			DestinatarioPTVListar.urlEditar = '<%= Url.Action("DestinatarioEditar", "PTV") %>';
			DestinatarioPTVListar.urlExcluirConfirm = '<%= Url.Action("DestinatarioExcluirConfirm", "PTV") %>';
			DestinatarioPTVListar.urlExcluir = '<%= Url.Action("DestinatarioExcluir", "PTV") %>';
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central"><% Html.RenderPartial("Destinatario/ListarFiltros", Model); %></div>
</asp:Content>