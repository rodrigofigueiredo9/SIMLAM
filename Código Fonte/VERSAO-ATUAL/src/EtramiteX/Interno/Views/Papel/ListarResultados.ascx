<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPapel" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th>Nome</th>
				<th class="semOrdenacao" width="13%">Ações</th>
			</tr>
		</thead>

		<tbody>
			<% foreach (var papel in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(papel.Nome)%>"><%= Html.Encode(papel.Nome)%></td>
				<td>
					<input type="hidden" class="itemId" value="<%= papel.Id %>" />

					<%if (Model.PodeVisualizar) {%><input type="button" title="Visualizar" class="icone visualizar btnVisualizar" /><% } %>
					<%if (Model.PodeEditar) {%><input type="button" title="Editar" class="icone editar btnEditar" /><% } %>
					<%if (Model.PodeExcluir) {%><input type="button" title="Excluir" class="icone excluir btnExcluir" /><% } %>
				</td>
			</tr>
			<% } %>
		</tbody>
	</table>
</div>