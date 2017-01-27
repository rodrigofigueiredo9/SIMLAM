<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<CulturaListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Culturas</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/ConfiguracaoVegetal/Cultura/listar.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
	<script>
	    $(function () {
	        CulturaListar.urlEditar = '<%= Url.Action("EditarCultura", "ConfiguracaoVegetal") %>';
	        CulturaListar.load($('#central'));
        });
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("Cultura/ListarFiltros", Model); %>
	</div>
</asp:Content>
