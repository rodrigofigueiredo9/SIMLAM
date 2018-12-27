<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InformacaoCorteVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>

<div class="containerDesenhador">
	<input type="hidden" class="hdnEmpreendimentoId" value="<%= Model.Empreendimento.EmpreendimentoId %>" />
    <fieldset class="block box">
        <legend>Empreendimento</legend>
        <div class="block">
            <div class="coluna20 append1">
                <label>Código</label>
                <%= Html.TextBox("EmpreendimentoId", Model.Empreendimento.EmpreendimentoCodigo, new { @class = "text cnpj disabled", @disabled = "disabled" })%>
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
            <div class="coluna25 append1">
                <label>Município</label>
                <%= Html.DropDownList("SelecionarPrimeiroItem", Model.Empreendimento.EmpreendimentoMunicipio, new { disabled = "disabled", @class = "text disabled" })%>
            </div>
            <div class="coluna20 append1">
                <label>CNPJ</label>
                <%= Html.TextBox("SelecionarPrimeiroItem", Model.Empreendimento.EmpreendimentoCNPJ, new { disabled = "disabled", @class = "text disabled" })%>
            </div>
            <div class="coluna20 ">
                <label>Área de Floresta Plantada *</label>
                <%= Html.TextBox("AreaPlantada", Model.AreaPlantada.ToStringTrunc(), new { @class = "text maskDecimalPonto areaPlantada disabled", @disabled = "disabled" })%>
            </div>
        </div>
    </fieldset>

	<fieldset class="box">
	<legend>Informações de corte</legend>

	<div class="block dataGrid">
		<div class="coluna100">
			<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th width="7%">Código</th>
						<th width="7%">Data</th>
						<th width="15%">Árvores isoladas (unid.)</th>
						<th width="10%">Área corte (ha)</th>
						<%if (Model.IsVisualizar){%><th width="6%">Ação</th><%} else{%><th width="11%">Ações</th><%} %>
					</tr>
				</thead>
				<tbody>
				<% foreach (var informacao in Model.InformacoesCortes){ %>
					<tr>
						<td>
							<span class="dataInformacao" title="<%:informacao.Id%>"><%:informacao.Id%></span>
						</td>
						<td>
							<span class="dataInformacao" title="<%:informacao.Data%>"><%:informacao.Data%></span>
						</td>
						<td>
							<span class="arvoresIsoladas" title="<%:informacao.ArvoreIsolada%>"><%:informacao.ArvoreIsolada%></span>
						</td>
						<td>
							<span class="areaCorte" title="<%:informacao.ArvoreCortada%>"><%:informacao.ArvoreCortada%></span>
						</td>
						<td class="tdAcoes">
							<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(informacao)%>' />
							<input type="hidden" class="itemId" value="<%= informacao.Id %>" />
							<input title="Visualizar" type="button" class="icone visualizar btnVisualizarInformacaoCorte" value="" />
							<%if (!Model.IsVisualizar){%><input title="Editar" type="button" class="icone editar btnEditarInformacaoCorte" value="" /><%} %>
							<%if (!Model.IsVisualizar){%><input title="Excluir" type="button" class="icone excluir btnExcluirInformacaoCorte" value="" /><%} %>
						</td>
					</tr>
					<% } %>
					<%--<% if(!Model.IsVisualizar) { %>
						<tr class="trTemplateRow hide">
							<td><span class="dataInformacao"></span></td>
							<td><span class="arvoresIsoladas"></span></td>
							<td><span class="areaCorte"></span></td>
							<td><span class="arvoresIsoladasRestante"></span></td>
							<td><span class="areaCorteRestante"></span></td>
							<td class="tdAcoes">
								<input type="hidden" class="hdnItemJSon" value="" />
								<input title="Visualizar" type="button" class="icone visualizar btnVisualizarInformacaoCorte" value="" />
								<input title="Editar" type="button" class="icone editar btnEditarInformacaoCorte" value="" />
								<input title="Excluir" type="button" class="icone excluir btnExcluirInformacaoCorte" value="" />
							</td>
						</tr>
					<% } %>--%>
				</tbody>
			</table>
		</div>
	</div>
	<br />
	<%--<fieldset class="boxBranca">
		<legend>Total do empreendimento</legend>
		
		<div class="block">
			<div class="coluna30 append2">
				<label for="InformacaoCorte_ArvoresIsoladasTotal">Árvores isoladas (unid.)*</label>
				<%= Html.TextBox("InformacaoCorte.ArvoresIsoladasTotal", Model.Caracterizacao.ArvoresIsoladasTotal, ViewModelHelper.SetaDisabled(true, new { @class = "text maskNumInt txtArvoresIsoladasTotal", @maxlength = "3" }))%>
			</div>

			<div class="coluna30 append2">
				<label for="InformacaoCorte_AreaCorteTotal">Área de corte (ha)*</label>
				<%= Html.TextBox("InformacaoCorte.AreaCorteTotal", Model.Caracterizacao.AreaCorteTotal, ViewModelHelper.SetaDisabled(true, new { @class = "text txtAreaCorteTotal", @maxlength = "10" }))%>
			</div>
		</div>
	</fieldset>--%>

	<%if(!Model.IsVisualizar) {%>
	<div class="block">
		<div class="direita" style="right: 15px; position: relative" >
			<button type="button" style="width:160px" class="inlineBotao btnAdicionarInformacaoCorte" title="Adicionar">+ Informação de corte</button>
		</div>
	</div>
	<%} %>
	<div class="block">
		<div position: relative" >
			<button type="button" style="width:160px" class="inlineBotao btnVoltar" title="Adicionar">Voltar</button>
		</div>
	</div>

</fieldset>
</div>
