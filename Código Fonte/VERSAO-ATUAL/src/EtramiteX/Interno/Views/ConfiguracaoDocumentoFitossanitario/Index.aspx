<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoDocumentoFitossanitario" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ConfiguracaoNumeracaoListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Numeração de CFO/CFOC/PTV</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/ConfiguracaoDocumentoFitossanitario/listar.js") %>"></script>

	<script>
	    $(function () {
	        ConfigDocFitossanitarioListar.load($("#central"), {
	            urls: {
	                buscar: '<%= Url.Action("Index", "ConfiguracaoDocumentoFitossanitario") %>',
	                validarBusca: '<%= Url.Action("ValidarBusca", "ConfiguracaoDocumentoFitossanitario") %>',
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Numeração de CFO/CFOC/PTV</h1>
		<br />

        <div>
		    <%Html.RenderPartial("ListarFiltros", Model); %>
        </div>

        <div>
            
        </div>
	</div>
</asp:Content>