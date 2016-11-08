<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PragaListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="40%">Nome Científico</th>
				<th>Nome Comum</th>
				<th class="semOrdenacao" width="10%">Ações</th>
			</tr>
		</thead>

		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.NomeCientifico)%>"><%= Html.Encode(item.NomeCientifico)%></td>
				<td title="<%= Html.Encode(item.NomeComum)%>"><%= Html.Encode(item.NomeComum)%></td>
				<td>
					<input type="hidden" class="itemJson" value="<%: ViewModelHelper.Json(new { Id = item.Id, NomeCientifico = item.NomeCientifico, NomeComum = item.NomeComum, Cultura = item.CulturasTexto }) %>" />
					<%if (Model.PodeEditar) {%>
						<input type="button" title="Editar" class="icone editar btnEditar" />
					<% } %>

					<%if (Model.PodeAssociar) {%>
						<input type="button" title="Associar Culturas à Praga" class="icone associar btnAssociar" />
					<% } %>

						<%if (Model.Associar) {%>
						<input type="button" title="Associar Praga" class="icone associar btnAssociarPraga" />
					<% } %>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>