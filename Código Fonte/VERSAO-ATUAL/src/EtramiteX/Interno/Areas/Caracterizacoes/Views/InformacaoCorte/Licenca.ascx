<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InformacaoCorteInformacaoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="informacaoCorteInformacaoEspecieContainer">

	<fieldset class="box" id="">
		<legend>Licença</legend>
		<div class="block">
			<div class="coluna17 append6">
				<label for="InformacaoCorteInformacao_DataInformacao_DataTexto">N° Licença</label>
				<%= Html.TextBox("InformacaoCorteInformacao.DataInformacao.DataTexto", Model.Entidade.DataInformacao.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData txtDataInformacao" }))%>
			</div>

			<div class="coluna25 append6">
				<label for="InformacaoCorteInformacao_ArvoresIsoladasRestantes">Atividade</label>
				<%= Html.TextBox("InformacaoCorteInformacao.ArvoresIsoladasRestantes", Model.Entidade.ArvoresIsoladasRestantes, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskNumInt txtArvoresIsoladasRestantes", @maxlength="5" }))%>
			</div>

			<div class="coluna25">
				<label for="InformacaoCorteInformacao_AreaCorteRestante">Área Licenciada / Plantada (ha)</label>
				<%= Html.TextBox("InformacaoCorteInformacao.AreaCorteRestante", Model.Entidade.AreaCorteRestante, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtAreaCorteRestante maskDecimalPonto4", @maxlength = "14" }))%>
			</div>
			<div class="coluna17 append6">
				<label for="InformacaoCorteInformacao_DataInformacao_DataTexto">Data Vencimento</label>
				<%= Html.TextBox("InformacaoCorteInformacao.DataInformacao.DataTexto", Model.Entidade.DataInformacao.DataTexto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text maskData txtDataInformacao" }))%>
			</div>
			<div class="coluna5">
				<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarEspecie btnAddItem" title="Adicionar">+</button>
			</div>
		</div>

		<div class="block dataGrid">
		<div class="coluna100">
			<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th width="23%">N° Licença</th>
						<th width="25%">Atividade</th>
						<th width="21%">Área Licenciada / Plantada (ha)</th>
						<th width="24%">Data Vencimento</th>
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
	</fieldset>
</div>
<br />