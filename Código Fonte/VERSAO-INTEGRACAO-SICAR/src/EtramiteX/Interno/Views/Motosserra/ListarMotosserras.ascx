<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMMotosserra" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MotosserraListarVM>" %>

<%if (Model.Resultados.Count > 0) {%>
<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
	<thead>
		<tr>
			<th width="13%">Nº Registro</th>
			<th>Nº Fabricação/Série</th>
			<th width="13%">Marca/Modelo</th>
			<th width="30%">Nome/Razao social</th>
			<th width="8%">Situação</th>
			<th class="semOrdenacao" width="16%">Ações</th>
		</tr>
	</thead>

	<tbody>
		<% foreach (var item in Model.Resultados) { %>
		<tr>
			<td title="<%= Html.Encode(item.RegistroNumero)%>"><%= Html.Encode(item.RegistroNumero)%></td>
			<td title="<%= Html.Encode(item.SerieNumero)%>"><%= Html.Encode(item.SerieNumero)%></td>
			<td title="<%= Html.Encode(item.Modelo)%>"><%= Html.Encode(item.Modelo)%></td>
			<td title="<%= Html.Encode(item.Proprietario.NomeRazaoSocial)%>"><%= Html.Encode(item.Proprietario.NomeRazaoSocial)%></td>
			<td title="<%= Html.Encode(item.SituacaoTexto)%>" class="tdItemSituacao"><%= Html.Encode(item.SituacaoTexto)%></td>
			<td>
				<input type="hidden" class="itemJson" value="<%: ViewModelHelper.Json(item) %>" />
				<input type="hidden" class="itemSituacaoId" value="<%: item.SituacaoId %>" />

				<button type="button" class="icone visualizar btnVisualizar" title="Visualizar"></button>
				<button type="button" class="icone editar btnEditar" title="Editar" ></button>
			</td>
		</tr>
		<% } %>
	</tbody>
</table>
<%}else{ %>
	<p style="font-weight: bold">Nenhum resultado encontrado.</p>
<%} %>