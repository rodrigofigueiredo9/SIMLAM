<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CondicionanteVM>" %>

<%= Html.Hidden("Condicionante.Id", Model.Condicionante.Id, new { @class = "hdnCondicionanteId" }) %>

<h2 class="titTela">Visualizar Condicionante</h2>

<div class="box">
	<div class="block">
		<div class="coluna25">
			<label for="_data_de_recebimento_">Data de criação*</label>
			<%= Html.TextBox("_data_de_recebimento_", Model.Condicionante.DataCriacao.DataTexto, new { @maxlength = "80", @class = "text maskData txtDataCriacao disabled", @disabled = "disabled" })%>
		</div>
		<div class="coluna25 prepend2 ultima">
			<label for="DataEnvio">Situação*</label>
			<%= Html.TextBox("_situacao_", Model.Condicionante.Situacao.Texto, new { @maxlength = "80", @class = "disabled text txtSituacao", @disabled = "disabled" })%>
		</div>
	</div>

	<div class="block">
		<div class="coluna90">
			<label for="Descricao">Descrição *</label>
			<%= Html.TextArea("Condicionante.Descricao", null, new { @maxlength = "4000", @class = "text textareaPequeno txtDescricao disabled", @readonly = "readonly" })%> 
		</div>
	</div>

	<div class="block">
		<div class="coluna30">
			<label for="Descricao">Possui prazo? *</label>
			<div class="block">
				<%= Html.RadioButton("Condicionante.PossuiPrazo", true, Model.Condicionante.PossuiPrazo, new { @class = "radio radPossuiPrazo disabled", @disabled = "disabled" })%><label>Sim</label>
				<%= Html.RadioButton("Condicionante.PossuiPrazo", false, !Model.Condicionante.PossuiPrazo, new { @class = "radio radPossuiPrazo disabled", @disabled = "disabled" })%><label>Não</label>
			</div>
		</div>
		<div class="coluna10 <%= Model.Condicionante.PossuiPrazo ? "" : "hide" %> containerParaPrazo">
			<label for="Condicionante_Prazo">Prazo (dias) *</label>
			<%= Html.TextBox("Condicionante.Prazo", null, new { @maxlength = "10", @class = "text txtPrazo disabled", @disabled = "disabled" })%>
		</div>
		<div class="coluna50 <%= Model.Condicionante.PossuiPrazo ? "" : "hide" %> containerParaPrazo ultima">
			<div class="espacoLinhaTopo">* O prazo possui a mesma data de início do título</div>
		</div>
	</div>

	<div class="block <%= Model.Condicionante.PossuiPrazo ? "" : "hide" %>">
		<div class="coluna30">
			<label for="Descricao">Possui periodicidade? *</label>
			<div class="block">
				<%= Html.RadioButton("Condicionante.PeriodicidadeTipo.Id", true, Model.Condicionante.PossuiPeriodicidade, new { @class = "radio radPossuiPeriodicidade disabled", @disabled = "disabled" })%><label>Sim</label>
				<%= Html.RadioButton("Condicionante.PeriodicidadeTipo.Id", false, !Model.Condicionante.PossuiPeriodicidade, new { @class = "radio radPossuiPeriodicidade disabled", @disabled = "disabled" })%><label>Não</label>
			</div>
		</div>
	
		<div class="coluna20 <%= Model.Condicionante.PossuiPeriodicidade ? "" : "hide" %>">
			<label for="Condicionante_Prazo">Período *</label>
			<%= Html.TextBox("Condicionante.PeriodicidadeValor", null, new { @maxlength = "10", @class = "text txtPeriodicidade disabled", @disabled = "disabled" })%>
		</div>
		<div class="coluna20 <%= Model.Condicionante.PossuiPeriodicidade ? "" : "hide" %> containerParaPeriodicidade">
			<%= Html.DropDownList("Condicionante.PeriodicidadeTipo.Id", Model.Periodicidades, new { @class = "text espacoLinhaTopo ddlPeriodicidadeTipo disabled", @disabled = "disabled" })%>
		</div>
	</div>
</div>