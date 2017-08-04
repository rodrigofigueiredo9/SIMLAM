<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="block dataGrid">
	<div class="coluna80">
		<table class="dataGridTable tabProdutos" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
			<thead>
				<tr>
					<th>Item</th>
					<th width="30%">Unidade</th>
					<th width="25%">Ações</th>
				</tr>
			</thead>
			
			<tbody>
                <% foreach (var produto in Model.ListaProdutos){ %>
				    <tr>
					    <td>
						    <span class="nomeItem" title="<%:produto.Item%>"><%:produto.Item%></span>
					    </td>
					    <td>
						    <span class="unidadeMedida" title="<%:produto.Unidade%>"><%:produto.Unidade%></span>
					    </td>
					    <td class="tdAcoes">
						    <input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(produto)%>' />
						    <input type="hidden" value="<%= produto.Id %>" class="produtoId" />
                            <input type="hidden" value="<%= produto.Ativo %>" class="produtoAtivo" />

						    <input title="Editar produto" type="button" class="icone editar btnEditarProduto" value="" />
                            <% if (produto.Ativo){ %>
						        <input title="Desativar produto" type="button" class="icone cancelar btnDesativarProduto" value="" />
						        <input title="Ativar produto" type="button" class="icone recebido btnAtivarProduto" disabled="disabled" value="" />
                            <% }else{ %>
						        <input title="Desativar produto" type="button" class="icone cancelar btnDesativarProduto" disabled="disabled" value="" />
						        <input title="Ativar produto" type="button" class="icone recebido btnAtivarProduto" value="" />
                            <% } %>
						    <input title="Excluir produto" type="button" class="icone excluir btnExcluirProduto" value="" />
					    </td>
				    </tr>
                <%} %>

				<tr class="trTemplateRow hide">
					<td><span class="nomeItem"></span></td>
					<td><span class="unidadeMedida"></span></td>
					<td class="tdAcoes">
						<input type="hidden" class="hdnItemJSon" value="" />
						<input type="hidden" value="" class="produtoId" />
                        <input type="hidden" value="" class="produtoAtivo" />

						<input title="Editar produto" type="button" class="icone editar btnEditarProduto" disabled="disabled" value="" />
						<input title="Desativar produto" type="button" class="icone cancelar btnDesativarProduto" disabled="disabled" value="" />
						<input title="Ativar produto" type="button" class="icone recebido btnAtivarProduto" disabled="disabled" value="" />
						<input title="Excluir produto" type="button" class="icone excluir btnExcluirProduto" value="" />
					</td>
				</tr>
			</tbody>
		</table>
	</div>
</div>