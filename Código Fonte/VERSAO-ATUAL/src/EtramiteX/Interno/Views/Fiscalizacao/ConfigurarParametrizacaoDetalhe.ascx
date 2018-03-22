<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="block dataGrid"> 
  <div class="coluna50"> 
    <table class="dataGridTable tabDetalhe" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all"> 
      <thead> 
        <tr> 
          <th width="25%">Valor Inicial (R$)</th> 
          <th width="25%">Valor Final (R$)</th> 
          <th width="25%">Nº Máx. Parcelas</th>
			<% if (!Model.IsVisualizar) {%>
				<th width="20%">Ações</th>
			<%} %>
        </tr>
      </thead>
       
      <tbody>
          <% foreach (var detalhe in Model.Entidade.ParametrizacaoDetalhes){ %>
            <tr> 
              <td> 
                <span class="valorInicial" title="<%:detalhe.ValorInicial%>"><%:String.Format("{0:N2}", detalhe.ValorInicial)%></span> 
              </td> 
              <td> 
                <span class="valorFinal" title="<%:detalhe.ValorFinal%>"><%:String.Format("{0:N2}", detalhe.ValorFinal)%></span> 
              </td>
			  <td> 
                <span class="maximoParcelas" title="<%:detalhe.MaximoParcelas%>"><%:detalhe.MaximoParcelas%></span> 
              </td>
			<% if (!Model.IsVisualizar) {%>
				  <td class="tdAcoes"> 
					<input type="hidden" class="hdnDetalheJSon" value='<%: ViewModelHelper.Json(detalhe)%>' /> 
					<input type="hidden" value="<%= detalhe.Id %>" class="DetalheId" /> 
 
					<input title="Editar Detalhe" type="button" class="icone editar btnEditarDetalhe" value="" /> 
					<input title="Excluir Detalhe" type="button" class="icone excluir btnExcluirDetalhe" value="" /> 
				  </td> 
			<%} %>
            </tr> 
          <%} %> 
 
        <tr class="trTemplateRow hide"> 
          <td><span class="valorInicial" title=""></span></td> 
          <td><span class="valorFinal" title=""></span></td> 
          <td><span class="maximoParcelas" title=""></span></td> 
          <td class="tdAcoes"> 
                <input type="hidden" value="" class="hdnDetalheJSon" /> 
                <input type="hidden" value="" class="DetalheId" /> 
 
                <input title="Editar Detalhe" type="button" class="icone editar btnEditarDetalhe" disabled="disabled" value="" />
                <input title="Excluir Detalhe" type="button" class="icone excluir btnExcluirDetalhe" value="" /> 
          </td>
        </tr>
      </tbody> 
    </table> 
  </div> 
</div>