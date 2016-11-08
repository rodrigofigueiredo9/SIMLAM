<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SilviculturaATVCaracteristicaVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<%if(!Model.IsVisualizar) {%>
<script type="text/javascript">	
	SilviculturaATVCaracteristica.settings.mensagens = <%= Model.Mensagens %>;
</script>
<%} %>

<div class="block filtroCorpo divSilviculturaSilvicult">
	
	<input type="hidden" class="hdnSilviculturaSilvicultId" value="<%:Model.Caracterizacao.Id%>" />

	<div class="block">
		<div class="coluna12 append2">
			<label for="Silvicultura_Identificacao">Identificação</label>
			<%= Html.TextBox("Caracterizacao.Identificacao", Model.Caracterizacao.Identificacao, new { @class = "text txtIdentificacao disabled", disabled = "disabled" })%>
		</div>

		<div class="coluna14 append2">
			<label for="Silvicultura_GeometriaTipo">Geometria</label>
			<%= Html.DropDownList("Caracterizacao.GeometriaTipo", Model.GeometriaTipo, new { @class = "text ddlGeometriaTipo disabled", disabled = "disabled" })%>
		</div>	
	</div>

	<div class="block">	
		<div class="coluna50">
			<label for="">Tipo do fomento*</label><br />
			<span style="display: inline-block; border-style: solid; border-width: 1px; border-color: transparent;" class="text" id="spanRblFomento<%= Model.Caracterizacao.Identificacao %>">
			<%foreach (var item in Model.FomentoTipo){ %>
				<label><%= Html.RadioButton("rblFomento" + Model.Caracterizacao.Identificacao, item.Value, item.Selected, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rblFomento" }))%><%: item.Text %></label>				
			<% } %>
			</span>
		</div>		
	</div>

	<div class="block">	
		<div class="coluna30 append2">
			<label for="Caracterizacao_Declividade">Declividade predominante (graus) *</label>
			<%= Html.TextBox("Caracterizacao.Declividade" + Model.Caracterizacao.Identificacao, Model.Caracterizacao.Declividade, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtDeclividadeCarac maskDecimal", @maxlength = "5" }))%>
		</div>
		<div class="coluna30 append2">
			<label for="Caracterizacao_TotalRequerida">Total requerida (m²) *</label>
			<%= Html.TextBox("Caracterizacao.TotalRequerida" + Model.Caracterizacao.Identificacao, Model.Caracterizacao.TotalRequerida, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtTotalRequerida maskDecimal", @maxlength = "10" }))%>
		</div>
		<div class="coluna30 append2">
			<label for="Caracterizacao_TotalCroqui">Total croqui (m²)</label>
			<%= Html.TextBox("Caracterizacao.TotalCroqui" + Model.Caracterizacao.Identificacao, Model.Caracterizacao.TotalCroqui.ToStringTrunc(), ViewModelHelper.SetaDisabled(true, new { @class = "text txtTotalCroqui" }))%>
            <input type="hidden" class="hdnTotalCroqui" value="<%=Model.Caracterizacao.TotalCroqui%>" />
		</div>
	</div>

	<div class="block">	
		<div class="coluna30 append2">
			<label for="Caracterizacao_TotalPlantadaComEucalipto">Área total plantada com eucalipto em relação a ATP (%) *</label>
			<%= Html.TextBox("Caracterizacao.TotalPlantadaComEucalipto" + Model.Caracterizacao.Identificacao, Model.Caracterizacao.TotalPlantadaComEucalipto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtTotalPlantadaComEucalipto maskDecimal", @maxlength = "6" }))%>
		</div>
	</div>

	<% if(!Model.IsVisualizar) { %>
	<div class="block">
		<div class="coluna20 append2 divTipoCultivo">
			<label for="TipoCultura<%=Model.Caracterizacao.Identificacao%>">Cobertura existente *</label>
			<%= Html.DropDownList("TipoCultura" + Model.Caracterizacao.Identificacao, Model.TipoCobertura, new { @class = "text ddlTipoCultura" })%>
		</div>

		<div class="coluna18 append1">
			<label for="AreaCultura<%=Model.Caracterizacao.Identificacao%>">Área da cobertura (m²) *</label>
			<%= Html.TextBox("AreaCultura" + Model.Caracterizacao.Identificacao, String.Empty, new { @class = "text txtAreaCultura maskDecimal", @maxlength = "10" })%>
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
					<th>Cobertura existente</th>
					<th width="40%">Área da cobertura (m²)</th>
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
						<span class="AreaCultura" title="<%:cultura.AreaCultura%>"><%:cultura.AreaCultura%></span>
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