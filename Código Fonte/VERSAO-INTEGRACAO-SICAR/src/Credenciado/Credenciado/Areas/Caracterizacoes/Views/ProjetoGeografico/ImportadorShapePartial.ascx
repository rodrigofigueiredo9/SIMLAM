<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ImportadorShapeVM>"%>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels.VMProjetoGeografico" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>

<div class="containerImportadorShape">
	<div class="box block divSituacaoProjeto <%= Model.ArquivoEnviado.Id == 0 ? "hide" : "" %>">
		<div class="coluna50">
			Situação do Processamento:
			<label class="lblSituacaoProcessamento"><%= Model.ArquivoEnviado.SituacaoTexto %></label>
		</div>
		<div class="coluna30">
			<% if (!Model.IsVisualizar) { %>
			<span class="StatusFinalizado <%= Model.SituacaoProjeto == (int)eProjetoGeograficoSituacao.Finalizado ? "hide" : "" %>">
				<button class="btnReenviarArquivo <%= Model.ArquivoEnviado.MostrarReenviar ? "" : "hide" %>">Reenviar</button>
			</span>
			<span class="StatusFinalizado <%= Model.SituacaoProjeto == (int)eProjetoGeograficoSituacao.Finalizado ? "hide" : "" %>">
				<button class="btnCancelarProcessamento <%= Model.ArquivoEnviado.MostrarCancelar ? "" : "hide" %>">Cancelar</button>
			</span>
			<% } %>
			<input type="hidden" class="hdnArquivoEnviadoId" value="<%= Model.ArquivoEnviado.IdRelacionamento %>" />
			<input type="hidden" class="hdnArquivoEnviadoFilaTipo" value="<%= Model.ArquivoEnviado.ArquivoEnviadoFilaTipo %>" />
			<input type="hidden" class="hdnArquivoEnviadoSituacaoId" value="<%= Model.ArquivoEnviado.Situacao %>" />
			<input type="hidden" class="hdnProcessamentoFilaId" value="<%= Model.ArquivoEnviado.ProcessamentoId %>" />
			<input type="hidden" class="hdnProcessamentoEtapa" value="<%= Model.ArquivoEnviado.Etapa %>" />
			<input type="hidden" class="hdnProcessamentoMecanismo" value="<%= Model.ArquivoEnviado.Mecanismo %>" />
		</div>
	</div>

	<% if (!Model.IsVisualizar) { %>
	<div class="box block divImportadorShape <%= Model.ArquivoEnviado.Id == 0 ? "" : "hide" %>">
		<div class="coluna55 buscarEnviar">
			<input type="file" name="file" class="text fileProjetoGeo" />
		</div>
		<div class="coluna10 buscarEnviar">
			<button class="btnEnviarProjetoGeo">Enviar</button>
		</div>
		<div class="enviado">
			<input type="hidden" class="hdnArquivo" />
			<span class="labNomeArquivo"></span>
		</div>
	</div>
	<% } %>

	<div class="block dataGrid divImportarArquivo divArquivosProcessados <%= Model.ArquivosProcessados.Count > 0 ? "" : "hide" %>">
		<table class="dataGridTable gridArquivos gridImportarArquivos">
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
						<input type="button" class="icone btnBaixar <%= arquivo.IsPDF ? "pdf" : "download"%>" title="baixar"/>
						<input type="hidden" class="hdnArquivoProcessadoId" value="<%= arquivo.Id %>" />
					</td>
				</tr>
				<% } %>
			</tbody>
		</table>

		<table class="hide">
			<tbody>
				<tr class="trTemplateArqProcessado">
					<td class='arquivoNome'></td>
					<td>
						<input type="hidden" class="hdnArquivoProcessadoId" value="0" />
						<input type="button" class="icone btnBaixar" />
					</td>
				</tr>
			</tbody>
		</table>
	</div>
</div>