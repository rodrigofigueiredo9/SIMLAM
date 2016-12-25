<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.DeclaracaoAdicional" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<DeclaracaoAdicionalListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Declaração Adicional</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/ConfiguracaoVegetal/DeclaracaoAdicional/listar.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
	<script>
	    $(function () {
	        DeclaracaoAdicionalListar.urlEditar = '<%= Url.Action("EditarDeclaracaoAdicional", "ConfiguracaoVegetal") %>';
	        DeclaracaoAdicionalListar.urlExcluir = '<%= Url.Action("Excluir", "ConfiguracaoVegetal") %>';
	        DeclaracaoAdicionalListar.urlExcluirConfirm = '<%= Url.Action("ExcluirConfirm", "ConfiguracaoVegetal") %>';
	        DeclaracaoAdicionalListar.load($('#central'));
        });
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("DeclaracaoAdicional/ListarFiltros", Model); %>
	</div>
</asp:Content>
