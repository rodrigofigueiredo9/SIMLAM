<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMDocumento" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ConsultarInformacaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Consultar Informações do Documento</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Documento/consultarInformacoes.js") %>"></script>
	<script>
		$(function () {
			ConsultarInformacoes.urlGerarEntregaPdf = '<%= Url.Action("GerarPdfEntrega", "Titulo") %>';
			ConsultarInformacoes.urlGerarRecebimentoPdf = '<%= Url.Action("GerarPdfDocRegistroRecebimento", "Processo") %>';
			ConsultarInformacoes.urlPdfHistoricoTramitacao = '<%= Url.Action("Historico", "Tramitacao") %>';
			ConsultarInformacoes.urlPdfAnalise = '<%= Url.Action("GerarPdfAnaliseDocumeto", "AnaliseItens") %>';
			ConsultarInformacoes.urlPdfDocumentosJuntados = '<%= Url.Action("GerarPdfDocJuntadoProcApensado", "Processo") %>';
			ConsultarInformacoes.urlPdfProcessosApensados = '<%= Url.Action("GerarPdf", "") %>';
			ConsultarInformacoes.urlPdfArquivamento = '<%= Url.Action("GerarPdfArquivamento", "Tramitacao") %>';
			ConsultarInformacoes.urlAbrirMapa = '<%= Url.Action("DownloadDeProtocolo", "AnaliseGeografica", new {area = "GeoProcessamento"}) %>'
			ConsultarInformacoes.load($('#central'));
		});
	</script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Consultar Informações do Documento</h1>
		<br />
		<%= Html.Hidden("Id", Model.Id, new { @class = "hdnDocumentoId" } )%>
		<div class="block box">
			<div class="block">
				<div class="coluna15">
					<label>Número de registro</label>
					<%= Html.TextBox("DocumentoNumero", null, new { @readonly = "true", @class = "text disabled" })%>
				</div>
				<div class="coluna30 prepend2">
					<label>Tipo</label>
					<%= Html.TextBox("DocumentoTipo", null, new { @readonly = "true", @class = "text disabled" })%>
				</div>
			</div>
			<div class="block">
				<div class="coluna48">
					<label>Localização atual</label>
					<%= Html.TextBox("DocumentoLocalizacao", null, new { @readonly = "true", @class = "text disabled" })%>
				</div>
				<% if (!String.IsNullOrWhiteSpace(Model.LabelEnviadoPor))
				{ %>
				<div class="coluna43 prepend2">
					<label>
						<%= Model.LabelEnviadoPor %></label>
					<%= Html.TextBox("DocumentoEnviadoPor", null, new { @readonly = "true", @class = "text disabled" })%>
				</div>
				<% } %>
			</div>
		</div>
		<div class="block box">
			<table class="dataGridTable tabInformacoes" width="100%" border="0" cellspacing="0"
				cellpadding="0">
				<tbody>
					<% foreach (InformacaoProtocoloVM informacao in Model.Informacoes)
					{%>
					<tr>
						<td class="coluna93">
							<p><label><%= informacao.Texto %></label></p>
						</td>
						<td>
							<input type="hidden" class="hdnId" value="<%= informacao.Valor %>" />
							<%if (informacao.Mostrar)
							{ %>
							<button type="button" class="icone <%= informacao.Tipo.ToString().ToLower()+" "+ informacao.Chave  %>" title="<%= informacao.Tipo.ToString() %>">
							</button>
							<%} %>
						</td>
					</tr>
					<% } %>
				</tbody>
			</table>
		</div>
		<div class="block box">
			<span class="cancelarCaixa"><a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>
