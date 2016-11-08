<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="9%">Nº título</th>
				<th width="9%">Modelo</th>
				<th width="19%">Processo/Documento</th>
				<th width="17%">Empreendimento</th>
				<th width="11%">Situação</th>
				<th width="12%">Vencimento</th>				
				<%--<th class="semOrdenacao" width="9%">Ações</th>--%>
			</tr>
		</thead>

		<tbody>
			<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.Numero.Texto)%>"><%= Html.Encode(item.Numero.Texto)%></td>
				<td title="<%= Html.Encode(item.Modelo.Nome)%>"><%= Html.Encode(item.Modelo.Sigla)%></td>
				<td title="<%= Html.Encode(item.Protocolo.Numero)%>"><%= Html.Encode(item.Protocolo.Numero)%></td>
				<td title="<%= Html.Encode(item.EmpreendimentoTexto)%>"><%= Html.Encode(item.EmpreendimentoTexto)%></td>
				<td title="<%= Html.Encode(item.Situacao.Texto)%>"><%= Html.Encode(item.Situacao.Texto)%></td>
				<td title="<%= Html.Encode(item.DataVencimento.DataTexto)%>"><%= Html.Encode(item.DataVencimento.DataTexto)%></td>				
				<%--					
					<td>
					<input type="hidden" class="itemJson" value="<%= Model.ObterJSon(item) %>" />

					<input type="button" title="PDF do título" class="icone pdf btnPDF" />
					<input type="button" title="Visualizar" class="icone visualizar btnVisualizar" />
				</td>--%>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>