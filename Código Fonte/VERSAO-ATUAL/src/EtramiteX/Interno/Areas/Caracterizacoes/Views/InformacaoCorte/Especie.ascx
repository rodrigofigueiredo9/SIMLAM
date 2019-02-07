<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InformacaoCorteInformacaoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="informacaoCorteInformacaoEspecieContainer">
<% if (!Model.IsVisualizar){ %>
	<div class="block">
		<div class="coluna17 append2">
			<label for="InformacaoCorteInformacao_EspecieTipo">Espécie informada *</label>
			<%= Html.DropDownList("InformacaoCorteInformacao.EspecieTipo", Model.EspecieTipo, new { @class = "text ddlEspecieTipo " })%>
		</div>

		<div class="coluna11 divEspecificar hide append2">
			<label for="InformacaoCorteInformacao_EspecieEspecificarTexto">Especificar *</label>
			<%= Html.TextBox("InformacaoCorteInformacao.EspecieEspecificarTexto", String.Empty, new { @class = "text txtEspecieEspecificarTexto ", @maxlength="30" })%>
		</div>

		<div class="coluna19 append2">
			<label for="InformacaoCorteInformacao_ArvoresIsoladas">Árvores isoladas (unid.)</label>
			<%= Html.TextBox("InformacaoCorteInformacao.ArvoresIsoladas", String.Empty, new { @class = "text maskNumInt txtArvoresIsoladas ", @maxlength = "5" })%>
		</div>

		<div class="coluna15 append2">
			<label for="InformacaoCorteInformacao_AreaCorte">Área de corte (ha)</label>
			<%= Html.TextBox("InformacaoCorteInformacao.AreaCorte", String.Empty, new { @class = "text txtAreaCorte maskDecimalPonto4", @maxlength = "14" })%>
		</div>

		<div class="coluna18">
			<label for="InformacaoCorteInformacao_IdadePlantio">Idade do plantio (anos)</label>
			<%= Html.TextBox("InformacaoCorteInformacao.IdadePlantio", String.Empty, new { @class = "text maskNumInt txtIdadePlantio", @maxlength = "7" })%>
		</div>

		<div class="coluna5">
			<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarEspecie btnAddItem" title="Adicionar">+</button>
		</div>
	</div>
	<% } %>

	<div class="block dataGrid">
		<div class="coluna100">
			<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th width="23%">Espécie informada</th>
						<th width="25%">Árvores isoladas (unid.)</th>
						<th width="21%">Área de corte (ha)</th>
						<th width="24%">Idade do plantio (anos)</th>
						<%if (!Model.IsVisualizar){%><th width="7%">Ação</th><%} %>
					</tr>
				</thead>
				<% foreach (var especie in Model.Entidade.Especies){ %>
				<tbody>
					<tr>
						<td>
							<span class="especieTipo" title="<%:especie.EspecieTipoTexto%>"><%:especie.EspecieTipoTexto%></span>
						</td>
						<td>
							<span class="arvoresIsoladas" title="<%:especie.ArvoresIsoladas%>"><%:especie.ArvoresIsoladas%></span>
						</td>
						<td>
							<span class="areaCorte" title="<%:especie.AreaCorte%>"><%:especie.AreaCorte%></span>
						</td>
						<td>
							<span class="areaCorte" title="<%:especie.IdadePlantio%>"><%:especie.IdadePlantio%></span>
						</td>
						<%if (!Model.IsVisualizar){%>
							<td class="tdAcoes">
								<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(especie)%>' />
								<input title="Excluir" type="button" class="icone excluir btnExcluirEspecie" value="" />
							</td>
						<%} %>
					</tr>
					<% } %>
					<% if(!Model.IsVisualizar) { %>
						<tr class="trTemplateRow hide">
							<td><span class="especieTipo"></span></td>
							<td><span class="arvoresIsoladas"></span></td>
							<td><span class="areaCorte"></span></td>
							<td><span class="idadePlantio"></span></td>
							<td class="tdAcoes">
								<input type="hidden" class="hdnItemJSon" value="" />
								<input title="Excluir" type="button" class="icone excluir btnExcluirEspecie" value="" />
							</td>
						</tr>
					<% } %>
				</tbody>
			</table>
		</div>
	</div>
</div>
<br />