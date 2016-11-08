<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMAtividade" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarAtividadeVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="10%">Setor</th>
				<th>Nome</th>
				<th width="20%">Agrupador</th>
				<th class="semOrdenacao" width="7%">Ações</th>
			</tr>
		</thead>
		<tbody>
			<% foreach (var item in Model.Resultados) { %>
				<tr>
					<td title="<%= Html.Encode(item.SetorTexto)%>"><%= Html.Encode(item.SetorSigla)%></td>
					<td title="<%= Html.Encode(item.AtividadeNome)%>"><%= Html.Encode(item.AtividadeNome)%></td>
					<td title="<%= Html.Encode(item.AgrupadorTexto)%>"><%= Html.Encode(item.AgrupadorTexto)%></td>
					<td>
						<input type="hidden" value="<%= Model.ObterJSon(item) %>" class="itemJson" />
						<input type="button" name="associar" title="Associar" class="icone associar btnAssociarAtividade" />
					</td>
				</tr>
			<% } %>
		</tbody>
	</table>
</div>