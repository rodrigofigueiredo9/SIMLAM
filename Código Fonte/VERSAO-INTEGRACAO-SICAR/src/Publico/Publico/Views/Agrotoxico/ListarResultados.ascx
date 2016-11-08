<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMAgrotoxico" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="18%">Nº do cadastro</th>
				<th width="30%">Nome Comercial</th>
				<th>Titular do Registro</th>
				<th width="12%">Situação</th>
				<th class="semOrdenacao" width="7%">Ações</th>
			</tr>
		</thead>

		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td class="tdNumero" title="<%= Html.Encode(item.NumeroCadastro)%>"><%= Html.Encode(item.NumeroCadastro)%></td>
				<td title="<%= Html.Encode(item.NomeComercial)%>"><%= Html.Encode(item.NomeComercial)%></td>
				<td title="<%= Html.Encode(item.TitularRegistro)%>"><%= Html.Encode(item.TitularRegistro)%></td>
				<td title="<%= Html.Encode(item.Situacao)%>"><%= Html.Encode(item.Situacao)%></td>
				<td>
					<input type="hidden" class="itemJson" value="<%: ViewModelHelper.Json(new { Id = item.Id, InternoId = item.Id, ArquivoId = item.ArquivoId }) %>" />
					<% if (item.ArquivoId>0){ %><input type="button" title="PDF do Agrotóxico" class="icone pdf btnPDF" /><% } %>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>