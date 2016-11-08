<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMRoteiro" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarItemVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0)?string.Empty:"hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="15%">Tipo</th>
				<th >Nome</th>
				<th >Condicionante</th>
				<th class="semOrdenacao" width="14%">Ações</th>
			</tr>
		</thead>
		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td class="itemRoteiroTipoTexto" title="<%= Html.Encode(item.TipoTexto)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.TipoTexto))%></td>
				<td class="itemRoteiroNome" title="<%= Html.Encode(item.Nome)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Nome))%></td>
				<td class="itemRoteiroCondicionante" title="<%= Html.Encode(item.Condicionante)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Condicionante))%></td>
				<td>
					<input type="hidden" value="<%= item.Id %>" class="hdnItemId" />
					<input type="hidden" value="<%= item.Nome %>" class="hdnItemNome" />
					<input type="hidden" value="<%= item.Condicionante %>" class="hdnItemCondicionante" />
					<input type="hidden" value="<%= item.Tipo %>" class="hdnItemtipo" /> 
					<input type="hidden" value="<%= item.TipoTexto %>" class="hdnItemtipoTexto" />
					<input type="hidden" value="<%= item.ProcedimentoAnalise %>" class="hdnItemProcedimento" />
					<input type="hidden" value="<%= item.Tid %>" class="hdnItemTid" />
					
					<%if (Model.PodeVisualizar) {%><button type="button" title="Visualizar" class="icone visualizar btnVisualizar"></button><% } %>
					<%if (Model.PodeAssociar) {%><button type="button" title="Associar" class="icone associar btnAssociar"></button><% } %>
					<%if (Model.PodeEditar) {%><button type="button" title="Editar" class="icone editar btnEditarItem"></button><% } %>
					<%if (Model.PodeExcluir) {%><button type="button" title="Excluir" class="icone excluir btnExcluirItem"></button><% } %>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>