<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ReceberVM>" %>

<div class="receberExternoPartial">
	<h1 class="titTela">Retirar Processo/Documento de Órgão Externo</h1>
	<br />

	<fieldset class="block box">
		<legend>Destinatário</legend>
		<div class="block divDropDown">
			<div class="coluna45">
				<label for="Executor_Nome">Funcionário *</label>
				<%= Html.TextBox("Executor.Nome", null, new { @class = "text disabled txtExecutorNome", @disabled = "disabled" })%>
			</div>
			<div class="coluna48 prepend2">
				<label for="SetorDestinatario_Id">Setor de destino *</label>
				<%= Html.DropDownList("SetorDestinatario.Id", Model.FuncionarioSetores, new { @class = "text ddlSetorDestinatarioId" })%>
			</div>
		</div>
	</fieldset>

	<fieldset class="block box mostrarSeTiverSetor <%= Model.SetorDestinatario.Id > 0 ? "" : "hide" %>">
		<legend>Dados da Retirada</legend>
		<div class="block">
			<div class="coluna15">
				<label for="_data_de_recebimento_">Data *</label>
				<%= Html.TextBox("_data_de_recebimento_", Model.DataRecebimento.DataTexto, new { @class = "disabled text maskData txtDataRecebimento", @disabled = "disabled" })%>
			</div>

			<div class="coluna28 prepend2">
				<label for="OrgaoExterno">Órgão Externo *</label>
				<%= Html.DropDownList("OrgaoExterno", Model.OrgaosExterno, new { @class = "text ddlOrgaoExterno" })%>
			</div>
		</div>
	</fieldset>

	<fieldset class="block box mostrarSeTiverSetor <%= Model.SetorDestinatario.Id > 0 ? "" : "hide" %>">
		<legend>Processo/Documento em Órgão Externo</legend>
		<div class="block divTramitacoesReceber">
			<% Html.RenderPartial("ReceberTramitacoesExterno"); %>
		</div>
	</fieldset>
</div>