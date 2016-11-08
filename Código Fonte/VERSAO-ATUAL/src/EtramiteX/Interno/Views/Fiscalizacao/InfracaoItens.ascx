<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="block dataGrid">
	<div class="coluna65">
		<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
			<thead>
				<tr>
					<th width="40%"><%:Model.TituloGrid %></th>
					<th width="12%">Situação</th>
					<%if (!Model.IsVisualizar){%><th width="19%">Ações</th><%} %>
				</tr>
			</thead>
			<% foreach (var item in Model.Entidade.Itens){ %>
			<tbody>
				<tr>
					<td>
						<span class="nomeCampo" title="<%:item.Texto%>"><%:item.Texto%></span>
					</td>
					<td>
						<span class="situacao" title="<%:item.SituacaoTexto%>"><%:item.SituacaoTexto%></span>
					</td>
					<td class="tdAcoes">
						<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(item)%>' />
						<input type="hidden" value="<%= item.Id %>" class="itemId" />
						<input title="Editar <%:Model.Nome %>" type="button" class="icone editar btnEditarItem" value="" />
						<input title="Desativar <%:Model.Nome %>" type="button" class="icone cancelar btnDesativarItem" value="" />
						<input title="Ativar <%:Model.Nome %>" type="button" class="icone recebido btnAtivarItem" value="" />
						<input title="Excluir <%:Model.Nome %>" type="button" class="icone excluir btnExcluirItem" value="" />
					</td>
					<%} %>
				</tr>
				<tr class="trTemplateRow hide">
					<td><span class="nomeCampo"></span></td>
					<td><span class="situacao"></span></td>
					<td class="tdAcoes">
						<input type="hidden" class="hdnItemJSon" value="" />
						<input title="Editar <%:Model.Nome %>" type="button" class="icone editar btnEditarItem" value="" />
						<input title="Desativar <%:Model.Nome %>" type="button" class="icone cancelar btnDesativarItem" value="" />
						<input title="Ativar <%:Model.Nome %>" type="button" class="icone recebido btnAtivarItem" value="" />
						<input title="Excluir <%:Model.Nome %>" type="button" class="icone excluir btnExcluirItem" value="" />
					</td>
				</tr>
			</tbody>
		</table>
	</div>
</div>