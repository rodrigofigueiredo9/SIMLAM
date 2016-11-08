<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DesarquivarVM>" %>


<% if (Model.Tramitacoes.Count <= 0) { %>
<tr class="trTramitacao">
	<td colspan="5">
		<%: Model.MsgNaoEncontrouRegistros %>
	</td>
</tr>
<% } %>

<% foreach (Tramitacao item in Model.Tramitacoes) { %>
<tr class="trTramitacao">
	<td>
		<input type="checkbox" class="ckbIsSelecionado" value="false" />
		<span class="trNumero iconeInline <%= (item.Protocolo.IsProcesso ? "processo" : "documento")%>" title="<%= Html.Encode(item.Protocolo.Numero) %>"> <%= Html.Encode(item.Protocolo.Numero)%></span>
	</td>
	<td>
		<span class="trDataEnvio" title="<%= Html.Encode(item.DataEnvio.DataHoraTexto) %>"> <%= Html.Encode(item.DataEnvio.DataHoraTexto)%></span>
	</td>
	<td>
		<span class="trRemetenteSetor" title="<%= Html.Encode(item.RemetenteSetor.Nome) %>"> <%= Html.Encode(item.RemetenteSetor.Sigla)%></span>
	</td>
	<td>
		<span class="trSituacao" title="<%= Html.Encode(item.Objetivo.Texto) %>"> <%= Html.Encode(item.Objetivo.Texto)%></span>
	</td>
	<td>
		<input type="hidden" class="hdnTramitacaoId" value="<%= Html.Encode(item.Id) %>" />
		<input type="hidden" class="hdnProtocoloId" value= "<%= Html.Encode(item.Protocolo.Id) %>" />
		<input type="hidden" class="hdnProtocoloNumero" value= "<%= Html.Encode(item.Protocolo.Numero) %>" />
		<input type="hidden" class="hdnIsProcesso" value= "<%= Html.Encode(item.Protocolo.IsProcesso) %>" />
		<input title="Visualizar" type="button" class="icone visualizar btnVisualizar" />
		<input title="Histórico" type="button" class="icone historico btnHistorico" />
		<input title="PDF de arquivamento" type="button" class="icone pdf btnPdf" />
	</td>
</tr>
<% } %>