<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InformacaoCorteAntigoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>

<input type="hidden" class="hdnEmpreendimentoId" value="<%: Model.Caracterizacao.EmpreendimentoId%>" />
<input type="hidden" class="hdnCaracterizacaoId" value="<%: Model.Caracterizacao.Id %>" />
<input type="hidden" class="hdnCaracterizacaoTipo" value="<%:(int)eCaracterizacao.InformacaoCorte%>" />

<script>
	InformacoesCortesInformacoes.settings.urls.visualizar = '<%= Url.Action("InformacaoCorteInformacaoVisualizar", "InformacaoCorte") %>';
</script>

<fieldset class="box">
	<legend>Informações de corte</legend>

	<div class="block dataGrid">
		<div class="coluna100">
			<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th width="7%">Data</th>
						<th width="15%">Árvores isoladas (unid.)</th>
						<th width="10%">Área corte (ha)</th>
						<th width="18%">Árvores isoladas rest. (unid.)</th>
						<th width="14%">Área corte rest. (ha)</th>
						<%if (Model.IsVisualizar){%><th width="6%">Ação</th><%} else{%><th width="11%">Ações</th><%} %>
					</tr>
				</thead>
				<tbody>
				<% foreach (var informacao in Model.Caracterizacao.InformacoesCortes){ %>
					<tr>
						<td>
							<span class="dataInformacao" title="<%:informacao.DataInformacao.DataTexto%>"><%:informacao.DataInformacao.DataTexto%></span>
						</td>
						<td>
							<span class="arvoresIsoladas" title="<%:informacao.DataInformacao%>"><%:informacao.ArvoresIsoladas%></span>
						</td>
						<td>
							<span class="areaCorte" title="<%:informacao.DataInformacao%>"><%:informacao.AreaCorte%></span>
						</td>
						<td>
							<span class="arvoresIsoladasRestante" title="<%:informacao.DataInformacao%>"><%:informacao.ArvoresIsoladasRestantes%></span>
						</td>
						<td>
							<span class="areaCorteRestante" title="<%:informacao.DataInformacao%>"><%:informacao.AreaCorteRestante%></span>
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
					<% if(!Model.IsVisualizar) { %>
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
					<% } %>
				</tbody>
			</table>
		</div>
	</div>
	<br />
	<fieldset class="boxBranca">
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
	</fieldset>

	<%if(!Model.IsVisualizar) {%>
	<div class="block">
		<div class="direita" style="right: 15px; position: relative" >
			<button type="button" style="width:160px" class="inlineBotao btnAdicionarInformacaoCorte" title="Adicionar">+ Informação de corte</button>
		</div>
	</div>
	<%} %>

</fieldset>

<div class="divInformacao"></div>