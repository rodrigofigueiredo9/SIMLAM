<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Relatorios.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PersonalizadoVM>" %>

<h5>Ordenar Colunas do Relatório</h5>
<div class="block">
	<div class="coluna50">
		<ul class="sortable">
			<% foreach (var item in Model.ConfiguracaoRelatorio.CamposSelecionados) { %>
			<li>
				<input type="hidden" class="hdnCampoId" value="<%= item.Campo.Id %>" />
				<span><%: item.Campo.DimensaoNome + " - " + item.Campo.Alias %></span>
			</li>
			<% } %>
		</ul>
	</div>
</div>