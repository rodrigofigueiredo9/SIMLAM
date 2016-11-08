<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DesenhadorVM>"%>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels.VMProjetoGeografico" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>

<div class="containerDesenhador">
	<div class="block">
		<span class="StatusFinalizado">
			<button class="btnDesenhador"><%: (Model.IsVisualizar || Model.IsFinalizado) ? "Visualizar Desenhador" : "Abrir Desenhador" %></button>
		</span>
	</div>

	<div class="box divSituacaoProjeto <%= Model.ArquivoEnviado.ProcessamentoId == 0 ? "hide" : "" %>">
		<div class="coluna50">Situação do Processamento:
			<label class="lblSituacaoProcessamento"><%= Model.ArquivoEnviado.SituacaoTexto %></label></div>
		<div class="coluna30">
			<% if (!Model.IsVisualizar) { %>
				<span class="StatusFinalizado <%= Model.IsFinalizado ? "hide" : "" %>">
					<button class="btnReprocessarGeo <%= Model.ArquivoEnviado.MostrarReenviar ? "" : "hide" %>">Reprocessar</button>
				</span>
				<span class="StatusFinalizado <%= Model.IsFinalizado ? "hide" : "" %>">
					<button class="btnCancelarProcessamentoGeo <%= Model.ArquivoEnviado.MostrarCancelar ? "" : "hide" %>">Cancelar</button>
				</span>
			<%} %>
			<input type="hidden" class="hdnArquivoEnviadoId" value="<%= Model.ArquivoEnviado.IdRelacionamento %>" />
			<input type="hidden" class="hdnArquivoEnviadoSituacaoId" value="<%= Model.ArquivoEnviado.Situacao %>" />
			<input type="hidden" class="hdnProcessamentoFilaId" value="<%= Model.ArquivoEnviado.ProcessamentoId %>" />
			<input type="hidden" class="hdnProcessamentoEtapa" value="<%= Model.ArquivoEnviado.Etapa %>" />
			<input type="hidden" class="hdnProcessamentoMecanismo" value="<%= Model.ArquivoEnviado.Mecanismo %>" />
		</div>
	</div>
	<br />

	<div class="block dataGrid divDesenhadorArquivo divArquivosProcessados <%= Model.ArquivosProcessados.Count > 0 ? "" : "hide" %>">
		<table class="dataGridTable gridArquivos desenhadorArquivosGrid">
			<thead>
				<tr>
					<th>Arquivos processados para download</th>
					<th width="12%">Ação </th>
				</tr>
			</thead>
			<tbody>
				<%foreach (ArquivoProcessamentoVM arquivo in Model.ArquivosProcessados) { %>
				<tr>
					<td class="arquivoNome" title="<%: arquivo.Texto %>"><%: arquivo.Texto %></td>
					<td>
						<input type="hidden" class="hdnArquivoProcessadoId" value="<%= arquivo.Id %>" />
						<input type="hidden" class="hdnArquivoProcessadoIsPdf" value="<%= arquivo.IsPDF %>" />
						<input type="button" class="icone btnBaixar <%= arquivo.IsPDF ? "pdf" : "download"%>" />
					</td>
				</tr>
				<% } %>
			</tbody>
		</table>
	</div>
</div>