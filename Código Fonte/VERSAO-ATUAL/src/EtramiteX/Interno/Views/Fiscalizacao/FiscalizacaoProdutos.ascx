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
                <% foreach (var item in Model.ListaProdutos){ %>
				    <tr>
					    <td>
						    <span class="nomeItem" title="<%:item.Item%>"><%:item.Item%></span>
					    </td>
					    <td>
						    <span class="unidadeMedida" title="<%:item.Unidade%>"><%:item.Unidade%></span>
					    </td>
					    <td class="tdAcoes">
						    <input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(item)%>' />
						    <input type="hidden" value="<%= item.Id %>" class="itemId" />
						    <input title="Editar campo" type="button" class="icone editar btnEditarItem" value="" />
						    <input title="Desativar campo" type="button" class="icone cancelar btnDesativarItem" value="" />
						    <input title="Ativar campo" type="button" class="icone recebido btnAtivarItem" value="" />
						    <input title="Excluir campo" type="button" class="icone excluir btnExcluirItem" value="" />
					    </td>
				    </tr>
                <%} %>
				<tr class="trTemplateRow hide">
					<td><span class="nomeItem"></span></td>
					<td><span class="unidadeMedida"></span></td>
					<td class="tdAcoes">
						<input type="hidden" class="hdnItemJSon" value="" />
                        <input type="hidden" value="" class="itemId" />
						<input title="Editar campo" type="button" class="icone editar btnEditarItem" value="" />
						<input title="Desativar campo" type="button" class="icone cancelar btnDesativarItem" value="" />
						<input title="Ativar campo" type="button" class="icone recebido btnAtivarItem" value="" />
						<input title="Excluir campo" type="button" class="icone excluir btnExcluirItem" value="" />
					</td>
				</tr>
			</tbody>
		</table>
	</div>
</div>