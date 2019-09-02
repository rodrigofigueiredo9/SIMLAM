<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AlterarSituacaoVM>" %>


<h2 class="titTela">Alterar Situação de Título</h2>
<br />

<div class="divAlterarSitucao">
	<input type="hidden" class="hdnTituloId" value="<%= Model.Id %>" />
	<input type="hidden" class="hdnModeloId" value="<%= Model.ModeloId %>" />
	<input type="hidden" class="hdnCodigoSicar" value="<%= Model.CodigoSicar %>" />
	<input type="hidden" class="hdnArquivoIntegrado" value="<%= Model.ArquivoIntegrado %>" />

	<fieldset class="block box">
		<legend>Título</legend>
		<div class="block">
			<div class="coluna20 append2">
				<label for="Numero">Número</label>
				<%= Html.TextBox("Numero", (String.IsNullOrEmpty(Model.Numero)) ? "Gerado após emissão" : Model.Numero, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna60">
				<label for="Modelo">Modelo</label>
				<%= Html.TextBox("Modelo", Model.Modelo, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna10">
				<button class="icone pdf inlineBotao btnPdfTitulo" title="PDF">PDF</button>
			</div>
		</div>
	</fieldset>

	<fieldset class="block box">
		<legend>Situação Atual</legend>
		<div class="block">
			<div class="coluna20 append2">
				<label for="SituacaoAtual">Situação</label>
				<%= Html.TextBox("SituacaoAtual", Model.Situacao, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>
			<% if (Model.ModeloCodigo == (int)Tecnomapas.Blocos.Entities.Etx.ModuloCore.eTituloModeloCodigo.AutorizacaoExploracaoFlorestal) { %>
				<div class="coluna20 append2">
					<label for="SituacaoIntegracao">Situação Integração</label>
					<%= Html.TextBox("SituacaoIntegracao", (string.IsNullOrWhiteSpace(Model.CodigoSinaflor) ? "Em Cadastro" : "Integrado"), new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
				<div class="coluna20 append2">
					<label for="CodigoSinaflor">Código Integração</label>
					<%= Html.TextBox("CodigoSinaflor", Model.CodigoSinaflor, new { @class = "text disabled txtCodigoSinaflor", @disabled = "disabled" })%>
				</div>
			<%} %>
		</div>
	</fieldset>

	<fieldset class="block box">
		<legend>Nova Situação</legend>
		<div class="block">
			<div class="coluna90">
				<% for (int i = 0; i < Model.AcoesAlterar.Count; i++) { %>
					<% if (Model.AcoesAlterar[i].Mostrar) { %>
					<label class="append5"><input type="radio" name="OpcaoSituacao" value="<%= Model.AcoesAlterar[i].Id %>" <%= Model.AcoesAlterar[i].Habilitado ? "" : "disabled=\"disabled\"" %> 
					class="radio rdbOpcaoSituacao <%= Model.AcoesAlterar[i].Habilitado ? "" : "disabled" %>" /><%= Model.AcoesAlterar[i].Texto %></label>
					<% } %>
				<% } %>
			</div>
		</div>
		<br />

		<div class="block hide divEmitirParaAssinatura divCamposSituacao">
			<% if(Model.MostrarPrazo) { %>
			<div class="coluna20 append2">
				<label for="Prazo">Prazo em <%= Model.LabelPrazo %> *</label>
				<% if (Model.PrazoAutomatico) { %>
					<%= Html.TextBox("Prazo", "Gerado automaticamente", new { @class = "text txtPrazo disabled", @disabled = "disabled" })%>
				<% } else { %>
					<%= Html.TextBox("Prazo", string.Empty, new { @class = "text txtPrazo maskNumInt", @maxlength = "4" })%>
				<% } %>
			</div>
			<% } %>

			<div class="coluna20 append2">
				<label for="DataEmissao">Data de emissão *</label>
				<%= Html.TextBox("DataEmissao", DateTime.Now.ToString("dd/MM/yyyy"), new { @class = "text txtDataEmissao maskData disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna20">
				<label for="SituacaoNova">Situação *</label>
				<%= Html.TextBox("SituacaoNova", string.Empty, new { @class = "text disabled txtSituacaoNova", @disabled = "disabled" })%>
			</div>
		</div>

		<div class="block hide divCancelarEmissao divCamposSituacao">
			<div class="coluna20 append2">
				<label for="DataCancelamento">Data de cancelamento *</label>
				<%= Html.TextBox("DataCancelamento", DateTime.Now.ToShortDateString(), new { @class = "text disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna20">
				<label for="SituacaoNova">Situação *</label>
				<%= Html.TextBox("SituacaoNova", string.Empty, new { @class = "text disabled txtSituacaoNova", @disabled = "disabled" })%>
			</div>
		</div>

		<div class="block hide divAssinar divCamposSituacao">
			<div class="coluna20 append2">
				<label for="DataAssinatura">Data de assinatura *</label>
				<%= Html.TextBox("DataAssinatura", string.Empty, new { @class = "text txtDataAssinatura maskData" })%>
			</div>

			<div class="coluna20">
				<label for="SituacaoNova">Situação *</label>
				<%= Html.TextBox("SituacaoNova", string.Empty, new { @class = "text disabled txtSituacaoNova", @disabled = "disabled" })%>
			</div>
		</div>

		<div class="block hide divProrrogar divCamposSituacao">
			<div class="coluna20 append2">
				<label for="DataVencimento">Data de vencimento *</label>
				<%= Html.TextBox("DataVencimento", Model.DataVencimento, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna20 append2">
				<label for="DiasProrrogados">Dias prorrogados *</label>
				<%= Html.TextBox("DiasProrrogados", string.Empty, new { @class = "text txtDiasProrrogados maskNumInt", @maxlength = "5" })%>
			</div>

			<div class="coluna20">
				<label for="SituacaoNova">Situação *</label>
				<%= Html.TextBox("SituacaoNova", string.Empty, new { @class = "text disabled txtSituacaoNova", @disabled = "disabled" })%>
			</div>
		</div>

		<div class="block hide divEncerrar divCamposSituacao">
			<div class="block">
				<div class="coluna20 append2">
					<label for="DataEncerramento">Data do encerramento *</label>
					<%= Html.TextBox("DataEncerramento", DateTime.Now.ToString("dd/MM/yyyy"), new { @class = "text disabled txtDataEncerramento maskData", @disabled = "disabled" })%>
				</div>

				<div class="coluna20">
					<label for="SituacaoNova">Situação *</label>
					<%= Html.TextBox("SituacaoNova", string.Empty, new { @class = "text disabled txtSituacaoNova", @disabled = "disabled" })%>
				</div>
			</div>

			<div class="block motivoCancelamento">
				<div class="coluna43">
					<label for="Motivo">Motivo *</label>
					<%= Html.DropDownList("Motivo", Model.MotivosEncerramento, new { @class = "text ddlMotivo" })%>
				</div>
			</div>
		</div>

		<div class="block hide divConcluir divCamposSituacao">
			<div class="block">
				<div class="coluna20 append2">
					<label for="DataConclusao">Data de conclusão *</label>
					<%= Html.TextBox("DataConclusao", Model.DataConclusao, new { @class = "text txtDataConclusao maskData" })%>
				</div>

				<div class="coluna20">
					<label for="SituacaoNova">Situação *</label>
					<%= Html.TextBox("SituacaoNova", string.Empty, new { @class = "text disabled txtSituacaoNova", @disabled = "disabled" })%>
				</div>
			</div>
		</div>
	</fieldset>
</div>