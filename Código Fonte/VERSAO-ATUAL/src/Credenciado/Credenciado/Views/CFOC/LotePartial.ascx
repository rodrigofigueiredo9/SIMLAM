<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCFOCFOC.CFOC" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LoteVM>" %>

<% if (Model.IsVisualizar)
   { %>
<h1 class="titTela">Visualizar Lote</h1>
<br />
<% } %>

<input type="hidden" class="hdLoteId" value="<%= Model.Lote.Id %>" />
<div class="block box">
	<div class="block">
		<div class="coluna50">
			<label for="Empreemdimento">Empreendimento *</label>
			<%= Html.DropDownList("Empreendimento", Model.EmpreendimentoList, ViewModelHelper.SetaDisabled(Model.Lote.Id > 0, new { @class = "ddlEmpreemdimento" }) )%>
		</div>
		<div class="coluna15 prepend1">
			<label for="DataCriacao">Data de criação *</label>
			<%= Html.TextBox("DataCriacao", Model.Lote.DataCriacao.DataTexto, ViewModelHelper.SetaDisabled(true, new { @class = "text maskData txtDataCriacao campoTipoNumero" }))%>
		</div>
	</div>
</div>

<fieldset class="block box">
	<legend>Número do Lote</legend>
	<input type="hidden" class="hdnUCId" value="0" />

	<div class="block">
		<div class="coluna15">
			<label for="CodigoUC">Código da UC *</label>
			<%= Html.TextBox("CodigoUC", Model.Lote.CodigoUC > 0 ? (object)Model.Lote.CodigoUC :  null, ViewModelHelper.SetaDisabled(true, new { @maxlength = "11", @class = "text maskNumInt txtUCCodigo campoTipoNumero" }))%>
		</div>
		<div class="coluna5 prepend1">
			<label for="Ano">Ano *</label>
			<%= Html.TextBox("Ano", Model.Lote.Ano, ViewModelHelper.SetaDisabled(true, new { @maxlength = "2", @class = "text maskNumInt txtAno campoTipoNumero" }))%>
		</div>
		<div class="coluna23 prepend1">
			<label for="Numero">Nº sequencial *</label>
			<%= Html.TextBox("Numero", string.IsNullOrEmpty(Model.Lote.Numero) ? "Gerado automaticamente" : Model.Lote.Numero.PadLeft(4, '0'), ViewModelHelper.SetaDisabled(true, new { @maxlength = "4", @class = "text maskNumInt txtNumero" }))%>
		</div>
	</div>
</fieldset>

