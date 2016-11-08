<%@ Import Namespace="System.Security" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloTitulo" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TituloCondicionanteVM>" %>

<script type="text/javascript">
	TituloCondicionante.settings.urls = {
		adicionar: '<%= Url.Action("CondicionanteCriar", "Titulo", new { area = "" }) %>',
		editar: '<%= Url.Action("CondicionanteEditar", "Titulo", new { area = "" }) %>',
		visualizar: '<%= Url.Action("CondicionanteVisualizar", "Titulo", new { area = "" }) %>'
	};
	TituloCondicionante.settings.isVisualizar = <%= (!Model.MostrarBotoes).ToString().ToLower() %>;
</script>

<div class="block mostrarBotoes col20 <%= Model.MostrarBotoes ? "" : "hide" %>">
	<button type="button" title="Adicionar" class="floatRight inlineBotao botaoAdicionarIcone">Adicionar</button>
</div>

<div class="dataGrid">
	<table class="dataGridTable dgCondicionantes" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th>Descrição</th>
				<th width="15%">Situação</th>
				<th class="acoesCol" width="22%">Ações</th>
			</tr>
		</thead>
		<tbody>
			<% foreach (TituloCondicionante cond in Model.Condicionantes) { %>
			<tr>
				<td class="tdDescricao" title="<%: cond.Descricao %>"><%: cond.Descricao %></td>
				<td class="tdSituacao" title="<%: cond.Situacao.Texto %>"><%: cond.Situacao.Texto %></td>
				<td class="tdAcoes">
					<input type="hidden" class="hdnItemId" value="<%: cond.Id %>" />
					<input type="hidden" class="hdnItemJson" value="<%= SecurityElement.Escape(ViewModelHelper.JsSerializer.Serialize(cond)) %>" />
					<button type="button" title="Descer" class="icone abaixo mostrarBotoes btnDescer <%= Model.MostrarBotoes ? "" : "hide" %>" ></button>
					<button type="button" title="Subir" class="icone acima mostrarBotoes btnSubir <%= Model.MostrarBotoes ? "" : "hide" %>"></button>
					<button type="button" title="Editar" class="icone editar mostrarBotoes btnEditar <%= Model.MostrarBotoes ? "" : "hide" %>"></button>
					<button type="button" title="Visualizar" class="icone visualizar mostrarBotoes btnVisualizar"></button>
					<button type="button" title="Excluir" class="icone excluir mostrarBotoes btnExcluir <%= Model.MostrarBotoes ? "" : "hide" %>"></button>
				</td>
			</tr>
			<% } %>
			<tr class="trCondTemplate hide">
				<td class="tdDescricao" title="#DESCRICAO">#DESCRICAO</td>
				<td class="tdSituacao" title="#SITUACAO">#SITUACAO</td>
				<td class="tdAcoes">
					<input type="hidden" class="hdnItemId" value="#ITEMID" />
					<input type="hidden" class="hdnItemJson" value="#ITEMJSON" />
					<button type="button" title="Descer" class="icone abaixo mostrarBotoes btnDescer" ></button>
					<button type="button" title="Subir" class="icone acima mostrarBotoes btnSubir"></button>
					<button type="button" title="Editar" class="icone editar mostrarBotoes btnEditar"></button>
					<button type="button" title="Visualizar" class="icone visualizar mostrarBotoes btnVisualizar"></button>
					<button type="button" title="Excluir" class="icone excluir mostrarBotoes btnExcluir"></button>
				</td>
			</tr>
		</tbody>
	</table>
</div>