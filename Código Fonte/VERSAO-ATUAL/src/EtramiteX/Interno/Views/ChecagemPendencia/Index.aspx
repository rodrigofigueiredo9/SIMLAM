<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMChecagemPendencia" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Checagens de Pendências</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/ChecagemPendencia/listar.js") %>" ></script>
	<script>
		$(function () {
			ChecagemPendenciaListar.urlExcluir = '<%= Url.Action("Excluir", "ChecagemPendencia") %>';
			ChecagemPendenciaListar.urlExcluirConfirm = '<%= Url.Action("ExcluirConfirm", "ChecagemPendencia") %>';
			ChecagemPendenciaListar.load($('#central'));
		});
	</script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros"); %>
	</div>
</asp:Content>