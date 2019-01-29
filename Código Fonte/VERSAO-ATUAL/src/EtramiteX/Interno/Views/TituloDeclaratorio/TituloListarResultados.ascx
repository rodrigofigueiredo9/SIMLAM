<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="10%">Nº título</th>
				<th width="9%">Modelo</th>
				<th width="19%">Nº Requerimento</th>
				<th>Empreendimento</th>
				<th width="11%">Situação</th>
				<th width="12%">Vencimento</th>
				<th class="semOrdenacao" width=" <%= (Model.PodeAssociar) ? "9%" : "23%" %>">Ações</th>
			</tr>
		</thead>

		<tbody>
			<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.Numero.Texto)%>"><%= Html.Encode(item.Numero.Texto)%></td>
				<td title="<%= Html.Encode(item.Modelo.Nome)%>"><%= Html.Encode(item.Modelo.Sigla)%></td>
				<td title="<%= Html.Encode(item.RequerimetoId)%>"><%= Html.Encode(item.RequerimetoId)%></td>
				<td title="<%= Html.Encode(item.EmpreendimentoTexto)%>"><%= Html.Encode(item.EmpreendimentoTexto)%></td>
				<td title="<%= Html.Encode(item.Situacao.Texto)%>"><%= Html.Encode(item.Situacao.Texto)%></td>
				<td title="<%= Html.Encode(item.DataVencimento.DataTexto)%>"><%= Html.Encode(item.DataVencimento.DataTexto)%></td>
				<td>
					<input type="hidden" class="itemJson" value="<%= Model.ObterJSon(item) %>" />

					<%if (Model.PodeAssociar) {%><input type="button" title="Associar" class="icone associar btnAssociar" /><% } %>
					<%if (Model.PodeVisualizar){%><input type="button" title="PDF do título" class="icone pdf btnPDF" /><% } %>
					<%if (!Model.PodeAssociar && Model.PodeVisualizar) {%><input type="button" title="Visualizar" class="icone visualizar btnVisualizar" /><% } %>
					<%if (Model.PodeEditar) {%><input type="button" title="Editar" class="icone editar btnEditar" /><% } %>
					<%if (Model.PodeExcluir) {%><input type="button" title="Excluir" class="icone excluir btnExcluir" /><% } %>
					<%if (Model.PodeAlterarSituacao){%><input type="button" title="Alterar situação" class="icone sitTitulo btnAlterarSituacao" /><% } %>
					<%if (item.Modelo.Sigla == "IC"){%><input type="button" title="Emitir DUA" class="icone notificacao btnEmitirDua" /><% } %>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>