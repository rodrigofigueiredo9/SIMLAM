<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EnviarVM>" %>

<div class="enviarRegistroPartial">

	<h1 class="titTela">Enviar Processo/Documento por Registro</h1>

	<br />

	<fieldset class="block box">
		<legend>Executor da Tramitação</legend>
			<div class="block">
				<div class="coluna48">
					<input type="hidden" class="hdnExecutorId" value="<%= Model.Enviar.Executor.Id %>" />
					<label for="Enviar_Executor_Nome">Funcionário *</label>
					<%= Html.TextBox("Enviar.Executor.Nome", Model.Enviar.Executor.Nome, new { @class = "text disabled txtExecutorNome", @maxlength = "250", @disabled = "disabled" })%>
				</div>
			</div>
	</fieldset>

	<fieldset class="block box">
		<legend>Remetente</legend>
			<div class="block divDropDown">
				<div class="coluna48">
					<label for="Enviar_RemetenteSetor_Id">Setor de origem *</label>
					<%= Html.DropDownList("Enviar.RemetenteSetor.Id", Model.SetoresRemente, new { @class = "text ddlSetoresRemetente" })%>
				</div>
				<div class="coluna48 prepend2">
					<label for="Enviar_Remetente_Id">Funcionário *</label>
					<%= Html.DropDownList("Enviar.Remetente.Id", Model.RemetenteFuncionarios, new { @class = "text ddlRemetentes" })%>
				</div>
			</div>
	</fieldset>

	<div class="divEnviarRegistroContent <%= (Model.Tramitacoes.Count <= 0) ? "hide" : "" %>">
		<fieldset class="block box">
			<legend>Processo/Documento em Posse</legend>
				<div class="block">
					<div class="coluna50">
						<label class="append5"><%= Html.RadioButton("OpcaoBusca", 1, true, new { @class = "radio rdbOpcaoBuscaProcesso" })%>Listar todos</label>
						<label class="append5"><%= Html.RadioButton("OpcaoBusca", 2, false, new { @class = "radio rdbOpcaoBuscaProcesso" })%>Nº Processo/Documento</label>
					</div>
				</div>
				<div class="block divNumeroProtocolo" style="display: none;">
					<div class="coluna15">
						<label for="NumeroProtocolo">Nº registro *</label>
						<%= Html.TextBox("NumeroProtocolo", Model.NumeroProtocolo, new { @maxlength = "80", @class = "text txtNumeroProcDoc" })%>
					</div>
					<div class="coluna20">
						<button type="button" class="inlineBotao botaoAdicionarIcone btnAddProcDoc" style="width:35px" title="Adicionar Processo/Documento">Adicionar</button>
					</div>
				</div>
				<div class="block divTramitacoes">
					<% Html.RenderPartial("~/Views/Tramitacao/EnviarEmPosse.ascx"); %>
				</div>
		</fieldset>

		<fieldset class="block box">
			<legend>Dados do Envio</legend>
				<div class="block">
					<div class="coluna15">
						<label for="Enviar_DataEnvio_DataTexto">Data *</label>
						<%= Html.TextBox("Enviar.DataEnvio.DataTexto", Model.Enviar.DataEnvio.DataTexto, new { @maxlength = "80", @class = "text maskData txtDataEnvio disabled", @disabled = "disabled" })%>
					</div>
					<div class="coluna30 prepend2">
						<label for="Enviar_Objetivo">Motivo *</label>
						<%= Html.DropDownList("Enviar.Objetivo.Id", Model.Objetivos, new { @class = "text ddlObjetivos" })%>
					</div>
				</div>
				<div class="block">
					<label for="Enviar_Despacho">Despacho</label>
					<%= Html.TextArea("Enviar.Despacho", Model.Enviar.Despacho, new { @class = "textarea media txtDespacho", @maxlength = "500" })%>
				</div>
		</fieldset>

		<fieldset class="block box">
			<legend>Destinatário</legend>
				<div class="block divDropDown">
					<div class="coluna48">
						<label for="Enviar_DestinatarioSetor_Id">Setor de destino *</label>
						<%= Html.DropDownList("Enviar.DestinatarioSetor.Id", Model.SetoresDestinatario, new { @class = "text ddlSetoresDestinatario" })%>
					</div>
					<div class="coluna45 prepend2">
						<label for="Enviar_Destinatario_Id">Funcionário</label>
						<%= Html.DropDownList("Enviar.Destinatario.Id", Model.DestinatarioFuncionarios, new { @class = "text ddlDestinatarios disabled", @disabled = "disabled" })%>
					</div>
				</div>
		</fieldset>
	</div>
</div>

<table style="display:none">
	<tbody>
		<tr class="trTramitacao trTramitacaoTemplate">
			<td>
				<input type="checkbox" class="ckbIsSelecionado" value="false" />
				<span class="trNumero iconeInline processo btnProcesso" title=""></span>
				<span class="trNumero iconeInline documento btnDocumento" title=""></span>
			</td>
			<td>
				<span class="trDataEnvio" title=""></span>	
			</td>
			<td>
				<span class="trSetorRemetente" title=""></span>
			</td>
			<td>
				<span class="trObjetivo" title=""></span>
			</td>
			<td>
				<span class="trDataRecebido" title=""></span>
			</td>
			<td>
				<input type="hidden" class="hdnTramitacaoId" value="" />
				<input type="hidden" class="hdnTramitacaoHistoricoId" value="" />
				<input type="hidden" class="hdnProtocoloId" value="" />
				<input type="hidden" class="hdnProtocoloNumero" value="" />
				<input type="hidden" class="hdnIsProcesso" value="" />
				<input title="Remover" type="button" class="icone excluir btnCancelarEnvio"/>
				<input title="Histórico" type="button" class="icone historico btnHistorico"/>
				<input title="PDF" type="button" class="icone pdf btnPdf"/>
			</td>
		</tr>
	</tbody>
</table>