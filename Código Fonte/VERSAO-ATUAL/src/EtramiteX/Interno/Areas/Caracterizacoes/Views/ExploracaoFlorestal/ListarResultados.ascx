<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMExploracaoFlorestal" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="15%">Tipo de Exploração</th>
				<th width="15%">Código Exploração</th>
				<th width="18%">Data Cadastro da Exploração</th>
				<th width="17%">Geometria</th>
				<th width="17%">Área Requerida (ha)</th>
				<th width="19%">N° de Árvores Requeridas (und)</th>
				<th width="10%">Ações</th>
			</tr>
		</thead>

		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.TipoExploracaoTexto)%>"><%= Html.Encode(item.TipoExploracaoTexto)%></td>
				<td title="<%= Html.Encode(item.CodigoExploracaoTexto)%>"><%= Html.Encode(item.CodigoExploracaoTexto)%></td>
				<td title="<%= Html.Encode(item.DataCadastro.DataTexto)%>" ><%= Html.Encode(item.DataCadastro.DataTexto)%></td>
				<td title="<%= Html.Encode(item.GeometriaPredominanteTexto)%>"><%= Html.Encode(item.GeometriaPredominanteTexto)%></td>
				<td title="<%= Html.Encode(item.GeometriaPredominanteTexto == "Ponto" ? "-" : item.Quantidade)%>"><%= Html.Encode(item.GeometriaPredominanteTexto == "Ponto" ? "-" : item.Quantidade)%></td>
				<td title="<%= Html.Encode(item.GeometriaPredominanteTexto == "Ponto" ? item.Quantidade : "-")%>"><%= Html.Encode(item.GeometriaPredominanteTexto == "Ponto" ? item.Quantidade : "-")%></td>
				<td>
					<input type="hidden" value="<%= item.Id %>" class="itemId" />
					<%if (Model.PodeVisualizar && Model.Filtros.IsVisualizar) {%><button type="button" title="Visualizar" class="icone visualizar btnVisualizar"></button><% } %>
					<% if (Model.PodeEditar && !Model.Filtros.IsVisualizar) { %><button title="Editar" class="icone editar btnEditar" type="button"></button><% } %>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>
<br />