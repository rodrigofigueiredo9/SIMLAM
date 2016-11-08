<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProtocolo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DocumentoVM>" %>

<div class="divModalDocumento divVisualizarDocumento">
	<input type="hidden" class="hdnDocumentoId" value="<%= Model.Documento.Id %>" />

	<div class="block box">
		<% if (Model.Documento.IsArquivado) { %>
		<div class="block">
			<div class="ultima">
				<label class="erro">ARQUIVADO</label>
			</div>
		</div>
		<% } %>

		<div class="block">
			<div class="coluna18">
				<label>Data de registro *</label>
				<%= Html.TextBox("Documento.DataCadastro.DataTexto", Model.Documento.DataCadastro.DataTexto, new { @class = "text maskData disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna22 prepend2">
				<label>Nº de registro *</label>
				<%= Html.TextBox("Documento.Numero", Model.Documento.Numero, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna40 prepend2">
				<label>Tipo *</label>
				<%= Html.DropDownList("Documento.Tipo.Id", Model.DocumentoTipos, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>
		</div>
		
		<div class="block">
			<div class="coluna18">
				<label>Qtd. de documento *</label>
				<%= Html.TextBox("Documento.Volume", Model.Documento.Volume, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>

			<div class="block ultima prepend2">
				<span class="floatLeft inputFileDiv coluna82">
					<label for="ArquivoTexto">Arquivo complementar</label>
					<% if(Model.Documento.Arquivo.Id.GetValueOrDefault() > 0) { %>
						<%= Html.ActionLink(ViewModelHelper.StringFit(Model.Documento.Arquivo.Nome, 43), "Baixar", "Protocolo", new { @id = Model.Documento.Arquivo.Id }, new { @Style = "display: block", @title = Model.Documento.Arquivo.Nome })%>
					<% } else { %>
						<input type="text" value="*** Nenhum arquivo associado ***" class="text txtArquivoNome disabled" disabled="disabled" />
					<% } %>
				</span>
			</div>
		</div>

		<div class="block">
			<div class="coluna86">
				<label>Nome *</label>
				<%= Html.TextBox("Documento.Nome", Model.Documento.Nome, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>
		</div>
	</div>
	
	<% if (Model.Documento.ProtocoloAssociado.Id.GetValueOrDefault() > 0)
	{ %>
	<fieldset class="block box">
		<legend>Processo</legend>

		<div class="coluna23">
			<label>Nº de registro *</label>
			<input type="hidden" class="hdnProtocoloAssociadoId" value="<%= Html.Encode(Model.Documento.ProtocoloAssociado.Id) %>" />
			<%= Html.TextBox("Documento.ProtocoloAssociado.Numero", Model.Documento.ProtocoloAssociado.Numero, new { @class = "text disabled", @disabled = "disabled" })%>
		</div>

		<div class="coluna30 prepend1">
			<a title="Visualizar processo" class="icone esquerda inlineBotao visualizar btnVisualizarProcesso" href="#">Visualizar processo</a>
		</div>
	</fieldset>
	<% } %>

	<% if(Model.RequerimentoVM.Id > 0) { %>
	<fieldset class="block box">
		<legend>Requerimento Padrão</legend>

		<div class="block">
			<div class="coluna23">
				<label>Número *</label>
				<%= Html.TextBox("Requerimento.Requerimento.Id", Model.RequerimentoVM.Numero, new { @class = "text disabled txtNumeroReq", @disabled = "disabled" })%>
			</div>

			<div class="coluna30 prepend1">
				<a title="PDF do requerimento" class="icone esquerda inlineBotao pdf btnPdfRequerimento" href="#">PDF do requerimento</a>
			</div>
		</div>
	</fieldset>

	<div class="divRequerimento">
		<% Html.RenderPartial("RequerimentoVisualizar", Model.RequerimentoVM); %>
	</div>
	<% } %>

	<% if(Model.RequerimentoVM.Id <= 0) { %>
	<fieldset class="block box">
		<legend>Interessado</legend>

		<input type="hidden" class="hdnInteressadoId" value="<%= Html.Encode(Model.RequerimentoVM.Interessado.Id) %>" />
		<div class="coluna60">
			<label>Nome/Razão social *</label>
			<%= Html.TextBox("Documento.Interessado.NomeRazaoSocial", Model.RequerimentoVM.Interessado.NomeRazaoSocial, new { @class = "text disabled", @disabled = "disabled" })%>
		</div>

		<div class="coluna16 prepend2">
			<label>CPF/CNPJ *</label>
			<%= Html.TextBox("Documento.Interessado.CPFCNPJ", Model.RequerimentoVM.Interessado.CPFCNPJ, new { @class = "text disabled", @disabled = "disabled" })%>
		</div>

		<div class="prepend2">
			<a title="Visualizar interessado" class="icone esquerda inlineBotao visualizar btnEditarInteressado" href="#">Visualizar interessado</a>
		</div>
	</fieldset>
	<% } %>
</div>