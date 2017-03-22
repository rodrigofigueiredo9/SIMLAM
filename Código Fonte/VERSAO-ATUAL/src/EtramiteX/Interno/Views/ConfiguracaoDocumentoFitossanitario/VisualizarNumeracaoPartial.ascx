<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoDocumentoFitossanitario" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ConfiguracaoNumeracaoListarVM>" %>

<%= Html.Hidden("Configuracao.ID", Model.Configuracao.ID, new { @class = "configuracaoID" })%>
<fieldset class="block box">

	<div class="block">
		<div class="coluna20">
			<label>Tipo do Documento*</label>
			<%= Html.DropDownList("BlocoTipoDocumento", Model.TiposDocumento, new { @class = "text ddlBloco ddlTipoDocumento setarFoco" })%>
		</div>
		<div class="coluna20">
            <label>Tipo de Numeração*</label>
			<%= Html.DropDownList("BlocoTipoNumeracao", Model.TiposNumeracao, new { @class = "text ddlBloco ddlTipoNumeracao"}) %>
		</div>
		<div class="coluna20">
            <label>Ano*</label>
			<%= Html.TextBox("BlocoAno", null, new { @class = "text txtAno txtBloco maskNum4"} ) %>
		</div>
		<div class="coluna20">
			<button type="button" class="inlineBotao botaoBuscarIcone btnBuscarNumero btnBuscarItem" title="Buscar">Buscar</button>
		</div>
	</div>

	<div class="block dataGrid">
		<div class="coluna75">
			<table class="dataGridTable dgNumeros dgNumerosBloco" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th>Tipo do Documento</th>
						<th width="25%">Número Inicial</th>
						<th width="25%">Número Final</th>
                        <th width="15%">Ações</th>
					</tr>
				</thead>
				<%--<tbody>
					<% foreach (var item in Model.NumerosBloco) { %>
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
                        <td>
                            <input type="hidden" class="ItemID" value="<%:item.ID%>" />
                            <input type="button" title="Editar" class="icone editar btnEditar" />
                            <input type="button" title="Excluir" class="icone excluir btnExcluir" />
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
                        <td><span class="Acoes"></span></td>
					</tr>
				</tbody>--%>
			</table>
		</div>
	</div>
</fieldset>