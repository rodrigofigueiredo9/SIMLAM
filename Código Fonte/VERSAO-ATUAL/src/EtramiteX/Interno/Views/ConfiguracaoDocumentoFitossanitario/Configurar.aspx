<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoDocumentoFitossanitario" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ConfiguracaoDocumentoFitossanitarioVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Configurar Numeração de CFO/CFOC/PTV</asp:Content>

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
                    excluir: '<%= Url.Action("Excluir", "ConfiguracaoDocumentoFitossanitario") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Configurar Numeração de CFO/CFOC/PTV</h1>
		<br />

		<%Html.RenderPartial("DocumentoFitossanitarioPartial", Model); %>

		<div class="block box">
			<button type="button" class="btnSalvar floatLeft"  value="Salvar"><span>Salvar</span></button>
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Content("~/") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>