<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<AcompanhamentoAlterarSituacaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Alterar Situação do Acompanhamento</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Fiscalizacao/acompanhamentoAlterarSituacao.js") %>" ></script>
	<script>
		$(function () {
			AcompanhamentoAlterarSituacao.load($('#central'), {
				situacaoCancelado: '<%= Model.SituacaoCancelado %>',
				urls: {
					alterarSituacao: '<%= Url.Action("AcompanhamentoAlterarSituacao", "Fiscalizacao") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h2 class="titTela">Alterar Situação do Acompanhamento</h2><br />

		<%=Html.Hidden("Acompanhamento_Id", Model.Acompanhamento.Id, new { @class = "hdnAcompanhamentoId"}) %>
		<%=Html.Hidden("Acompanhamento_FiscalizacaoId", Model.Acompanhamento.FiscalizacaoId, new { @class = "hdnFiscalizacaoId"}) %>
		<fieldset class="block box">
			<legend>Acompanhamento</legend>

			<div class="block">
				<div class="coluna20 append4">
					<label for="Numero">Nº do Acompanhamento</label>
					<%= Html.TextBox("Numero", Model.Acompanhamento.Numero, new { @class = "text disabled", disabled = "disabled" })%>
				</div>

				<div class="coluna20">
					<label for="DataVistoria">Data da vistoria</label>
					<%= Html.TextBox("DataVistoria", Model.Acompanhamento.DataVistoria.DataTexto, new { @class = "text disabled", disabled = "disabled" })%>
				</div>
			</div>
		</fieldset>

		<fieldset class="block box">
			<legend>Situação</legend>

			<div class="block">
				<div class="coluna20 append4">
					<label for="SituacaoAtual">Situação atual</label>
					<%= Html.TextBox("SituacaoAtual", Model.Acompanhamento.SituacaoTexto, new { @class = "text disabled", disabled = "disabled" })%>
				</div>

				<div class="coluna20">
					<label for="DataSituacaoAtual">Data da situação atual</label>
					<%= Html.TextBox("DataSituacaoAtual", Model.Acompanhamento.DataSituacao.DataTexto, new { @class = "text disabled", disabled = "disabled" })%>
				</div>
			</div>

			<div class="block">
				<div class="coluna20 append4">
					<label for="SituacaoNova">Nova situação</label>
					<%= Html.DropDownList("SituacaoNova", Model.SituacaoNova, new { @class = "text ddlSituacaoNova" })%>
				</div>

				<div class="coluna20">
					<label for="DataSituacaoNova">Data da nova situação</label>
					<%= Html.TextBox("DataSituacaoNova", DateTime.Now.ToShortDateString(), new { @class = "text maskData txtDataSituacaoNova" })%>
				</div>
			</div>

			<div class="block divMotivo hide">
				<div class="ultima">
					<label for="Motivo">Motivo *</label>
					<%= Html.TextArea("Motivo", null, new { @class = "text media txtMotivo", @maxlength = "500" })%>
				</div>
			</div>
		</fieldset>

		<div class="block box">
			<input class="floatLeft btnAlterarSituacao" type="button" value="Salvar" />
			<span class="cancelarCaixa">ou <a class="linkCancelar" href="<%= Url.Action("Acompanhamentos", "Fiscalizacao", new { id = Model.Acompanhamento.FiscalizacaoId })%>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>