<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloTitulo" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DestinatarioEmailVM>" %>

<%
	string classeDisabledSeVisualizar = (Model.IsVisualizar ? "disabled" : "");
	string tagDisabledSeVisualizar = (Model.IsVisualizar ? "disabled=\"disabled\"" : "");
%>
<fieldset class="block box divDestinatarioEmail <%: (Model.Destinatarios.Count > 0 ? "" : "hide") %>">
	<legend>Destinatário de E-mail</legend>
	<div class="divNaoExisteDestinatario hide"><label>Não existe destinatário com e-mail cadastrado.</label></div>
	<div class="block divDestinatariosContainer">
		<% foreach (DestinatarioEmail destinatario in Model.Destinatarios) { %>
			<label class="append5">
				<input type="checkbox" <%: tagDisabledSeVisualizar %> class="checkDestinatarioEmail <%: classeDisabledSeVisualizar %>" <%: destinatario.Selecionado ? "checked='checked'" : "" %> value="<%: destinatario.PessoaId %>" />
				<span class="destinatarioEmailText"><%: destinatario.PessoaNome %></span>
			</label>
		<% } %>
	</div>
</fieldset>

<div class="destinatarioEmailTemplate hide">
	<label class="append5"><input type="checkbox" class="checkDestinatarioEmail" value="" /><span class="destinatarioEmailText"></span></label>
</div>