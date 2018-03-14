<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="block dataGrid">
    <div class="coluna100">
        <table class="dataGridTable tabParcelas" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
            <thead>
                <tr>
                    <th width="10%">Parcela</th>
                    <th width="10%">N° DUA</th>
                    <th width="9%">Vencimento</th>
                    <th width="10%">Valor (R$)</th>
                    <th width="10%">Valor Pago (R$)</th>
                    <th width="8%">VRTE</th>
                    <th width="8%">Pagamento</th>
                    <th width="10%">Situação</th>
                    <th width="22%">Informações Complementares</th>
					<% if (!Model.IsVisualizar) { %>
						<th width="3%">Ações</th>
					<%} %>
                </tr>
            </thead>

            <tbody>
                <% foreach (var parcela in Model.Parcelamento.DUAS)
					{ %>
                <tr>
                    <td>
                        <span class="parcela" title="<%:parcela.Parcela%>"><%:parcela.Parcela%></span>
                    </td>
                    <td>
						<% if (Model.IsVisualizar) { %>
							<span class="numeroDUA" title="<%:parcela.NumeroDUA%>"><%:parcela.NumeroDUA%></span>
						<%} else {%>
							<input class="text numeroDUA maskNum10" value="<%:parcela.NumeroDUA%>" style="width: 100%;" />
						<%} %>
                    </td>
                    <td>
						<% if (Model.IsVisualizar || parcela.DataVencimento.IsValido) { %>
							<span class="dataVencimento" title="<%:parcela.DataVencimento.DataTexto%>"><%:parcela.DataVencimento.DataTexto%></span>
						<%} else {%>
							<input class="text dataVencimento maskData" value="<%:parcela.DataVencimento.DataTexto%>" style="width: 100%;" />
						<%} %>
                    </td>
                    <td>
                        <span class="valorDUA" title="<%:parcela.ValorDUA%>"><%: String.Format("{0:N}", parcela.ValorDUA) %></span>
                    </td>
                    <td>
						<% if (Model.IsVisualizar) { %>
							<span class="valorPago" title="<%:parcela.ValorPago%>"><%:parcela.ValorPago%></span>
						<%} else {%>
							<% string valorPago = parcela.ValorPago.ToString("N2"); %>
							<%= Html.TextBox("valorPago", valorPago, new { @class = "text maskDecimalPonto2 valorPago", @maxlength = "13", @width = "100%" })%> 
						<%} %>
                    </td>
                    <td>
                        <span class="vrte" title="<%:parcela.VRTE%>"><%: String.Format("{0:N4}", parcela.VRTE) %></span>
                    </td>
                    <td>
						<% if (Model.IsVisualizar) { %>
							<span class="dataPagamento" title="<%:parcela.DataPagamento.DataTexto%>"><%:parcela.DataPagamento.DataTexto%></span>
						<%} else {%>
							<input class="text dataPagamento maskData" value="<%:parcela.DataPagamento.DataTexto%>" style="width: 100%;" />
						<%} %>
                    </td>
                    <td>
                        <span class="situacao" title="<%:parcela.Situacao ?? ""%>"><%:parcela.Situacao ?? ""%></span>
                    </td>
                    <td>
						<% if (Model.IsVisualizar) { %>
							<span class="informacoesComplementares" title="<%:parcela.InformacoesComplementares ?? ""%>"><%:parcela.InformacoesComplementares ?? ""%></span>
						<%} else {%>
							<input class="text informacoesComplementares" value="<%:parcela.InformacoesComplementares ?? ""%>" style="width: 100%;" maxlength="100" />
						<%} %>
                    </td>
					<% if (!Model.IsVisualizar) { %>
						<td class="tdAcoes">
							<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(parcela)%>' />
							<input type="hidden" value="<%= parcela.Id %>" class="parcelaId" />

							<input title="Adicionar Subparcela" type="button" class="icone adicionar btnAddSubparcela" value="" />
						</td>
					<%} %>
                </tr>
                <%} %>

                <tr class="trTemplateRow hide">
                    <td><span class="parcela" title=""></span></td>
                    <td><span class="numeroDUA" title=""></span></td>
                    <td><input class="dataVencimento" title="" /></td>
                    <td><span class="valorDUA" title=""></span></td>
                    <td><span class="valorPago" title=""></span></td>
                    <td><span class="vrte" title=""></span></td>
                    <td><span class="dataPagamento" title=""></span></td>
                    <td><span class="situacao" title=""></span></td>
                    <td><span class="informacoesComplementares" title=""></span></td>
                    <td class="tdAcoes">
                        <input type="hidden" value="" class="hdnItemJSon" />
                        <input type="hidden" value="" class="parcelaId" />

                        <input title="Adicionar Subparcela" type="button" class="icone adicionar btnAddSubparcela" disabled value="" />
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</div>
