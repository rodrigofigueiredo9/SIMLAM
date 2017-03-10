<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DocumentoFitossanitario>" %>

<fieldset class="block box">
    <% if (Model.Tipo == 1){ %>
	<legend>N° Bloco</legend>
    <% } %>
    <% else{ %>
    <legend>N° Digital</legend>
    <% } %>

	<div class="block">
        <input type="hidden" class="ItemID" value="<%:Model.ID%>" />
		<div class="coluna20">
			<label>Tipo do Documento*</label>
			<%= Html.TextBox("BlocoTipoDocumento", Model.TipoDocumentoTexto, new { @class = "text ddlBloco ddlTipoDocumento", @readonly="readonly" })%>
		</div>
		<div class="coluna20">
			<label>Número inicial*</label>
			<%= Html.TextBox("BlocoNumeroInicial", Model.NumeroInicial, new { @class = "text txtNumeroInicial txtBloco maskNum8"}) %>
		</div>
		<div class="coluna20">
			<label>Número final*</label>
			<%= Html.TextBox("BlocoNumeroFinal", Model.NumeroFinal, new { @class = "text txtNumeroFinal txtBloco maskNum8"} ) %>
		</div>
	</div>
</fieldset>