<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>	
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarVM>" %>

<%
	string classeDisabledSeVisualizar = (Model.IsVisualizar ? "disabled" : "");
	string tagDisabledSeVisualizar = (Model.IsVisualizar ? "disabled=\"disabled\"" : "");
%>

<fieldset class="block box" id="blocoAssinante">
	<legend>Pdf do título</legend>

	<div class="block ultima prepend2">
		<span class="floatLeft inputFileDiv coluna73">
			<label for="ArquivoTexto">Arquivo *</label>
			<% if(Model.Titulo.Id > 0) { %>
				<%= Html.ActionLink(ViewModelHelper.StringFit(Model.ArquivoTexto, 33), "GerarPdf", "Titulo", new { @id = Model.Titulo.Id }, new { @Style = "display: block", @class = "lnkArquivo", @title = Model.ArquivoTexto })%>
			<% } %>			
			<input type="text" value="<%= Model.ArquivoTexto %>" class="text txtArquivoNome disabled hide" disabled="disabled" />
			<%if (!Model.IsVisualizar) {%>
			<span id="spanInputFile" class="spanInputFile <%= string.IsNullOrEmpty(Model.ArquivoTexto) ? "" : "hide" %>"><input type="file" class="inputFileArqComplementar" style="display: block; width: 100%" name="file" /></span>
			<% } %>			
			<input type="hidden" class="hdnArquivoJson" value="<%= Html.Encode(Model.ArquivoJSon) %>" />
		</span>
		<%if (!Model.IsVisualizar) {%>
		<span class="spanBotoes prepend3">
			<button type="button" class="inlineBotao btnArqComplementar <%= string.IsNullOrEmpty(Model.ArquivoTexto) ? "" : "hide" %>" title="Enviar arquivo"><span>Enviar</span></button>
			<button type="button" class="inlineBotao btnArqComplementarLimpar <%= string.IsNullOrEmpty(Model.ArquivoTexto) ? "hide" : "" %>" title="Limpar arquivo" ><span>Limpar</span></button>
		</span>
		<% } %>
	</div>

</fieldset>