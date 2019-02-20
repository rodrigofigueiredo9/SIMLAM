<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTituloDeclaratorioConfiguracao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="11%">Nº título</th>
				<th width="7%">Login</th>
				<th>Nome de usuário</th>
				<th>Nome do interessado</th>
				<th  width="14%">CPF/CNPJ do interessado</th>
				<th width="7%">Data da situação</th>
				<th width="7%">Situação</th>
				<th width="9%">IP</th>
			</tr>
		</thead>
		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.NumeroTitulo)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.NumeroTitulo))%></td>
				<td title="<%= Html.Encode(item.Login)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Login))%></td>
				<td title="<%= Html.Encode(item.NomeUsuario)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.NomeUsuario))%></td>
				<td title="<%= Html.Encode(item.NomeInteressado)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.NomeInteressado))%></td>
				<td title="<%= Html.Encode(item.CPFCNPJInteressado)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.CPFCNPJInteressado))%></td>
				<td title="<%= Html.Encode(item.DataSituacao)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.DataSituacao))%></td>
				<td title="<%= Html.Encode(item.SituacaoTexto)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.SituacaoTexto))%></td>
				<td title="<%= Html.Encode(item.IP)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.IP))%></td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>