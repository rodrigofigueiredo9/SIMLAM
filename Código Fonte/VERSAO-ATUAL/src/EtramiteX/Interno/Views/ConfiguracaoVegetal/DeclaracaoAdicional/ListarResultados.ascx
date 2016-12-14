<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DeclaracaoAdicionalListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="40%">Texto</th>
				<th class="semOrdenacao" width="10%">Outro Estado?</th>
                <th class="semOrdenacao" width="13%">Ações</th>
			</tr>
		</thead>

		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.Texto)%>"><%= Html.Encode(item.Texto)%></td>
                <td title="<%= Html.Encode(item.OutroEstado==1 ? "Sim" : "Não")%>"><%= Html.Encode(item.OutroEstado==1 ? "Sim" : "Não")%></td>
				<td>
					<input type="hidden" class="itemJson" value="<%: ViewModelHelper.Json(new { Id = item.Id, Texto = item.Texto, TextoFormatado = item.TextoFormatado, OutroEstado = item.OutroEstado }) %>" />
					<input type="button" title="Editar" class="icone editar btnEditar" />
					<input type="button" title="Excluir" class="icone excluir btnExcluir" />
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>