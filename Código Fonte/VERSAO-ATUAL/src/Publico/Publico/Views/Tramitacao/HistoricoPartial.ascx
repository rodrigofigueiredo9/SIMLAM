<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMTramitacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<HistoricoVM>" %>

<div class="divHistoricoTramitacao">
	<% Html.RenderPartial("Mensagem"); %>
	<h1 class="titTela">Histórico de Tramitação</h1>
	<br />

	<% if (Model.TipoHistoricoId > 0) { %>
		<fieldset class="block box">
			<legend><%= Model.TipoHistorico %></legend>
			
			<div class="block">
				<div class="coluna20">
					<label for="Numero">Número de registro</label>
					<input type="text" disabled="disabled" class="text disabled" value="<%: Model.Numero %>" />
				</div>

				<div class="coluna40 prepend2">
					<label for="Tipo">Tipo</label>
					<%= Html.DropDownList("Tipo", Model.TipoTexto, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
			</div>

			<div class="block">
				<div class="coluna48">
					<label>Localização atual</label>
					<%= Html.TextBox("ProcessoLocalizacao", Model.Localizacao, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
			</div>
		</fieldset>

		<fieldset class="block box">
			<legend>Histórico de Tramitação</legend>
			<table class="dataGridTable tabFuncionarios" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th>Origem</th>
						<th>Ação</th>
						<th width="15%">Data da Ação</th>
						<th>Destino</th>
						<th>Motivo</th>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.Historico) { %>
					<tr>
						<% if (!string.IsNullOrEmpty(item.StrHistoricoTramitacao)){ %>
						<td colspan="5" title="<%= Html.Encode(item.StrHistoricoTramitacao) %>"><%= Html.Encode(item.StrHistoricoTramitacao)%></td>
						<% } else { %>
						<td title="<%= Html.Encode(item.RemetenteSetor.Nome) %>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.RemetenteSetor.Sigla)) %></td>
						<td title="<%= Html.Encode(item.Acao) %>"><%= Html.Encode(item.Acao) %></td>
						<td title="<%= Html.Encode(item.AcaoData.Data.Value.ToString()) %>"><%= Html.Encode(item.AcaoData.Data.Value.ToString()) %></td>
						<td title="<%= Html.Encode(item.DestinatarioSetor.Nome) %>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.DestinatarioSetor.Sigla)) %></td>
						<td title="<%= Html.Encode(item.Objetivo) %>"><%= Html.Encode(item.Objetivo) %></td>						
						<% } %>
					</tr>
					<% } %>
				</tbody>
			</table>
		</fieldset>
	<% } %>
</div>