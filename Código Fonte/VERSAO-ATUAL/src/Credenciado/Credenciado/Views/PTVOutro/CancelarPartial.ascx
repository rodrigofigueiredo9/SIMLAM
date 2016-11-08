<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPTVOutro" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PTVOutro>" %>

<h1 class="titTela">Cancelar PTV de Outro Estado</h1>
<br />

<input type="hidden" class="hdnEmissaoId" value="<%= Model.Id %>" />

<div class="block box">
	<div class="block"><label>Número PTV: <%= Model.Numero %></label></div><br />

	<div class="coluna37">
		<label for="DataCancelamento">Data de Cancelamento *</label>
		<%= Html.TextBox("DataCancelamento", DateTime.Today.ToShortDateString(), new { @class="text maskData txtDataCancelamento disabled", @disabled = "disabled" })%>
	</div>
</div>