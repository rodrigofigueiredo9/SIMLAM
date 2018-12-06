<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<RetificacaoNFCaixaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Retificação NF Caixa</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/PTV/retificacaoNFCaixa.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Empreendimento/listar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/ConfiguracaoVegetal/Cultura/listar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/unidadeProducao.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/unidadeProducaoItem.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/PTV/emitirPTV.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>"></script>
	<script>
		$(function () {
			NFCaixa.settings.urls = {
				urlExcluirConfirm: '<%= Url.Action("RetificacaoNFCaixaExcluirConfirm", "PTV")%>',
				urlExcluir: '<%= Url.Action("RetificacaoNFCaixaExcluir", "PTV") %>',
				urlEditar: '<%= Url.Action("RetificacaoNFCaixaEditar", "PTV") %>',
				urlSalvar: '<%= Url.Action("RetificacaoNFCaixaSalvar", "PTV") %>',
				urlPTVNFCaixaPag: '<%= Url.Action("PTVNFCaixaPaginacao", "PTV") %>',
			}
			NFCaixa.load($("#central"));

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])) { %>
				ContainerAcoes.load($(".containerAcoes"), {
					urls:{
						
						urlGerarPdf: '<%= Url.Action("GerarPdf", "PTV", new { id = Request.Params["acaoId"].ToString() }) %>',
						urlAtivar: '<%= Url.Action("Ativar", "PTV", new { id = Request.Params["acaoId"].ToString() }) %>',
						urlNovo: '<%= Url.Action("Criar", "PTV") %>'
					}
				});
			<% } %>
		});
	</script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Retificação Nota Fiscal de Caixa</h1>
		<br />

		<%Html.RenderPartial("RetificacaoNFCaixaListarFiltros", Model); %>

	</div>
</asp:Content>