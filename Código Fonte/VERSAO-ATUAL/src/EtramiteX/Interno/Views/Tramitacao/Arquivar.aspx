<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ArquivarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Arquivar Processo/Documento</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
<script src="<%= Url.Content("~/Scripts/Tramitacao/arquivar.js") %>"></script>

<script>
	$(function () {
		Arquivar.load($('#central'), {
			urls: {
				arquivarObterTodos: '<%= Url.Action("ArquivarObterTodos", "Tramitacao") %>',
				arquivarBuscarEstantes: '<%= Url.Action("ArquivarBuscarEstantes", "Tramitacao") %>',
				arquivarBuscarPrateleiras: '<%= Url.Action("ArquivarBuscarPrateleiras", "Tramitacao") %>',
				arquivarAdicionarItem: '<%= Url.Action("ArquivarAdicionarItem", "Tramitacao") %>',
				arquivosCadastrados: '<%= Url.Action("ObterArquivosCadastradosSetor", "Tramitacao") %>',
				arquivar: '<%= Url.Action("Arquivar", "Tramitacao") %>',
				abrirPdfItem: '<%= Url.Action("GerarPdf", "Tramitacao") %>',
				visualizarProc: '<%= Url.Action("Visualizar", "Processo") %>',
				visualizarDoc: '<%= Url.Action("Visualizar", "Documento") %>',
				visualizarHistorico: '<%= Url.Action("Historico", "Tramitacao") %>'
			},
			Mensagens: <%= Model.Mensagens %>
		});
	});
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ArquivarPartial", Model); %>

		<div class="block box btnEnviarContainer">
			<input class="floatLeft btnArquivar" type="button" value="Arquivar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>