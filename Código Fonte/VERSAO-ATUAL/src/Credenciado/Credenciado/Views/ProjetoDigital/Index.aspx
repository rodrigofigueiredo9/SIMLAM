<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProjetoDigital" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<ProjetoDigitalListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Projeto Digital</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/ProjetoDigital/listar.js") %>"></script>
	<script type="text/javascript">
		$(function () {
			ProjetoDigitalListar.urlOperar = '<%= Url.Action("Operar", "ProjetoDigital") %>';
			ProjetoDigitalListar.urlConfirmarExcluir = '<%= Url.Action("ExcluirConfirm", "ProjetoDigital") %>';
			ProjetoDigitalListar.urlExcluir = '<%= Url.Action("Excluir", "ProjetoDigital") %>';
			ProjetoDigitalListar.urlConfirmarCancelarEnvio = '<%= Url.Action("CancelarEnvioConfirm", "ProjetoDigital") %>';
			ProjetoDigitalListar.urlCancelarEnvio = '<%= Url.Action("CancelarEnvio", "ProjetoDigital") %>';
			ProjetoDigitalListar.urlDocumentosGerados = '<%= Url.Action("DocumentosGerados", "ProjetoDigital") %>';
			ProjetoDigitalListar.urlNotificacaoCorrecao = '<%= Url.Action("NotificacaoCorrecao", "ProjetoDigital") %>';
			ProjetoDigitalListar.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros", Model); %>
	</div>
</asp:Content>