<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPTV" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<PTVNFCaixaResultado>>" %>



<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
	<thead>
		<tr>
			<th width="20%">Número PTV</th>
			<th width="20%">Empreendimento</th>
			<th>Origem</th>
			<th>Situação</th>
			<th width="20%">Nome interessado</th>
			<th>Qtd Caixas</th>
			<th>Data emissão</th>
		</tr>
	</thead>

	<tbody>
	<% foreach (var item in Model) { %>
		<tr>
			<td title="<%= Html.Encode(item.NumeroPTV)%>"><%= Html.Encode(item.NumeroPTV)%></td>
			<td title="<%= Html.Encode(item.Empreendimento)%>"><%= Html.Encode(item.Empreendimento)%></td>
			<td title="<%= Html.Encode(item.Origem)%>"><%= Html.Encode(item.Origem)%></td>
			<td title="<%= Html.Encode(item.Situacao)%>"><%= Html.Encode(item.Situacao)%></td>
			<td title="<%= Html.Encode(item.Interessado)%>"><%= Html.Encode(item.Interessado)%></td>
			<td title="<%= Html.Encode(item.QtdCaixa)%>"><%= Html.Encode(item.QtdCaixa)%></td>
			<td title="<%= Html.Encode(item.DataEmissao)%>"><%= Html.Encode(item.DataEmissao.ToShortDateString())%></td>
		</tr>
	<% } %>
	</tbody>
</table>
