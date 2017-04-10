<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoDocumentoFitossanitario" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ConfiguracaoNumeracaoListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Numeração de CFO/CFOC/PTV</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/ConfiguracaoDocumentoFitossanitario/listar.js") %>"></script>

	<script>
	    $(function () {
	        ConfigDocFitossanitarioListar.load($("#central"));
	    });
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central" class="principal">
		<h1 class="titTela">Numeração de CFO/CFOC/PTV</h1>
		<br />
        
        <input type="hidden" class="ehEnter" name="ehEnter" value="0" />
		    <%Html.RenderPartial("ListarFiltros", Model); %>

        <br />
        <br />

            <%Html.RenderPartial("ListarFiltrosConsolidado", Model); %>
        
	</div>
</asp:Content>