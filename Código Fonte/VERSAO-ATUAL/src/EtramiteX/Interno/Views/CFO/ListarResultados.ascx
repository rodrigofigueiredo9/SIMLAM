<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCFOCFOC.CFO" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0)?string.Empty:"hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="15%">Número CFO</th>
				<th>Empreendimento</th>
				<th>Cultura/Cultivar</th>
				<th width="13%">Situação</th>
				<th class="semOrdenacao" width="19%">Ações</th>
			</tr>
		</thead>
		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%: item.Numero %>"><%: item.Numero %></td>
				<td title="<%: item.EmpreendimentoTexto %>"><%: item.EmpreendimentoTexto %></td>
				<td title="<%: item.CulturaCultivar %>"><%: item.CulturaCultivar %></td>
				<td title="<%: item.SituacaoTexto%>"><%: item.SituacaoTexto%></td>
				<td> 
					<input type="hidden" class="itemJson" value="<%: ViewModelHelper.Json(item) %>" />
					<% if (Model.PodeVisualizar) { %><input type="button" title="Visualizar" class="icone visualizar btnVisualizar" /><% } %>
					<% if (Model.PodeGerarPDF) { %><input type="button" title="PDF do CFO" class="icone pdf btnPDF" /><% } %>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>