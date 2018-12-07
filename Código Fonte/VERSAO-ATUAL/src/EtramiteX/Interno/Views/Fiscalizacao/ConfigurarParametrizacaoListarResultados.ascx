<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ParametrizacaoListarVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
	<% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
				<th width="10%">Código Receita</th>
				<th width="8%">Inicio Vigência</th>
				<th width="8%">Fim Vigência</th>
				<th width="12%">N° Máximo Parcelas</th>
				<th width="10%">Valor Mínimo PF</th>
				<th width="10%">Valor Mínimo PJ</th>
				<th width="6%">Multa (%)</th>
				<th width="7%">Juros (%)</th>
				<th width="9%">Desconto (%)</th>
				<th width="10%">Prazo p/ Desconto</th>
				<th class="semOrdenacao" width="10%">Ações</th>
			</tr>
		</thead>
		<tbody>
		<% foreach (var item in Model.Resultados) { %>
			<tr>
				<td title="<%= Html.Encode(item.CodigoReceitaTexto)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.CodigoReceitaTexto))%></td>
				<td title="<%= Html.Encode(item.InicioVigencia)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.InicioVigencia.ToShortDateString()))%></td>
				<td title="<%= Html.Encode(item.FimVigencia)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.FimVigencia != null ? item.FimVigencia.Value.ToShortDateString() : ""))%></td>
				<td title="<%= Html.Encode(item.MaximoParcelas)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.MaximoParcelas))%></td>
				<td title="<%= Html.Encode(item.ValorMinimoPF)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.ValorMinimoPF))%></td>
				<td title="<%= Html.Encode(item.ValorMinimoPJ)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.ValorMinimoPJ))%></td>
				<td title="<%= Html.Encode(item.MultaPercentual)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.MultaPercentual))%></td>
				<td title="<%= Html.Encode(item.JurosPercentual)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.JurosPercentual))%></td>
				<td title="<%= Html.Encode(item.DescontoPercentual)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.DescontoPercentual))%></td>
				<td title="<%= Html.Encode(item.PrazoDescontoUnidade)%>">
					<%= ViewModelHelper.CampoVazioListar(Html.Encode(item.PrazoDescontoUnidade))%>
					-<%= ViewModelHelper.CampoVazioListar(Html.Encode(item.PrazoDescontoDecorrencia == 1 ? "Dia" : (item.PrazoDescontoDecorrencia == 2 ? "Mês" : "Ano")))%>
				</td>
				<td>
					<input type="hidden" value="<%= item.Id %>" class="itemId" />

					<button type="button" title="Visualizar" class="icone visualizar btnVisualizar"></button>
					<button type="button" title="Editar" class="icone editar btnEditar"></button>
					<button type="button" title="Excluir" class="icone excluir btnExcluir"></button>
				</td>
			</tr>
		<% } %>
		</tbody>
	</table>
</div>