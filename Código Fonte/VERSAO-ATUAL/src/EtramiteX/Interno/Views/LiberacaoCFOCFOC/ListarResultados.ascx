<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMLiberacaoCFOCFOC" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th>Responsável Técnico</th>
				<th class="semOrdenacao" width="10%">Ações</th>
			</tr>
		</thead>

		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td class="itemResponsavelNome" title="<%= Html.Encode(item.ResponsavelNome)%>"> <%= Html.Encode(item.ResponsavelNome)%></td>
				<td>
					<input type="hidden" value="<%= item.LiberacaoId%>" class="itemId" />
					<button type="button" title="Visualizar" class="icone visualizar btnVisualizar"></button>
					<button type="button" title="Gerar PDF" class="icone pdf btnGerarPDF"></button>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>