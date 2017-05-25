<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCredenciado" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0)?string.Empty:"hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th>Responsável técnico</th>
				<th width="20%">Nº da habilitação</th>
				<th width="10%">Situação</th>
                <th width="20%">Motivo</th>
				<th class="semOrdenacao" width="17%">Ações</th>
			</tr>
		</thead>
		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td class="responsavelNomeRazaoSocial" title="<%= Html.Encode(item.NomeRazaoSocial)%>"><%= Html.Encode(item.NomeRazaoSocial)%></td>
				<td class="numeroHabilitacao" title="<%= Html.Encode(item.NumeroHabilitacao)%>"><%= Html.Encode(item.NumeroHabilitacao)%></td>
				<td class="situacao" title="<%= item.SituacaoTexto%>"><%= item.SituacaoTexto%></td>
                <td class="motivo" title="<%= item.MotivoTexto%>"><%= item.MotivoTexto%></td>
				<td>
					<input type="hidden" class="itemId" value="<%= item.Id %>" />
					<input type="hidden" class="itemTid" value="<%= item.Tid %>" />
					<%if (Model.PodeVisualizar){%><input type="button" title="PDF do Habilitar Emissão" class="icone pdf btnPDF" /><% } %>
					<%if (Model.PodeVisualizar) { %><input type="button" title="Visualizar" class="icone visualizar btnVisualizar" /><%}%>
					<%if (Model.PodeEditar) {%><button type="button" title="Editar" class="icone editar btnEditar"></button><% } %>
					<%if (Model.PodeAlterarSituacao) {%>
						<input type="button" name="alterar situação" title="Alterar situação" class="icone altStatus btnAltStatus" />
					<% }%>
                    <%if (Model.PodeVisualizar) { %><input type="button" title="Histórico" class="icone historico btnHistorico" /><%}%>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>