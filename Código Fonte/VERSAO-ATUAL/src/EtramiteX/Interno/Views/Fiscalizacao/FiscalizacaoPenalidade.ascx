<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="block dataGrid">
	<div class="coluna80">
		<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
			<thead>
				<tr>
					<th width="10%">Artigo</th>
					<th width="15%">Item</th>
					<th width="40%">Descrição</th>
                    <th>Situação</th>
					<%if (!Model.IsVisualizar){%><th width="22%">Ações</th><%} %>
				</tr>
			</thead>
			<% foreach (var item in Model.Entidade.Itens){ %>
			<tbody>
				<tr>
					<td>
						<span class="artigo" title="<%:item.Artigo%>"><%:item.Artigo%></span>
					</td>
					<td>
						<span class="item" title="<%:item.Item%>"><%:item.Item%></span>
					</td>
					<td>
						<span class="descricao" title="<%:item.Descricao%>"><%:item.Descricao%></span>
					</td>
                    <td>
						<span class="situacao" title="<%:item.SituacaoTexto%>"><%:item.SituacaoTexto%></span>
					</td>
				
					<td class="tdAcoes">
						<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(item)%>' />
						<input type="hidden" value="<%= item.Id %>" class="itemId" />
						<input title="Editar penalidade" type="button" class="icone editar btnEditarItem" value="" />
                         <input title="Desativar penalidade" type="button" class="icone cancelar btnDesativarItem" value="" />
						<input title="Ativar penalidade" type="button" class="icone recebido btnAtivarItem" value="" />
						<input title="Excluir penalidade" type="button" class="icone excluir btnExcluirItem" value="" />
                       
					</td>
					<%} %>
				</tr>
				<tr class="trTemplateRow hide">
					<td><span class="artigo"></span></td>
					<td><span class="item"></span></td>
					<td><span class="descricao"></span></td>
					<td class="tdAcoes">
						<input type="hidden" class="hdnItemJSon" value="" />
						<input title="Editar campo" type="button" class="icone editar btnEditarItem" value="" />
						<input title="Excluir campo" type="button" class="icone excluir btnExcluirItem" value="" />
                        <input title="Desativar campo" type="button" class="icone cancelar btnDesativarItem" value="" />
						<input title="Ativar campo" type="button" class="icone recebido btnAtivarItem" value="" />
					</td>
				</tr>
			</tbody>
		</table>
	</div>
</div>