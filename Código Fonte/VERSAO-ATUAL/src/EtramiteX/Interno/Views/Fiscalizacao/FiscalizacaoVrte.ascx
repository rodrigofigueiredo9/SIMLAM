<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="block dataGrid"> 
  <div class="coluna45"> 
    <table class="dataGridTable tabVrte" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all"> 
      <thead> 
        <tr> 
          <th width="15%">Ano</th> 
          <th width="33%">VRTE</th> 
          <th width="20%">Ações</th> 
        </tr>
      </thead>
       
      <tbody>
          <% foreach (var vrte in Model.ListaVrte){ %>
            <tr> 
              <td> 
                <span class="ano" title="<%:vrte.Ano%>"><%:vrte.Ano%></span> 
              </td> 
              <td> 
                <span class="vrte" title="<%:vrte.VrteEmReais%>"><%:vrte.VrteEmReais%></span> 
              </td> 
              <td class="tdAcoes"> 
                <input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(vrte)%>' /> 
                <input type="hidden" value="<%= vrte.Id %>" class="VrteId" /> 
 
                <input title="Editar VRTE" type="button" class="icone editar btnEditarVrte" value="" /> 
                <input title="Excluir VRTE" type="button" class="icone excluir btnExcluirVrte" value="" /> 
              </td> 
            </tr> 
          <%} %> 
 
        <tr class="trTemplateRow hide"> 
          <td><span class="ano" title=""></span></td> 
          <td><span class="vrte" title=""></span></td> 
          <td class="tdAcoes"> 
                <input type="hidden" value="" class="hdnItemJSon" /> 
                <input type="hidden" value="" class="VrteId" /> 
 
                <input title="Editar VRTE" type="button" class="icone editar btnEditarVrte" disabled="disabled" value="" />
                <input title="Excluir VRTE" type="button" class="icone excluir btnExcluirVrte" value="" /> 
          </td>
        </tr>
      </tbody> 
    </table> 
  </div> 
</div>