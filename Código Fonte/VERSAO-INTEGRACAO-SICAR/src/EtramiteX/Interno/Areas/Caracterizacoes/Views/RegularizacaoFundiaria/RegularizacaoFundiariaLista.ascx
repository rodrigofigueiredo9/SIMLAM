<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RegularizacaoFundiariaVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script type="text/javascript">
	RegularizacaoFundiaria.settings.zona = <%: Model.Caracterizacao.Posse.Zona %>;
	RegularizacaoFundiaria.settings.empreendimentoID = <%: Model.Caracterizacao.EmpreendimentoId %>;
	RegularizacaoFundiaria.settings.caracterizacaoID = <%: Model.Caracterizacao.Id %>;
	RegularizacaoFundiaria.settings.matriculas = <%= ViewModelHelper.Json(Model.Caracterizacao.Matriculas) %>;
</script>

<fieldset class="box block" id="fsRegularizacoesFundiarias">
	<legend>Regularizações Fundiárias</legend>
	<div class="block dataGrid">
		<table class="dataGridTable tabRegularizacoesFundiarias" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
			<thead>
				<tr>
					<th>Identificação / Comprovação</th>
					<th width="20%">Área requerida (m²)</th>
					<th width="20%">Área total da posse (m²)</th>
					<th width="14%">Ações</th>
				</tr>
			</thead>
			<tbody>
			<% foreach (var posse in Model.Caracterizacao.Posses) { %>
				<tr>
					<td><span title="<%: posse.Identificacao + "/" + posse.ComprovacaoTexto %>"><%: posse.Identificacao + "/" + posse.ComprovacaoTexto %></span></td>
					<td><span class="areaRequerida" title="<%: posse.AreaRequerida.ToStringTrunc() %>"><%: posse.AreaRequerida.ToStringTrunc() %></span></td>
					<td><span class="areaCroqui" title="<%: posse.AreaCroqui.ToStringTrunc(2) %>"><%: posse.AreaCroqui.ToStringTrunc(2) %></span></td>

					<td class="tdAcoes">
						<input type="hidden" class="hdnItemJSon" value="<%: ViewModelHelper.Json(posse) %>" />
						<span class="spnVisualizar <%= (posse.AreaRequerida == 0) ? "hide" : "" %>"><button title="Visualizar" class="icone visualizar btnVisualizarPosse" type="button"></button></span>
						<% if(!Model.IsVisualizar) { %> <button title="Editar" class="icone editar btnEditarPosse" type="button"></button><% } %>
					</td>
				</tr>
			<% } %>
			</tbody>
		</table>
	</div>
</fieldset>

<div class="block divRegularizacaoFundiaria"></div>