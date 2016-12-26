<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPessoa" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarVM>" %>

<script src="<%= Url.Content("~/Scripts/comparar.js") %>"></script>

<div class="block">
	<div class="ultima">
		<fieldset class="block box">
			<legend><h5><strong>Base Credenciado</strong></h5></legend>
			<% Html.RenderPartial("EntidadePartial", Model.PessoaCredenciado); %>
		</fieldset>
	</div>
</div>