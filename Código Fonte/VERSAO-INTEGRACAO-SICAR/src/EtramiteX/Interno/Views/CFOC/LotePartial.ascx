<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCFOCFOC.CFOC" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LoteVM>" %>

<% if (Model.IsVisualizar) { %>
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
	<div class="gridContainer">
		<table class="dataGridTable gridLote" width="100%" border="0" cellspacing="0" cellpadding="0">
			<thead>
				<tr>
					<th width="23%">Origem</th>
					<th>Cultivar</th>
					<th width="16%">Quantidade</th>
					<th width="16%">Unidade de medida</th>
				</tr>
			</thead>

			<tbody>
				<% foreach (var item in Model.Lote.Lotes) { %>
				<tr>
					<td>
						<label class="lblOrigem" title="<%= item.OrigemTipoTexto +"-"+ item.OrigemNumero %>"><%= item.OrigemTipoTexto +"-"+item.OrigemNumero %></label>
					</td>
					<td>
						<label class="lblCultivar" title="<%= item.CulturaTexto + " " + item.CultivarTexto %>"><%= item.CulturaTexto + " " + item.CultivarTexto %></label>
					</td>
					<td>
						<label class="lblQuantidade" title="<%= item.Quantidade.ToStringTrunc(4) %>"><%= item.Quantidade.ToStringTrunc(4) %></label>
					</td>
					<td>
						<label class="lblUnidadeMedida" title="<%= item.UnidadeMedidaTexto %>"><%= item.UnidadeMedidaTexto %></label>
					</td>
				</tr>
				<% } %>
			</tbody>
		</table>
	</div>
</fieldset>