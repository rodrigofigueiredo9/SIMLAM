<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FiscalizacaoVM>" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/Fiscalizacao/fiscalizacaoDocumentosGerados.js") %>"></script>
<script type="text/javascript">
	$(function () {
		FiscalizacaoDocumentosGerados.load($('.documentosGeradosContainer'), {
			urls: {
				download: '<%= Url.Action("Baixar", "Fiscalizacao") %>',
				pdfAuto: '<%= Url.Action("AutoTermoFiscalizacaoPdf", "Fiscalizacao") %>',
				pdfLaudo: '<%= Url.Action("LaudoFiscalizacaoPdf", "Fiscalizacao") %>',
				pdfAcompanhamento: '<%= Url.Action("LaudoAcompanhamentoFiscalizacaoPdf", "Fiscalizacao") %>'
			},
			situacao: <%=Model.Fiscalizacao.SituacaoId %>
		});
	});
</script>

<div class="documentosGeradosContainer">
	<h1 class="titTela">Documentos gerados</h1>
	<br />

	<input type="hidden" class="hdnFiscalizacaoId" value="<%:Model.Fiscalizacao.Id%>" />
	
	<div class="box">
		<fieldset class="boxBranca">
			<legend>Documentos gerados</legend>

			<div class="block dataGrid">
				<div class="coluna100">
					<% Html.RenderPartial("DocumentosGerados", Model); %>
				</div>
			</div>
		</fieldset>

		<fieldset class="boxBranca">
			<legend>Documentos cancelados</legend>

			<div class="block dataGrid">
				<div class="coluna100">
					<%if(Model.DocumentosCancelados.Count > 0 || Model.Acompanhamentos.Count(x => x.SituacaoId == (int)eAcompanhamentoSituacao.Cancelado) > 0) {%>
						<% Html.RenderPartial("DocumentosCancelados", Model); %>
					<%}else {%>
						<p style="font-weight: bold">Não possui histórico de documentos cancelados.</p>
					<%} %>
				</div>
			</div>
		</fieldset>
	</div>
</div>