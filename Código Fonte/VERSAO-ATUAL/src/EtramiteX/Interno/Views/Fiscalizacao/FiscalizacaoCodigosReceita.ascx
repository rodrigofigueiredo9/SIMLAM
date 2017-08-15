<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<div class="block dataGrid"> 
  <div class="coluna80"> 
    <table class="dataGridTable tabCodigosReceita" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all"> 
      <thead> 
        <tr> 
          <th width="20%">Código da Receita</th> 
          <th>Descrição</th> 
          <th width="25%">Ações</th> 
        </tr> 
      </thead> 
       
      <tbody>
          <% foreach (var codReceita in Model.ListaCodigosReceita){ %>
            <tr> 
              <td> 
                <span class="codigoReceita" title="<%:codReceita.Codigo%>"><%:codReceita.Codigo%></span> 
              </td> 
              <td> 
                <span class="descricao" title="<%:codReceita.Descricao%>"><%:codReceita.Descricao%></span> 
              </td> 
              <td class="tdAcoes"> 
                <input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(codReceita)%>' /> 
                <input type="hidden" value="<%= codReceita.Id %>" class="codigoReceitaId" /> 
                <input type="hidden" value="<%= codReceita.Ativo %>" class="codigoReceitaAtivo" /> 
 
                <input title="Editar código da receita" type="button" class="icone editar btnEditarCodigoReceita" value="" /> 
                <% if (codReceita.Ativo){ %> 
                    <input title="Desativar código da receita" type="button" class="icone cancelar btnDesativarCodigoReceita" value="" /> 
                    <input title="Ativar código da receita" type="button" class="icone recebido btnAtivarCodigoReceita" disabled="disabled" value="" /> 
                <% }else{ %> 
                    <input title="Desativar código da receita" type="button" class="icone cancelar btnDesativarCodigoReceita" disabled="disabled" value="" /> 
                    <input title="Ativar código da receita" type="button" class="icone recebido btnAtivarCodigoReceita" value="" /> 
                <% } %> 
                <input title="Excluir código da receita" type="button" class="icone excluir btnExcluirCodigoReceita" value="" /> 
              </td> 
            </tr> 
          <%} %> 
 
        <tr class="trTemplateRow hide"> 
          <td><span class="codigoReceita" title=""></span></td> 
          <td><span class="descricao" title=""></span></td> 
          <td class="tdAcoes"> 
                <input type="hidden" value="" class="hdnItemJSon" /> 
                <input type="hidden" value="" class="codigoReceitaId" /> 
                <input type="hidden" value="" class="codigoReceitaAtivo" /> 
 
                <input title="Editar código da receita" type="button" class="icone editar btnEditarCodigoReceita" disabled="disabled" value="" />
                <input title="Desativar código da receita" type="button" class="icone cancelar btnDesativarCodigoReceita" disabled="disabled" value="" /> 
                <input title="Ativar código da receita" type="button" class="icone recebido btnAtivarCodigoReceita" disabled="disabled" value="" /> 
                <input title="Excluir código da receita" type="button" class="icone excluir btnExcluirCodigoReceita" value="" /> 
          </td>
        </tr>
      </tbody> 
    </table> 
  </div> 
</div>