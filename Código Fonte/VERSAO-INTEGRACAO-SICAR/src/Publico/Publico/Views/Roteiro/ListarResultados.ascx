<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMRoteiro" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0)?string.Empty:"hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="10%">Número</th>
				<th width="10%">Versão</th>
				<th>Nome</th>
				<th width="10%">Situação</th>
				<th class="semOrdenacao" width="16%">Ações</th>
			</tr>
		</thead>

		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td class="roteiroNumero" title="<%= Html.Encode(item.Numero)%>"><%= Html.Encode(item.Numero)%></td>
				<td class="roteiroVersao" title="<%= Html.Encode(item.Versao)%>"><%= Html.Encode(item.Versao)%></td>
				<td class="roteiroNome" title="<%= Html.Encode(item.Nome)%>"><%= Html.Encode(item.Nome)%></td>
				<td class="roteiroSituacao" title="<%= Html.Encode(item.SituacaoTexto)%>"><%= Html.Encode(item.SituacaoTexto)%></td>
				<td>
					<input type="hidden" class="itemId" value="<%= item.Id %>" />
					<input type="hidden" class="roteiroTid" value="<%= item.Tid %>" />
					<input type="button" title="PDF do Roteiro" class="icone pdf btnPdf" />
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>