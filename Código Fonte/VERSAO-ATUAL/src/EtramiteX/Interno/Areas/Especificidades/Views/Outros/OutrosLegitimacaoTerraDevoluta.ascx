<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Outros" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<OutrosLegitimacaoTerraDevolutaVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Outros/OutrosLegitimacaoTerraDevoluta.js") %>"></script>
<script>
	OutrosLegitimacaoTerraDevoluta.settings.urls.urlObterDadosOutrosLegitimacaoTerraDevoluta = '<%= Url.Action("ObterDadosOutrosLegitimacaoTerraDevoluta", "Outros", new {area="Especificidades"}) %>';
	OutrosLegitimacaoTerraDevoluta.settings.Mensagens = <%= Model.Mensagens %>;
</script>

<fieldset class="block box">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>

	<div class="block divDestinatarios">
		<%if(!Model.IsVisualizar) {%>
		<div class="block">
			<div class="coluna68 append1">
				<label >Destinatários *</label>
				<%= Html.DropDownList("Outros.Destinatario", Model.Destinatarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Destinatarios.Count <= 2, new { @class = "text ddlDestinatarios" }))%>
			</div>

			<div class="coluna5">
				<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarDestinatario btnAddItem" title="Adicionar">+</button>
			</div>
		</div>
		<%} %>

		<div class="block dataGrid coluna75">
			<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th width="91%">Destinatários</th>
						<%if (!Model.IsVisualizar){%><th width="9%">Ação</th><%} %>
					</tr>
				</thead>
				<% foreach (var item in Model.Outros.Destinatarios){ %>
				<tbody>
					<tr>
						<td>
							<span class="Destinatario" title="<%:item.Nome%>"><%:item.Nome%></span>
						</td>
						<%if (!Model.IsVisualizar){%>
							<td class="tdAcoes">
								<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(item)%>' />
								<input title="Excluir" type="button" class="icone excluir btnExcluirDestinatario" value="" />
							</td>
						<%} %>
					</tr>
					<% } %>
					<% if(!Model.IsVisualizar) { %>
						<tr class="trTemplateRow hide">
							<td><span class="Destinatario"></span></td>
							<td class="tdAcoes">
								<input type="hidden" class="hdnItemJSon" value="" />
								<input title="Excluir" type="button" class="icone excluir btnExcluirDestinatario" value="" />
							</td>
						</tr>
					<% } %>
				</tbody>
			</table>
		</div>
	</div>

	<div class="block">
		<div class="coluna75">
			<label>Posse *</label>
			<%= Html.DropDownList("Outros.Dominio", Model.Dominios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Dominios.Count <= 2, new { @class = "text ddlDominios" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna15 append1">
			<label>Valor do Terreno *</label>
			<%= Html.TextBox("Outros.ValorTerreno", Model.Outros.ValorTerreno, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtValorTerreno maskDecimal", @maxlength="12" }))%>
		</div>
		<div class="coluna21">
			<label>Possui Inalienabilidade? *</label>
			<br />
			<label><%= Html.RadioButton("Outros.IsInalienabilidade", 1, Model.Outros.IsInalienabilidade.HasValue ? Model.Outros.IsInalienabilidade.Value : false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdbIsInalienabilidade rdbSim" }))%>Sim</label>
			<label class="append5"><%= Html.RadioButton("Outros.IsInalienabilidade", 0, Model.Outros.IsInalienabilidade.HasValue ? !Model.Outros.IsInalienabilidade.Value : false, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rdbIsInalienabilidade rdbNao" }))%>Não</label>
		</div>
		<div class="coluna30">
			<label>Município da Gleba *</label>
			<%=Html.DropDownList("Outros.MunicipioGlebaId", Model.MunicipiosGleba, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class="text ddlMunicipioGleba"}))%>
		</div>
	</div>
</fieldset>