<fieldset class="block box">
	<% if (!Model.IsVisualizar)
	{ %>
	<div class="block divNumeroEnter">
		<div class="coluna25">
			<label for="Origem_Tipo">Documento de origem *</label>
			<%= Html.DropDownList("Origem_Tipo", Model.OrigemList, new { @class = "ddlOrigem" })%>
		</div>
		<div class="coluna25 prepend1">
			<label for="Origem">Número do documento *</label>
			<%= Html.TextBox("Origem", (object)String.Empty, new { @maxlength="12",  @class = "text txtNumeroOrigem" })%>
		</div>
		<div class="coluna9 prepend1 divVerificarNumero">
			<button type="button" class="inlineBotao btnVerificar">Verificar</button>
		</div>
	</div>

	<div class="block divAdicionar">
		<div class="block">
			<div class="coluna25 divVerificarNumero">
				<label for="Cultura">Cultura *</label>
				<%= Html.DropDownList("Cultura", ViewModelHelper.CriarSelectList(new List<String>()), ViewModelHelper.SetaDisabled(true, new { @class = "ddlCultura" }))%>
			</div>

			<div class="coluna25 divObterCultura hide">
				<label for="Cultura">Cultura *</label>
				<input type="hidden" class="hdnCulturaId" id="hdnCulturaId" name="hdnCulturaId" value="0" />
				<%= Html.TextBox("Cultura", (object)String.Empty, ViewModelHelper.SetaDisabled(true, new { @class = "text txtCultura" }))%>
			</div>
			<div class="coluna10 prepend1 divObterCultura hide">
				<button type="button" class="inlineBotao btnAssociarCultura">Buscar</button>
			</div>
		</div>

		<div class="coluna25">
			<label for="Cultivar">Cultivar *</label>
			<%= Html.DropDownList("Cultivar", ViewModelHelper.CriarSelectList(new List<String>()), ViewModelHelper.SetaDisabled(true, new { @class = "ddlCultivar" }))%>
		</div>
		<div class="coluna25 prepend1">
			<label for="UnidadeMedida">Unidade de Medida *</label>
			<%= Html.DropDownList("UnidadeMedida", ViewModelHelper.CriarSelectList(new List<String>()), ViewModelHelper.SetaDisabled(true, new { @class = "ddlUnidadeMedida" }))%>
		</div>
		<div class="coluna15 prepend1">
			<label for="Quantidade">Quantidade *</label>
			<%= Html.TextBox("Quantidade",(object)String.Empty, ViewModelHelper.SetaDisabled(true,   new { @maxlength="12",  @class = "text maskDecimalPonto4 txtQuantidade" }))%>
		</div>
		<div class="coluna1 prepend1">
			<button type="button" class="inlineBotao bloco btnAdicionar">Adicionar</button>
		</div>
	</div>
	<% } %>
	<div class="gridContainer">
		<table class="dataGridTable gridLote" width="100%" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th width="23%">Origem</th>
					<th>Cultivar</th>
					<th width="16%">Quantidade</th>
					<th width="16%">Unidade de medida</th>
					<% if (!Model.IsVisualizar)
		{ %>
					<th width="7%">Ações</th>
					<% } %>
				</tr>
			</thead>

			<tbody>
				<% foreach (var item in Model.Lote.Lotes)
	               {
                       decimal qtd = 0;
                       var unid = "";
                       if (item.ExibeKg)
                       {
                           qtd = item.Quantidade * 1000;
                           unid = "KG";
                       }
                       else
                       {
                           qtd = item.Quantidade;
                           unid = item.UnidadeMedidaTexto;
                       }   
           %>
				<tr>
					<td>
						<label class="lblOrigem" title="<%= item.OrigemTipoTexto +"-"+ item.OrigemNumero %>"><%= item.OrigemTipoTexto +"-"+item.OrigemNumero %></label>
					</td>
					<td>
						<label class="lblCultivar" title="<%= item.CulturaTexto + " " + item.CultivarTexto %>"><%= item.CulturaTexto + " " + item.CultivarTexto %></label>
					</td>
					<td>
						<label class="lblQuantidade" title="<%= qtd.ToStringTrunc(4) %>"><%= qtd.ToStringTrunc(4) %></label>
					</td>
					<td>
						<label class="lblUnidadeMedida" title="<%= unid %>"><%= unid %></label>
					</td>
					<% if (!Model.IsVisualizar)
		{ %>
					<td>
						<a class="icone excluir btnExcluir" title="Remover"></a>
						<input type="hidden" value="<%= item.Origem %>" class="hdnOrigemID" />
						<input type="hidden" value='<%: ViewModelHelper.Json(item) %>' class="hdnItemJson" />
					</td>
					<% } %>
				</tr>
				<% } %>

				<tr class="hide trTemplate">
					<td>
						<label class="lblOrigem">Origem</label>
					</td>
					<td>
						<label class="lblCultivar">Cultivar</label>
					</td>
					<td>
						<label class="lblQuantidade">Quantidade</label>
					</td>
					<td>
						<label class="lblUnidadeMedida">UnidadeMedida</label>
					</td>
					<td>
						<a class="icone excluir btnExcluir" title="Remover"></a>
						<input type="hidden" value="" class="hdnOrigemID" />
						<input type="hidden" value="0" class="hdnItemJson" />
					</td>
				</tr>
			</tbody>
		</table>
	</div>
</fieldset>
