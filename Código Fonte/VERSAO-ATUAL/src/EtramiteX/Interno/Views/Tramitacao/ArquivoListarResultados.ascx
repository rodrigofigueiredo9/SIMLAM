<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ArquivoListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0)?string.Empty:"hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th>Nome</th>
				<th>Setor</th>
				<th class="semOrdenacao" width="13%">Ações</th>
			</tr>
		</thead>

		<tbody>
			<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.Nome)%>"><%= Html.Encode(item.Nome)%></td>
				<td title="<%= Html.Encode(item.SetorNome)%>"><%= Html.Encode(item.SetorNome)%></td>
				<td>
					<input type="hidden" class="itemId" value="<%= item.Id %>" />
					<%if (Model.PodeVisualizar) {%><input type="button" title="Visualizar" class="icone visualizar btnVisualizar" /><% } %>
					<%if (Model.PodeEditar) {%><input type="button" title="Editar" class="icone editar btnEditar" /><% } %>
					<%if (Model.PodeExcluir) {%><input type="button" title="Excluir" class="icone excluir btnExcluir" /><% } %>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>