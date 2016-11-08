<%@ Import Namespace="Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCredenciado" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0)?string.Empty:"hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th>Nome/Razão Social</th>
				<th>CPF/CNPJ</th>
				<th>Tipo</th>
				<th>Situação</th>
				<th>Data de Ativação</th>
				<th class="semOrdenacao" width="13%">Ações</th>
			</tr>
		</thead>
		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td class="credenciadoNomeRazaoSocial" title="<%= Html.Encode(item.NomeRazaoSocial)%>"><%= Html.Encode(item.NomeRazaoSocial)%></td>
				<td class="credenciadoCpfCnpj" title="<%= Html.Encode(item.CpfCnpj)%>"><%= Html.Encode(item.CpfCnpj)%></td>
				<td class="credenciadoTipoTexto" title="<%: item.TipoTexto%>"><%: item.TipoTexto%></td>
				<td class="credenciadoSituacaoTexto" title="<%= item.SituacaoTexto%>"><%= item.SituacaoTexto%></td>
				<td class="credenciadoDataAtivacao" title="<%= Html.Encode(item.DataAtivacao)%>"><%= Html.Encode(item.DataAtivacao)%></td>
				<td>
					<input type="hidden" class="credenciadoId" value="<%= item.Id %>" />
					<input type="hidden" class="credenciadoTid" value="<%= item.Tid %>" />
					<%if (Model.IsAssociar) { %><input type="button" title="Associar" class="icone associar btnAssociar" /><%}%>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>