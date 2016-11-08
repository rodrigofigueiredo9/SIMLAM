<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<PragaListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Pragas</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/ConfiguracaoVegetal/Praga/listar.js") %>" ></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
	<script type="text/javascript">
	    $(function () {
	        PragaListar.urlEditar = '<%= Url.Action("EditarPraga", "ConfiguracaoVegetal") %>';
	    	PragaListar.urlAssociarCultura = '<%= Url.Action("AssociarCulturas", "ConfiguracaoVegetal") %>';

	    	PragaListar.load($('#central'));
        });
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("Praga/ListarFiltros", Model); %>
	</div>
</asp:Content>
