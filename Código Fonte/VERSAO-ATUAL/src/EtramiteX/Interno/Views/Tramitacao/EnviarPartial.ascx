<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EnviarVM>" %>

<div class="enviarPartial">
    <fieldset class="block box">
        <legend>Remetente</legend>
        <div class="block divDropDown">
            <div class="coluna45">
                <input type="hidden" class="hdnRemetenteId" value="<%= Model.Enviar.Remetente.Id %>" />
                <label for="Enviar_Remetente_Nome">Funcionário *</label>
                <%= Html.TextBox("Enviar.Remetente.Nome", Model.Enviar.Remetente.Nome, new { @class = "text disabled txtRementeNome", @maxlength = "250", @disabled = "disabled" })%>
            </div>
            <div class="coluna48 prepend2">
                <label for="Enviar_RemetenteSetor_Id">Setor de origem *</label>
                <%= Html.DropDownList("Enviar.RemetenteSetor.Id", Model.SetoresRemente, new { @class = "text ddlSetoresRemetente" })%>
            </div>
        </div>
    </fieldset>

    <div class="divEnviarContent <%= (Model.Tramitacoes.Count <= 0) ? "hide" : "" %>">
        <fieldset class="block box">
            <legend>Processo/Documento em Posse</legend>
            <div class="block">
                <div class="coluna50">
                    <label class="append5"><%= Html.RadioButton("OpcaoBusca", 1, true, new { @class = "radio rdbOpcaoBuscaProcesso" })%>Listar todos</label>
                    <label class="append5"><%= Html.RadioButton("OpcaoBusca", 2, false, new { @class = "radio rdbOpcaoBuscaProcesso" })%>Nº Processo/Documento</label>
                </div>
            </div>
            <div class="block divNumeroProtocolo" style="display: none;">
                <div class="coluna16">
                    <label for="NumeroProtocolo">Número de registro *</label>
                    <%= Html.TextBox("NumeroProtocolo", Model.NumeroProtocolo, new { @maxlength = "80", @class = "text txtNumeroProcDoc" })%>
                </div>
                <div class="coluna20 ">
                    <button type="button" class="inlineBotao botaoAdicionarIcone btnAddProcDoc" style="width: 35px" title="Adicionar Processo/Documento">Adicionar</button>
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
                <div class="coluna42 prepend2">
                    <label for="Enviar_ObjetivoId">Motivo *</label>
                    <%= Html.DropDownList("Enviar.Objetivo.Id", Model.Objetivos, new { @class = "text ddlObjetivos" })%>
                </div>
            </div>
            <div class="block">
                <label for="Enviar_Despacho" class="lblDespacho">Despacho</label>
                <%= Html.TextArea("Enviar.Despacho", Model.Enviar.Despacho, new { @class = "textarea txtDespacho" })%>
            </div>
        </fieldset>

        <fieldset class="block box">
            <legend>Destinatário</legend>
            <div class="block divDropDown">
                <div class="coluna48">
                    <label for="Enviar_Destinatario_SetorId">Setor de destino *</label>
                    <%= Html.DropDownList("Enviar.DestinatarioSetor.Id", Model.SetoresDestinatario, new { @class = "text ddlSetoresDestinatario" })%>
                </div>
                <div class="coluna45 prepend2 ddlFuncionario">
                    <label for="Enviar_Destinatario_Id">Funcionário</label>
                    <%= Html.DropDownList("Enviar.Destinatario.Id", Model.DestinatarioFuncionarios, new { @class = "text ddlDestinatarios disabled", @disabled = "disabled" })%>
                </div>
				<div class="coluna25 prepend2 numAutuacao hide">
                    <label for="Enviar_Destinatario_NumeroAutuacao">N° de autuação (SEP) *</label>
                    <%= Html.TextBox("Enviar.Destinatario.NumeroAutuacao", Model.NumeroAutuacao, new { @class = "text txtNumeroAutuacao", @maxlength = "15" })%>
                </div>
				<br />
                <div class="pnlOutros hide">
                    <div class="coluna75">
                        <label for="Enviar_Destinatario_DestinoExterno">Destino Externo *</label>
                        <%= Html.TextBox("Enviar.Destinatario.DestinoExterno", Model.DestinoExterno, new { @class = "text txtDestinoExterno", @maxlength="100" })%>
                    </div>
					<br />
                    <div class="coluna25">
                        <label for="Enviar_Destinatario_FormaEnvio">Forma de Envio *</label>
                        <%= Html.DropDownList("Enviar.Destinatario.FormaEnvio", Model.FormaEnvio, new { @class = "text ddlFormaEnvio" })%>
                    </div>
					<div class="coluna25 rastreio hide">
                        <label for="Enviar_Destinatario_CodigoRastreio">Código de Rastreio *</label>
                        <%= Html.TextBox("Enviar.Destinatario.CodigoRastreio", Model.CodigoRastreio, new { @class = "text txtCodigoRastreio", @maxlength="13" })%>
                    </div>
                </div>
            </div>
        </fieldset>
    </div>
</div>

<table style="display: none">
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
                <input title="Remover" type="button" class="icone excluir btnCancelarEnvio" />
                <input title="Histórico" type="button" class="icone historico btnHistorico" />
                <input title="PDF" type="button" class="icone pdf btnPdf" />
            </td>
        </tr>
    </tbody>
</table>
