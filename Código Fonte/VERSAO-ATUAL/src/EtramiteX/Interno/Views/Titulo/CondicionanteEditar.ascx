<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CondicionanteVM>" %>

<%= Html.Hidden("Condicionante.Id", Model.Condicionante.Id, new { @class = "hdnCondicionanteId" }) %>
<%= Html.Hidden("Condicionante.tituloId", Model.Condicionante.tituloId, new { @class = "hdnTituloId" })%>
<%= Html.Hidden("Condicionante.Situacao.Id", Model.Condicionante.Situacao.Id, new { @class = "hdnSituacao" })%>

<script src="<%= Url.Content("~/Scripts/Titulo/condicionanteDescricaoListar.js") %>" ></script>
<script>
	CondicionanteSalvar.settings.urls = {
		salvar: '<%= Url.Action("CondicionanteEditarValidar", "Titulo") %>',
		buscarDescricao: '<%= Url.Action("CondicionanteDescricaoFiltrar", "Titulo") %>'
	}
</script>

<h2 class="titTela">Editar Condicionante</h2>

<div class="box">
	<div class="block">
		<div class="coluna25">
			<label for="_data_de_recebimento_">Data de criação*</label>
			<%= Html.TextBox("_data_de_recebimento_", Model.Condicionante.DataCriacao.DataTexto, new { @maxlength = "80", @class = "disabled text maskData txtDataCriacao", @disabled = "disabled" })%>
		</div>
		<div class="coluna25 prepend2 ultima">
			<label for="DataEnvio">Situação*</label>
			<%= Html.TextBox("_situacao_", Model.Condicionante.Situacao.Texto, new { @maxlength = "80", @class = "disabled text txtSituacao", @disabled = "disabled" })%>
		</div>
	</div>

	<div class="block">
		<div class="coluna100">
			<label for="Descricao">Descrição *</label>
		</div>
		<div class="coluna90">
			<%= Html.TextArea("Condicionante.Descricao", null, new { @maxlength = "4000", @class = "text textareaPequeno txtDescricao" })%>
		</div>
		<div class="coluna8">
			<button type="button" class="btnBuscarDescricoes">Buscar</button>
		</div>
	</div>

	<div class="block">
		<div class="coluna30">
			<label for="Descricao">Possui prazo? *</label>
			<div class="block">
				<%= Html.RadioButton("Condicionante.PossuiPrazo", true, Model.Condicionante.PossuiPrazo, new { @class = "radio radPossuiPrazo" })%><label>Sim</label>
				<%= Html.RadioButton("Condicionante.PossuiPrazo", false, !Model.Condicionante.PossuiPrazo, new { @class = "radio radPossuiPrazo" })%><label>Não</label>
			</div>
		</div>
		<div class="coluna10 <%= Model.Condicionante.PossuiPrazo ? "" : "hide" %> containerParaPrazo">
			<label for="Condicionante_Prazo">Prazo (dias) *</label>
			<%= Html.TextBox("Condicionante.Prazo", null, new { @maxlength = "5", @class = "text txtPrazo" })%>
		</div>
		<div class="coluna50 <%= Model.Condicionante.PossuiPrazo ? "" : "hide" %> containerParaPrazo ultima">
			<div class="espacoLinhaTopo">* O prazo possui a mesma data de início do título</div>
		</div>
	</div>

	<div class="block containerParaPrazo <%= Model.Condicionante.PossuiPrazo ? "" : "hide" %>">
		<div class="coluna30">
			<label for="Descricao">Possui periodicidade? *</label>
			<div class="block">
				<label><%= Html.RadioButton("Condicionante.PeriodicidadeTipo.Id", true, Model.Condicionante.PossuiPeriodicidade, new { @class = "radio radPossuiPeriodicidade" })%>Sim</label>
				<%= Html.RadioButton("Condicionante.PeriodicidadeTipo.Id", false, !Model.Condicionante.PossuiPeriodicidade, new { @class = "radio radPossuiPeriodicidade" })%><label>Não</label>
			</div>
		</div>

		<div class="coluna20 <%= Model.Condicionante.PossuiPeriodicidade ? "" : "hide" %> containerParaPeriodicidade">
			<label for="Condicionante_Prazo">Período *</label>
			<%= Html.TextBox("Condicionante.PeriodicidadeValor", null, new { @maxlength = "5", @class = "text txtPeriodicidade" })%>
		</div>
		<div class="coluna20 <%= Model.Condicionante.PossuiPeriodicidade ? "" : "hide" %> containerParaPeriodicidade">
			<%= Html.DropDownList("Condicionante.PeriodicidadeTipo.Id", Model.Periodicidades, new { @class = "text espacoLinhaTopo ddlPeriodicidadeTipo" })%>
		</div>
	</div>
</div>