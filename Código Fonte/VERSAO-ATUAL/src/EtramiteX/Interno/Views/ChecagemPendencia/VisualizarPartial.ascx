<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMChecagemPendencia" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarVM>" %>

<script src="<%= Url.Content("~/Scripts/ChecagemPendencia/salvar.js") %>"></script>

<script>
	ChecagemPendencia.settings.urls.pdfTitulo = '<%= Url.Action("GerarPdf", "Titulo") %>';
	$(function () { ChecagemPendencia.load($('.divVisualizarPartial')); });
</script>

<div class="divVisualizarPartial">
	<h1 class="titTela">Visualizar Checagem de Pendência</h1>
	<br />

	<% if (Model.ChecagemPendencia.Id > 0)
	{ %>
		<div class="block box">
			<div class="block">
				<div class="coluna23">
					<label for="Id">Número *</label>
					<%= Html.TextBox("Id", Model.ChecagemPendencia.Numero, new { disabled = "disabled", @class = "text disabled" })%>
				</div>
			</div>
		</div>

		<fieldset class="block box">
			<legend>Titulo de Pendência</legend>
			<div class="block">
				<div class="coluna23">
					<label for="NumeroTitulo">Número *</label>
					<%= Html.Hidden("tituloAssociadoId", Model.ChecagemPendencia.TituloId, new { @class = "hdnTituloId" })%>
					<%= Html.TextBox("ChecagemPendencia.TituloNumero", Model.ChecagemPendencia.TituloNumero, new { @disabled = "disabled", @class = "text disabled" })%>
				</div>
				<div class="coluna23 prepend1">
					<button class="inlineBotao btnPdfTitulo btnPdf icone pdf" type="button" title="PDF do título"></button>
				</div>
			</div>
			<div class="block">
				<div class="coluna23">
					<label for="ChecagemPendencia.ProtocoloNumero">Nº de registro do processo / documento  *</label>
					<%= Html.TextBox("ChecagemPendencia.ProtocoloNumero", Model.ChecagemPendencia.ProtocoloNumero, new { @disabled = "disabled", @class = "text disabled" })%>
				</div>
				<div class="coluna60 prepend1">
					<label for="NomeRazaoInteressado">Interessado *</label>
					<%= Html.TextBox("ChecagemPendencia.InteressadoNome", Model.ChecagemPendencia.InteressadoNome, new { @disabled = "disabled", @class = "text disabled" })%>
				</div>
			</div>
		</fieldset>

		<fieldset class="block box">
			<legend>Itens</legend>
			<div class="block">
				<div class="dataGrid">
					<table class="dataGridTable dgItens" width="100%" border="0" cellspacing="0" cellpadding="0">
						<thead>
							<tr>
								<th>Nome</th>
								<th width="15%">Situação</th>
							</tr>
						</thead>
						<tbody>
							<% Html.RenderPartial("SalvarItens", Model); %>
						</tbody>
					</table>
				</div>
			</div>
		</fieldset>
	<% } %>
</div>