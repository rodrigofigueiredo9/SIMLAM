<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DocumentoFitossanitario>" %>

<% if (Model.Tipo == 1){ %>
<fieldset class="block box">
	<legend>N° Bloco</legend>

	<div class="block">
        <input type="hidden" class="ItemID" value="<%:Model.ID%>" />
		<div class="coluna20">
			<label>Tipo do Documento*</label>
            <%--<%=Html.TextBox(, , ViewModelHelper.SetaDisabled(Model.CFO.TipoNumero != (int)eDocumentoFitossanitarioTipoNumero.Digital || Model.IsVisualizar, new { @class="text txtDataEmissao maskData"}))%>--%>
            <%--<%= Html.TextBox("BlocoTipoDocumento", Model.TipoDocumentoTexto, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlBloco ddlTipoDocumento" })%>--%>
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
<% } %>

<% else{ %>
<fieldset class="block box">
	<legend>N° Digital</legend>

	<div class="block">
		<div class="coluna20">
			<label>Tipo do Documento*</label>
			<%= Html.TextBox("DigitalTipoDocumento", Model.TipoDocumentoTexto, new { @class= "text ddlDigital ddlTipoDocumento " }) %>
		</div>
		<div class="coluna20">
			<label>Número inicial*</label>
			<%= Html.TextBox("DigitalNumeroInicial", Model.NumeroInicial, new { @class = "text txtNumeroInicial txtDigital maskNum8"}) %>
		</div>
		<div class="coluna20">
			<label>Número final*</label>
			<%= Html.TextBox("DigitalNumeroFinal", Model.NumeroFinal, new {@class = "text txtNumeroFinal txtDigital maskNum8"}) %>
		</div>
	</div>
</fieldset>
<% } %>