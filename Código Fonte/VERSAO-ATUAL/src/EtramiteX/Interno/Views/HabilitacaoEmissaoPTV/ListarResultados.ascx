<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMHabilitacaoEmissaoPTV" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th>Funcionário</th>
				<th width="25%">Nº da habilitação</th>
				<th width="15%">Situação</th>
				<th width="16%" class="semOrdenacao">Ações</th>
			</tr>
		</thead>

		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td class="tdNumero" title="<%= Html.Encode(item.Funcionario)%>"><%= Html.Encode(item.Funcionario)%></td>
				<td title="<%= Html.Encode(item.NumeroHabilitacao)%>"><%= Html.Encode(item.NumeroHabilitacao)%></td>
				<td class="situacao" title="<%= Html.Encode(item.SituacaoTexto)%>"><%= Html.Encode(item.SituacaoTexto)%></td>
				<td>
					<input type="hidden" class="itemJson" value="<%: ViewModelHelper.Json(new { Id = item.Id}) %>" />
					<input type="button" title="Visualizar" class="icone visualizar btnVisualizar" />
					<input type="button" title="Editar" class="icone editar btnEditar" />
					<div class="<%=item.SituacaoTexto == "Ativo"? "":"hide" %>">
						<input type="button" title="Desativar" class="icone dispensado btnDesativar" />
					</div>
					<div class="<%=item.SituacaoTexto == "Ativo"? "hide":"" %>">
						<input type="button" title="Ativar" class="icone recebido btnAtivar" />
					</div>
					<input type="button" title="Gerar PDF" class="icone pdf btnGerarPDF" />
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>