<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAnaliseItens" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ItemAnaliseVME>" %>

<h1 class="titTela">Analisar Item</h1>
<br />

<fieldset class="block box">
	<legend>Item</legend>
	<div class="block">
		<div class="ultima">
			<label for="Nome">Nome *</label>
			<%= Html.TextBox("Nome", Model.Nome, new { @class = "text txtNome disabled", @maxlength = "250", @disabled = "disabled" })%>
		</div>
	</div>
	<div class="block">
		<div class="ultima">
			<label for="ProcedimentoAnalise">Procedimentos de análise</label>
			<%= Html.TextArea("ProcedimentoAnalise", Model.ProcedimentoAnalise, new { @class = "textarea media txtProcedimento disabled", @disabled = "disabled", @maxlength = "500" })%>
		</div>
	</div>
</fieldset>

<fieldset class="block box">
	<legend>Análise</legend>
	<div class="block">
		<div class="coluna15">
			<label for="_DataAnalise">Data</label>
			<%= Html.TextBox("_DataAnalise", Model.DataAnalise, new { @disabled = "disabled", @class = "text disabled datepicker maskData txtDataAnalise" })%>
		</div>
		<div class="coluna25 prepend2">
			<label for="Situacao">Situação do Item *</label>
			<%= Html.DropDownList("Situacao", Model.ListaSituacaoItem, new { @class = "text ddlSituacao" })%>
		</div>
	</div>
	<div class="divMotivo hide block">
		<div class="ultima">
			<label for="Motivo">Motivo *</label>
			<%= Html.TextArea("Motivo", Model.Motivo, new { @class = "textarea media txtMotivo txtDescricao", @maxlength = "4000" })%>
		</div>
	</div>
</fieldset>