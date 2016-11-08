<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMHabilitarEmissaoCFOCFOC" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0)?string.Empty:"hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="50%">Responsável técnico</th>
				<th width="20%">Nº da habilitação</th>
				<th width="20%">Situação</th>
				<th class="semOrdenacao" width="10%">Ações</th>
			</tr>
		</thead>
		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td class="responsavelNomeRazaoSocial" title="<%= Html.Encode(item.NomeRazaoSocial)%>"><%= Html.Encode(item.NomeRazaoSocial)%></td>
				<td class="numeroHabilitacao" title="<%= Html.Encode(item.NumeroHabilitacao)%>"><%= Html.Encode(item.NumeroHabilitacao)%></td>				
				<td class="situacao" title="<%= item.SituacaoTexto%>"><%= item.SituacaoTexto%></td>
				<td>
					<input type="hidden" class="itemId" value="<%= item.Id %>" />
					<input type="hidden" class="itemTid" value="<%= item.Tid %>" />                    
					<%if (Model.PodeVisualizar) { %><input type="button" title="Visualizar" class="icone visualizar btnVisualizar" /><%}%>					
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>