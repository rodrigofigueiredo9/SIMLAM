<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CondicionanteVM>" %>


<%= Html.Hidden("Condicionante.tituloId", Model.Condicionante.tituloId, new { @class = "hdnTituloId" })%>


<script type="text/javascript" src="<%= Url.Content("~/Scripts/Titulo/condicionanteDescricaoListar.js") %>" ></script>
<script type="text/javascript">
	CondicionanteSalvar.settings.urls = {
		salvar: '<%= Url.Action("CondicionanteCriarValidar", "Titulo") %>',
		buscarDescricao: '<%= Url.Action("CondicionanteDescricaoFiltrar", "Titulo") %>'
	}
</script>

<h2 class="titTela">Adicionar Condicionante</h2>

<div class="box">
	<div class="block">
		<div class="coluna25">
			<label for="_data_de_recebimento_">Data de criação*</label>
			<%= Html.TextBox("_data_de_recebimento_", Model.Condicionante.DataCriacao.DataTexto, new { @maxlength = "80", @class = "disabled text maskData txtDataCriacao", @disabled = "disabled" })%>
		</div>
		<div class="coluna25 prepend2 ultima">
			<label for="DataEnvio">Situação*</label>
			<%= Html.Hidden("Condicionante.Situacao.Id", Model.Condicionante.Situacao.Id, new { @class = "hdnSituacao" }) %>
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
				<label><%= Html.RadioButton("Condicionante.PossuiPrazo", true, Model.Condicionante.PossuiPrazo, new { @class = "radio radPossuiPrazo" })%>Sim</label>
				<label><%= Html.RadioButton("Condicionante.PossuiPrazo", false, !Model.Condicionante.PossuiPrazo, new { @class = "radio radPossuiPrazo" })%>Não</label>
			</div>
		</div>
		<div class="coluna10 hide containerParaPrazo">
			<label for="Condicionante_Prazo">Prazo (dias) *</label>
			<%= Html.TextBox("Condicionante.Prazo", null, new { @maxlength = "5", @class = "text txtPrazo maskNumInt" })%>
		</div>
		<div class="coluna50 hide containerParaPrazo ultima">
			<div class="espacoLinhaTopo">* O prazo possui a mesma data de início do título</div>
		</div>
	</div>

	<div class="block containerParaPrazo hide">
		<div class="coluna30">
			<label for="Descricao">Possui periodicidade? *</label>
			<div class="block">
				<label><%= Html.RadioButton("Condicionante.PeriodicidadeTipo.Id", true, Model.Condicionante.PeriodicidadeTipo.Id > 0, new { @class = "radio radPossuiPeriodicidade" })%>Sim</label>
				<label><%= Html.RadioButton("Condicionante.PeriodicidadeTipo.Id", false, Model.Condicionante.PeriodicidadeTipo.Id <= 0, new { @class = "radio radPossuiPeriodicidade" })%>Não</label>
			</div>
		</div>
		<div class="coluna20 hide containerParaPeriodicidade">
			<label for="Condicionante_Prazo">Período *</label>
			<%= Html.TextBox("Condicionante.PeriodicidadeValor", null, new { @maxlength = "5", @class = "text txtPeriodicidade maskNumInt" })%>
		</div>
		<div class="coluna20 hide containerParaPeriodicidade">
			<%= Html.DropDownList("Condicionante.PeriodicidadeTipo.Id", Model.Periodicidades, new { @class = "text espacoLinhaTopo ddlPeriodicidadeTipo" })%>
		</div>
	</div>
</div>