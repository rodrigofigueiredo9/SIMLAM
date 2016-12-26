<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCARSolicitacao" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<CARSolicitacaoAlterarSituacaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Alterar Situação da Solicitação de Inscrição</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/CARSolicitacao/alterarSituacao.js") %>" ></script>

	<script>
		$(function () {
			CARSolicitacaoAlterarSituacao.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("AlterarSituacao", "CARSolicitacao") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<input type="hidden" class="hdnSolicitacaoId" value="<%:Model.Solicitacao.Id%>" />
	<div id="central">
		<h1 class="titTela">Alterar Situação da Solicitação de Inscrição</h1>
		<br />
		<fieldset class="box">
			<legend>Solicitação de Inscrição do CAR/ES</legend>
			<div class="block">
				<div class="coluna20">
					<label for="Situacao_NumeroControle">Nº de controle da solicitação*</label>
					<%= Html.TextBox("Situacao.NumeroControle", Model.Solicitacao.Numero, new { @class = "disabled text txtSituacaoNumeroControle", @disabled="disabled", @maxlength="7" })%>
				</div>

				<div class="coluna10 prepend1">
					<label for="Situacao_DataEmissao">Data de emissão</label>
					<%= Html.TextBox("Situacao.Emissao.DataTexto", Model.Solicitacao.DataEmissao.DataTexto, new { @class = "disabled text txtDataEmissao maskData", @disabled="disabled" })%>
				</div>

			</div>
		</fieldset>
		<fieldset class="box">
			<legend>Situação</legend>
			<div class="block">
				<div class="coluna20">
					<label for="Situacao_Atual">Situação atual*</label>
					<%= Html.TextBox("Situacao.Atual", Model.Solicitacao.SituacaoTexto, new { @class = "disabled text txtSituacaoAtual", @disabled="disabled" })%>
				</div>

				<div class="coluna15 prepend1">
					<label for="Situacao_DataAtual">Data de situação atual*</label>
					<%= Html.TextBox("Situacao.DataAtual", Model.Solicitacao.DataSituacao.DataTexto, new { @class = "disabled text txtSituacaoDataAnterior maskData", @disabled="disabled" })%>
				</div>
			</div>
			<div class="block">
				<div class="coluna20">
					<label for="Situacao_Nova">Nova situação*</label>
					<%= Html.DropDownList("Situacao.Nova", Model.Situacoes, new {@class = "text ddlSituacaoNova"}) %>
				</div>

				<div class="coluna15 prepend1">
					<label for="Situacao_DataNova">Data da nova situação*</label>
					<%= Html.TextBox("Situacao.DataNova", DateTime.Now.Date.ToShortDateString(), new { @class = "disabled text txtDataSituacaoNova", @disabled="disabled" })%>
				</div>
			</div>

			<div class="ultima divMotivo <%= Model.Solicitacao.SituacaoId == 4? "" : " hide" %> ">
				<label for="AlterarSituacao_Motivo">Motivo*</label>
				<%= Html.TextArea("Situacao.Motivo", Model.Solicitacao.Motivo, new { @class = "media text txtSituacaoMotivo", @maxlength="300"})%>
			</div>

		</fieldset>

		<div class="block box botoesSalvarCancelar">
			<div class="block">
				<button class="btnSalvar floatLeft" type="button" value="Salvar"><span>Salvar</span></button>
				<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>

			</div>
		</div>
	</div>
</asp:Content>