<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMMotosserra" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<MotosserraListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Motosserras</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Motosserra/motosserraListar.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			MotosserraListar.urlExcluirConfirm = '<%= Url.Action("ExcluirConfirm", "Motosserra") %>';
			MotosserraListar.urlExcluir = '<%= Url.Action("Excluir", "Motosserra") %>';
			MotosserraListar.urlEditar = '<%= Url.Action("Editar", "Motosserra") %>';
			MotosserraListar.urlDesativar = '<%= Url.Action("Desativar", "Motosserra") %>';
			MotosserraListar.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros", Model); %>
	</div>
</asp:Content>