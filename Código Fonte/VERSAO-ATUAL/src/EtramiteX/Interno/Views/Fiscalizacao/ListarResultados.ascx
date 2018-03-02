<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="10%">Número</th>
				<th width="34%">Nome/Razão social/Denominação do autuado</th>
				<th width="11%">Nº Processo</th>
				<th  width="14%">Data da vistoria</th>
				<th width="11%">Situação</th>
				<%if (Model.PodeAssociar){%><th class="semOrdenacao" width="7%">Ação</th><%}%>
				<%else{ %><th class="semOrdenacao" width="25%">Ações</th><%} %>
			</tr>
		</thead>
		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.NumeroFiscalizacao)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.NumeroFiscalizacao))%></td>
				<td title="<%= Html.Encode(item.NomeRazaoSocialAtuado)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.NomeRazaoSocialAtuado))%></td>
				<td title="<%= Html.Encode(item.NumeroProcesso)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.NumeroProcesso))%></td>
				<td title="<%= Html.Encode(item.DataFiscalizacao)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.DataFiscalizacao))%></td>
				<td title="<%= Html.Encode(item.SituacaoTexto)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.SituacaoTexto))%></td>
				<td>
					<input type="hidden" value="<%= item.Id %>" class="itemId" />
					<input type="hidden" value="<%= item.SituacaoId %>" class="itemSituacao" />
					<input type="hidden" value="" class="itemDataCriacao" />

					<%if (Model.PodeAssociar) {%><button type="button" title="Associar" class="icone associar btnAssociar"></button><% } %>
					<%if (Model.PodeVisualizar) {%><button type="button" title="Visualizar" class="icone visualizar btnVisualizar"></button><% } %>
					<%if (Model.PodeEditar) {%><button type="button"title="Editar" class="icone editar btnEditar"></button><% } %>
					<%if (Model.PodeExcluir) {%><button type="button" title="Excluir" class="icone excluir btnExcluir"></button><% } %>
					<%if (Model.PodeAlterarSituacao){%><button type="button" title="Alterar Situação" class="icone sitTitulo btnAlterarSituacao"></button><% } %>
					<%if (Model.PodeVisualizar) {%><button type="button" title="Documentos gerados" class="icone anexos btnDocumentos"></button><% } %>
					<%if (Model.PodeVisualizarAcompanhamentos) {%><button type="button" title="Acompanhamentos" class="icone btnAcompanhamentos acompanhamento"></button><% } %>
					<%if (Model.PodeVisualizar) {%><button type="button" title="Notificação" class="icone pendencias btNotificacao"></button><% } %>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>