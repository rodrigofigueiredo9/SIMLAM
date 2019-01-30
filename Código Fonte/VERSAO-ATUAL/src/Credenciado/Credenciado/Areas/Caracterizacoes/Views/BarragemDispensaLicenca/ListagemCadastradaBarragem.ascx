<%@ Import Namespace="Tecnomapas.EtramiteX.Configuracao" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BarragemDispensaLicencaVM>" %>

<fieldset class="block box">
	<legend class="titFiltros">Barragens Cadastradas</legend>

	<div class="dataGrid">
		<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th width="35%">Finalidade</th>
						<th width="20%">Área alagada(ha)</th>
						<th width="21%">Volume armazenado(m3)</th>
						<th width="28%">Situação</th>
						<th width="11%">Ações</th>
					</tr>
				</thead>

				<tbody>
					<% foreach (var item in Model.CaracterizacoesCadastradas)
						{%>
					<tr>
						<td title="<%=Html.Encode(item.Atividade)%>"><%=Html.Encode(item.Atividade)%></td>
						<td title="<%=Html.Encode(item.areaAlagada)%>"><%=Html.Encode(item.areaAlagada)%></td>
						<td title="<%=Html.Encode(item.volumeArmazanado)%>"><%=Html.Encode(item.volumeArmazanado)%></td>
						<td title="Valido">Válido</td>

						<td>
							<input type="hidden" class="hdnId" value="<%= item.Id %>" />
							<input type="hidden" class="hdnTid" value="<%= item.Tid %>" />
							<input title="Associar ao projeto digital" class="icone associar btnAssociar ui-button ui-widget ui-state-default ui-corner-all" type="button" role="button"/>
						</td>
					</tr>
					<%	} %>
				</tbody>
		</table>
	</div>
	
</fieldset>