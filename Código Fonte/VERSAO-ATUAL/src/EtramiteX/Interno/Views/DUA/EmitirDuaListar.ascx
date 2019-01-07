<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMDUA" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloDUA" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DUAVM>" %>


<%= Html.Hidden("TituloId", Model.Titulo.Id, new { @class = "hdnTituloId" })%>
<%--<%= Html.Hidden("TituloSituacao", Model.Titulo.Situacao.Id, new { @class = "hdnTituloSituacao" })%>--%>

<div class="block box">
	<table class="dataGridTable gridDua" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="10%">Código</th>
				<th width="9%">Valor R$	</th>
				<th width="19%">Situação</th>
				<th>N° do DUA</th>
				<th width="11%">Validade</th>
				<th class="semOrdenacao" width="9%">Ações</th>
			</tr>
		</thead>

		<tbody>
			<% foreach (var item in Model.DuaLst) { %>
			<tr>
				<td title="<%= Html.Encode(item.Codigo)%>"><%= Html.Encode(item.Codigo)%></td>
				<td title="<%= Html.Encode(item.Valor)%>"><%= Html.Encode(item.Valor)%></td>
				<td title="<%= Html.Encode(item.SituacaoTexto)%>"><%= Html.Encode(item.SituacaoTexto)%></td>
				<td title="<%= Html.Encode(item.Numero)%>"><%= Html.Encode(item.Numero)%></td>
				<td title="<%= Html.Encode(item.Validade.DataTexto)%>"><%= Html.Encode(item.Validade.DataTexto)%></td>
				<td>
					<input type="hidden" class="itemJson" value="<%= Model.ObterJSon(item) %>" />

					<%if (true){%><input type="button" title="PDF do DUA" class="icone pdf btnPDF" /><% } %>
					<%if (item.Situacao == eSituacaoDua.Vencido){%><input type="button" title="Reemitir DUA" class="icone notificacao btnReemitir" /><% } %>
					<%--<%if (true){%><input type="button" title="Reemitir DUA" class="icone notificacao btnReemitir" /><% } %>--%>
				</td>
			</tr>
		<% } %>
		<tr class="trTemplate hide">
				<td class="Codigo">
					<label class="lblCodigo"></label>
				</td>
				<td class="ValorDua">
					<label class="lblValor"></label>
				</td>
				<td class="Situacao">
					<label class="lblSituacao"></label>
				</td>
				<td class="NumeroDua">
					<label class="lblNumero"></label>
				</td>
				<td class="ValidadeDua">
					<label class="lblValidade"></label>
				</td>
				<td>
					<a class="icone pdf btnPDF" title="PDF do DUA"></a>
					<input type="hidden" value="0" class="hdnItemJson" />
				</td>
			</tr>
		</tbody>
	</table>
</div>
