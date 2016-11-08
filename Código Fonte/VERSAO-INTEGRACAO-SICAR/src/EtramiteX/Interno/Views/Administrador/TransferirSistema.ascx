<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAdministrador" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TransferirVM>" %>

<h2 class="titTela">Transferir Usuário Sistema</h2>
<br />

<input type="hidden" class="hdnAdmId" value="<%= Model.Id %>" />
<div class="box">
	<div class="block">
		<label>Tem certeza que deseja <b>transferir</b> as permissões de sistema para o funcionário "<%= Model.Nome %>"?</label>
	</div>

	<div class="block">
		<label for="Motivo">Motivo *</label>
		<%= Html.TextArea("Motivo", string.Empty, new { @class = "textarea media txtMotivo", @maxlength = "100" })%>
	</div>
</div>