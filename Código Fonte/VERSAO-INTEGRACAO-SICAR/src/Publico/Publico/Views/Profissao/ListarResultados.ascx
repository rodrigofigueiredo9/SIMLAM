<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMProfissao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ProfissaoListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th>Profissão</th>
				<th class="semOrdenacao" width="8%">Ações</th>
			</tr>
		</thead>

		<tbody>
			<% foreach (var Profissao in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(Profissao.Texto)%>"><%= Html.Encode(Profissao.Texto)%></td>
				<td>
					<input type="hidden" class="itemId" value="<%= Profissao.Id %>" />
					<% if (Model.IsAssociar) { %><button type="button" title="Associar" class="icone associar btnAssociar"></button><% } %>
				</td>
			</tr>
			<% } %>
		</tbody>
	</table>
</div>