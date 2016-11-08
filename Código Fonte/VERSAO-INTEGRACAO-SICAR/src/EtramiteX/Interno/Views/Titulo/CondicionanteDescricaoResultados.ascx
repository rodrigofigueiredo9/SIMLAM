<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CondicionanteDescricaoListarVM>" %>

<%= Html.Hidden("Paginacao.PaginaAtual", null, new { @class = "paginaAtual" })%>
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0)?string.Empty:"hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>
	
	<table class="dataGridTable ordenavel" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th>Descrição</th>
				<th class="semOrdenacao" width="12%">Ações</th>
			</tr>
		</thead>
		<tbody>
			<% for (int i = 0; i < Model.Resultados.Count; i++){ %>
			<tr>
				<td class="tdDescricao" title="<%= Html.Encode(Model.Resultados[i].Descricao)%>"><%= Html.Encode(Model.Resultados[i].Descricao)%></td>
				<td>
					<%= Html.Hidden("_descricao_", Model.Resultados[i].Descricao, new { @class = "hdnDescricao" })%>
					<%= Html.Hidden("_itemid_", Model.Resultados[i].Id, new { @class = "hdnItemId" })%>
					<input type="button" title="Associar descrição de condicionante" class="icone associar inlineBotao btnAssociar"/>
					<input type="button" title="Editar" class="icone editar inlineBotao btnEditar"/>
					<input type="button" title="Excluir" class="icone excluir inlineBotao btnExcluir"/>
				</td>
			</tr>
			<% } %>
		</tbody>
	</table>
</div>