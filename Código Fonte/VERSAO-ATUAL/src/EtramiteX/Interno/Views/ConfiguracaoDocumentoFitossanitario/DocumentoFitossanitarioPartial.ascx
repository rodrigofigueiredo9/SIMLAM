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
			<%= Html.TextBox("BlocoNumeroInicial", null, new { @class = "text txtNumeroInicial txtBloco maskNum10"}) %>
		</div>
		<div class="coluna20">
			<label>Número final*</label>
			<%= Html.TextBox("BlocoNumeroFinal", null, new { @class = "text txtNumeroFinal txtBloco maskNum10"} ) %>
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
						<th width="25%">Número Inicial</th>
						<th width="25%">Número Final</th>
                        <th width="15%">Ações</th>
					</tr>
				</thead>
				<tbody>
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
			<%= Html.TextBox("DigitalNumeroInicial", null, new { @class = "text txtNumeroInicial txtDigital maskNum10"}) %>
		</div>
		<div class="coluna20">
			<label>Número final*</label>
			<%= Html.TextBox("DigitalNumeroFinal", null, new {@class = "text txtNumeroFinal txtDigital maskNum10"}) %>
		</div>
        <div class="coluna5">
			<label>Série</label>
			<%= Html.DropDownList("DigitalSerie", new List<SelectListItem>() {  new SelectListItem(){ Text = "", Value="" }, 
                                                                                new SelectListItem(){ Text = "A", Value="A" },
                                                                                new SelectListItem(){ Text = "B", Value="B" },
                                                                                new SelectListItem(){ Text = "C", Value="B" },
                                                                                new SelectListItem(){ Text = "D", Value="D" },
                                                                                new SelectListItem(){ Text = "E", Value="E" },
                                                                                new SelectListItem(){ Text = "F", Value="F" },
                                                                                new SelectListItem(){ Text = "G", Value="G" },
                                                                                new SelectListItem(){ Text = "H", Value="H" },
                                                                                new SelectListItem(){ Text = "I", Value="I" },
                                                                                new SelectListItem(){ Text = "J", Value="J" },
                                                                                new SelectListItem(){ Text = "K", Value="K" },
                                                                                new SelectListItem(){ Text = "L", Value="L" },
                                                                                new SelectListItem(){ Text = "M", Value="M" },
                                                                                new SelectListItem(){ Text = "N", Value="N" },
                                                                                new SelectListItem(){ Text = "O", Value="O" },
                                                                                new SelectListItem(){ Text = "P", Value="P" },
                                                                                new SelectListItem(){ Text = "Q", Value="Q" },
                                                                                new SelectListItem(){ Text = "R", Value="R" },
                                                                                new SelectListItem(){ Text = "S", Value="S" },
                                                                                new SelectListItem(){ Text = "T", Value="T" },
                                                                                new SelectListItem(){ Text = "U", Value="U" },
                                                                                new SelectListItem(){ Text = "V", Value="V" },
                                                                                new SelectListItem(){ Text = "W", Value="W" },
                                                                                new SelectListItem(){ Text = "X", Value="X" },
                                                                                new SelectListItem(){ Text = "Y", Value="Y" },
                                                                                new SelectListItem(){ Text = "Z", Value="Z" },}, new { @class = "text ddlDigitalSerie " })%>
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
						<th width="25%">Número Inicial</th>
						<th width="25%">Número Final</th>
                        <th width="15%">Ações</th>
					</tr>
				</thead>
				<tbody>
					<% foreach (var item in Model.NumerosDigitais) {

                            string itemSerieInicial = item.NumeroInicial.ToString();
                            string itemSerieFinal = item.NumeroFinal.ToString();

                            if (!string.IsNullOrEmpty(item.Serie))
                            {
                                itemSerieInicial += " / " + item.Serie;
                                itemSerieFinal += " / " + item.Serie;
                            } 
            
            %>
					<tr class="Linha">
						<td>
							<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(item)%>' />
							<span class="TipoDocumentoTexto" title="<%:item.TipoDocumentoTexto%>"><%:item.TipoDocumentoTexto%></span>
						</td>
						<td>
							<span class="NumeroInicial" title="<%:itemSerieInicial %>"><%:itemSerieInicial %></span>
						</td>
						<td>
							<span class="NumeroFinal" title="<%:itemSerieFinal %>"><%:itemSerieFinal %></span>
						</td>
                        <td>
                            <input type="hidden" class="ItemID" value="<%:item.ID%>" />
                            <input type="button" title="Editar" class="icone editar btnEditar" />
                            <button type="button" title="Excluir" class="icone excluir btnExcluir"></button>
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
				</tbody>
			</table>
		</div>
	</div>
</fieldset>