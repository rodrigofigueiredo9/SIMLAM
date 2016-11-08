<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFuncionario" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th>Nome</th>
				<th>Função</th>
				<th>Setor</th>
				<th>Situação</th>
				<th class="semOrdenacao" width="13%">Ações</th>
			</tr>
		</thead>

		<tbody>
			<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.Nome)%>"><%= Html.Encode(item.Nome)%></td>
				<td title="<%= Html.Encode(item.CargosStragg)%>"><%= Html.Encode(item.CargosStragg)%></td>
				<td title="<%= Html.Encode(item.SetoresStragg)%>"><%= Html.Encode(item.SetoresStragg)%></td>
				<td title="<%= Html.Encode(item.SituacaoTexto)%>"><%= Html.Encode(item.SituacaoTexto)%></td>
				<td>
					<input type="hidden" class="itemId" value="<%= item.Id %>" />
					<input type="hidden" class="itemNome" value="<%= item.Nome %>" />
					<input type="hidden" class="itemSituacao" value="<%= item.SituacaoTexto %>" />

					<%if (Model.PodeAssociar) {%><input type="button" title="Associar" class="icone associar btnAssociar" /><% } %>
					<%if (Model.PodeVisualizar) {%><input type="button" title="Visualizar" class="icone visualizar btnVisualizar" /><% } %>
					<%if (Model.PodeEditar) {%><input type="button" title="Editar" class="icone editar btnEditar" /><% } %>
					<%if (Model.PodeAlterarSituacao) {%>
						<% if (item.Situacao != 3) { %>
							<input type="button" name="alterar situação" title="Alterar situação" class="icone altStatus btnAltStatus" />
						<% }%>
					<% }%>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>