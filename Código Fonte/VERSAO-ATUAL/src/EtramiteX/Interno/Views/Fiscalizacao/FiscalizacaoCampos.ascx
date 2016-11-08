<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="block dataGrid">
	<div class="coluna80">
		<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
			<thead>
				<tr>
					<th width="30%">Nome do campo</th>
					<th width="15%">Tipo do campo</th>
					<th width="20%">Unidade de medida</th>
					<th>Situação</th>
					<%if (!Model.IsVisualizar){%><th width="22%">Ações</th><%} %>
				</tr>
			</thead>
			<% foreach (var item in Model.Entidade.Itens){ %>
			<tbody>
				<tr>
					<td>
						<span class="nomeCampo" title="<%:item.Texto%>"><%:item.Texto%></span>
					</td>
					<td>
						<span class="tipo" title="<%:item.TipoCampoTexto%>"><%:item.TipoCampoTexto%></span>
					</td>
					<td>
						<span class="unidadeMedida" title="<%:item.UnidadeMedidaTexto%>"><%:item.UnidadeMedidaTexto%></span>
					</td>
					<td>
						<span class="situacao" title="<%:item.SituacaoTexto%>"><%:item.SituacaoTexto%></span>
					</td>
					<td class="tdAcoes">
						<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(item)%>' />
						<input type="hidden" value="<%= item.Id %>" class="itemId" />
						<input title="Editar campo" type="button" class="icone editar btnEditarItem" value="" />
						<input title="Desativar campo" type="button" class="icone cancelar btnDesativarItem" value="" />
						<input title="Ativar campo" type="button" class="icone recebido btnAtivarItem" value="" />
						<input title="Excluir campo" type="button" class="icone excluir btnExcluirItem" value="" />
					</td>
					<%} %>
				</tr>
				<tr class="trTemplateRow hide">
					<td><span class="nomeCampo"></span></td>
					<td><span class="tipo"></span></td>
					<td><span class="unidade"></span></td>
					<td><span class="situacao"></span></td>
					<td class="tdAcoes">
						<input type="hidden" class="hdnItemJSon" value="" />
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