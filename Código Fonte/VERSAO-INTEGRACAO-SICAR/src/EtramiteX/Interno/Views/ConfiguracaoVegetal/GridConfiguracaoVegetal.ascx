<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ConfiguracaoVegetalVM>" %>

<div>
	<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="80%"><%=Model.Label  %></th>
				<th class="semOrdenacao" width="15%">Ações</th>
			</tr>
		</thead>
		<tbody>
			<%foreach (var item in Model.Itens) {%>
				<tr>
					<td>
						<label title="<%=item.Texto %>"> <%=item.Texto %> </label>
					</td>
					<td>
						<a class="icone editar btnEditar"></a>
						<input type="hidden" value="<%=item.Id %>" class="hdnItemId" />
					</td>
				</tr>
			<% } %>
		</tbody>
	</table>
</div>