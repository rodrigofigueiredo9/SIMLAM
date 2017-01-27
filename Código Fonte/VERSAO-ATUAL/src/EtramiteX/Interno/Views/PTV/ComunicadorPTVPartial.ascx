<%@ Import Namespace="Tecnomapas.Blocos.Entities.Etx.ModuloCore" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPTV" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PTVComunicadorVW>" %>

<script>
	ComunicadorPTV.settings.urls.urlEnviar = '<%= Url.Action("EnviarComunicadorPTV", "PTV") %>';
	ComunicadorPTV.settings.Mensagens = <%= Model.Mensagens %>;
</script>

<h1 class="titTela">Comunicador PTV - <%= Model.Comunicador.PTVNumero.ToString() %> </h1>
<br />

<div class="block">
<% PTVConversa conversa = null;
		for (int i = 0; i < Model.Comunicador.Conversas.Count; i++)  {
			conversa = Model.Comunicador.Conversas[i]; %>
			<% if (conversa.TipoComunicador == (int)eExecutorTipo.Interno) { %>
			<div class="block">
				<fieldset class="boxVerdeEscuro" style="-ms-word-break: break-word; word-break: break-word;">
					<legend class="fieldDireita"><%= conversa.NomeComunicador %> - <%= conversa.DataConversa.DataHoraTexto %> </legend>
					<div class="coluna99">
						<div class="floatRight">
							<label for="Conversa"><%= conversa.Texto %></label>
						</div>
					</div>
					<% if ((conversa.ArquivoId > 0) && (Model.Comunicador.ArquivoInterno.Id == conversa.ArquivoId))
						{ %>
						<div class="coluna99">
							<div class="floatRight">
								<%= Html.ActionLink(ViewModelHelper.StringFit(conversa.ArquivoNome, 45), "Baixar", "Arquivo", new { @id = conversa.ArquivoId }, new { @Style = "display: block", @class = "lnkArquivo", @title = conversa.ArquivoNome })%>
							</div>
						</div>
					<% } %>
				</fieldset>
			</div>
		<% } else { %>
			<div class="block">
				<fieldset class="box" style="-ms-word-break: break-word; word-break: break-word;">
					<legend class="fieldEsquerda"><%= conversa.NomeComunicador %> - <%= conversa.DataConversa.DataHoraTexto %> </legend>
					<div class="coluna99">
						<label for="Conversa">
							<%= conversa.Texto %>
						</label>
					</div>
					<% if ((conversa.ArquivoId > 0) && (Model.Comunicador.ArquivoCredenciado.Id == conversa.ArquivoId)) { %>
						<div class="coluna99">
							<%= Html.ActionLink(ViewModelHelper.StringFit(conversa.ArquivoNome, 45), "BaixarCredenciado", "Arquivo", new { @id = conversa.ArquivoId }, new { @Style = "display: block", @class = "lnkArquivo", @title = conversa.ArquivoNome })%>
						</div>
					<% } %>
				</fieldset>
			</div>
		<% } %>
	<% } %>
</div>

<div class="block box rodape">
	<!-- Arquivo -->
	<div class="block">
		<div class="coluna85 inputFileDiv">
			<label for="ArquivoTexto">Anexar Arquivo</label>
			<%= Html.TextBox("txtArquivoNome", null, new { readOnly = "true", @class = "text txtArquivoNome hide disabled", @disabled = "disabled" })%>
			<span class="spanInputFile">
			<input type="file" id="ArquivoId" class="inputFile" style="display: block; width: 100%" name="file" /></span>
			<input type="hidden" class="hdnArquivo hdnAnexoArquivoJson" name="hdnAnexoArquivoJson" value="" />
		</div>
		<div class="ultima spanBotoes">
			<button type="button" class="inlineBotao btnArq" title="Enviar anexo" onclick="ComunicadorPTV.onEnviarAnexoArquivoClick('<%= Url.Action("arquivo", "arquivo") %>');">Anexar</button>
			<button type="button" class="inlineBotao btnArqLimpar hide" title="Limpar arquivo" onclick="ComunicadorPTV.onLimparArquivo();" ><span>Limpar</span></button>
		</div>
	</div>

	<div class="block box">
		<label for="Mensagem">Justificativa *</label>
		<%= Html.TextArea("txtJustificativa", new { @class = "media txtJustificativa", @maxlength = "300"}) %>
		<%= Html.Hidden("hdnIdPTV",Model.Comunicador.PTVId, new { @class = "hdnIdPTV" })%>
		<%= Html.Hidden("hdnPTVNumero",Model.Comunicador.PTVNumero.ToString(), new { @class = "hdnPTVNumero" })%>
		<%= Html.Hidden("hdnArqInternoId",Model.Comunicador.ArquivoInternoId, new { @class = "hdnArqInternoId" })%>
		<%= Html.Hidden("hdnArqCredenciadoId",Model.Comunicador.ArquivoCredenciadoId, new { @class = "hdnArqCredenciadoId" })%>
		<%= Html.Hidden("hdnId",Model.Comunicador.Id, new { @class = "hdnId" })%>
	</div>
</div>