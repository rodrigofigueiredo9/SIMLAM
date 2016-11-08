<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PerguntaListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="5%">Nº</th>
				<th width="40%">Pergunta</th>
				<th width="25%">Resposta</th>
				<th  width="10%">Situação</th>
				<th class="semOrdenacao" width="20%">Ações</th>
			</tr>
		</thead>
		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.Id)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Id))%></td>
				<td title="<%= Html.Encode(item.Texto)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Texto))%></td>
				<td title="<%= Html.Encode(item.Resposta.Texto)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Resposta.Texto))%></td>
				<td title="<%= Html.Encode(item.SituacaoTipoTexto)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.SituacaoTipoTexto))%></td>
				<td>
					<input type="hidden" value="<%= item.Id %>" class="itemId" />
					<input type="hidden" value="<%= item.SituacaoTipoId %>" class="itemSituacao" />

					<button type="button" title="Visualizar" class="icone visualizar btnVisualizar"></button>
					<button type="button"title="Editar" class="icone editar btnEditar"></button>
					<input title="Desativar pergunta" type="button" class="icone cancelar btnDesativar" value="" />
					<input title="Ativar pergunta" type="button" class="icone recebido btnAtivar" value="" />
					<button type="button" title="Excluir" class="icone excluir btnExcluir"></button>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>