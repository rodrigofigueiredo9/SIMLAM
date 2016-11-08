<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Tecnomapas.Blocos.Etx.ModuloCore.View.Sessao>" %>

<div class="quadroToggle">
	<span class="titToggle aberto"><%: Model.Titulo %></span>

	<div class="corpoToggle block">
		<% foreach (var obj in Model.Objetos) { %>
		<div class="divItemGrupo">
			<% foreach (var id in obj.Ids) { %>
			<input type="hidden" class="<%= id.Classe %>" value="<%= id.Valor %>" />
			<% } %>

			<% if(!string.IsNullOrEmpty(obj.Status)) { %>
			<div class="statusComparar"><%: obj.Status %></div>
			<% } %>

			<ul>
				<% foreach (var item in obj.Campos) { %>
				<li class="block">
					<div class="coluna100"><strong><%: item.Alias %>: </strong><span class="<%= item.Classe %>"><%: item.Valor %></span>
						<% foreach (var link in item.Links) { %>
						<input type="button" title="<%: link.Nome %>" class="<%: link.Classe %>" />
						<% } %>
					</div>
				</li>
				<% } %>
			</ul>

			<% foreach (var tab in obj.Tabelas) { %>
			<table class="dataGridTable" border="0" cellpadding="0" cellspacing="0" width="100%">				<thead><tr>				<% foreach (var col in tab.Colunas) {%>					<th><%: col %></th>				<% } %>				</tr></thead>				<tbody>				<% foreach (var lin in tab.Linhas) {%>				<tr>					<% foreach (var n in tab.Colunas) {%>					<td><%: lin[n] %></td>					<% } %>				</tr>				<% } %>				</tbody>			</table>			<% } %>
		</div>
		<% } %>
	</div>
</div>