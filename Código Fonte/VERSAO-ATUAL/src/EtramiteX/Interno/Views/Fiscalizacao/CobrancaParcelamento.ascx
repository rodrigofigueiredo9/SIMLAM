<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="block dataGrid">
    <div class="coluna25">
        <table class="dataGridTable tabParcelas" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
            <thead>
                <tr>
                    <th width="10%">Parcela</th>
                    <th width="10%">N° DUA</th>
                    <th width="10%">Vencimento</th>
                    <th width="10%">Valor (R$)</th>
                    <th width="10%">Valor Pago (R$)</th>
                    <th width="10%">VRTE</th>
                    <th width="10%">Pagamento</th>
                    <th width="10%">Situação</th>
                    <th width="10%">Informações COmplementares</th>
                    <th width="10%">Ações</th>
                </tr>
            </thead>

            <tbody>
                <% foreach (var parcela in Model.DUAS)
					{ %>
                <tr>
                    <td>
                        <span class="parcela" title="<%:parcela.Parcela%>"><%:parcela.Parcela%></span>
                    </td>
                    <td>
                        <input class="numeroDUA maskNum8" value="<%:parcela.NumeroDUA%>" />
                    </td>
                    <td>
                        <input class="dataVencimento maskData" value="<%:parcela.DataVencimento%>" />
                    </td>
                    <td>
                        <span class="valorDUA" title="<%:parcela.ValorDUA%>"><%:parcela.ValorDUA%></span>
                    </td>
                    <td>
                        <input class="valorPago maskDecimal" value="<%:parcela.ValorPago%>" />
                    </td>
                    <td>
                        <span class="vrte" title="<%:parcela.VRTE%>"><%:parcela.VRTE%></span>
                    </td>
                    <td>
                        <input class="dataPagamento maskData" value="<%:parcela.DataPagamento%>" />
                    </td>
                    <td>
                        <span class="situacao" title="<%:parcela.Situacao%>"><%:parcela.Situacao%></span>
                    </td>
                    <td>
                        <input class="informacoesComplementares maskData" value="<%:parcela.InformacoesComplementares%>" />
                    </td>
                    <td class="tdAcoes">
                        <input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(parcela)%>' />
                        <input type="hidden" value="<%= parcela.Id %>" class="parcelaId" />

                        <input title="Adicionar Subparcela" type="button" class="icone adicionar btnAddSubparcela" value="" />
                    </td>
                </tr>
                <%} %>

                <tr class="trTemplateRow hide">
                    <td><span class="parcela" title=""></span></td>
                    <td><span class="numeroDUA" title=""></span></td>
                    <td><span class="dataVencimento" title=""></span></td>
                    <td><span class="valorDUA" title=""></span></td>
                    <td><span class="valorPago" title=""></span></td>
                    <td><span class="vrte" title=""></span></td>
                    <td><span class="dataPagamento" title=""></span></td>
                    <td><span class="situacao" title=""></span></td>
                    <td><span class="informacoesComplementares" title=""></span></td>
                    <td class="tdAcoes">
                        <input type="hidden" value="" class="hdnItemJSon" />
                        <input type="hidden" value="" class="parcelaId" />

                        <input title="Adicionar Subparcela" type="button" class="icone adicionar btnAddSubparcela" disabled="disabled" value="" />
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</div>
