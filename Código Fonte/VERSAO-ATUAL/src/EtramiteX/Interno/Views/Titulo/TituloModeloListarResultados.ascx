<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TituloModeloListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0)?string.Empty:"hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th>Nome</th>
				<th width="15%">Tipo</th>
				<th width="10%">Situação</th>
				<th class="semOrdenacao" width="13%">Ações</th>
			</tr>
		</thead>

		<tbody>
			<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.Nome)%>"><%= Html.Encode(item.Nome)%></td>
				<td title="<%= Html.Encode(item.TipoTexto)%>"><%= Html.Encode(item.TipoTexto)%></td>
				<td title="<%= Html.Encode(item.SituacaoTexto)%>"><%= Html.Encode(item.SituacaoTexto)%></td>
				<td>
					<input type="hidden" class="itemId" value="<%= item.Id %>" />

					<%if (Model.PodeVisualizar) {%><input type="button" title="Visualizar" class="icone visualizar btnVisualizar" /><% } %>
					<%if (Model.PodeEditar) {%><input type="button" title="Editar" class="icone editar btnEditar" /><% } %>
					<%if (Model.PodeAlterarSituacao && item.SituacaoId == 2){%><input type="button" title="Ativar" class="icone recebido btnAtivar" /><% } %>
					<%if (Model.PodeAlterarSituacao && item.SituacaoId == 1){%><input type="button" title="Desativar" class="icone cancelar btnDesativar" /><% } %>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>