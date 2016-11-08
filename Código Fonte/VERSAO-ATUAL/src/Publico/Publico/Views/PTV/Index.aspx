<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMPTV" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Publico.Master" Inherits="System.Web.Mvc.ViewPage<PTVListarVM>" %>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">PTVs e EPTVs</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/PTV/listar.js") %>" ></script>

    <script type="text/javascript">
    	    $(function () {
    	        PTVListar.load($('#central'), {
    	            urls: {
    	            	urlPDFInterno: '<%= Url.Action("GerarPdfInterno", "PTV") %>',
    	            	urlPDFCredenciado: '<%= Url.Action("GerarPdfCredenciado", "PTV") %>'
	            }
	        });
	    });
	</script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros", Model); %>
	</div>
</asp:Content>