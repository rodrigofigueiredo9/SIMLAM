<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCARSolicitacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="13%">Número</th>
				<th>Nome/ Razão Social/ Denominação/ Imóvel</th>
				<th width="21%">Município</th>
				<th width="10%">Situação</th>
                <th width="10%">Arquivo SICAR</th>
				<th class="semOrdenacao" width=" <%= (Model.PodeAssociar) ? "9%" : "20%" %>">Ações</th>
			</tr>
		</thead>

		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td class="tdNumero" title="<%= Html.Encode(item.NumeroTexto)%>"><%= Html.Encode(item.NumeroTexto)%></td>
				<td title="<%= Html.Encode(item.EmpreendimentoDenominador)%>"><%= Html.Encode(item.EmpreendimentoDenominador)%></td>
				<td title="<%= Html.Encode(item.MunicipioTexto)%>"><%= Html.Encode(item.MunicipioTexto)%></td>
				<td title="<%= Html.Encode(item.SituacaoTexto)%>"><%= Html.Encode(item.SituacaoTexto)%></td>
                <td title="<%= Html.Encode(item.SituacaoArquivoCarTexto)%>"><%= Html.Encode(item.SituacaoArquivoCarTexto)%></td>
				<td>
					<input type="hidden" class="itemJson" value="<%: ViewModelHelper.Json(new { Id = item.Id, InternoId = item.InternoId, Motivo = item.SituacaoMotivo, SituacaoSolicitacaoId = item.SituacaoID, SituacaoSolicitacaoTexto = item.SituacaoTexto, SituacaoArquivoCarId = item.SituacaoArquivoCarID, SituacaoArquivoCarTexto = item.SituacaoArquivoCarTexto, UrlPdfReciboSICAR = item.UrlPdfReciboSICAR, Origem = item.Origem, ArquivoSICAR=item.ArquivoSICAR }) %>" />
					<% if (Model.PodeVisualizar) { %><input type="button" title="PDF da Solicitação" class="icone pdf btnPDF" /><% } %>
					<% if (Model.PodeVisualizar && !item.NumeroTexto.Contains('/')) { %><input type="button" title="Visualizar" class="icone visualizar btnVisualizar" /><% } %>
					<% if (Model.PodeEditar) { %><input type="button" title="Editar" class="icone editar btnEditar" /><% } %>
					<% if (!string.IsNullOrEmpty(item.SituacaoMotivo) && item.SituacaoID == (int)eCARSolicitacaoSituacao.Suspenso) { %><input type="button" title="Notificação" class="icone pendencias btnNotificacao" /><% } %>
                    <% if (Model.PodeVisualizar) { %><input type="button" title="Enviar para o SICAR" class="icone enviar btnEnviar" /><% } %>
					<% if (Model.PodeVisualizar && (item.SituacaoID == (int)eCARSolicitacaoSituacao.Pendente || item.SituacaoID == (int)eCARSolicitacaoSituacao.Suspenso || item.SituacaoID == (int)eCARSolicitacaoSituacao.Invalido) && item.SituacaoArquivoCarID == (int)eStatusArquivoSICAR.ArquivoReprovado) 
                       { %><input type="button" title="Relatório de pendencias" class="icone pdfGeo btnPdfPendencia" /><% } %>
					<% if (Model.PodeVisualizar && item.SituacaoArquivoCarID == (int)eStatusArquivoSICAR.ArquivoEntregue) 
                       { %> <input type="button" title="Recibo de Inscrição no SICAR" class="icone link btnPdfSicar" /><% } %>
					<% if (Model.PodeVisualizar && !String.IsNullOrWhiteSpace(item.ArquivoSICAR)) { %> <input type="button" title="Baixar arquivo .CAR" class="icone download btnBaixarArquivoSicar" /><% } %>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>