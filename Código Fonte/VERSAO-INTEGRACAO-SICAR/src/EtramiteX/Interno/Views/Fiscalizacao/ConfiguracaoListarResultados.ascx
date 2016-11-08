<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ConfiguracaoListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="10%">Número</th>
				<th width="17%">Classificação</th>
				<th width="18%">Tipo de infração</th>
				<th width="42%">Item</th>
				<th class="semOrdenacao" width="15%">Ações</th>
			</tr>
		</thead>
		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.Id)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Id))%></td>
				<td title="<%= Html.Encode(item.ClassificacaoTexto)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.ClassificacaoTexto))%></td>
				<td title="<%= Html.Encode(item.TipoTexto)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.TipoTexto))%></td>
				<td title="<%= Html.Encode(item.ItemTexto)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.ItemTexto))%></td>
				<td>
					<input type="hidden" value="<%= item.Id %>" class="itemId" />
					<input type="hidden" value="" class="itemSituacao" />
					<input type="hidden" value="" class="itemDataCriacao" />
					
					<%if (Model.PodeVisualizar) {%><button type="button" title="Visualizar" class="icone visualizar btnVisualizar"></button><% } %>
					<%if (Model.PodeEditar) {%><button type="button"title="Editar" class="icone editar btnEditar"></button><% } %>
					<%if (Model.PodeExcluir) {%><button type="button" title="Excluir" class="icone excluir btnExcluir"></button><% } %>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>