<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ArquivarVM>" %>

<% for (int i = 0; i < Model.Itens.Count; i++) {
	Tramitacao item = Model.Itens[i];
	string itemJson = Model.ItensJson[i];
%>
<tr class="trItem trTramitacao">
	<td>
		<input type="checkbox" class="ckbSelecionavel" value="false" />
		<span class="trNumero iconeInline <%= (item.Protocolo.IsProcesso ? "processo" : "documento")%>" title="<%= Html.Encode(item.Protocolo.Numero) %>"> <%= Html.Encode(item.Protocolo.Numero)%></span>
	</td>
	<td>
		<span class="trDataEnvio" title="<%= Html.Encode(item.DataEnvio.DataHoraTexto) %>"> <%= Html.Encode(item.DataEnvio.DataHoraTexto)%></span>
	</td>
	<td>
		<span class="trSetorRemetente" title="<%= Html.Encode(item.RemetenteSetor.Nome) %>"> <%= Html.Encode(item.RemetenteSetor.Sigla)%></span>
	</td>
	<td>
		<span class="trObjetivo" title="<%= Html.Encode(item.Objetivo.Texto) %>"> <%= Html.Encode(item.Objetivo.Texto)%></span>
	</td>
	<td>
		<span class="trDataRecebido" title="<%= Html.Encode(item.DataRecebido.DataHoraTexto) %>"> <%= Html.Encode(item.DataRecebido.DataHoraTexto)%></span>
	</td>
	<td>
		<input type="hidden" class="hdnTramitacaoJson" value= "<%= Html.Encode(itemJson) %>" />
		<input type="hidden" class="hdnProtocoloId" value= "<%= Html.Encode(item.Protocolo.Id) %>" />
		<input type="hidden" class="hdnHistorico" value= "<%= Html.Encode(item.HistoricoId) %>" />
		<input type="hidden" class="hdnIsProcesso"  value= "<%= Html.Encode(item.Protocolo.IsProcesso) %>" />
		<input type="hidden" class="hdnNumero" value= "<%= Html.Encode(item.Protocolo.Numero) %>" />
		<input type="hidden" class="hdnTipo" value="<%= Html.Encode(item.Protocolo.Tipo)%>" />
		<input type="hidden" class="hdnSetorId" value="<%= Html.Encode(item.DestinatarioSetor.Id)%>" />
		<input type="hidden" class="hdnObjetivoId" value="<%= Html.Encode(item.Objetivo.Id)%>" />

		<input title="Visualizar" type="button" class="icone visualizar btnVisualizar" />
		<% if (item.HistoricoId != 0) { %>
			<input title="Histórico" type="button" class="icone historico btnHistorico" />
			<input title="PDF de despacho" type="button" class="icone pdf btnPdf" />
		<% } %>
	</td>
</tr>
<%  } %>