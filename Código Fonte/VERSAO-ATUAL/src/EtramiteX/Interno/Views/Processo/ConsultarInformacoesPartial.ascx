<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProcesso" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ConsultarInformacaoVM>" %>

<script>
	$(function () {
		ConsultarInformacoes.urlGerarEntregaPdf = '<%= Url.Action("GerarPdfEntrega", "Titulo") %>';
		ConsultarInformacoes.urlGerarRecebimentoPdf = '<%= Url.Action("GerarPdfDocRegistroRecebimento", "Processo") %>';
		ConsultarInformacoes.urlPdfHistoricoTramitacao = '<%= Url.Action("Historico", "Tramitacao") %>';
		ConsultarInformacoes.urlPdfAnalise = '<%= Url.Action("GerarPdfAnaliseProcesso", "AnaliseItens") %>';
		ConsultarInformacoes.urlPdfDocumentosJuntados = '<%= Url.Action("GerarPdfDocJuntadoProcApensado", "Processo") %>';
		ConsultarInformacoes.urlPdfProcessosApensados = '<%= Url.Action("GerarPdf", "") %>';
		ConsultarInformacoes.urlPdfArquivamento = '<%= Url.Action("GerarPdfArquivamento", "Tramitacao") %>';
		ConsultarInformacoes.urlAbrirMapa = '<%= Url.Action("DownloadDeProtocolo", "AnaliseGeografica", new {area = "GeoProcessamento"}) %>'
	});
</script>


<h1 class="titTela">Consulta de Informações do Processo</h1>
<br />

<%= Html.Hidden("Id", Model.Id, new { @class = "hdnProcessoId" } )%>

<fieldset class="block box">
	<legend>Processo</legend>
	<div class="block">
		<div class="coluna20">
			<label>Número do Registro</label>
			<%= Html.TextBox("ProcessoNumero", null, new { @class = "text disabled", @disabled = "disabled" })%>
		</div>
		<div class="coluna25 prepend2">
			<label>Tipo</label>
			<%= Html.TextBox("ProcessoTipo", null, new { @class = "text disabled", @disabled = "disabled" })%>
		</div>
	</div>
	<div class="block">
		<div class="coluna48">
			<label>Localização atual</label>
			<%= Html.TextBox("ProcessoLocalizacao", null, new { @class = "text disabled", @disabled = "disabled" })%>
		</div>
		<% if (!String.IsNullOrWhiteSpace(Model.LabelEnviadoPor)) { %>
		<div class="coluna48 prepend2">
			<label><%= Model.LabelEnviadoPor %></label>
			<%= Html.TextBox("ProcessoEnviadoPor", null, new { @class = "text disabled", @disabled = "disabled" })%>
		</div>
		<% } %>
	</div>
</fieldset>

<div class="block box">
<table class="dataGridTable tabInformacoes" width="100%" border="0" cellspacing="0" cellpadding="0">
	<tbody>
		<% foreach (InformacaoProtocoloVM informacao in Model.Informacoes)
		{%>
			<tr>
				<td class="coluna93">
					<p><label><%= informacao.Texto %></label></p>
				</td>
				<td>
					<input type="hidden" class="hdnId" value="<%= informacao.Valor %>" />
					<%if (informacao.Mostrar) { %>
						<button type="button" class="icone <%= informacao.Tipo.ToString().ToLower()+" "+ informacao.Chave  %>" title="<%= informacao.Tipo.ToString() %>"></button>
					<% } %>
				</td>
			</tr>
		<% } %>
	</tbody>
</table>
</div>