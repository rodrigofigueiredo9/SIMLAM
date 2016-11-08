<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPTVOutro" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PTVOutroListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="18%">Número PTV</th>
				<th>Nome do Interessado</th>
				<th width="20%">Destinatário</th>
				<th width="10%">Situação</th>
				<th class="semOrdenacao" width="15%">Ações</th>
			</tr>
		</thead>

		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.Numero)%>"><%= Html.Encode(item.Numero)%></td>
				<td title="<%= Html.Encode(item.Interessado)%>"><%= Html.Encode(item.Interessado)%></td>
				<td title="<%= Html.Encode(item.Destinatario)%>"><%= Html.Encode(item.Destinatario)%></td>
				<td title="<%= Html.Encode(item.SituacaoTexto)%>"><%= Html.Encode(item.SituacaoTexto)%></td>
				<td>
					<input type="hidden" class="itemJson" value="<%: ViewModelHelper.Json(new { Id = item.ID, Numero = item.Numero, CulturaCultivar = item.CulturaCultivar, Situacao = item.SituacaoTexto } ) %>" />
					<% if (Model.PodeVisualizar) { %><input type="button" title="Visualizar PTV Outro estado" class="icone visualizar btnVisualizar" /><% } %>
					<% if (Model.PodeCancelar && item.SituacaoID == 2)
                        { %><input type="button" title="Cancelar PTV Outro estado" class="icone cancelar btnCancelar" /><% } %>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>