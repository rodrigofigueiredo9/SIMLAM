<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMMotosserra" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MotosserraListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="13%">Nº Registro</th>
				<th>Nº Fabricação/Série</th>
				<th width="13%">Marca/Modelo</th>
				<th width="13%">Nº Nota fiscal</th>
				<th>Nome/Razao social</th>
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
				<td title="<%= Html.Encode(item.NotaFiscalNumero)%>"><%= Html.Encode(item.NotaFiscalNumero)%></td>
				<td title="<%= Html.Encode(item.Proprietario.NomeRazaoSocial)%>"><%= Html.Encode(item.Proprietario.NomeRazaoSocial)%></td>
				<td title="<%= Html.Encode(item.SituacaoTexto)%>" class="tdItemSituacao"><%= Html.Encode(item.SituacaoTexto)%></td>
				<td>
					<input type="hidden" class="itemJson" value="<%: ViewModelHelper.Json(item) %>" />
					<input type="hidden" class="itemSituacaoId" value="<%: item.SituacaoId %>" />

					<%if (Model.PodeAssociar){%><button type="button" class="icone associar btnAssociar" title="Associar"></button><% } %>
					<%if (Model.PodeVisualizar) {%><button type="button" class="icone visualizar btnVisualizar" title="Visualizar"></button><% } %>
					<%if (Model.PodeEditar) {%><button type="button" class="icone editar btnEditar" title="Editar" ></button><% } %>
					<%if (Model.PodeExcluir) {%><button type="button" title="Excluir" class="icone excluir btnExcluir"></button><% } %>
					<%if (Model.PodeAlterarSituacao){%><button type="button" title="Desativar" class="icone cancelar btnDesativar"></button><% } %>
				</td>
			</tr>
			<% } %>
		</tbody>
	</table>
</div>