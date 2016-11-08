<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CondicionanteSituacaoProrrogarVM>" %>


<script type="text/javascript">
	CondicionanteProrrogar.settings.urls = {
		prorrogar: '<%= Url.Action("CondicionanteProrrogarSalvar", "Titulo") %>'
	}
</script>

<h2 class="titTela">Prorrogar Condicionante</h2>

<%= Html.Hidden("CondicionanteId", null, new { @class = "hdnCondicionanteId" }) %>
<%= Html.Hidden("PeriodicidadeId", null, new { @class = "hdnPeriodicidadeId" })%>

<div class="block box">
	<div class="coluna20">
		<label for="DataEnvio">Dias prorrogados*</label>
		<%= Html.TextBox("Dias", null, new { @maxlength = "5", @class = "text txtDias" })%>
	</div>
</div>