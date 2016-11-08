<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMOrgaosParceirosConveniados" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="20%">Sigla</th>
				<th width="50%">Nome do órgão</th>
				<th width="15%">Situação</th>
				<th class="semOrdenacao" width="14%">Ações</th>
			</tr>
		</thead>

		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.Sigla)%>"><%= Html.Encode(item.Sigla)%></td>
				<td title="<%= Html.Encode(item.Nome)%>"><%= Html.Encode(item.Nome)%></td>
				<td title="<%= Html.Encode(item.SituacaoTexto)%>"><%= Html.Encode(item.SituacaoTexto)%></td>
				<td>
					<input type="hidden" class="itemJson" value="<%: ViewModelHelper.Json(new { Id = item.Id, Situacao = item.SituacaoId, Nome = item.Nome }) %>" />
					<%if (Model.PodeVisualizar) {%><input type="button" title="Visualizar" class="icone visualizar btnVisualizar" /><% } %>
					<%if (Model.PodeEditar) {%><input type="button" title="Editar" class="icone editar btnEditar" /><% } %>
					<%if (Model.PodeAlterarSituacao){%><input type="button" title="Alterar Situação" class="icone sitTitulo btnAlterarSituacao" /><% } %>
					<%if (Model.PodeGerenciar){%><input type="button" title="Gerenciar" class="icone opcoes btnGerenciar" /><% } %>
					
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>