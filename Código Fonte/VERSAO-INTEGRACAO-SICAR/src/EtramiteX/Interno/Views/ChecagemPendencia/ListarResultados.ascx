<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMChecagemPendencia" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0)?string.Empty:"hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="12%">Nº Checagem</th>
				<th width="16%">Processo/Documento</th>
				<th>Interessado do título</th>
				<th width="20%">Situação da checagem</th>
				<th class="semOrdenacao" width="9%">Ações</th>
			</tr>
		</thead>
		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.Numero)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Numero))%></td>
				<td title="<%= Html.Encode(item.ProtocoloNumero)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.ProtocoloNumero))%></td>
				<td title="<%= Html.Encode(item.InteressadoNome)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.InteressadoNome))%></td>
				<td title="<%= Html.Encode(item.SituacaoTexto)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.SituacaoTexto))%></td>
				<td>
					<input type="hidden" value="<%= item.Id %>" class="itemId" />
					<input type="hidden" value="<%= item.SituacaoId %>" class="itemSituacaoId" />

					<%if (Model.PodeAssociar) {%><button type="button" title="Associar" class="icone associar btnAssociar"></button><% } %>
					<%if (Model.PodeVisualizar) {%><button type="button" title="Visualizar" class="icone visualizar btnVisualizar"></button><% } %>
					<%if (Model.PodeExcluir) {%><button type="button" title="Excluir" class="icone excluir btnExcluir"></button><% } %>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>