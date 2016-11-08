<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AquiculturaAquicultVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="divCultivos">
	<%if(!Model.IsVisualizar) {%>
	<div class="block">
		<div class="coluna14 append2">
			<label for="Aquicultura_Identificador">Identificador</label>
			<%= Html.TextBox("Aquicultura.Identificador", (Model.Caracterizacao.Cultivos.Count > 0) ? (Model.Caracterizacao.Cultivos.Count + 1).ToString() : String.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtIdentificador disabled", @disabled = "disabled" }))%>
		</div>

		<div class="coluna25 append2">
			<label for="Aquicultura_Volume<%:Model.Caracterizacao.Identificador%>">Volume p/ unidade (m³) *</label>
			<%= Html.TextBox("Aquicultura.Volume" + Model.Caracterizacao.Identificador, String.Empty, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtVolume", @maxlength = "7" }))%>
		</div>

		<div class="coluna10">
			<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarCultivo btnAddItem" title="Adicionar">+</button>
		</div>
	</div>
	<%} %>

	<div class="block dataGrid">
		<div class="coluna50">
			<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th width="15%">Identificador </th>
						<th width="30%">Volume p/ unidade (m³)</th>
						<%if (!Model.IsVisualizar){%><th width="9%">Ação</th><%} %>
					</tr>
				</thead>
				<% foreach (var cultivo in Model.Caracterizacao.Cultivos){ %>
				<tbody>
					<tr>
						<td>
							<span class="identificador" title="<%:cultivo.Identificador%>"><%:cultivo.Identificador%></span>
						</td>
						<td>
							<span class="volume" title="<%:cultivo.Volume%>"><%:cultivo.Volume%></span>
						</td>
						<%if (!Model.IsVisualizar){%>
							<td class="tdAcoes">
								<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(cultivo)%>' />
								<input title="Excluir" type="button" class="icone excluir btnExcluirCultivo" value="" />
							</td>
						<%} %>
					</tr>
					<% } %>
					<% if(!Model.IsVisualizar) { %>
						<tr class="trTemplateRow hide">
							<td><span class="identificador"></span></td>
							<td><span class="volume"></span></td>
							<td class="tdAcoes">
								<input type="hidden" class="hdnItemJSon" value="" />
								<input title="Excluir" type="button" class="icone excluir btnExcluirCultivo" value="" />
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
			<label for="Aquicultura_VolumeTotalCultivos">Volume total de cultivo (m³) *</label>
			<%= Html.TextBox("Aquicultura.VolumeTotalCultivos", Model.VolumeTotalCultivos, new { @class = "text txtVolumeTotalCultivos disabled", @disabled = "disabled" })%>
		</div>
	</div>
</div>