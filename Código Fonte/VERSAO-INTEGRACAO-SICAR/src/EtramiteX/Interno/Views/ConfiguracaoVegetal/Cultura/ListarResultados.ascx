<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CulturaListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="40%">Cultura</th>
				<th>Cultivar</th>
				<th class="semOrdenacao" width="7%">Ações</th>
			</tr>
		</thead>

		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.Cultura)%>"><%= Html.Encode(item.Cultura)%></td>
				<td title="<%= Html.Encode(item.Cultivar)%>"><%= Html.Encode(item.Cultivar)%></td>
				<td>
					<input type="hidden" class="itemJson" value="<%: ViewModelHelper.Json(new { Id = item.Id, Nome = item.Cultura, Cultivar =item.Cultivar, CultivarId = item.CultivarId }) %>" />
					<%if (Model.PodeEditar) {%>
						<input type="button" title="Editar" class="icone editar btnEditar" />
					<% } %>
					<%if (Model.Associar) {%>
						<input type="button" title="Associar praga" class="icone associar btnAssociar" />
					<% } %>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>