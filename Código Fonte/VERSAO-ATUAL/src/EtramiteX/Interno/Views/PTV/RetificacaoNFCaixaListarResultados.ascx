<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPTV" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RetificacaoNFCaixaVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>
    

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="30%">N° do Documento</th>
				<th>CPF/CNPJ</th>
				<th>Tipo de Caixa</th>
				<th>Saldo Inicial</th>
				<th>Saldo Atual</th>
				<th class="semOrdenacao" width="10%">Ações</th>
			</tr>
		</thead>

		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.NumeroNFCaixa)%>"><%= Html.Encode(item.NumeroNFCaixa)%></td>
				<td title="<%= Html.Encode(item.CPFCNPJ)%>"><%= Html.Encode(item.CPFCNPJ)%></td>
				<td title="<%= Html.Encode(item.TipoCaixaTexto)%>"><%= Html.Encode(item.TipoCaixaTexto)%></td>
				<td title="<%= Html.Encode(item.SaldoInicial)%>"><%= Html.Encode(item.SaldoInicial)%></td>
				<td title="<%= Html.Encode(item.SaldoAtual)%>"><%= Html.Encode(item.SaldoAtual)%></td>
				<td>
					<input type="hidden" class="itemJson" value="<%: ViewModelHelper.Json(new { id = item.Id, numeroNFCaixa = item.NumeroNFCaixa, PTV = item.idPTV, tipoPessoa = item.TipoPessoa, cpfCnpj = item.CPFCNPJ, tipoCaixa = item.TipoCaixa } ) %>" />
					<input type="button" title="Editar nota fiscal" class="icone editar btnEditar" />
					<input type="button" title="Excluir nota fiscal" class="icone excluir btnExcluir" />
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>