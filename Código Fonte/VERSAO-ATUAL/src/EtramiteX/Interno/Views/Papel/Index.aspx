<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPapel" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Papéis</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Papel/listar.js") %>"></script>

	<script>
		$(function () {
			PapelListar.urlExcluirConfirm = '<%= Url.Action("ExcluirConfirm", "Papel") %>';
			PapelListar.urlExcluir = '<%= Url.Action("Excluir", "Papel") %>';
			PapelListar.urlEditar = '<%= Url.Action("Editar", "Papel") %>';
			PapelListar.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros", Model); %>
	</div>
</asp:Content>