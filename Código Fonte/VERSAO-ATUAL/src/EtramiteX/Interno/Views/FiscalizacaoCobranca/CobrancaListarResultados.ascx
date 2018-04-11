<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarCobrancasVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<div class="dataGrid <%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> ">
    <% Html.RenderPartial("Paginacao", Model.Paginacao); %>

	<table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">
        <thead>
            <tr>
                <th width="7%">Nº Processo</th>
                <th width="15%">Nome/Razão Social</th>
                <th width="7%">Nº AI/IUF</th>
                <th width="7%">Data Emissão</th>
                <th width="8%">Valor Multa (R$)</th>
                <th width="8%">Valor Atualizado (R$)</th>
                <th width="8%">Valor Pago (R$)</th>
                <th width="7%">Situação</th>
                <th class="semOrdenacao" width="7%">Ações</th>

            </tr>
        </thead>
        <tbody>
            <% foreach (var item in Model.Resultados)
				{%>
            <tr>
                <td title="<%= Html.Encode(item.ProcNumero)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.ProcNumero))%></td>
                <td title="<%= Html.Encode(item.NomeRazaoSocial)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.NomeRazaoSocial))%></td>
                <td title="<%= Html.Encode(item.NumeroIUF)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.NumeroIUF))%></td>
                <td title="<%= Html.Encode(item.DataEmissao.DataTexto)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.DataEmissao.DataTexto))%></td>
                <td title="<%= Html.Encode(String.Format("{0:N2}", item.ValorMulta))%>" style="text-align: right;"><%= ViewModelHelper.CampoVazioListar(Html.Encode(String.Format("{0:N2}", item.ValorMulta)))%></td>
                <td title="<%= Html.Encode(String.Format("{0:N2}", item.ValorMultaAtualizado))%>" style="text-align: right;"><%= ViewModelHelper.CampoVazioListar(Html.Encode(String.Format("{0:N2}", item.ValorMultaAtualizado)))%></td>
                <td title="<%= Html.Encode(String.Format("{0:N2}", item.ValorPago))%>" style="text-align: right;"><%= ViewModelHelper.CampoVazioListar(Html.Encode(String.Format("{0:N2}", item.ValorPago)))%></td>
                <td title="<%= Html.Encode(item.Situacao)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Situacao))%></td>
                <td>
					<input type="hidden" value="<%= item.Fiscalizacao %>" class="itemId" />

					<%if (Model.PodeVisualizar) {%><button type="button" title="Visualizar" class="icone visualizar btnVisualizar"></button><% } %>
					<%if (Model.PodeEditar) {%><button type="button"title="Editar" class="icone editar btnEditar"></button><% } %>
				</td>
            </tr>
            <% };%>
        </tbody>
    </table>
</div>
