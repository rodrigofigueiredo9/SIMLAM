<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCFOCFOC.CFOC" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LoteListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0)?string.Empty:"hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="15%">Número do lote</th>
				<th width="15%">Data de criação</th>
				<th>Cultivar</th>
				<th width="13%">Saldo Remanescente</th>
				<th class="semOrdenacao" width="19%">Ações</th>
			</tr>
		</thead>
		<tbody>
			<% foreach (var item in Model.Resultados)
	  { %>
			<tr>
				<td title="<%: item.NumeroCompleto %>"><%: item.NumeroCompleto %></td>
				<td title="<%: item.DataCriacao.DataTexto %>"><%: item.DataCriacao.DataTexto %></td>
				<td title="<%: item.CulturaCultivar %>"><%: item.CulturaCultivar %></td>
				<td title="<%: item.Item.Quantidade + " " + item.Item.UnidadeMedidaTexto %>"><%: item.Item.Quantidade + " " + item.Item.UnidadeMedidaTexto %></td>
				<td>
					<input type="hidden" class="itemJson" value="<%= HttpUtility.HtmlEncode(ViewModelHelper.Json(item)) %>" />

					<% if (Model.PodeVisualizar)
		{ %><input type="button" title="Visualizar" class="icone visualizar btnVisualizar" /><% } %>
					<% if (Model.PodeEditar)
		{ %><input type="button" title="Editar" class="icone editar btnEditar" /><% } %>
					<% if (Model.PodeExcluir)
		{ %><input type="button" title="Excluir" class="icone excluir btnExcluir" /><% } %>
					<% if (Model.PodeAssociar)
		{ %><input type="button" title="Associar" class="icone associar btnAssociar" /><% } %>
				</td>
			</tr>
			<% } %>
		</tbody>
	</table>
</div>
