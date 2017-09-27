<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<CodigosReceitaVM>" %> 
 
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Códigos da Receita</asp:Content> 
 
<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server"> 
  <script src="<%= Url.Content("~/Scripts/Fiscalizacao/fiscalizacaoCodigosReceita.js") %>" ></script> 
  <script> 
      $(function () { 
          ConfigurarCodigosReceita.load($('#central'), { 
              urls: { 
                  salvar: '<%= Url.Action("ConfigurarCodigosReceita", "Fiscalizacao") %>',
                  excluir: '<%= Url.Action("ExcluirCodigoReceita", "Fiscalizacao") %>'
              }, 
              Mensagens: <%= Model.Mensagens %>
          }); 
      }); 
  </script> 
</asp:Content> 
 
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server"> 
  <div id="central"> 
 
    <h1 class="titTela">Códigos da Receita</h1><br /> 
 
    <fieldset class="box">
      <div class="block"> 
        <div class="coluna10 append2"> 
          <label for="Item_CodigoReceita">Código da Receita</label> 
          <%= Html.TextBox("Codigo", String.Empty, new { @class = "text txtCodigo", @maxlength = "5" } )%> 
        </div> 
 
        <div class="coluna35 append1"> 
          <label for="Item_Descricao">Descrição</label> 
          <%= Html.TextBox("Descricao",String.Empty, new { @class = "text txtDescricao", @maxlength = "100"})%> 
        </div> 
 
        <div class="coluna7">
            <button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarCodigoReceita" title="Adicionar Produto">+</button> 
        </div> 
 
        <input type="hidden" class="hdnItemId" value='0' /> 
        <input type="hidden" class="hdnItemIsAtivo" value='true' /> 
      </div> 
 
      <div class="DivItens"> 
        <% Html.RenderPartial("FiscalizacaoCodigosReceita"); %> 
      </div> 
    </fieldset>

    <div class="block box"> 
      <input id="salvar" type="submit" value="Salvar" class="floatLeft btnSalvar"/> 
    </div> 
  </div> 
</asp:Content> 