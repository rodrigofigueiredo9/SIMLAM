<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<IngredienteAtivoListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Ingredientes Ativos</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/ConfiguracaoVegetal/IngredienteAtivo/listar.js") %>"></script>

	<script>
	    $(function () {
	        IngredienteAtivoListar.urlEditar = '<%= Url.Action("EditarIngredienteAtivo", "ConfiguracaoVegetal") %>';
		    IngredienteAtivoListar.urlAlterarSituacao = '<%= Url.Action("AlterarSituacaoIngredienteAtivo", "ConfiguracaoVegetal") %>';
		    IngredienteAtivoListar.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("IngredienteAtivo/ListarFiltros", Model); %>
	</div>
</asp:Content>