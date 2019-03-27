<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<PTVListarVM>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">EPTVs</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/PTV/listarEPTV.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/PTV/comunicadorptv.js") %>"></script>

	<script>
		$(function () {
			EPTVListar.load($('#central'), {
				urls: {
					urlAnalisar: '<%= Url.Action("EPTVAnalisar", "PTV") %>',
					urlValidarAcessoComunicador: '<%= Url.Action("ValidarAcessoComunicador", "PTV") %>',
					urlComunicadorPTV: '<%= Url.Action("ComunicadorPTV", "PTV") %>',
					urlAnalisarDesbloqueio: '<%= Url.Action("AnalisarDesbloqueio", "PTV") %>',
					urlVisualizar: '<%= Url.Action("EPTVVisualizar", "PTV") %>',
					urlPDFEPTV: '<%= Url.Action("GerarPdfEPTV", "PTV") %>',
					urlValidarAcessoAnalisarDesbloqueio: '<%= Url.Action("ValidarAcessoAnalisarDesbloqueio", "PTV") %>',
				}
			});

			
			<% if (!String.IsNullOrEmpty(Request.Params["acaoGerarPdfId"])) { %>
			PTVListar.gerarPDFLoad(<%= Request.Params["acaoGerarPdfId"] %>);
			<% } %>

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])) { %>
				ContainerAcoes.load($(".containerAcoes"), {
					botoes: [
						{ label: 'Gerar PDF', url: '<%= Url.Action("GerarPdfEPTV", "PTV", new {id = Request.Params["acaoId"].ToString() }) %>' }
					]
				});
			<% } %>
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("EPTVListarFiltros", Model); %>
	</div>
</asp:Content>