<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTVOutro" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<PTVOutroVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Cadastrar PTV de Outro Estado</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/ConfiguracaoVegetal/Cultura/listar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/PTVOutro/emitirPTVOutro.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>"></script>

	<script>
		$(function () {
			PTVOutroEmitir.load($("#central"), {
				urls: {
					urlVerificarNumeroPTV: '<%= Url.Action("VerificarNumeroPTV", "PTVOutro") %>',
					urlAssociarCultura: '<%= Url.Action("Caracterizacoes", "ConfiguracaoVegetal/AssociarCultura") %>',
					urlObterCultivar: '<%= Url.Action("ObterCultivar","PTVOutro") %>',
					urlObterMunicipio: '<%= Url.Action("ObterMunicipio", "PTVOutro") %>',
					urlAdicionarProdutos: '<%= Url.Action("ValidarIdentificacaoProduto","PTVOutro") %>',
					urlValidarDocumento: '<%= Url.Action("ValidarDocDestinatario","PTV") %>',
					urlAssociarDestinatario: '<%= Url.Action("DestinatarioModal","PTV") %>',
					urlObterDestinatario: '<%= Url.Action("ObterDestinatario","PTV") %>',
					urlSalvar: '<%= Url.Action("Salvar", "PTVOutro") %>'
				},
			});

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"]))  { %>
			ContainerAcoes.load($(".containerAcoes"), {
				botoes: [
					{ label: 'Visualizar', url: '<%= Url.Action("Visualizar", "PTVOutro", new { id= Request.Params["acaoId"].ToString() })%>' },
					{ label: 'Cancelar', url: '<%= Url.Action("Cancelar", "PTVOutro", new { id= Request.Params["acaoId"].ToString() })%>' }
				]
			});
			<% } %>
		});
	</script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Cadastrar PTV de Outro Estado</h1>
		<br />

		<%Html.RenderPartial("PTVOutroPartial", Model); %>

		<div class="block box">
			<button class="btnSalvar floatLeft <%= Model.IsVisualizar ? "hide":"" %>" type="button" value="Salvar"><span>Salvar</span></button>
			<span class="cancelarCaixa"><span class="btnModalOu <%= Model.IsVisualizar ? "hide":"" %>">ou</span> <a class="linkCancelar" href="<%= Url.Action("Index", "PTVOutro") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>