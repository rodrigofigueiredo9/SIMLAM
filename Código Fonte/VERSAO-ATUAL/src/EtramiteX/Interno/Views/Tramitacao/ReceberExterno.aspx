<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ReceberVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Retirar Processo/Documento de Órgão Externo</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Tramitacao/receberExterno.js") %>"></script>
	<script>
		$(function () {
			ReceberExterno.load($('#central'), {
				urls: {
					receberFiltrar: '<%= Url.Action("ReceberExternoFiltrar") %>',
					receberSalvar: '<%= Url.Action("ReceberExterno", "Tramitacao")%>',//post
					receberSucesso: '<%= Url.Action("ReceberExterno")%>',//get
					visualizarHistorico: '<%= Url.Action("Historico", "Tramitacao") %>',
					abrirPdf: '<%= Url.Action("GerarPdf", "Tramitacao") %>'
				},
				Mensagens: <%= Model.Mensagens %>
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ReceberExternoPartial", Model); %>

		<div class="block box btnSalvarContainer">
			<input class="floatLeft btnReceber" type="button" value="Retirar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>