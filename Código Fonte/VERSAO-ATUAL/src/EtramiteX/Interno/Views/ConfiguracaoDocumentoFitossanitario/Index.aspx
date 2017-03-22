<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoDocumentoFitossanitario" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ConfiguracaoDocumentoFitossanitarioVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Numeração de CFO/CFOC/PTV</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/ConfiguracaoDocumentoFitossanitario/configDocFitossanitario.js") %>"></script>

	<script>
		$(function () {
			ConfigDocFitossanitario.load($("#central"), {
				urls: {
					salvar: '<%= Url.Action("Configurar", "ConfiguracaoDocumentoFitossanitario") %>',
				    validarIntervalo: '<%= Url.Action("ValidarIntervalo", "ConfiguracaoDocumentoFitossanitario") %>',
				    editar: '<%= Url.Action("EditarNumeracao", "ConfiguracaoDocumentoFitossanitario")%>',
				    salvarEdicao: '<%= Url.Action("SalvarEdicao", "ConfiguracaoDocumentoFitossanitario")%>',
				    excluir: '<%= Url.Action("Excluir", "ConfiguracaoDocumentoFitossanitario") %>',
                    validarEdicao: '<%= Url.Action("ValidarEdicao", "ConfiguracaoDocumentoFitossanitario") %>',
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Numeração de CFO/CFOC/PTV</h1>
		<br />
        <h1>TESTE</h1>
		</div>
	</div>
</asp:Content>