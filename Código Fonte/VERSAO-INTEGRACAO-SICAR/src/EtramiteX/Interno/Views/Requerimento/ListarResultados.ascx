<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMRequerimento" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="10%">Número</th>
				<th>Interessado</th>
				<th>Empreendimento</th>
				<th width="15%">Situação</th>
				<th class="semOrdenacao" width=" <%= (Model.PodeAssociar) ? "9%" : "17%" %>">Ações</th>
			</tr>
		</thead>
		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.Numero)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Numero))%></td>
				<td title="<%= Html.Encode(item.Interessado.NomeRazaoSocial)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Interessado.NomeRazaoSocial))%></td>
				<td title="<%= Html.Encode(item.Empreendimento.Denominador)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Empreendimento.Denominador))%></td>
				<td title="<%= Html.Encode(item.SituacaoTexto)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.SituacaoTexto))%></td>
				<td>
					<input type="hidden" value="<%= item.Id %>" class="itemId" />
					<input type="hidden" value="<%= item.SituacaoId %>" class="itemSituacao" />
					<input type="hidden" value="<%= item.DataCadastroTexto %>" class="itemDataCriacao" />
					<input type="hidden" value="<%= item.Empreendimento.Id %>" class="itemEmpreendimentoId" />
					<input type="hidden" value="<%= item.Empreendimento.Denominador %>" class="itemEmpreendimento" />

					<%if (Model.PodeAssociar) {%><button type="button" title="Associar" class="icone associar btnAssociar"></button><% } %>
					
					<%if (item.IsRequerimentoDigital) {%>
						<button type="button" title="PDF do requerimento digital" class="icone pdf btnPdf"></button>
					<%}else{%>
						<button type="button" title="PDF do requerimento" class="icone pdf btnPdf"></button>
					<%}%>

					<%if (Model.PodeVisualizar) {%><button type="button" title="Visualizar" class="icone visualizar btnVisualizar"></button><% } %>
					<%if (Model.PodeEditar && !item.IsRequerimentoDigital) {%><button type="button"title="Editar" class="icone editar btnEditar"></button><% } %>
					<%if (Model.PodeExcluir && !item.IsRequerimentoDigital) {%><button type="button" title="Excluir" class="icone excluir btnExcluir"></button><% } %>
					
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>