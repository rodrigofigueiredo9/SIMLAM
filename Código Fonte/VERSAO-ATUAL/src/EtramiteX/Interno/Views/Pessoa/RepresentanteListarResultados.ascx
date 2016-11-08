<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPessoa" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th>Nome</th>
				<th width="15%">CPF</th>
				<th class="semOrdenacao" width="9%">Ações</th>
			</tr>
		</thead>

		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.NomeRazaoSocial)%>"><%= Html.Encode(item.NomeRazaoSocial)%></td>
				<td title="<%= Html.Encode(item.CPFCNPJ)%>" ><%= Html.Encode(item.CPFCNPJ)%></td>
				<td>
					<input type="hidden" class="itemId" value="<%= item.Id %>" />
					<%if (Model.PodeAssociar) {%><button type="button" title="Associar" class="icone associar btnAssociar"></button><% } %>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>