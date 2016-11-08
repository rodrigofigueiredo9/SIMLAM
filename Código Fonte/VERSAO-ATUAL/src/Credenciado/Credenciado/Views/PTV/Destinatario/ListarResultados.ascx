<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPTV" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DestinatarioPTVListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th>Nome</th>
				<th width="20%">CPF/CNPJ</th>
				<th class="semOrdenacao" width="13%">Ações</th>
			</tr>
		</thead>

		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.Nome)%>"><%= Html.Encode(item.Nome)%></td>
				<td title="<%= Html.Encode(item.CPFCNPJ)%>"><%= Html.Encode(item.CPFCNPJ)%></td>
				<td>
					<input type="hidden" class="itemJson" value="<%: ViewModelHelper.Json(new { Id = item.ID, Nome = item.Nome, CPFCNPJ = item.CPFCNPJ }) %>" />
					<input type="button" title="Visualizar" class="icone visualizar btnVisualizar" />
					<% if (Model.Associar) { %><input type="button" title="Associar praga" class="icone associar btnAssociar" /><% } %>
					<% if (Model.PodeEditar) { %><input type="button" title="Editar" class="icone editar btnEditar" /><% } %>
					<% if (Model.PodeExcluir) { %><input type="button" title="Excluir" class="icone excluir btnExcluir" /><% } %>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>