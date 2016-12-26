<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DominialidadeVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/dominio.js") %>"></script>
<script>
	Dominios.settings.idsTela = <%= Model.IdsTela %>;
	Dominios.settings.urls.editar = '<%= Url.Action("Dominio", "Dominialidade") %>';
	Dominios.settings.urls.visualizar = '<%= Url.Action("DominioVisualizar", "Dominialidade") %>';
</script>

<div class="block coluna100 dataGrid">
	<table class="dataGridTable tabDominios" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
		<thead>
			<tr>
				<th>Identificação </th>
				<th>Tipo</th>
				<th>Matrícula / Comprovante</th>
				<th>Área croqui (m²)</th>
				<% if(Model.IsVisualizar) { %>
				<th width="7%">Ação</th>
				<% } else { %>
				<th width="10%">Ações</th>
				<% } %>
			</tr>
		</thead>
		<tbody>
			<% string matriculaComprovacao = string.Empty; %>
			<%foreach (var dominio in Model.Caracterizacao.Dominios) { %>
			<% matriculaComprovacao = (string.IsNullOrEmpty(dominio.Matricula)) ? dominio.ComprovacaoTexto : dominio.Matricula; %>
			<tr>
				<td>
					<span class="identificacao" title="<%:dominio.Identificacao%>"><%: dominio.Identificacao%></span>
				</td>
				<td>
					<span class="tipo" title="<%:dominio.TipoTexto%>"><%: dominio.TipoTexto%></span>
				</td>
				<td>
					<span class="matriculaComprovacao" title="<%:matriculaComprovacao%>"><%:matriculaComprovacao%></span>
				</td>
				<td>
					<span class="areaMatriculaCroqui" title="<%:dominio.AreaCroquiTexto%>"><%: dominio.AreaCroquiTexto %></span>
				</td>
				<td>
					<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(dominio)%>' />
					<button title="Visualizar" class="icone visualizar btnDominioVisualizar" type="button"></button>
					<% if(!Model.IsVisualizar) { %><button title="Editar" class="icone editar btnDominioEditar" type="button"></button><% } %>
				</td>
			</tr>
			<% }%>
		</tbody>
	</table>
</div>