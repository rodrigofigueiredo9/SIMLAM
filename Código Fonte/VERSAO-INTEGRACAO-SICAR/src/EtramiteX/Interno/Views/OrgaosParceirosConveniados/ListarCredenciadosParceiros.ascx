<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMOrgaosParceirosConveniados" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarCredenciadoParceiroVM>" %>

<div class="block">
	<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="5%"><input title="Marcar todos" type="checkbox" class="cbMarcarTodos" /></th>
				<th>Nome</th>
				<th width="20%">CPF</th>
				<th width="30%">Unidade</th>
				<th  width="7%">Ações</th>
			</tr>
		</thead>

		<tbody>
		<% foreach (var item in Model.Credenciados) { %>
			<tr>
				<td title="Marcar este item"><input type="checkbox" class="cb" /></td>
				<td title="<%= Html.Encode(item.Nome)%>"><%= Html.Encode(item.Nome)%></td>
				<td title="<%= Html.Encode(item.Pessoa.CPFCNPJ)%>"><%= Html.Encode(item.Pessoa.CPFCNPJ)%></td>
				<td title="<%= Html.Encode(item.OrgaoParceiroUnidadeSiglaNome)%>"><%= Html.Encode(item.OrgaoParceiroUnidadeSiglaNome)%></td>
				<td>
					<input type="hidden" class="hdnItemJson" value="<%=Model.ObterJSon(item) %>" />
					<input type="button" title="Visualizar" class="icone visualizar btnVisualizar" />
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>

