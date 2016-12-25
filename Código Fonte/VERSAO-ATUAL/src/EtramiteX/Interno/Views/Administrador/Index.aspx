<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAdministrador" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Administradores</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Administrador/listar.js") %>"></script>

	<script>
		$(function () {
			AdministradorListar.urlEditar = '<%= Url.Action("Editar", "Administrador") %>';
			AdministradorListar.urlVisualizar = '<%= Url.Action("Visualizar", "Administrador") %>';
			AdministradorListar.urlAlterarSituacao = '<%= Url.Action("AlterarSituacao", "Administrador") %>';
			AdministradorListar.urlPromoverParaSistemaConfirm = '<%= Url.Action("TransferirSistema", "Administrador") %>';
			AdministradorListar.urlPromoverParaSistema = '<%= Url.Action("TransferirSistema", "Administrador") %>';
			AdministradorListar.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros", Model); %>
	</div>
</asp:Content>