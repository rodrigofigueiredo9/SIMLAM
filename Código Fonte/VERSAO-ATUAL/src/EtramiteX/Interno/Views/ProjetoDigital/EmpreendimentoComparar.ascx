<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMEmpreendimento" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarVM>" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/comparar.js") %>"></script>

<div class="block">
	<div class="ultima">
		<fieldset class="block box">
			<legend><h5><strong>Base Credenciado</strong></h5></legend>
			<% Html.RenderPartial("EntidadePartial", Model.EmpreendimentoCredenciado); %>
		</fieldset>
	</div>
</div>