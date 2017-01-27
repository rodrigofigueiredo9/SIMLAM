<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAgrotoxico" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Agrotóxicos</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Agrotoxico/listar.js") %>" ></script>
	<script>
		$(function () {
			AgrotoxicoListar.load($('#central'));
			AgrotoxicoListar.urlExcluir = '<%= Url.Action("Excluir", "Agrotoxico") %>';
			AgrotoxicoListar.urlExcluirConfirm = '<%= Url.Action("ExcluirConfirm", "Agrotoxico") %>';
			AgrotoxicoListar.urlEditar = '<%= Url.Action("Editar", "Agrotoxico") %>';
			AgrotoxicoListar.urlVisualizar = '<%= Url.Action("Visualizar", "Agrotoxico") %>';
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central"><% Html.RenderPartial("ListarFiltros", Model); %></div>
</asp:Content>