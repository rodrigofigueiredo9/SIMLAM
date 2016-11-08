<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAtividade" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AtividadeVME>" %>

<div class="divEncerrarAtividadeMotivo">
	<h2 class="titTela">Encerrar Atividade Solicitada</h2>
	<br />

	<% if(Model.IsVisualizar) { %>
		<div class="block box">
			<label for="Atividade.Motivo">Motivo *</label>
			<%= Html.TextArea("Atividade.Motivo", Model.Atividade.Motivo, new { @class = "textarea media disabled", @maxlength = "100", @disabled = "disabled" })%>
		</div>
	<% } else { %>
		<div class="block box">
			<label for="Atividade.Motivo">Motivo *</label>
			<%= Html.TextArea("Atividade.Motivo", Model.Atividade.Motivo, new { @class = "textarea media txtMotivo", @maxlength = "100" })%>
		</div>

		<%= Html.Hidden("Atividade.Id", Model.Atividade.Id, new { @class = "hdnAtividadeId" })%>
		<%= Html.Hidden("Atividade.IdRelacionamento", Model.Atividade.IdRelacionamento, new { @class = "hdnAtividadeRelacionamentoId" })%>
		<%= Html.Hidden("Atividade.Protocolo.Id", Model.Atividade.Protocolo.Id, new { @class = "hdnAtividadeIdProtocolo" })%>
		<%= Html.Hidden("Atividade.Protocolo.IsProcesso", Model.Atividade.Protocolo.IsProcesso, new { @class = "hdnAtividadeIsProcesso" })%>
		<%= Html.Hidden("Id", Model.ProcessoId, new { @class = "hdnPaiProtocoloId" })%>
		<%= Html.Hidden("IsProcesso", Model.IsProcesso, new { @class = "hdnPaiProtocoloIsProcesso" })%>
		
	<% } %>
</div>