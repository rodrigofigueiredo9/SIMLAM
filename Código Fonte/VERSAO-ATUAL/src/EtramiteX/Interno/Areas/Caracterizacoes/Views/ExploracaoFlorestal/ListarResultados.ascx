﻿<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMExploracaoFlorestal" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="17%">Tipo de Atividade</th>
				<th width="17%">Código Exploração</th>
				<th width="17%">Data Exploração</th>
				<th>Localizador</th>
				<th width="15%">Ações</th>
			</tr>
		</thead>

		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.TipoAtividadeTexto)%>"><%= Html.Encode(item.TipoAtividadeTexto)%></td>
				<td title="<%= Html.Encode(item.CodigoExploracaoTexto)%>"><%= Html.Encode(item.CodigoExploracaoTexto)%></td>
				<td title="<%= Html.Encode(item.DataCadastro.DataTexto)%>" ><%= Html.Encode(item.DataCadastro.DataTexto)%></td>
				<td title="<%= Html.Encode(item.Localizador)%>"><%= Html.Encode(item.Localizador)%></td>
				<td>
					<input type="hidden" value="<%= item.Id %>" class="itemId" />
					<%if (Model.PodeVisualizar) {%><button type="button" title="Visualizar" class="icone visualizar btnVisualizar"></button><% } %>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>
