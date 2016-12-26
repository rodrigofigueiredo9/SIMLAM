<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMRequerimento" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Requerimentos</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Requerimento/requerimentoListar.js") %>" ></script>
	<script>
		$(function () {
			RequerimentoListar.ExcluirConfirm = '<%= Url.Action("ExcluirConfirm", "Requerimento") %>';
			RequerimentoListar.urlExcluir = '<%= Url.Action("Excluir", "Requerimento") %>';
			RequerimentoListar.urlEditar = '<%= Url.Action("Editar", "Requerimento") %>';
			RequerimentoListar.urlEditarValidar = '<%= Url.Action("EditarValidar", "Requerimento") %>';
			RequerimentoListar.urlImportar = '<%= Url.Action("Importar", "Requerimento") %>';
			RequerimentoListar.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros"); %>
	</div>
</asp:Content>