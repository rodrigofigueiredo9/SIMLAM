<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProjetoDigital" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<ProjetoDigitalImprimirDocumentosVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Imprimir Documentos</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">

	<script type="text/javascript" src="<%= Url.Content("~/Scripts/ProjetoDigital/imprimirDocumentos.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/ProjetoDigital/documentosGerados.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			ImprimirDocumentos.load($('#central'), {
				urls: {
					fechar: '<%= Url.Action("ImprimirDocumentos", "ProjetoDigital") %>'
				}
			});

			ProjetoDigitalDocumentoGerados.urlRoteiro = '<%= Url.Action("RelatorioRoteiro", "Roteiro") %>';
			ProjetoDigitalDocumentoGerados.urlRequerimento = '<%= Url.Action("GerarPdf", "Requerimento") %>';
			ProjetoDigitalDocumentoGerados.urlRelatorioTecnico = '<%= Url.Action("GerarPdfRelatorioTecnico", "ProjetoDigital") %>';
			ProjetoDigitalDocumentoGerados.urlArquivo = '<%= Url.Action("Baixar", "Arquivo") %>';
			ProjetoDigitalDocumentoGerados.projetoDigitalID = '<%= Model.ProjetoDigital.Id %>';
			ProjetoDigitalDocumentoGerados.tiposPDF = <%= Model.DocumentosGeradosVM.TiposPDF %>
			ProjetoDigitalDocumentoGerados.urlFichaInscricaoUnidadeProducao = '<%= Url.Action("GerarFichaInscricaoPdf", "UnidadeProducao") %>';
			ProjetoDigitalDocumentoGerados.urlFichaInscricaoUnidadeConsolidacao = '<%= Url.Action("GerarFichaInscricaoPdf", "UnidadeConsolidacao") %>';
			ProjetoDigitalDocumentoGerados.load($('.fsDocumentosGerados'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Imprimir Documentos</h1>
		<br />

		<input type="hidden" class="hdnProjetoDigitalId" value="<%=Model.ProjetoDigital.Id %>" />

		<fieldset class="block box">
			<legend>Projeto Digital</legend>
			<div class="block">
				<div class="coluna22">
					<label for="DataCriacao">Data de criação</label>
					<%= Html.TextBox("DataCriacao", Model.ProjetoDigital.DataCriacao.DataTexto, ViewModelHelper.SetaDisabled(true, new { @class = "text" }))%>
				</div>
				<div class="coluna22 prepend2">
					<label for="RequerimentoId">Nº do Projeto Digital</label>
					<%= Html.TextBox("RequerimentoId", Model.ProjetoDigital.RequerimentoId, ViewModelHelper.SetaDisabled(true, new { @class = "text" }))%>
				</div>
				<div class="coluna22 prepend2">
					<label for="SituacaoTexto">Situação do Projeto Digital</label>
					<%= Html.TextBox("SituacaoTexto", Model.ProjetoDigital.SituacaoTexto, ViewModelHelper.SetaDisabled(true, new { @class = "text" }))%>
				</div>
			</div>
		</fieldset>

		<fieldset class="block box fsDocumentosGerados">
			<legend>Orientações</legend>
			<div class="dataGrid">
				<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
					<thead>
						<tr>
							<th>Roteiros Orientativos</th>
							<th class="semOrdenacao" width="9%">Ação</th>
						</tr>
					</thead>
					<tbody>
					<% foreach (var item in Model.DocumentosGeradosVM.RoteirosOrientativos) { %>
						<tr>
							<td title="<%: item.Texto %>"><%: item.Texto %></td>
							<td>
								<input type="hidden" class="itemJson" value='<%= ViewModelHelper.Json(item) %>' />
								<input type="button" title="PDF" class="icone pdf btnPdf" />
							</td>
						</tr>
					<% } %>
					</tbody>
				</table>
			</div>
		</fieldset>

		<% 
			bool situacaoPermitida =	Model.ProjetoDigital.Situacao == (int)eProjetoDigitalSituacao.AguardandoImportacao || 
										Model.ProjetoDigital.Situacao == (int)eProjetoDigitalSituacao.EmCorrecao; 
		%>

		<% if (situacaoPermitida){ %>
		<div class="box msgInline"  style="color: red">
			<span style="font-weight: bold; font-size: 16px">Alerta!</span>
			<span>&nbsp;&nbsp; Junte os documentos listados no roteiro orientativo, imprima os "Documentos Gerados" e leve-os ao escritório do IDAF no município onde se localiza o imóvel.</span>
		</div>
		<%} %>

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
					<% foreach (var item in Model.DocumentosGeradosVM.DocumentosGerados) { %>
						<tr>
							<td title="<%: item.Texto %>"><%: item.Texto %></td>
							<td>
								<input type="hidden" class="itemJson" value='<%= ViewModelHelper.Json(item) %>' />
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

		<div class="block box">
			<% if (!situacaoPermitida || String.IsNullOrWhiteSpace(Request.Params["MostrarFechar"])){ %>
				<span class="cancelarCaixa"><a class="linkCancelar" href="<%= Url.Action("Operar", "ProjetoDigital", new { id = Model.ProjetoDigital.Id }) %>">Cancelar</a></span>
			<% } else { %>
				<input class="btnFechar floatLeft" type="button" value="Fechar" />
			<%} %>
		</div>
	</div>
</asp:Content>