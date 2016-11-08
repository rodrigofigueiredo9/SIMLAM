<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMChecagemPendencia" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarVM>" %>

<%= Html.Hidden("tituloAssociadoId", Model.ChecagemPendencia.TituloId, new { @class = "hdnTituloId" })%>
<%= Html.Hidden("tituloAssociadoTipo", Model.ChecagemPendencia.TituloTipoSigla, new { @class = "hdnTituloTipo" })%>
<%= Html.Hidden("tituloAssociadoVencimento", Model.ChecagemPendencia.TituloVencimento.DataTexto, new { @class = "hdnTituloVencimento" })%>

<div class="block box">
	<div class="block">
		<div class="coluna23">
			<label for="Id">Número *</label>
			<%= Html.TextBox("Id", "Gerado Automaticamente", new { disabled = "disabled", @class = "text disabled" })%>
		</div>
	</div>
</div>

<fieldset class="block box">
	<legend>Titulo de Pendência</legend>
	<div class="block">
		<div class="coluna23">
			<label for="NumeroTitulo">Número *</label>
			<%= Html.TextBox("ChecagemPendencia.TituloNumero", null, new { @class = "text txtTituloNumero disabled", @disabled = "disabled" })%>
		</div>
		<div class="coluna23 prepend1">
			<% if (!Model.IsEditar) { %><button type="button" title="Buscar título" class="inlineBotao btnAssociarTitulo floatLeft">Buscar</button><% } %>
			<span class="spnPdfTitulo <%= (Model.IsEditar) ? "" : "hide" %>"><button class="inlineBotao btnPdfTitulo btnPdf icone pdf" type="button" title="PDF do título"></button></span>
		</div>
	</div>
	<div class="block">
		<div class="coluna23">
			<label for="ChecagemPendencia.ProtocoloNumero">Nº de registro do processo / documento *</label>
			<%= Html.TextBox("ChecagemPendencia.ProtocoloNumero", Model.ChecagemPendencia.ProtocoloNumero, new { @class = "text disabled txtProcessoNumero", @disabled = "disabled" })%>
		</div>
		<div class="coluna60 prepend1">
			<label for="NomeRazaoInteressado">Interessado *</label>
			<%= Html.TextBox("ChecagemPendencia.InteressadoNome", null, new { @class = "text disabled txtInteressadoNome", @disabled = "disabled" })%>
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
						<th width="10%">Ações</th>
					</tr>
				</thead>
				<tbody>
					<% Html.RenderPartial("SalvarItens", Model); %>
				</tbody>
			</table>
		</div>
	</div>
</fieldset>