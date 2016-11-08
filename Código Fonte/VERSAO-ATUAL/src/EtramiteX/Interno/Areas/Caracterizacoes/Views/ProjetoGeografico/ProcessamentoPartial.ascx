<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMProjetoGeografico" %>

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
						<input type="hidden" class="hdnArquivoProcessadoTipoId" value="<%= arquivo.Tipo %>" />
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
					<input type="hidden" class="hdnArquivoProcessadoTipoId" value="" />
					<button class="icone download btnBaixar"></button>
				</td>
			</tr>
		</tbody>
	</table>
</div>