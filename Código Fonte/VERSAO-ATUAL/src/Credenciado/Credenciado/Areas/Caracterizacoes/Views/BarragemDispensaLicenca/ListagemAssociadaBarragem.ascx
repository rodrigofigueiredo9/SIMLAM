<%@ Import Namespace="Tecnomapas.EtramiteX.Configuracao" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BarragemDispensaLicencaVM>" %>

<fieldset class="block box">
	<legend class="titFiltros">Barragem Associada</legend>

	<div class="dataGrid">
		
		<button title="Cadastrar" class="btnAdicionar floatRight" role="button" style="margin:0.5%"><strong>+ Barragem</strong></button>		
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
					<% foreach (var item in Model.CaracterizacoesAssociadas)
						{%>
					<tr>
						
						<td title="<%=Html.Encode(item.FinalidadeAtividade)%>"><%=Html.Encode(item.FinalidadeAtividade)%></td>
						<td title="<%=Html.Encode(item.AreaAlagada)%>"><%=Html.Encode(item.AreaAlagada)%></td>
						<td title="<%=Html.Encode(item.VolumeArmazanado)%>"><%=Html.Encode(item.VolumeArmazanado)%></td>
						<td title="Cadastrada">Cadastrada</td>

						<td>
							<input type="hidden" class="hdnId" value="<%= item.Id %>" />
							<input type="hidden" class="hdnTid" value="<%= item.Tid %>" />
							<input title="Visualizar" class="icone visualizar btnVisualizar ui-button ui-widget ui-state-default ui-corner-all" type="button" role="button" aria-disabled="false">
							<input title="Editar" class="icone editar btnEditar ui-button ui-widget ui-state-default ui-corner-all" type="button" role="button" aria-disabled="false">
						</td>
					</tr>
					<%	} %>
				</tbody>
		</table>
	</div>
	
</fieldset>