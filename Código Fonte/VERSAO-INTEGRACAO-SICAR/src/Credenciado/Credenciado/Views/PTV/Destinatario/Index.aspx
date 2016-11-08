<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPTV" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<DestinatarioPTVListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Destinatários PTV</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/PTV/destinatarioListar.js") %>"></script>

	<script type="text/javascript">
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