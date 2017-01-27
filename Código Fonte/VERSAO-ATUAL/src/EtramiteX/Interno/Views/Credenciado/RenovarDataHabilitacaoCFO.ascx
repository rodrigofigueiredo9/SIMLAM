<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCredenciado" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<HabilitarEmissaoCFOCFOCVM>" %>

<script>
	$.extend(RenovarDataHabilitacaoCFO.settings, {
		urls: {
			renovarData: '<%= Url.Action("RenovarDataHabilitacaoCFO", "Credenciado") %>'
		}
	});
</script>

<h1 class="titTela">Renovar Data de Habilitação da Praga</h1>
<br />

<div class="box block">
	<div class="block ultima">
		<div class="coluna30">
			<label for="RenovarDataHabilitarEmissao.DataInicial">Data inicial da habilitação *</label>
			<%= Html.TextBox("RenovarDataHabilitarEmissao.DataInicial", null, new { @class = "text maskData txtDataInicial" })%>
		</div>
		<div class="coluna30 prepend2">
			<label for="RenovarDataHabilitarEmissao.DataFinal">Data final da habilitação *</label>
			<%= Html.TextBox("RenovarDataHabilitarEmissao.DataFinal", null, new { @class = "text maskData txtDataFinal" })%>
		</div>
	</div>
</div>