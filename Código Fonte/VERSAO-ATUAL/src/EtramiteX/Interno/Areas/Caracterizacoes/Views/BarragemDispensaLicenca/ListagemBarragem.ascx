<%@ Import Namespace="Tecnomapas.EtramiteX.Configuracao" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BarragemDispensaLicencaVM>" %>

<div class="block">
	<div class="coluna80">
		<label for="Atividade">Atividade *</label>
		<%= Html.DropDownList("Atividade", Model.Atividades, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlAtividade" }))%>
	</div>
		
</div>

<fieldset class="block box">
	<legend class="titFiltros">Barragens dispensadas de licenciamento ambiental</legend>

	<div class="dataGrid">
		<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th width="35%">Requerimento</th>
						<th width="35%">Título</th>
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
						<td title="<%=Html.Encode(item.RequerimentoId)%>"><%=Html.Encode(item.RequerimentoId)%></td>
						<td title="<%=Html.Encode(item.TituloNumero)%>"><%=Html.Encode(item.TituloNumero)%></td>
						<td title="<%=Html.Encode(item.Atividade)%>"><%=Html.Encode(item.Atividade)%></td>
						<td title="<%=Html.Encode(item.areaAlagada)%>"><%=Html.Encode(item.areaAlagada)%></td>
						<td title="<%=Html.Encode(item.volumeArmazanado)%>"><%=Html.Encode(item.volumeArmazanado)%></td>
						<td title="Valido">Válido</td>

						<td>
							<input type="hidden" class="hdnId" value="<%= item.Id %>" />
							<input type="hidden" class="hdnTid" value="<%= item.Tid %>" />
							<input type="button" title="Visualizar" class="icone visualizar btnVisualizar" />
							<input type="button" title="Excluir" class="icone excluir btnExcluir" />
						</td>
					</tr>
					<%	} %>
				</tbody>
		</table>
	</div>
	
</fieldset>