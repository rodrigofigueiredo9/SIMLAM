<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IngredienteAtivoListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th>Ingrediente ativo</th>
				<th width="30%">Situação</th>
				<th class="semOrdenacao" width="10%">Ações</th>
			</tr>
		</thead>

		<tbody>
			<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.Texto)%>"><%= Html.Encode(item.Texto)%></td>
				<td title="<%= Html.Encode(item.SituacaoTexto)%>" class="tdItemSituacao"><%= Html.Encode(item.SituacaoTexto)%></td>
				<td>
					<input type="hidden" class="itemJson" value="<%: ViewModelHelper.Json(item) %>" />

					<%if (Model.PodeAssociar){%><button type="button" class="icone associar btnAssociar" title="Associar"></button><% } %>
					<%if (Model.PodeEditar) {%><button type="button" class="icone editar btnEditar" title="Editar" ></button><% } %>
					<%if (Model.PodeAlterarSituacao){%><button type="button" title="Alterar Situação" class="icone sitTitulo btnAlterarSituacao"></button><% } %>
				</td>
			</tr>
			<% } %>
		</tbody>
	</table>
</div>