<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProcesso" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarVM>" %>

<div class="divModalProcesso divVisualizarProcesso">
	<input type="hidden" class="hdnProcessoId" value="<%= Model.Processo.Id %>" />

	<div class="block box">
		<% if (Model.Processo.IsArquivado) { %>
		<div class="block">
			<div class="ultima">
				<label class="erro">ARQUIVADO</label>
			</div>
		</div>
		<% } %>

		<div class="block">
			<div class="coluna18">
				<label>Data de registro *</label>
				<%= Html.TextBox("Processo.DataCriacao", Model.Processo.DataCadastro.DataTexto, new { @class = "text txtDataCriacao disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna22 prepend2">
				<label>Nº de registro *</label>
				<%= Html.TextBox("Processo.Numero", Model.Processo.Numero, new { @class = "text txtNumero disabled", @maxlength = 12, @disabled = "disabled" })%>
			</div>

			<div class="coluna40 prepend2">
				<label>Tipo *</label>
				<%= Html.DropDownList("Processo.Tipo.Id", Model.ProcessoTipos, new { @class = "text ddlProcessoTipos disabled", @disabled = "disabled" })%>
			</div>
		</div>

		<div class="block">
			<div class="coluna18">
				<label>Qtd. de volumes *</label>
				<%= Html.TextBox("Processo.Volumes", Model.Processo.Volume, new { @class = "text txtQuantidadeVolumes disabled", @disabled = "disabled", @maxlength = 2 })%>
			</div>

			<div class="block ultima prepend2">
				<span class="floatLeft inputFileDiv coluna82">
					<label for="ArquivoTexto">Arquivo complementar</label>
					<% if(Model.Processo.Arquivo.Id.GetValueOrDefault() > 0) { %>
						<%= Html.ActionLink(ViewModelHelper.StringFit(Model.Processo.Arquivo.Nome, 43), "Baixar", "Processo", new { @id = Model.Processo.Arquivo.Id }, new { @Style = "display: block", @title = Model.Processo.Arquivo.Nome })%>
					<% } else { %>
						<input type="text" value="*** Nenhum arquivo associado ***" class="text txtArquivoNome disabled" disabled="disabled" />
					<% } %>
				</span>
			</div>
		</div>

		<div class="block">
			<div class="coluna18">
				<label>Nº de autuação (SEP) *</label>
				<%= Html.TextBox("Processo.NumeroAutuacao", string.IsNullOrEmpty(Model.Processo.NumeroAutuacao) ? "*** Não autuado ***" : Model.Processo.NumeroAutuacao, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna22 prepend2">
				<label>Data de autuação (SEP) *</label>
				<%= Html.TextBox("Processo.DataAutuacao", Model.Processo.DataAutuacao.DataTexto, new { @class = "text disabled maskData", @disabled = "disabled" })%>
			</div>
		</div>
	</div>

	<% if(Model.Processo.Fiscalizacao.Id > 0) { %>
	<fieldset class="block box">
		<legend>Fiscalização</legend>

		<div class="coluna15">
			<label>Número</label>
			<%= Html.TextBox("Processo.Fiscalizacao.Id", (Model.Processo.Fiscalizacao.Id > 0) ? Model.Processo.Fiscalizacao.Id.ToString() : string.Empty, new { @class = "text disabled txtNumeroFiscalizacao", @disabled = "disabled" })%>
		</div>

		<div class="ultima prepend2">
			<span class="<%= (Model.Processo.Fiscalizacao.Id > 0) ? "" : "hide" %>"><button type="button" title="Visualizar fiscalização" class="icone visualizar inlineBotao btnVisualizarFiscalizacao"></button></span>
		</div>
	</fieldset>
	<% } %>

	<% if(Model.Processo.ChecagemRoteiro.Id > 0) { %>
	<fieldset class="block box">
		<legend>Checagem de Itens de Roteiro</legend>

		<div class="coluna15">
			<label>Número</label>
			<%= Html.TextBox("Processo.ChecagemRoteiro.Id", (Model.Processo.ChecagemRoteiro.Id > 0) ? Model.Processo.ChecagemRoteiro.Id.ToString() : string.Empty, new { @class = "text disabled txtCheckListId", @disabled = "disabled" })%>
		</div>

		<div class="ultima prepend2">
			<span class="<%= (Model.Processo.ChecagemRoteiro.Id > 0) ? "" : "hide" %>"><button type="button" title="Visualizar checagem" class="icone visualizar inlineBotao btnVisualizarChecagem"></button></span>
		</div>
	</fieldset>
	<% } %>

	<% if(Model.RequerimentoVM.Id > 0) { %>
	<fieldset class="block box">
		<legend>Requerimento Padrão</legend>

		<div class="block">
			<div class="coluna15">
				<label>Número</label>
				<%= Html.TextBox("Requerimento.Requerimento.Id", (Model.RequerimentoVM.Numero > 0) ? Model.RequerimentoVM.Numero.ToString() : string.Empty, new { @class = "text disabled txtNumeroReq", @disabled = "disabled" })%>
			</div>

			<div class="coluna30">
				<span class="<%= (Model.RequerimentoVM.Id > 0) ? "" : "hide" %>"><button title="PDF do requerimento" class="icone pdf inlineBotao btnPdfRequerimento">Gerar PDF</button></span>
			</div>
		</div>
	</fieldset>

	<div class="divRequerimento">
		<% Html.RenderPartial("RequerimentoVisualizar", Model.RequerimentoVM); %>
	</div>
	<% } %>

	<% if(Model.RequerimentoVM.Id <= 0) { %>
	<fieldset class="block box">
		<legend class="labelInteressado"><%: Model.Tipo.LabelInteressado %></legend>
		<input type="hidden" class="hdnInteressadoId" value="<%= Html.Encode(Model.RequerimentoVM.Interessado.Id) %>" />
		<div class="coluna60">
			<label>Nome/Razão social *</label>
			<%= Html.TextBox("Processo.Interessado.NomeRazaoSocial", Model.RequerimentoVM.Interessado.NomeRazaoSocial, new { @class = "text disabled txtIntNome", @disabled = "disabled" })%>
		</div>

		<div class="coluna16 prepend2">
			<label>CPF/CNPJ *</label>
			<%= Html.TextBox("Processo.Interessado.CPFCNPJ", Model.RequerimentoVM.Interessado.CPFCNPJ, new { @class = "text disabled txtIntCnpj", @disabled = "disabled" })%>
		</div>

		<div class="prepend2">
			<button type="button" class="icone visualizar esquerda inlineBotao btnEditarInteressado" title="Visualizar interessado"></button>
		</div>
	</fieldset>
	<% } %>
</div>