<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<HistoricoVM>" %>

<div class="divHistoricoTramitacao">
	<h1 class="titTela">Histórico de Tramitação</h1>
	<br />

	<% if (Model.TipoHistoricoId > 0) { %>
		<fieldset class="block box">
			<legend><%= Model.TipoHistorico %></legend>
			<div class="coluna20">
				<label for="Numero">Número de registro</label>
				<input type="text" disabled="disabled" class="text disabled" value="<%: Model.Numero %>" />
			</div>

			<div class="coluna40 prepend2">
				<label for="Tipo">Tipo</label>
				<%= Html.DropDownList("Tipo", Model.TipoTexto, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>

			<div class="coluna10">
				<button title="PDF de histórico de tramitação" class="inlineBotao icone listapdf btnHistoricoTramitacaoPdf" onclick="MasterPage.redireciona('<%= Url.Action("GerarPdfHistorico", "Tramitacao", new { id = Model.ProtocoloId, tipo = Model.TipoHistoricoId }) %>	'); return false;"></button>
			</div>
			
		</fieldset>

		<fieldset class="block box">
			<legend>Histórico de Tramitação</legend>
			<table class="dataGridTable tabFuncionarios" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th>Origem</th>
						<th>Remetente</th>
						<th>Ação</th>
						<th width="15%">Data da Ação</th>
						<th>Destino</th>
						<th>Destinatário</th>
						<th>Motivo</th>
						<th width="7%">Ações</th>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.Historico) { %>
					<tr>
						<% if (!string.IsNullOrEmpty(item.StrHistoricoTramitacao)){ %>
						<td colspan="8" title="<%= Html.Encode(item.StrHistoricoTramitacao) %>"><%= Html.Encode(item.StrHistoricoTramitacao)%></td>
						<% } else { %>
						<td title="<%= Html.Encode(item.RemetenteSetor.Nome) %>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.RemetenteSetor.Sigla)) %></td>
						<td title="<%= Html.Encode(item.Remetente) %>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Remetente)) %></td>
						<td title="<%= Html.Encode(item.Acao) %>"><%= Html.Encode(item.Acao) %></td>
						<td title="<%= Html.Encode(item.AcaoData.Data.Value.ToString()) %>"><%= Html.Encode(item.AcaoData.Data.Value.ToString()) %></td>
						<td title="<%= Html.Encode(item.DestinatarioSetor.Nome) %>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.DestinatarioSetor.Sigla)) %></td>
						<td title="<%= Html.Encode(item.Destinatario) %>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Destinatario))%></td>
						<td title="<%= Html.Encode(item.Objetivo) %>"><%= Html.Encode(item.Objetivo) %></td>
						<td>
							<% if(item.MostrarPdf) { %>
								<input title="PDF de despacho" type="button" class="icone pdf btnPdfTramitacao" onclick="MasterPage.redireciona('<%= Url.Action("GerarPdf", "Tramitacao", new { id = item.HistoricoId, tipo = item.IsProcesso ? 1 : 2, obterHistorico = true, isGerarArquivamento = (item.Acao == "Arquivar") }) %>'); return false;"/>
							<% } %></td>
						<% } %>
					</tr>
					<% } %>
				</tbody>
			</table>
		</fieldset>
	<% } %>
</div>