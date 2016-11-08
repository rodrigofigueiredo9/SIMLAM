<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EnviarVM>" %>

<div class="enviarExternoPartial">
	<h1 class="titTela">Enviar Processo/Documento para Órgão Externo</h1>
	<br />

	<fieldset class="block box">
		<legend>Remetente</legend>
			<div class="block divDropDown">
				<div class="coluna45">
					<input type="hidden" class="hdnRemetenteId" value="<%= Model.Enviar.Remetente.Id %>" />
					<label for="Enviar_Remetente_Nome">Funcionário *</label>
					<%= Html.TextBox("Enviar.Remetente.Nome", Model.Enviar.Remetente.Nome, new { @class = "text disabled txtRementeNome", @maxlength = "80", @disabled = "disabled" })%>
				</div>
				<div class="coluna48 prepend2">
					<label for="Enviar_RemetenteSetor_Id">Setor de origem *</label>
					<%= Html.DropDownList("Enviar.RemetenteSetor.Id", Model.SetoresRemente, new { @class = "text ddlSetoresRemetente" })%>
				</div>
			</div>
	</fieldset>

	<div class="divEnviarExternoContent <%= (Model.Tramitacoes.Count <= 0) ? "hide" : "" %>">
		<fieldset class="block box">
			<legend>Processo/Documento em Posse</legend>
				<div class="block">
					<div class="coluna50">
						<label class="append5"><%= Html.RadioButton("OpcaoBusca", 1, true, new { @class = "radio rdbOpcaoBuscaProcesso" })%>Listar todos</label>
						<label class="append5"><%= Html.RadioButton("OpcaoBusca", 2, false, new { @class = "radio rdbOpcaoBuscaProcesso" })%>Nº Processo</label>
						<label class="append5"><%= Html.RadioButton("OpcaoBusca", 3, false, new { @class = "radio rdbOpcaoBuscaProcesso" })%>N° Documento</label>
					</div>
				</div>
				<div class="block divNumeroProtocolo" style="display: none;">
					<div class="coluna15">
						<label for="NumeroProtocolo">Número *</label>
						<%= Html.TextBox("NumeroProtocolo", Model.NumeroProtocolo, new { @maxlength = "12", @class = "text txtNumeroProcDoc" })%>
					</div>
					<div class="coluna27 prepend2 divTipoProcesso" style="display: none;">
						<label for="Enviar_TipoProcessoId">Tipo *</label>
						<%= Html.DropDownList("Enviar.TipoProcessoId", Model.TiposProcesso, new { @class = "text ddlProcessoTipo" })%>
					</div>
					<div class="coluna27 prepend2 divTipoDocumento" style="display: none;">
						<label for="Enviar_TipoDocumentoId">Tipo *</label>
						<%= Html.DropDownList("Enviar.TipoDocumentoId", Model.TiposDocumento, new { @class = "text ddlDocumentoTipo" })%>
					</div>
					<div class="coluna20 prepend1">
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
					<div class="coluna27 prepend2">
						<label for="Enviar_Objetivo_Id">Objetivo *</label>
						<%= Html.DropDownList("Enviar.Objetivo.Id", Model.Objetivos, new { @class = "text ddlObjetivos" })%>
					</div>
				</div>
				<div class="block coluna100">
					<label for="Enviar_Despacho">Despacho</label>
					<%= Html.TextArea("Enviar.Despacho", Model.Enviar.Despacho, new { @class = "textarea media txtDespacho", @maxlength = "500" })%>
				</div>
		</fieldset>

		<fieldset class="block box">
			<legend>Destinatário</legend>
				<div class="block divDropDown">
					<div class="coluna28">
						<label for="Enviar_OrgaoExterno_Id">Órgão externo *</label>
						<%= Html.DropDownList("Enviar.OrgaoExterno.Id", Model.OrgaosExterno, new { @class = "text ddlOrgaoExterno" })%>
					</div>
					<div class="coluna65 prepend2">
						<label for="Enviar_Destinatario_Nome">Funcionário</label>
						<%= Html.TextBox("Enviar.Destinatario.Nome", Model.Enviar.Destinatario.Nome, new { @maxlength = "80", @class = "text txtFuncExterno" })%>
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
				<span class="trNumero iconeInline processo btnProcesso"></span>
				<span class="trNumero iconeInline documento btnDocumento"></span>
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
				<input title="PDF de despacho" type="button" class="icone pdf btnPdf"/>
			</td>
		</tr>
	</tbody>
</table>