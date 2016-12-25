<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<TramitacaoArquivoVM>" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Arquivo</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Tramitacao/arquivo.js") %>"></script>
	<script>
		$(function () {
			TramitacaoArquivo.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("ArquivoSalvar", "Tramitacao") %>',
					validarExcluirEstante: '<%= Url.Action("ArquivoValidarExcluirEstante", "Tramitacao") %>',
					validarExcluirPrateleira: '<%= Url.Action("ArquivoValidarExcluirPrateleira", "Tramitacao") %>'
				},
				Mensagens: <%=  Model.Mensagens %>,
				id: '<%= Model.TramitacaoArquivo.Id %>'
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Visualizar Arquivo</h1>
		<br />

		<% Html.RenderPartial("ArquivoPartial");%>

		<div class="block box">
			<div class="block">
				<span class="cancelarCaixa"><a class="linkCancelar" title="Cancelar" href="<%= Url.Action("ArquivoListar") %>">Cancelar</a></span>
			</div>
		</div>
	</div>
</asp:Content>