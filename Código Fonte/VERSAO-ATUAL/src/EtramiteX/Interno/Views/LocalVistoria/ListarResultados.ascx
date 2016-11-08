<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMLocalVistoria" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LocalVistoriaListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th>Local Vistoria</th>
				<th class="semOrdenacao" width="10%">Ações</th>
			</tr>
		</thead>

		<tbody>
			<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.SetorTexto)%>"><%= Html.Encode(item.SetorTexto)%></td>
				<td>
					<input type="hidden" class="itemJson" value="<%: ViewModelHelper.Json(item) %>" />
                    <input type="button" title="Visualizar" class="icone visualizar btnVisualizar" />
                    <button type="button" class="icone editar btnEditar" title="Editar" ></button>
				</td>
			</tr>
			<% } %>
		</tbody>
	</table>
</div>