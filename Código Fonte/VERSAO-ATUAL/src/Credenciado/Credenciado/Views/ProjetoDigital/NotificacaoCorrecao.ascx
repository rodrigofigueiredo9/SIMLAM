<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProjetoDigital" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ProjetoDigitalVM>" %>

<h1 class="titTela">Notificação para correção</h1>
<br />

<div class="block">
	<div class="block box ultima">
		<label for="ProjetoDigital_MotivoRecusa">Motivo *</label>
		<%= Html.TextArea("ProjetoDigital.MotivoRecusa", Model.ProjetoDigital.MotivoRecusa, new { @class = "text disabled", @disabled = "disabled" })%>
	</div>
</div>