<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCredenciado" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<HabilitarEmissaoCFOCFOCVM>" %>

<div class="habilitarEmissaoCFOCFOCPartial">
	<% if (!Model.IsVisualizar) { %>
		<% Html.RenderPartial("HabilitarEmissaoCFOCFOCSalvar", Model); %>
	<% } else { %>
		<% Html.RenderPartial("HabilitarEmissaoCFOCFOCVisualizar", Model); %>
	<% } %>
</div>