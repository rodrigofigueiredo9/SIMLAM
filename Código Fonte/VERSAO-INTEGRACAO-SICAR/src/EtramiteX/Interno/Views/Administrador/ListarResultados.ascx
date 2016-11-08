<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAdministrador" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th>Nome</th>
				<th width="12%">CPF</th>
				<th width="15%">Login</th>
				<th width="12%">Situação</th>
				<th class="semOrdenacao" width="17%">Ações</th>
			</tr>
		</thead>
		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.Nome)%>"><%= Html.Encode(item.Nome)%></td>
				<td title="<%= Html.Encode(item.Cpf)%>"><%= Html.Encode(item.Cpf)%></td>
				<td title="<%= Html.Encode(item.Usuario.Login)%>"><%= Html.Encode(item.Usuario.Login)%></td>
				<td title="<%= Html.Encode(item.SituacaoTexto)%>"><%= Html.Encode(item.SituacaoTexto)%></td>
				<td>
					<input type="hidden" class="itemId" value="<%= item.Id %>" />

					<%if (Model.PodeVisualizar) {%><input type="button" title="Visualizar" class="icone visualizar btnVisualizar" /><% } %>
					<%if (Model.PodeEditar) { %>
						<input type="button" title="Editar" class="icone editar btnEditar" />
						<input type="button" title="Alterar situação" class="icone altStatus btnAlterarSituacao" />
					<% } %>

					<%if (!item.IsSistema) { %><input type="button" title="Transferir Usuário Sistema" class="icone promover_sistema btnPromoverParaSistema" /><% } %>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>