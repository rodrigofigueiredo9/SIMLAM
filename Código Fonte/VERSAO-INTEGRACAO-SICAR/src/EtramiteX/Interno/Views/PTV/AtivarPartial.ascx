<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPTV" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PTV>" %>

<h1 class="titTela">Ativar PTV</h1>
<br />

<input type="hidden" class="hdnEmissaoId" value="<%= Model.Id %>" />
<input type="hidden" class="hdnNumero" value="<%= Model.Numero %>" />
<input type="hidden" class="hdnNumeroTipo" value="<%= Model.NumeroTipo %>" />

<% if (Model.NumeroTipo == (int)eDocumentoFitossanitarioTipoNumero.Bloco) { %>
<div class="block box">
	<div class="block"><label>Número PTV: <%= Model.Numero %></label></div>

	<div class="coluna15">
		<label for="DataAtivacao">Data de ativação *</label>
		<%= Html.TextBox("DataAtivacao", DateTime.Today.ToShortDateString(), new { @class="text maskData txtDataAtivacao" })%>
	</div>
</div>
<% } else { %>
<div class="block box">
	<div class="block"><label>Número PTV: Gerado automaticamente</label></div>

	<div class="coluna15">
		<label for="DataAtivacao">Data de ativação *</label>
		<%= Html.TextBox("DataAtivacao", DateTime.Today.ToShortDateString(), new { @class="text maskData txtDataAtivacao disabled", @disabled="disabled" })%>
	</div>
</div>
<% } %>