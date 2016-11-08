<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ConfiguracaoVegetalVM>" %>

<fieldset class="block box">
	<legend></legend>
	<div class="block">
		<div class="coluna60 append2">
			<label for="LabelNome"><%=Model.Label %>*</label>
			<%= Html.TextBox("Valor", Model.ConfiguracaoVegetalItem.Texto, new { @class = "text txtValor limpar", @maxlength="100" })%>
			<input type="hidden" class="hdnId limpar" value="<%= Model.ConfiguracaoVegetalItem.Id %>" />
		</div>
	</div>
</fieldset>