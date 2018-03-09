<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="block dataGrid"> 
  <div class="coluna100"> 
    <table class="dataGridTable tabDua" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all"> 
      <thead> 
        <tr> 
          <th width="10%">N° DUA</th> 
          <th width="5%">Parcela</th> 
          <th width="10%">Data Emissão</th> 
          <th width="10%">Data Vencimento</th> 
          <th width="15%">Situação</th> 
          <th width="10%">Data Pagamento</th> 
          <th width="20%">Valor (R$)</th> 
          <th width="20%">Valor Pago (R$)</th> 
        </tr>
      </thead>
       
      <tbody>
          <% foreach (var parcela in Model.ListaCobranca){ %>
            <tr> 
              <td> 
                <span class="numeroDua" title="<%:parcela.NumeroDUA%>"><%:parcela.NumeroDUA%></span> 
              </td> 
              <td> 
                <span class="parcela" title="<%:parcela.Parcela%>"><%:parcela.Parcela%></span> 
              </td>     
              <td> 
                <span class="dataEmissao" title="<%:parcela.DataEmissao.DataTexto%>"><%:parcela.DataEmissao.DataTexto%></span> 
              </td> 
              <td> 
                <span class="dataVencimento" title="<%:parcela.DataVencimento.DataTexto%>"><%:parcela.DataVencimento.DataTexto%></span> 
              </td> 
              <td> 
                <span class="situacao" title="<%:parcela.Situacao%>"><%:parcela.Situacao%></span> 
              </td> 
              <td> 
                <span class="dataPagamento" title="<%:parcela.DataPagamento.DataTexto%>"><%:parcela.DataPagamento.DataTexto%></span> 
              </td> 
              <td> 
                <span class="valorDUA" title="<%:parcela.ValorDUA%>"><%:parcela.ValorDUA%></span> 
              </td> 
              <td> 
                <span class="valorPago" title="<%:parcela.ValorPago%>"><%:parcela.ValorPago%></span> 
              </td>          
            </tr> 
          <%} %> 
 
        <tr class="trTemplateRow hide"> 
          <td><span class="numeroDua" title=""></span></td> 
          <td><span class="parcela" title=""></span></td> 
          <td><span class="dataEmissao" title=""></span></td> 
          <td><span class="dataVencimento" title=""></span></td> 
          <td><span class="situacao" title=""></span></td> 
          <td><span class="dataPagamento" title=""></span></td> 
          <td><span class="valorDUA" title=""></span></td> 
          <td><span class="valorPago" title=""></span></td> 
        </tr>
      </tbody> 
    </table> 
  </div> 
</div>