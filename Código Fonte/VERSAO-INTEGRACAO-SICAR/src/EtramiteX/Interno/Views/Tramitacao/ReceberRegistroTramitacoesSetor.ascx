<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ReceberRegistroVM>" %>

<div class="block divDropDown">
	<div class="coluna48">
		<label for="Funcionario.Destinatario">Funcionário *</label>
		<%= Html.DropDownList("FuncionarioDestinatario.Id", Model.SetorFuncionarios, new { @class = "text ddlFuncionariosSetorId" })%>
	</div>
</div>
<div class="block">
	<table class="dataGridTable tabTramitacoesRecebidas" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="20%"><input type="checkbox" class="ckbCheckAllInMyColumn <%= Model.isRegistro ? "hide" : "" %>" />Processo/Documento</th>
				<th width="11%">Enviado em</th>
				<th width="15%">Origem</th>
				<th width="15%">Motivo</th>
				<th width="10%">Ações</th>
			</tr>
		</thead>
		<tbody>
			<% for (int i = 0; i < Model.TramitacoesSetor.Itens.Count; i++)
		{
			Tramitacao tramitacao = Model.TramitacoesSetor.Itens[i];
			 string tramitacaoJson = Model.TramitacoesSetorJson[i];
			%>
			<tr class="trTramitacao">
				<td>
					<input type="checkbox" class="ckbSelecionavel <%= Model.isRegistro ? "hide" : "" %>"  <%= tramitacao.Id == Model.TramitacaoSelecionadaId ? "checked=\"checked\"" : ""%> />
					<label class="trNumero iconeInline <%= (tramitacao.Protocolo.IsProcesso ? "processo" : "documento")%>" title="<%= Html.Encode(tramitacao.Protocolo.Numero) %>"><%= Html.Encode(tramitacao.Protocolo.Numero)%></label>
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
					<input type="hidden" class="hdnTramitacaoId" value="<%= Html.Encode(tramitacao.Id) %>" />
					<input type="hidden" class="hdnTramitacaoJson" value="<%= Html.Encode(tramitacaoJson) %>" />
					<input type="hidden" class="hdnProtocoloId" value="<%= Html.Encode(tramitacao.Protocolo.Id) %>" />
					<input type="hidden" class="hdnIsProcesso" value="<%= Html.Encode(tramitacao.Protocolo.IsProcesso ? 1 : 2) %>" />
					<input title="Histórico" type="button" class="icone historico btnHistorico"/>
					<input title="PDF de despacho" type="button" class="icone pdf btnPdf"/>
				</td>
			</tr>
			<%} %>
		</tbody>
	</table>
</div>