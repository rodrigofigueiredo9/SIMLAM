<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMLocalVistoria" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<LocalVistoriaListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Locais de Vistoria</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/LocalVistoria/listar.js") %>"></script>

	<script>
	    $(function () {
	        LocalVistoriaListar.urlEditar = '<%= Url.Action("OperarLocalVistoria", "LocalVistoria") %>';
	        LocalVistoriaListar.urlVisualizar = '<%= Url.Action("VisualizarLocalVistoria", "LocalVistoria") %>';
	        LocalVistoriaListar.PodeEditar = <%= Model.PodeEditar.ToString().ToLower() %>;
	        LocalVistoriaListar.PodeVisualizar = <%= Model.PodeVisualizar.ToString().ToLower() %>;
	        LocalVistoriaListar.Mensagens= <%= Model.Mensagens %>;
	        LocalVistoriaListar.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros", Model); %>
	</div>
</asp:Content>