<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InformacaoCorteInformacaoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>

<div class="informacaoCorteInformacaoProdutoContainer">
	<fieldset class="boxBranca">
		<legend>Finalidade do produto</legend>
		<% if(!Model.IsVisualizar) { %>
		<div class="block">
			<div class="coluna15 append2">
				<label for="InformacaoCorteInformacao_ProdutoTipo">Produto *</label>
				<%= Html.DropDownList("InformacaoCorteInformacao.ProdutoTipo", Model.ProdutoTipo, new { @class = "text ddlProdutoTipo " })%>
			</div>

			<div class="coluna22 append2">
				<label for="InformacaoCorteInformacao_DestinacaoTipo">Destinação do material *</label>
				<%= Html.DropDownList("InformacaoCorteInformacao.DestinacaoTipo", Model.DestinacaoTipo, new { @class = "text ddlDestinacaoTipo " })%>
			</div>

			<div class="coluna20 append2">
				<label for="InformacaoCorteInformacao_ProdutoQuantidade">Quantidade *</label>
				<%= Html.TextBox("InformacaoCorteInformacao.ProdutoQuantidade", String.Empty, new { @class = "text txtProdutoQuantidade maskDecimalPonto", @maxlength = "12" })%>
			</div>

			<div class="coluna10">
				<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarProduto btnAddItem" title="Adicionar">+</button>
			</div>
		</div>
		<% } %>

		<div class="block dataGrid">
			<div class="coluna100">
				<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
					<thead>
						<tr>
							<th width="30%">Produto</th>
							<th width="30%">Destinação do material</th>
							<th width="30%">Quantidade</th>
							<%if (!Model.IsVisualizar){%><th width="9%">Ação</th><%} %>
						</tr>
					</thead>
					<% foreach (var produto in Model.Entidade.Produtos){ %>
					<tbody>
						<tr>
							<td>
								<span class="produtoTipo" title="<%:produto.ProdutoTipoTexto%>"><%:produto.ProdutoTipoTexto%></span>
							</td>
							<td>
								<span class="destinacaoTipo" title="<%:produto.DestinacaoTipoTexto%>"><%:produto.DestinacaoTipoTexto%></span>
							</td>
							<td>
								<span class="quantidade" title="<%:produto.Quantidade%>"><%:produto.Quantidade%></span>
							</td>
							<%if (!Model.IsVisualizar){%>
								<td class="tdAcoes">
									<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(produto)%>' />
									<input title="Excluir" type="button" class="icone excluir btnExcluirProduto" value="" />
								</td>
							<%} %>
						</tr>
						<% } %>
						<% if(!Model.IsVisualizar) { %>
							<tr class="trTemplateRow hide">
								<td><span class="produtoTipo"></span></td>
								<td><span class="destinacaoTipo"></span></td>
								<td><span class="quantidade"></span></td>
								<td class="tdAcoes">
									<input type="hidden" class="hdnItemJSon" value="" />
									<input title="Excluir" type="button" class="icone excluir btnExcluirProduto" value="" />
								</td>
							</tr>
						<% } %>
					</tbody>
				</table>
			</div>
		</div>
	</fieldset>
</div>
<br />