<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Laudo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LaudoVistoriaFlorestalVM>" %>

<div class="dataGrid <%= ((Model.ExploracaoFlorestal.Count > 0)?string.Empty:"hide") %> ">
	<table class="dataGridTable ordenavel" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th>Caracterização selecionada</th>
				<th class="semOrdenacao" width="12%">Ações</th>
			</tr>
		</thead>
		<tbody>
			<% for (int i = 0; i < Model.ExploracaoFlorestal.Count; i++){ %>
			<tr>
				<td class="tdDescricao" title="<%= Html.Encode(Model.ExploracaoFlorestal[i].CodigoExploracaoTexto)%>"><%= Html.Encode(Model.ExploracaoFlorestal[i].CodigoExploracaoTexto)%></td>
				<td>
					<%= Html.Hidden("_codigoExploracao_", Model.ExploracaoFlorestal[i].CodigoExploracaoTexto, new { @class = "hdnCodigoExploracao" })%>
					<%= Html.Hidden("_itemid_", Model.ExploracaoFlorestal[i].Id, new { @class = "hdnItemId" })%>
					<input type="button" title="Excluir" class="icone excluir inlineBotao btnExcluirExploracao"/>
				</td>
			</tr>
			<% } %>
		</tbody>
	</table>
</div>