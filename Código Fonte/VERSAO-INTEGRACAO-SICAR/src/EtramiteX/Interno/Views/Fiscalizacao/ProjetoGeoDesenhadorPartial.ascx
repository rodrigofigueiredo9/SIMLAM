<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMProjetoGeografico" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DesenhadorVM>" %>

<div class="containerDesenhador">

	<div class="block">
		<span class="StatusFinalizado">
			<button class="btnDesenhador"><%: (Model.IsVisualizar || Model.IsFinalizado) ? "Visualizar Desenhador" : "Abrir Desenhador" %></button>
		</span>
	</div>
	
	<div class="box divSituacao <%= Model.ArquivoEnviado.IdRelacionamento == 0 ? "hide" : "" %>">
		<div class="coluna50">Situação do Processamento: <label class="lblSituacaoProcessamento"><%= Model.ArquivoEnviado.SituacaoTexto %></label></div>
		 
		 <% if (!Model.IsVisualizar) { %>
		 <div class="coluna30">
			<span class="StatusFinalizado <%= Model.IsFinalizado ? "hide" : "" %>">
				<button class="btnReprocessarGeo <%= Model.ArquivoEnviado.MostrarReenviar ? "" : "hide" %>">Reprocessar</button>
			</span>
			<span class="StatusFinalizado <%= Model.IsFinalizado ? "hide" : "" %>">
				<button class="btnCancelarProcessamentoGeo <%= Model.ArquivoEnviado.MostrarCancelar ? "" : "hide" %>">Cancelar</button>
			</span>
			<input type="hidden" class="hdnArquivoEnviadoId" value="<%= Model.ArquivoEnviado.IdRelacionamento %>" />
			<input type="hidden" class="hdnArquivoEnviadoSituacaoId" value="<%= Model.ArquivoEnviado.Situacao %>" />
		</div>
		<% } %>
	</div>

	<div class="block dataGrid divDesenhadorArquivo <%= Model.ArquivosProcessados.Count > 0 ? "" : "hide" %>">
		<table class="dataGridTable desenhadorArquivosGrid">
			<thead>
				<tr>
					<th>Arquivos processados para download</th>
					<th  width="12%">  Ação </th>
				</tr>
			</thead>
			<tbody>
				<%foreach (ArquivoProcessamentoVM arquivo in Model.ArquivosProcessados){ %>
					<tr>
						<td class="arquivoNome" title="<%: arquivo.Texto %>"><%: arquivo.Texto %></td>
						<td>
							<input type="hidden" class="hdnArquivoProcessadoId" value="<%= arquivo.Id %>" />
							<input type="hidden" class="hdnArquivoProcessadoIsPdf" value="<%= arquivo.IsPDF %>" />
							<button class="icone btnBaixar <%= arquivo.IsPDF ? "pdf" : "download"%>"></button>
						</td>
					</tr>
				<% } %>
			</tbody>
		</table>
		<table class="hide">
			<tbody>
				<tr class="trTemplateArqProcessado">
					<td class="arquivoNome"></td>
					<td>
						<input type="hidden" class="hdnArquivoProcessadoId" value="" />
						<button class="icone download btnBaixar"></button>
					</td>
				</tr>
			</tbody>
		</table>
	</div>

</div>