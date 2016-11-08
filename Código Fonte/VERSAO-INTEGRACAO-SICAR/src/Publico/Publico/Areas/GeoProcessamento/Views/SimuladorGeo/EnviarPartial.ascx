﻿<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMSimuladorGeo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SimuladorGeoVM>" %>

<div class="containerEnviar">

	<div class="box block divSituacaoProjeto <%= (Model.ArquivoEnviado.Id??0) == 0 ? "hide" : "" %>">
		<div class="coluna50">
			Situação do Processamento: <label class="lblSituacaoProcessamento"><%= Model.ArquivoEnviado.SituacaoTexto %></label>
		</div>
		<div class="coluna30">
			<%--<span class="StatusFinalizado <%= Model.SituacaoProjeto == (int)eProjetoGeograficoSituacao.Finalizado ? "hide" : "" %>">--%>
			<span class="StatusFinalizado">
				<button class="btnReenviarArquivo <%= Model.MostrarReenviar ? "" : "hide" %>">Reenviar</button>
			</span>
			<%--<span class="StatusFinalizado <%= Model.SituacaoProjeto == (int)eProjetoGeograficoSituacao.Finalizado ? "hide" : "" %>">--%>
			<span class="StatusFinalizado">
				<button class="btnCancelarProcessamento <%= Model.MostrarCancelar ? "" : "hide" %>">Cancelar</button>
			</span>
			<input type="hidden" class="hdnArquivoEnviadoId" value="<%= Model.ArquivoEnviado.IdRelacionamento %>" />
			<input type="hidden" class="hdnArquivoEnviadoSituacaoId" value="<%= Model.ArquivoEnviado.Situacao %>" />
		</div>
	</div>

	<div class="box block divEnviarProjeto <%= (Model.ArquivoEnviado.Id??0) == 0 ? "" : "hide" %>">	
		<div class="coluna50 buscarEnviar">
			<input type="file" name="file" class="text fileProjetoGeo"/> 
		</div>
		<div class="coluna10 buscarEnviar">
			<button class="btnEnviarProjetoGeo">Enviar</button>
		</div>
		<div class="enviado">
			<input type="hidden" class="hdnArquivo" />
			<span class="labNomeArquivo"></span>
		</div>
	</div>

	<fieldset class="block box" >

	<div class="block dataGrid divImportarArquivo <%= Model.ArquivosProcessados.Count > 0 ? "" : "hide" %>">
	<table  class="dataGridTable gridImportarArquivos">
		<thead>
			<tr>
				<th>Arquivos processados para download</th>
				<th width="12%"> Ação </th>
			</tr>
		</thead>
		<tbody>
			<%foreach (ArquivoItemGridVM arquivo in Model.ArquivosProcessados) { %>
				<tr>
					<td class="arquivoNome" title="<%: arquivo.Texto %>"><%: arquivo.Texto %></td>
					<td>
						<button class="icone btnBaixar <%= arquivo.IsPDF ? "pdf" : "download"%>" title="baixar"></button>
						<input type="hidden" class="hdnArquivoProcessadoId" value="<%= arquivo.Id %>" />
					</td>
				</tr>
			<% } %>
		</tbody>
	</table>

	<table class="hide" >
		<tbody>
			<tr class="hide trTemplateArqProcessado">
				<td class='arquivoNome'></td>
				<td>
					<input type="hidden" class="hdnArquivoProcessadoId" value="0" />
					<button class="icone download btnBaixar"></button>
				</td>
			</tr>
		</tbody>
	</table>
	</div>

	</fieldset>
</div>