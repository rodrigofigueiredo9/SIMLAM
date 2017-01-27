<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoDocumentoFitossanitario" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ConfiguracaoDocumentoFitossanitarioVM>" %>

<%= Html.Hidden("Configuracao.ID", Model.Configuracao.ID, new { @class = "configuracaoID" })%>
<fieldset class="block box">
	<legend>N° Bloco</legend>

	<div class="block">
		<div class="coluna20">
			<label>Tipo do Documento*</label>
			<%= Html.DropDownList("BlocoTipoDocumento", Model.TiposDocumento, new { @class = "text ddlBloco ddlTipoDocumento setarFoco" })%>
		</div>
		<div class="coluna20">
			<label>Número inicial*</label>
			<%= Html.TextBox("BlocoNumeroInicial", null, new { @class = "text txtNumeroInicial txtBloco maskNum8"}) %>
		</div>
		<div class="coluna20">
			<label>Número final*</label>
			<%= Html.TextBox("BlocoNumeroFinal", null, new { @class = "text txtNumeroFinal txtBloco maskNum8"} ) %>
		</div>
		<div class="coluna20">
			<button type="button" class="inlineBotao botaoAdicionarIcone btnAdicionarNumero btnAddItem" title="Adicionar">+</button>
		</div>
	</div>

	<div class="block dataGrid">
		<div class="coluna75">
			<table class="dataGridTable dgNumeros dgNumerosBloco" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th>Tipo do Documento</th>
						<th width="30%">Número Inicial</th>
						<th width="30%">Número Final</th>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.NumerosBloco) { %>
					<tr>
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
</fieldset>

<fieldset class="block box">
	<legend>N° Digital</legend>

	<div class="block">
		<div class="coluna20">
			<label>Tipo do Documento*</label>
			<%= Html.DropDownList("DigitalTipoDocumento", Model.TiposDocumento, new { @class= "text ddlDigital ddlTipoDocumento " }) %>
		</div>
		<div class="coluna20">
			<label>Número inicial*</label>
			<%= Html.TextBox("DigitalNumeroInicial", null, new { @class = "text txtNumeroInicial txtDigital maskNum8"}) %>
		</div>
		<div class="coluna20">
			<label>Número final*</label>
			<%= Html.TextBox("DigitalNumeroFinal", null, new {@class = "text txtNumeroFinal txtDigital maskNum8"}) %>
		</div>
		<div class="coluna20">
			<button type="button" class="inlineBotao botaoAdicionarIcone btnAdicionarNumero btnAddItem" title="Adicionar">+</button>
		</div>
	</div>

	<div class="block dataGrid">
		<div class="coluna75">
			<table class="dataGridTable dgNumeros dgNumerosDigital" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th>Tipo do Documento</th>
						<th width="30%">Número Inicial</th>
						<th width="30%">Número Final</th>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.NumerosDigitais) { %>
					<tr>
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
</fieldset>