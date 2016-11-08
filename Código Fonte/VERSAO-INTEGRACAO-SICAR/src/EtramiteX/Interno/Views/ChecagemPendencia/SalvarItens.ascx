<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloChecagemPendencia" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMChecagemPendencia" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarVM>" %>

<%
	foreach (ChecagemPendenciaItem item in Model.ChecagemPendencia.Itens) { %>
	<tr class="trCheckItem">
		<td>
			<input type="hidden" class="hdnId" value="<%= item.Id %>" />
			<input type="hidden" class="hdnTid" value="<%= item.Tid %>" />
			<input type="hidden" class="hdnIdRelacionamento" value="<%= item.IdRelacionamento %>" />
			<input type="hidden" class="hdnItemSituacaoId" value="<%= item.SituacaoId %>" />
			<span class="spnItemNome" title="<%: item.Nome %>"><%: item.Nome %></span>
		</td>
		<td>
			<span class="trItemRoteiroSituacaoTexto" title="<%: item.SituacaoTexto %>"><%: item.SituacaoTexto %></span>
		</td>
		<% if(!Model.IsVisualizar) { %>
		<td>
			<input title="Conferir" type="button" class="icone recebido btnConferido <%: item.SituacaoId == 2 ? "desativado" : "" %> btnMarcadorItemRoteiro" />
			<input title="Cancelar conferência" type="button" class="icone cancelar btnNaoConferido <%: item.SituacaoId == 1 ? "desativado" : "" %> btnMarcadorItemRoteiro" />
		</td>
		<% } %>
	</tr>
<% } %>