<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMEmpreendimento" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="17%">Cód. do empreendimento</th>
				<th width="17%">Segmento</th>
				<th>Razão social/Denominação/Nome</th>
				<th width="15%">CNPJ</th>
			</tr>
		</thead>

		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Codigo))%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Codigo))%></td>
				<td title="<%= Html.Encode(item.SegmentoTexto)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.SegmentoTexto))%></td>
				<td title="<%= Html.Encode(item.Denominador)%>" ><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Denominador))%></td>
				<td title="<%= Html.Encode(item.CNPJ)%>" ><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.CNPJ))%></td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>
