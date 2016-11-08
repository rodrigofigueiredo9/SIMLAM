<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMChecagemRoteiro" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarCheckListRoteiroVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="10%">Número</th>
				<th>Interessado</th>
				<th width="13%">Situação</th>
				<th class="semOrdenacao" width="14%">Ações</th>
			</tr>
		</thead>

		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.Id)%>"><%= Html.Encode(item.Id)%></td>
				<td title="<%= Html.Encode(item.Interessado)%>"><%= Html.Encode(item.Interessado)%></td>
				<td title="<%= Html.Encode(item.SituacaoTexto)%>"><%= Html.Encode(item.SituacaoTexto)%></td>
				<td>
					<input type="hidden" class="itemId" value="<%= item.Id %>" />
					<input type="hidden" class="itemSituacaoId" value="<%= item.Situacao %>" />

					<%if (Model.PodeAssociar) {%><button type="button" title="Associar" class="icone associar btnAssociar"></button><% } %>
					<%if (Model.PodeVisualizar) {%><button type="button" title="Visualizar" class="icone visualizar btnVisualizar"></button><% } %>
					<%if (Model.PodeExcluir) {%><button type="button" title="Excluir" class="icone excluir btnExcluir"></button><% } %>
					<%if (item.TemPendencia){%><button type="button" title="Gerar PDF" class="icone pdf btnGerarPdf"></button><% } %>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>