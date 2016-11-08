<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Especificidade" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DestinatarioEspecificidadeVM>" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script type="text/javascript">
	DestinatarioEspecificidade.settings.urls.obterDadosDestinatarioEspecificadade = '<%= Url.Action("ObterDestinatarioEspecificidade", "Titulo", new {area=""}) %>';
	DestinatarioEspecificidade.settings.mensagens = <%= Model.Mensagens %>;
</script>

<div class="block">
	<% if (!Model.IsVisualizar) { %>
		<div class="block">
			<div class="coluna75 append1">
				<label >Destinatários *</label>
				<%= Html.DropDownList("ddlDestinatarioEsp", Model.DestinatariosLst, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.DestinatariosLst.Count == 1, new { @class = "text ddlDestinatarioEsp" }))%>
			</div>
			<div class="coluna10">
				<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarDestinatariosEsp" title="Adicionar">+</button>
			</div>
		</div>
	<% } %>

	<div class="dataGrid">
		<table class="dataGridTable dgDestinatariosEsp" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
			<thead>
				<tr>
					<th>Destinatários</th>
					<%if (!Model.IsVisualizar){%><th width="9%">Ação</th><%} %>
				</tr>
			</thead>
			<% foreach (DestinatarioEspecificidade destinatario in Model.Destinatarios) { %>
			<tbody>
				<tr>
					<td>
						<span class="Destinatario" title="<%: destinatario.Nome %>"><%: destinatario.Nome %></span>
					</td>
					<%if (!Model.IsVisualizar){%>
						<td class="tdAcoes">
							<input type="hidden" class="hdnDestinatarioId" value='<%: destinatario.Id %>' />
							<input type="hidden" class="hdnDestinatarioJSon" value='<%: ViewModelHelper.Json(destinatario)%>' />
							<input title="Excluir" type="button" class="icone excluir btnExcluirDestinatarioEsp" value="" />
						</td>
					<%} %>
				</tr>
				<% } %>
				<% if(!Model.IsVisualizar) { %>
				<tr class="trTemplateRow hide">
					<td><span class="Destinatario"></span></td>
					<td class="tdAcoes">
						<input type="hidden" class="hdnDestinatarioId" value="" />
						<input type="hidden" class="hdnDestinatarioJSon" value="" />
						<input title="Excluir" type="button" class="icone excluir btnExcluirDestinatarioEsp" value="" />
					</td>
				</tr>
				<% } %>
			</tbody>
		</table>
	</div>
</div>