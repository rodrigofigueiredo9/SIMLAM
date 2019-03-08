<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMInformacaoCorte" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="containerDesenhador">
	<%= Html.Hidden("EmpreendimentoId", Model.Empreendimento.EmpreendimentoId, new { @class = "hdnEmpreendimentoId" }) %>
   <fieldset class="block box">
        <legend>Empreendimento</legend>
        <div class="block">
            <div class="coluna20 append1">
                <label>Código</label>
                <%= Html.TextBox("EmpreendimentoCodigo", Model.Empreendimento.EmpreendimentoCodigo, new { @class = "text cnpj disabled", @disabled = "disabled" })%>
            </div>
            <div class="coluna54 append1">
                <label>Denominação</label>
                <%= Html.TextBox("DenominadorValor", Model.Empreendimento.DenominadorValor, new { @maxlength = "100", @class = "text denominador disabled", @disabled = "disabled" })%>
            </div>
            <div class="coluna20">
                <label>Área do imóvel (ha)</label>
                <%= Html.TextBox("AreaImovel", Model.Empreendimento.AreaImovel.ToStringTrunc(), new { @maxlength = "100", @class = "text areaImovel disabled", @disabled = "disabled" })%>
            </div>
        </div>

        <div class="block">
            <div class="coluna20 append1">
                <label>Zona de localização *</label>
                <%= Html.DropDownList("SelecionarPrimeiroItem", Model.Empreendimento.EmpreendimentoZonaLocalizacao, new { disabled = "disabled", @class = "text disabled" })%>
            </div>
            <div class="coluna7 append1">
                <label>UF</label>
                <%= Html.DropDownList("SelecionarPrimeiroItem", Model.Empreendimento.EmpreendimentoUf, new { disabled = "disabled", @class = "text disabled" })%>
            </div>
            <div class="coluna45 append1">
                <label>Município</label>
                <%= Html.DropDownList("SelecionarPrimeiroItem", Model.Empreendimento.EmpreendimentoMunicipio, new { disabled = "disabled", @class = "text disabled" })%>
            </div>
            <div class="coluna20 ">
                <label>Área de Floresta Plantada *</label>
                <%= Html.TextBox("AreaPlantada", Model.AreaPlantada.ToStringTrunc(), ViewModelHelper.SetaDisabled(true, new { @class = "text maskDecimalPonto areaPlantada", @maxlength = "12" }))%>
            </div>
        </div>
    </fieldset>

	<div class="block dataGrid">
		<div class="coluna100">
			<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th width="7%">Código</th>
						<th width="7%">Data</th>
						<th width="15%">Área Corte (ha) / N° Árvores</th>
						<th width="11%">Ações</th>
					</tr>
				</thead>
				<tbody>
				<% foreach (var informacao in Model.Resultados){ %>
					<tr>
						<td>
							<span class="codigo" title="<%: String.Concat(informacao.Codigo.ToString().PadLeft(4, '0'), '-', informacao.DataInformacao.DataTexto)%>"><%:String.Concat(informacao.Codigo.ToString().PadLeft(4, '0'), '-', informacao.DataInformacao.DataTexto)%></span>
						</td>
						<td>
							<span class="dataInformacao" title="<%:informacao.DataInformacao.DataTexto%>"><%:informacao.DataInformacao.DataTexto%></span>
						</td>
						<td>
							<span class="areaCorte" title="<%:informacao.AreaCorteCalculada.ToStringTrunc()%>"><%:informacao.AreaCorteCalculada.ToStringTrunc()%></span>
						</td>
						<td class="tdAcoes">
							<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(informacao)%>' />
							<input type="hidden" class="itemId" value="<%= informacao.Id %>" />
							<input type="hidden" class="itemAntigo" value="<%= informacao.Antigo %>" />
							<%if (Model.PodeVisualizar){%><input title="Visualizar" type="button" class="icone visualizar btnVisualizarInformacaoCorte" value="" /><%} %>
							<%if (Model.PodeEditar){%><input title="Editar" type="button" class="icone editar btnEditarInformacaoCorte" value="" /><%} %>
							<%if (Model.PodeExcluir){%><input title="Excluir" type="button" class="icone excluir btnExcluirInformacaoCorte" value="" /><%} %>
						</td>
					</tr>
					<% } %>
				</tbody>
			</table>
		</div>
	</div>
	<br />
</div>
