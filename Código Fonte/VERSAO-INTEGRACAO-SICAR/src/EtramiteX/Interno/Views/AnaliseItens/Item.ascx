<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Item>>" %>

<table class="dataGridTable tbItens" border="0" cellspacing="0" cellpadding="0">
	<thead>
		<tr>
			<th width="10%">Recebido</th>
			<th width="30%">Item</th>
			<th width="30%">Condicionante</th>
			<th width="15%">Situação</th>
			<th width="14%">Ações</th>
		</tr>
	</thead>
	<tbody>
		<%	foreach (var item in Model) { %>
		<tr>
			<td>
				<input type="checkbox" disabled="disabled" <%= item.Recebido && item.Situacao != 5 ? "checked=\"checked\"" : ""  %> />
			</td>
			<td>
				<span class="itemNome" title="<%= Html.Encode(item.Nome) %>"><%= Html.Encode(item.Nome)%></span>
			</td>
			<td>
				<span class="itemCondicionante" title="<%= Html.Encode(item.Condicionante) %>"><%= Html.Encode(item.Condicionante)%></span>
			</td>
			<td>
				<span class="itemSituacaoTexto" title="<%= Html.Encode(item.SituacaoTexto) %>"><%= Html.Encode(item.SituacaoTexto)%></span>
			</td>
			<td>
				<input type="hidden" class="itemJSON" value='<%= ViewModelHelper.Json(item)  %>' />
				<input type="hidden" class="hdItemExcluido" value="false" />

				<input title="Analisar item" type="button" class="icone analisarItem btnAnalisarItem" />
				<input title="Histórico de análise" type="button" class="icone historico btnHistoricoItem" />
				<% if (item.Avulso) { %>
					<input title="Excluir" type="button" class="icone excluir btnExcluirItem" />
				<% } %>
			</td>
		</tr>
		<% } %>
	</tbody>
</table>