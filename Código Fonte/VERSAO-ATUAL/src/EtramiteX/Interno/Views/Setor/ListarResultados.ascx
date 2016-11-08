<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMSetor" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0)?string.Empty:"hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="15%">Agrupador</th>
				<th width="40%">Setor</th>
				<th width="25%">Município</th>
				<th class="semOrdenacao" width="10%">Ações</th>
			</tr>
		</thead>

		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td class="setorLocalizarAgrupador" title="<%= Html.Encode(item.Agrupador)%>"><%= Html.Encode(item.AgrupadorTexto)%></td>
				<td class="setorLocalizarSetor" title="<%= Html.Encode(item.Nome)%>"><%= Html.Encode(item.Nome)%></td>
				<td class="setorLocalizarMunicípio" title="<%= Html.Encode(item.Endereco.MunicipioTexto)%>"><%= Html.Encode(item.Endereco.MunicipioTexto)%></td>
				<td>
					<input type="hidden" class="itemId" value="<%= item.Id %>" />
					<input type="hidden" class="setorLocalizarTid" value="<%= item.Tid %>" />

					<%if (Model.PodeVisualizar) { %><input type="button" title="Visualizar" class="icone visualizar btnVisualizar" /><%}%>
					<%if (Model.PodeEditar) { %><input type="button" title="Editar" class="icone editar btnEditar" /><%}%>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>