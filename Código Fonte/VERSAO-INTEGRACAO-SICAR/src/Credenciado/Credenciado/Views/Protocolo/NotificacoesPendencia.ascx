<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProtocolo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarNotificacaoPendenciaVM>" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/Protocolo/listarNotificacaoPendencia.js") %>" ></script>
<script type="text/javascript">
	$(function () {
		NotificacaoPendenciaListar.load($('.containerNotificacoesPendencias'));
	});
</script>

<div class="containerNotificacoesPendencias">
	<h1 class="titTela">Notificações de pendências</h1>
	<br />

	<%= Html.Hidden("UrlPdf", Url.Action("GerarPdf", "Titulo"), new { @class = "urlPdf" })%>

	<div class="dataGrid">
		<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th width="15%">Documento</th>
					<th width="26%">Data da emissão</th>
					<th width="26%">Nº registro</th>
					<th class="semOrdenacao" width="10%">Ação</th>
				</tr>
			</thead>
			<tbody>
			<% foreach (var item in Model.Resultados) { %>
				<tr>
					<td title="<%= Html.Encode(item.Nome+" - "+item.NumeroTexto)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Nome+" - "+item.NumeroTexto))%></td>
					<td title="<%= Html.Encode(item.DataEmissao.DataTexto)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.DataEmissao.DataTexto))%></td>
					<td title="<%= Html.Encode(item.Protocolo.Numero)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Protocolo.Numero))%></td>
					<td>
						<input type="hidden" value="<%= item.Id %>" class="itemId" />
						<input type="hidden" value="<%= item.Numero %>" class="itemNumeroId" />

						<%if (Model.PodeVisualizar){%><input type="button" title="PDF do título" class="icone pdf btnPDF" /><% } %>
					</td>
				</tr>
			<% } %>
			</tbody>
		</table>
	</div>
</div>