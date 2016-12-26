<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<PTVVM>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Editar PTV</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/mensagem.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Empreendimento/listar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/ConfiguracaoVegetal/Cultura/listar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/PTV/emitirPTV.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>"></script>
	<script>
		$(function () {
			PTVEmitir.settings.idsTela = <%= Model.IdsTela %>;
			PTVEmitir.settings.idsOrigem = <%= Model.IdsOrigem %>;
			PTVEmitir.settings.dataAtual = '<%= Model.DataAtual%>';
			PTVEmitir.load($("#central"), {
				urls: {
					urlSalvar: '<%= Url.Action("Salvar", "PTV") %>',
					urlVerificarNumeroPTV: '<%= Url.Action("VerificarNumeroPTV", "PTV") %>',
					urlAssociarEmpreendimento: '<%= Url.Action("Associar","Empreendimento") %>',
					urlAssociarDestinatario: '<%= Url.Action("DestinatarioModal","PTV") %>',
					urlAssociarCultura: '<%= Url.Action("Caracterizacoes", "ConfiguracaoVegetal/AssociarCultura") %>',
					urlObterResponsaveisEmpreend:'<%= Url.Action("ObterResponsaveisEmpreendimento","PTV") %>',
					urlObterNumeroOrigem: '<%= Url.Action("ObterNumeroOrigem","PTV") %>',
					urlObterCultura: '<%= Url.Action("ObterCultura","PTV") %>',
					urlObterCultivar: '<%= Url.Action("ObterCultivar","PTV") %>',
					urlObterUnidadeMedida: '<%= Url.Action("ObterUnidadeMedida","PTV") %>',
					urlAdicionarProdutos: '<%= Url.Action("ValidarIdentificacaoProduto","PTV") %>',
					urlValidarDocumento: '<%= Url.Action("ValidarDocDestinatario","PTV") %>',
					urlObterDestinatario: '<%= Url.Action("ObterDestinatario","PTV") %>',
					urlObterLaboratorio: '<%= Url.Action("ObterLaboratorio","PTV") %>',
					urlObterTratamentoFisso: '<%= Url.Action("ObterTratamentoFitossanitario","PTV") %>',
					urlObterItinerario: '<%= Url.Action("ObterItinerario","PTV") %>',
					urlVerificarDocumentoOrigem: '<%= Url.Action("VerificarDocumentoOrigem","PTV") %>'
				},
			});
		});
	</script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Editar Permissão de Trânsito de Vegetais</h1>
		<br />

		<%Html.RenderPartial("PTVPartial", Model); %>

		<div class="block box">
			<button class="btnSalvar floatLeft <%= Model.IsVisualizar ? "hide":"" %>" type="button" value="Salvar"><span>Salvar</span></button>
			<span class="cancelarCaixa"><span class="btnModalOu <%= Model.IsVisualizar ? "hide":"" %>">ou</span> <a class="linkCancelar" href="<%= Url.Action("Index", "PTV") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>
