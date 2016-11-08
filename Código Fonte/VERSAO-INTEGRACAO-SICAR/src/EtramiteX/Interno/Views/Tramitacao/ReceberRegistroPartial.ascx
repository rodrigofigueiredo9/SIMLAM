<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ReceberRegistroVM>" %>

<fieldset class="block box">
	<legend>Executor da Tramitação</legend>
	<div class="block divDropDown">
		<div class="coluna48">
			<label for="Executor_Nome">Funcionário *</label>
			<%= Html.TextBox("Executor.Nome", null, new { @class = "text disabled txtExecutorNome", @maxlength = "250", @disabled = "disabled" })%>
		</div>
	</div>
</fieldset>

<fieldset class="block box">
	<legend>Destinatário</legend>
	<div class="coluna45">
		<label for="SetorDestinatario_Id">Setor de destino *</label>
		<%= Html.DropDownList("SetorDestinatario.Id", Model.FuncionarioSetores, new { @class = "text ddlSetorDestinatarioId" })%>
		<input type="hidden" name="SetorDestinatario.Id" value="<%= Model.SetorDestinatario.Id %>" />
	</div>
</fieldset>

<div class="mostrarSeSetorSelecionado <%= (Model.NumeroTramitacoes <= 0) ? "hide" : "" %>">
	<fieldset class="block box">
		<legend>Dados do Recebimento</legend>
		<div class="block">
			<div class="coluna25">
				<label for="_data_de_recebimento_">Data *</label>
				<%= Html.TextBox("_data_de_recebimento_", Model.DataRecebimento.DataTexto, new { @maxlength = "80", @class = "disabled text maskData txtDataRecebimento", @disabled = "disabled" })%>
			</div>
		</div>
	</fieldset>

	<fieldset class="block box">
		<legend>Processo/Documento a Receber (enviados para o funcionário)</legend>
		<div class="block divTramitacoesReceber">
			<% Html.RenderPartial("ReceberRegistroTramitacoes"); %>
		</div>
	</fieldset>

	<fieldset class="block box divTramitacoesReceberSetor">
		<legend>Processo/Documento a Receber (enviados para o setor)</legend>
		<% Html.RenderPartial("ReceberRegistroTramitacoesSetor"); %>
	</fieldset>
</div>