<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ProducaoCarvaoVegetalVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="divFornos">
	<fieldset class="box">
		<%if(!Model.IsVisualizar) {%>
		<div class="block">
			<div class="coluna14 append2">
				<label for="ProducaoCarvaoVegetal_Identificador">Identificador</label>
				<%= Html.TextBox("ProducaoCarvaoVegetal.Identificador", (Model.Caracterizacao.Fornos.Count > 0) ? (Model.Caracterizacao.Fornos.Count + 1).ToString() : String.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtIdentificador disabled", @disabled = "disabled" }))%>
			</div>

			<div class="coluna25 append2">
				<label for="ProducaoCarvaoVegetal_VolumeForno">Volume útil de cada forno (m³) *</label>
				<%= Html.TextBox("ProducaoCarvaoVegetal.VolumeForno", String.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtVolume maskDecimal", @maxlength = "10" }))%>
			</div>

			<div class="coluna10">
				<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarForno btnAddItem" title="Adicionar">+</button>
			</div>
		</div>
		<%} %>

		<div class="block dataGrid">
			<div class="coluna50">
				<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
					<thead>
						<tr>
							<th width="15%">Identificador</th>
							<th width="30%">Volume útil de cada forno (m³)</th>
							<%if (!Model.IsVisualizar){%><th width="9%">Ação</th><%} %>
						</tr>
					</thead>
					<% foreach (var forno in Model.Caracterizacao.Fornos){ %>
					<tbody>
						<tr>
							<td>
								<span class="identificador" title="<%:forno.Identificador%>"><%:forno.Identificador%></span>
							</td>
							<td>
								<span class="volumeForno" title="<%:forno.Volume%>"><%:forno.Volume%></span>
							</td>
							<%if (!Model.IsVisualizar){%>
								<td class="tdAcoes">
									<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(forno)%>' />
									<input title="Excluir" type="button" class="icone excluir btnExcluirForno" value="" />
								</td>
							<%} %>
						</tr>
						<% } %>
						<% if(!Model.IsVisualizar) { %>
							<tr class="trTemplateRow hide">
								<td><span class="identificador"></span></td>
								<td><span class="volumeForno"></span></td>
								<td class="tdAcoes">
									<input type="hidden" class="hdnItemJSon" value="" />
									<input title="Excluir" type="button" class="icone excluir btnExcluirForno" value="" />
								</td>
							</tr>
						<% } %>
					</tbody>
				</table>
			</div>
		</div>
		<br />

		<div class="block">
			<div class="coluna25">
				<label for="ProducaoCarvaoVegetal_VolumeTotal">Volume útil total de fornos (m³) *</label>
				<%= Html.TextBox("ProducaoCarvaoVegetal.VolumeTotal", Model.TotalVolumeFornos, new { @class = "text txtVolumeTotalFornos disabled", @disabled = "disabled" })%>
			</div>
		</div>
	</fieldset>
</div>