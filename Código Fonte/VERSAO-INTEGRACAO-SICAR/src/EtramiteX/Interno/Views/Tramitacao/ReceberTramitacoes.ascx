<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ReceberVM>" %>

<table class="dataGridTable tabTramitacoesRecebidas" width="100%" border="0" cellspacing="0" cellpadding="0">
	<thead>
		<tr>
			<th><input type="checkbox" class="ckbCheckAllInMyColumn <%= Model.isRegistro ? "hide" : "" %>" <%= Model.ValidarCheckTodos(Model.Tramitacoes) ? "checked=\"checked\"": "" %>/>Processo/Documento</th>
			<th width="15%">Enviado em</th>
			<th width="10%">Origem</th>
			<th width="15%">Motivo</th>
			<th width="10%">Ações</th>
		</tr>
	</thead>
	<tbody>
		<% for (int i = 0; i < Model.Tramitacoes.Itens.Count; i++) {
		  Tramitacao tramitacao = Model.Tramitacoes.Itens[i];
			string tramitacaoJson = Model.TramitacoesJson[i];
			bool selecionada = (Model.Tramitacoes.Itens[i].Id == Model.TramitacaoSelecionadaId);
		%>
		<tr class="trTramitacao">
			<td>
				<input type="checkbox" class="ckbSelecionavel <%= Model.isRegistro ? "hide" : "" %>" <%= selecionada ? "checked=\"checked\"" : "" %> />
				<label class="trNumero iconeInline <%= (tramitacao.Protocolo.IsProcesso ? "processo" : "documento")%>" title="<%= Html.Encode(tramitacao.Protocolo.Numero) %>"><%= Html.Encode(tramitacao.Protocolo.Numero) %></label>
			</td>
			<td>
				<span class="trDataRecebido" title="<%= Html.Encode(tramitacao.DataEnvio.DataTexto) %>"><%= Html.Encode(tramitacao.DataEnvio.DataHoraTexto)%></span>
			</td>
			<td>
				<span class="trSetorRemetente" title="<%= Html.Encode(tramitacao.RemetenteSetor.Texto) %>"><%= Html.Encode(tramitacao.RemetenteSetor.Sigla)%></span>
			</td>
			<td>
				<span class="trObjetivo" title="<%= Html.Encode(tramitacao.Objetivo.Texto) %>"><%= Html.Encode(tramitacao.Objetivo.Texto)%></span>
			</td>
			<td>
				<input type="hidden" class="hdnTramitacaoId" value="<%= Html.Encode(tramitacao.Id) %>" />
				<input type="hidden" class="hdnProtocoloId" value="<%= Html.Encode(tramitacao.Protocolo.Id) %>" />
				<input type="hidden" class="hdnIsProcesso" value="<%= Html.Encode(tramitacao.Protocolo.IsProcesso ? 1 : 2) %>" />
				<input type="hidden" class="hdnTramitacaoJson" value="<%= Html.Encode(tramitacaoJson) %>" />
				<input title="Histórico" type="button" class="icone historico btnHistorico"/>
				<input title="PDF" type="button" class="icone pdf btnPdf"/>
			</td>
		</tr>
		<%} %>
	</tbody>
</table>