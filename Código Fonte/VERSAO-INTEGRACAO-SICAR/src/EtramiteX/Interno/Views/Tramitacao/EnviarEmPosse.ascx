<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EnviarVM>" %>

<div class="dataGridTramitacoes">
	<input type="hidden" class="hdnIsTabTodos" value="true" />
	<table class="dataGridTable tabTramitacoes" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th><input type="checkbox" class="ckbIsTodosSelecionado" value="false" />Processo/Documento</th>
				<th width="15%">Enviado em</th>
				<th width="10%">Origem</th>
				<th width="17%">Objetivo</th>
				<th width="15%">Recebido em</th>
				<th width="13%">Ações</th>
			</tr>
		</thead>
		<tbody>
			<% foreach (Tramitacao tramitacao in Model.Tramitacoes)
			{ %>
			<tr class="trTramitacao <%= tramitacao.IsSelecionado ? "linhaSelecionada": "" %>">
				<td>
					<input type="checkbox" class="ckbIsSelecionado"  <%= tramitacao.IsSelecionado ? "checked=\"checked\"" : "" %> />
					<span class="trNumero iconeInline <%= (tramitacao.Protocolo.IsProcesso ? "processo" : "documento")%>" title="<%= Html.Encode(tramitacao.Protocolo.Numero) %>"><%= Html.Encode(tramitacao.Protocolo.Numero)%></span>
				</td>
				<td>
					<span class="trDataEnvio" title="<%= Html.Encode(tramitacao.DataEnvio.DataHoraTexto) %>"><%= Html.Encode(tramitacao.DataEnvio.DataHoraTexto)%></span>
				</td>
				<td>
					<span class="trSetorRemetente" title="<%= Html.Encode(tramitacao.RemetenteSetor.Nome) %>"><%= Html.Encode(tramitacao.RemetenteSetor.Sigla)%></span>
				</td>
				<td>
					<span class="trObjetivo" title="<%= Html.Encode(tramitacao.Objetivo.Texto) %>"><%= Html.Encode(tramitacao.Objetivo.Texto)%></span>
				</td>
				<td>
					<span class="trDataRecebido" title="<%= Html.Encode(tramitacao.DataRecebido.DataHoraTexto) %>"><%= Html.Encode(tramitacao.DataRecebido.DataHoraTexto)%></span>
				</td>
				<td>
					<input type="hidden" class="hdnTramitacaoId" value="<%= Html.Encode(tramitacao.Id) %>" />
					<input type="hidden" class="hdnTramitacaoHistoricoId" value="<%= Html.Encode(tramitacao.HistoricoId) %>" />
					<input type="hidden" class="hdnProtocoloId" value="<%= Html.Encode(tramitacao.Protocolo.Id) %>" />
					<input type="hidden" class="hdnProtocoloNumero" value="<%= Html.Encode(tramitacao.Protocolo.Numero) %>" />
					<input type="hidden" class="hdnIsProcesso" value="<%= Html.Encode(tramitacao.Protocolo.IsProcesso) %>" />
					<input title="Visualizar" type="button" class="icone visualizar btnVisualizar" />
					<% if(tramitacao.HistoricoId > 0) { %>
						<input title="Histórico" type="button" class="icone historico btnHistorico"/>
						<input type="button" title="PDF de despacho" class="icone pdf btnPdf"/>
					<% } %>
				</td>
			</tr>
		<%  } %>
		</tbody>
	</table>
</div>