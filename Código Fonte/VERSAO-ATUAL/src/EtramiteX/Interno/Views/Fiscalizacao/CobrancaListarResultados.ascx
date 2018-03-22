<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarCobrancasVM>" %>

<input type="hidden" class="paginaAtual" value="" />
<input type="hidden" class="numeroPaginas" value="<%= Model.Paginacao.NumeroPaginas %>" />

<%--//<%= ((Model.Paginacao.QuantidadeRegistros > 0) ? string.Empty : "hide") %> --%>
<div class="dataGrid ">
    <% Html.RenderPartial("Paginacao", Model.Paginacao); %>


    <table class="dataGridTable ordenavel" width="100%" border="0" cellspacing="0" cellpadding="0">

        <tr>
            <th width="10%">Parcela</th>
            <th width="10%">Nº DUA</th>
            <th width="10%">Nº Processo</th>
            <th width="10%">Nº AI/IUF</th>
            <th width="10%">Data Emissão</th>
            <th width="10%">Valor (R$)</th>
            <th width="10%">Valor Pago</th>
            <th width="10%">VRTE</th>
            <th width="10%">Data Pagamento</th>
            <th width="10%">Situação Parcela</th>
            <th width="10%">Ações</th>

        </tr>

        <% foreach (var item in Model.Resultados)
			{%>
        <tr>
			<td><%:item.Parcela%></td>
            <td><%:item.NumeroDUA%></td>
            <td><%:item.ProcNumero%></td>
            <td><%:item.iufNumero %></td>
            <td><%:item.DataEmissao.DataHoraTexto%></td>
            <td><%:item.ValorDUA%></td>
            <td><%:item.ValorPago%></td>
            <td><%:item.VRTE%></td>
            <td><%:item.DataPagamento.DataHoraTexto%></td>
            <td><%:item.Situacao%></td>
            <td>&nbsp;</td>
        </tr>
        <% };%>
    </table>


</div>
