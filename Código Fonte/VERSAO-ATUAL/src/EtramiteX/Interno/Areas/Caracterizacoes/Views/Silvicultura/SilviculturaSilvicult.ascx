<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SilviculturaSilvicultVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<%if(!Model.IsVisualizar) {%>
<script>
	SilviculturaSilvicult.settings.mensagens = <%= Model.Mensagens %>;
	SilviculturaSilvicult.settings.idsTela = <%= Model.IdsTela %>;
</script>
<%} %>

<div class="block filtroCorpo divSilviculturaSilvicult">
	
	<input type="hidden" class="hdnSilviculturaSilvicultId" value="<%:Model.Caracterizacao.Id%>" />
	<input type="hidden" class="hdnSilviculturaSilvicultAreaCroqui" value="<%= Model.Caracterizacao.AreaCroquiHa %>" />

	<div class="block">
		<div class="coluna12 append2">
			<label for="Silvicultura_Identificacao">Identificação</label>
			<%= Html.TextBox("Silvicultura.Identificacao", Model.Caracterizacao.Identificacao, new { @class = "text txtIdentificacao disabled", disabled = "disabled" })%>
		</div>

		<div class="coluna14 append2">
			<label for="Silvicultura_GeometriaTipo">Geometria</label>
			<%= Html.DropDownList("Silvicultura.GeometriaTipo", Model.GeometriaTipo, new { @class = "text ddlGeometriaTipo disabled", disabled = "disabled" })%>
		</div>
		
		<div class="coluna20 <%:(Model.Caracterizacao.GeometriaTipo == (int)eTipoGeometria.Poligono) ? "" : "hide" %> append2">
			<label for="Silvicultura_AreaCroqui<%=Model.Caracterizacao.Identificacao%>">Área croqui (ha)</label>
			<%= Html.TextBox("Silvicultura.AreaCroqui" + Model.Caracterizacao.Identificacao, Model.Caracterizacao.AreaCroquiHa.ToStringTrunc(4), ViewModelHelper.SetaDisabled(true, new { @class = "text txtAreaCroqui" }))%>
		</div>
	</div>

	<% if(!Model.IsVisualizar) { %>
	<div class="block">
		<div class="coluna20 append2 divTipoCultivo">
			<label for="Silvicultura_TipoCultura<%=Model.Caracterizacao.Identificacao%>">Cultura florestal *</label>
			<%= Html.DropDownList("Silvicultura.TipoCultura" + Model.Caracterizacao.Identificacao, Model.TipoCultura, new { @class = "text ddlTipoCultura " })%>
		</div>

		<div class="coluna22 append2 divEspecificar">
			<label for="Silvicultura_Especificar<%=Model.Caracterizacao.Identificacao%>">Especificar *</label>
			<%= Html.TextBox("Silvicultura.Especificar" + Model.Caracterizacao.Identificacao, String.Empty, new { @class = "text txtEspecificar ", @maxlength = "80" })%>
		</div>

		<div class="coluna18 append1">
			<label for="Silvicultura_AreaCultura<%=Model.Caracterizacao.Identificacao%>">Área da cultura (ha) *</label>
			<%= Html.TextBox("Silvicultura.AreaCultura" + Model.Caracterizacao.Identificacao, String.Empty, new { @class = "text txtAreaCultura maskDecimalPonto4", @maxlength = "14" })%>
		</div>

		<div class="coluna10">
			<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarCultura btnAddItem" title="Adicionar">+</button>
		</div>
	</div>
	<%} %>

	<div class="block coluna65 dataGrid">
		<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
			<thead>
				<tr>
					<th>Cultura florestal</th>
					<th width="40%">Área da cultura (ha)</th>
					<%if (!Model.IsVisualizar){%><th width="9%">Ação</th><%} %>
				</tr>
			</thead>
			<% foreach (var cultura in Model.Caracterizacao.Culturas){ %>
			<tbody>
				<tr>
					<td>
						<span class="CulturaTipo" title="<%:cultura.CulturaTipoTexto%>"><%:cultura.CulturaTipoTexto%></span>
					</td>
					<td>
						<span class="AreaCultura" title="<%:cultura.AreaCulturaHa%>"><%:cultura.AreaCulturaHa%></span>
					</td>
					<%if (!Model.IsVisualizar){%>
						<td class="tdAcoes">
							<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(cultura)%>' />
							<input title="Excluir" type="button" class="icone excluir btnExcluirCultura" value="" />
						</td>
					<%} %>
				</tr>
				<% } %>
				<% if(!Model.IsVisualizar) { %>
					<tr class="trTemplateRow hide">
						<td><span class="CulturaTipo"></span></td>
						<td><span class="AreaCultura"></span></td>
						<td class="tdAcoes">
							<input type="hidden" class="hdnItemJSon" value="" />
							<input title="Excluir" type="button" class="icone excluir btnExcluirCultura" value="" />
						</td>
					</tr>
				<% } %>
			</tbody>
		</table>
	</div>
</div>