<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProjetoDigital" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Etx.ModuloArquivo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DocumentosGeradosVM>" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/ProjetoDigital/documentosGerados.js") %>"></script>
<script type="text/javascript">
	$(function () {
		ProjetoDigitalDocumentoGerados.urlRoteiro = '<%= Url.Action("RelatorioRoteiro", "Roteiro") %>';
		ProjetoDigitalDocumentoGerados.urlRequerimento = '<%= Url.Action("GerarPdf", "Requerimento") %>';
		ProjetoDigitalDocumentoGerados.urlRelatorioTecnico = '<%= Url.Action("GerarPdfRelatorioTecnico", "ProjetoDigital") %>';
		ProjetoDigitalDocumentoGerados.urlArquivo = '<%= Url.Action("Baixar", "Arquivo") %>';
		ProjetoDigitalDocumentoGerados.urlFichaInscricaoUnidadeProducao = '<%= Url.Action("GerarFichaInscricaoPdf", "UnidadeProducao") %>';
		ProjetoDigitalDocumentoGerados.urlFichaInscricaoUnidadeConsolidacao = '<%= Url.Action("GerarFichaInscricaoPdf", "UnidadeConsolidacao") %>';
		ProjetoDigitalDocumentoGerados.projetoDigitalID = '<%= Model.ProjetoDigitalID %>';
		ProjetoDigitalDocumentoGerados.tiposPDF = <%= Model.TiposPDF %>
		ProjetoDigitalDocumentoGerados.load($('.fsDocumentosGerados'));
	});
</script>

<% if(Model.MostrarTitulo) { %>
	<h1 class="titTela">Documentos Gerados</h1>
	<br />
<% } %>

<%
	List<DocumentoGerado> documentos = Model.RoteirosOrientativos;
	documentos.AddRange(Model.DocumentosGerados);
%>

<fieldset class="block box fsDocumentosGerados">
	<legend>Documentos Gerados</legend>
	<div class="dataGrid">
		<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th>Documento Gerado</th>
					<th class="semOrdenacao" width="9%">Ação</th>
				</tr>
			</thead>
			<tbody>
			<% foreach (var item in documentos){ %>
				<tr>
					<td title="<%: item.Texto %>"><%: item.Texto %></td>
					<td>
						<input type="hidden" class="itemJson" value="<%= Model.ObterJSon(item) %>" />
						<input type="button" title="PDF" class="icone pdf btnPdf" />
						<%if(item.Tipo == (int)eProjetoDigitalDocGeradosTipo.RelatorioCaracterizacao){ %>
							<%foreach(var anexo in item.Anexos){ %>
								<input type="hidden" class="anexoId" value="<%= anexo.Id %>" />
								<input type="button" title="PDF Croqui" class="icone pdf pdfGeo btnCroqui" />
							<%} %>
						<%} %>
					</td>
				</tr>
			<% } %>
			</tbody>
		</table>
	</div>
</fieldset>