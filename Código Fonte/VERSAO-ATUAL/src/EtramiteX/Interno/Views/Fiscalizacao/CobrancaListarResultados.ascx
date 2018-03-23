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
                <th width="10%">Parcela</th>
                <th width="7%">Nº DUA</th>
                <th width="10%">Nº Processo</th>
                <th width="7%">Nº AI/IUF</th>
                <th width="9%">Data Emissão</th>
                <th width="8%">Valor (R$)</th>
                <th width="8%">Valor Pago</th>
                <th width="10%">VRTE</th>
                <th width="11%">Data Pagamento</th>
                <th width="10%">Situação Parcela</th>
                <th class="semOrdenacao" width="7%">Ações</th>

            </tr>
        </thead>
        <tbody>
            <% foreach (var item in Model.Resultados)
				{%>
            <tr>
                <td title="<%= Html.Encode(item.Parcela)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.Parcela))%></td>
                <td title="<%= Html.Encode(item.NumeroDUA)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.NumeroDUA))%></td>
                <td title="<%= Html.Encode(item.ProcNumero)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.ProcNumero))%></td>
                <td title="<%= Html.Encode(item.iufNumero)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.iufNumero))%></td>
                <td title="<%= Html.Encode(item.DataEmissao.DataTexto)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.DataEmissao.DataTexto))%></td>
                <td title="<%= Html.Encode(item.ValorDUA)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.ValorDUA))%></td>
                <td title="<%= Html.Encode(item.ValorPago)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.ValorPago))%></td>
                <td title="<%= Html.Encode(item.VRTE)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.VRTE))%></td>
                <td title="<%= Html.Encode(item.DataPagamento.DataTexto)%>"><%= ViewModelHelper.CampoVazioListar(Html.Encode(item.DataPagamento.DataTexto))%></td>
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
