<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProcesso" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="15%">Nº de registro</th>
				<th width="26%">Nome/Razão Social do Interessado</th>
				<th width="36%">Razão social/Denominação/Nome da propriedade/Imóvel</th>
				<th class="semOrdenacao" width="<%= (Model.PodeAssociar ? "10%" : "23%") %>">Ações</th>
			</tr>
		</thead>
		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.Numero)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Numero))%></td>
				<td title="<%= Html.Encode(item.Interessado.NomeRazaoSocial)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Interessado.NomeRazaoSocial))%></td>
				<td title="<%= Html.Encode(item.Empreendimento.Denominador)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Empreendimento.Denominador))%></td>
				<td>
					<input type="hidden" value="<%= item.Id %>" class="itemId" />
					<input type="hidden" value="<%= item.Numero %>" class="itemNumeroId" />
					<input type="hidden" value="<%= item.SituacaoId %>" class="itemSituacaoId" />

					<%if (Model.PodeAssociar){%><button type="button" class="icone associar btnAssociar" title="Associar"></button><% } %>
					<%if (Model.PodeVisualizar) {%><button type="button" class="icone visualizar btnVisualizar" title="Visualizar"></button><% } %>
					<%if (Model.PodeEditar) {%><button type="button" class="icone editar btnEditar" title="Editar" ></button><% } %>
					<%if (Model.PodeExcluir) {%><button type="button" title="Excluir" class="icone excluir btnExcluir"></button><% } %>

					<%if (Model.PodeEditar && !Model.PodeAssociar){%>
						<button type="button" title="Requerimentos adicionados" class="icone editarProcesso btnEditarApensadosJuntados"></button>
					<% } %>
					
					<%if (!Model.PodeAssociar){%>
						<button type="button" class="icone ativSolicitadas btnAtividadesSolicitadas" title="Atividades solicitadas" ></button>
						<button type="button" title="Consultar informação" class="icone informacoes btnConsultar"></button>
					<% } %>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>