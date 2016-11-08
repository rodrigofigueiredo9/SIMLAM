<%@ Import Namespace="Tecnomapas.Blocos.Entities.Etx.ModuloCore" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AlterarSituacaoVM>" %>

<h2 class="titTela">Alterar Situação de Título Declaratório</h2>
<br />

<div class="divAlterarSitucao">
	<input type="hidden" class="hdnTituloId" value="<%= Model.Id %>" />

	<fieldset class="block box">
		<legend>Título Declaratório</legend>
		<div class="block">
			<div class="coluna20">
				<label for="Numero">Número</label>
				<%= Html.TextBox("Numero", (String.IsNullOrEmpty(Model.Numero)) ? "Gerado ao se tornar Válido" : Model.Numero, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna60 prepend1">
				<label for="Modelo">Modelo</label>
				<%= Html.TextBox("Modelo", Model.Modelo, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>
		</div>
	</fieldset>

	<fieldset class="block box">
		<legend>Situação</legend>
		<div class="block">
			<div class="coluna20">
				<label for="SituacaoAtual">Situação atual</label>
				<%= Html.TextBox("SituacaoAtual", Model.Situacao, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>
			
			<div class="coluna20 prepend1">
				<label for="DataSituacaoAtual">Data situação atual</label>
				<%= Html.TextBox("DataSituacaoAtual", Model.DataSituacaoAtual, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>
		</div>
		<div class="block">
			<div class="coluna20">
				<label for="NovaSituacao">Nova situação *</label>
				<%= Html.DropDownList("NovaSituacao", Model.Situacoes, new { @class = "text ddlNovaSituacao" })%>
			</div>
			
			<div class="coluna20 prepend1">
				<label for="DataSituacaoAtual">Data da nova situação *</label>
				<%= Html.TextBox("DataSituacaoAtual", DateTime.Today.ToShortDateString(), new { @class = "text disabled", @disabled = "disabled" })%>
			</div>
		</div>

		<div class="block divEncerramento <%= Model.SituacaoId == (int)eTituloSituacao.EncerradoDeclaratorio ? "" : "hide" %>">
			<div class="coluna43">
				<label for="MotivoEncerramento">Motivo de encerramento *</label>
				<%= Html.DropDownList("MotivoEncerramento", Model.MotivosEncerramento, new { @class = "text ddlMotivoEncerramento" })%>
			</div>
		</div>

		<div class="block divSuspenso <%= Model.SituacaoId == (int)eTituloSituacao.Suspenso ? "" : "hide" %>">
			<div class="coluna80">
				<label for="Motivo">Motivo *</label>
				<%= Html.TextArea("Motivo", null, new { @class = "text media txtMotivo", maxlength = "1000" })%>
			</div>
		</div>
	</fieldset>
</div>