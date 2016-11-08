<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPTV" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PTV>" %>

<h1 class="titTela">Cancelar PTV</h1>
<br />

<input type="hidden" class="hdnEmissaoId" value="<%= Model.Id %>" />

<div class="block box">
	<div class="block"><label>Número PTV: <%= Model.Numero %></label></div>

	<div class="block coluna15">
		<label for="DataCancelamento">Data de Cancelamento *</label>
		<%= Html.TextBox("DataCancelamento", string.Empty, new { @class="text maskData txtDataCancelamento" })%>
	</div>

</div>
