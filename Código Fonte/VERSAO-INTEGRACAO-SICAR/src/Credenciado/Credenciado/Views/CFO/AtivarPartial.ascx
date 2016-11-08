<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EmissaoCFO>" %>

<h1 class="titTela">Ativar CFO</h1>
<br />

<input type="hidden" class="hdnEmissaoId" value="<%= Model.Id %>" />
<input type="hidden" class="hdnEmissaoNumero" value="<%= Model.Numero %>" />
<input type="hidden" class="hdnEmissaoTipoNumero" value="<%= Model.TipoNumero.GetValueOrDefault() %>" />

<div class="block box">
	<div class="block"><label>Número CFO: <%= Model.Numero %></label></div>

	<div class="coluna15">
		<label for="DataAtivacao">Data de ativação *</label>
		<%=Html.TextBox("DataAtivacao", Model.DataAtivacao.DataTexto, ViewModelHelper.SetaDisabled(Model.TipoNumero == (int)eDocumentoFitossanitarioTipoNumero.Digital, new { @class="text maskData txtDataAtivacao"}))%>
	</div>
</div>