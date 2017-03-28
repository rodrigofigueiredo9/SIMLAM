<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoDocumentoFitossanitario" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ConfiguracaoNumeracaoListarVM>" %>

<div class="block dataGrid">
	<div class="coluna75">
		<table class="dataGridTable dgNumeros dgNumerosBloco" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
			<thead>
				<tr>
					<th>Tipo do Documento</th>
					<th width="25%">Número Inicial</th>
					<th width="25%">Número Final</th>
				</tr>
			</thead>
			<tbody>
				<% foreach (var item in Model.Resultados) { %>
				<tr class="Linha">
					<td>
						<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(item)%>' />
						<span class="TipoDocumentoTexto" title="<%:item.TipoDocumentoTexto%>"><%:item.TipoDocumentoTexto%></span>
					</td>
					<td>
						<span class="NumeroInicial" title="<%:item.NumeroInicial.ToString() %>"><%:item.NumeroInicial.ToString() %></span>
					</td>
					<td>
						<span class="NumeroFinal" title="<%:item.NumeroFinal.ToString() %>"><%:item.NumeroFinal.ToString() %></span>
					</td>
				</tr>
				<% } %>

				<tr class="trTemplateRow hide">
					<td>
						<input type="hidden" class="hdnItemJSon" value="" />
						<span class="TipoDocumentoTexto"></span>
					</td>
					<td><span class="NumeroInicial"></span></td>
					<td><span class="NumeroFinal"></span></td>
				</tr>
			</tbody>
		</table>
	</div>
</div>