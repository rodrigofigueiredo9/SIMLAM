<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ProdutoDestinacaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Produtos Apreendidos/Destinação</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Fiscalizacao/fiscalizacaoProdutosDestinos.js") %>" ></script>
	<script>
		$(function () {
		    ConfigurarProdutosDestinos.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("ConfigurarProdutoDestino", "Fiscalizacao") %>',
					<%--podeDesativar: '<%= Url.Action("ConfiguracaoIsAssociadoCampoInfracao", "Fiscalizacao") %>',
					podeEditar: '<%= Url.Action("PodeEditarCampoInfracao", "Fiscalizacao") %>',
					Excluir: '<%= Url.Action("ExcluirCampoInfracao", "Fiscalizacao") %>',
					ExcluirConfirm: '<%= Url.Action("ExcluirCampoInfracaoConfirm", "Fiscalizacao") %>',
					alterarSituacao: '<%= Url.Action("AlterarSituacaoCampoInfracao", "Fiscalizacao") %>'--%>
				},
				<%--mensagens: <%= Model.Mensagens %>,
				idsTela: <%= Model.IdsTela %>--%>
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">

		<h1 class="titTela">Produtos Apreendidos/Destinação</h1><br />

		<fieldset class="box">
            
            <legend class="titFiltros">Produto</legend>

			<div class="block">
				<div class="coluna35 append2">
					<label for="Item_NomeProduto">Item</label>
					<%= Html.TextBox("Item", String.Empty, new { @class = "text txtProdutoItem", @maxlength = "100" } )%>
				</div>

				<div class="coluna15 append1">
					<label for="Item_UnidadeProduto">Unidade</label>
					<%= Html.TextBox("Unidade",String.Empty, new { @class = "text txtProdutoUnidade", @maxlength = "50"})%>
				</div>

				<div class="coluna7">
                    <button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarProduto" title="Adicionar Produto">+</button>
				</div>

				<input type="hidden" class="hdnItemId" value='0' />
				<input type="hidden" class="hdnItemIsAtivo" value='1' />
			</div>

            <div class="DivItens">
				<% Html.RenderPartial("FiscalizacaoProdutos"); %>
			</div>
		</fieldset>
	</div>
</asp:Content>
