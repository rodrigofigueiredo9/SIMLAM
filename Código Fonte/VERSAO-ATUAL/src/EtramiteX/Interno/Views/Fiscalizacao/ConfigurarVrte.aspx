<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<VrteVM>" %> 
 
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">VRTE</asp:Content> 
 
<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server"> 
  <script src="<%= Url.Content("~/Scripts/Fiscalizacao/fiscalizacaoVrte.js") %>" ></script> 
  <script> 
      $(function () { 
          ConfigurarVrte.load($('#central'), { 
              urls: { 
                  salvar: '<%= Url.Action("ConfigurarVrte", "Fiscalizacao") %>',
                  excluir: '<%= Url.Action("ExcluirVrte", "Fiscalizacao") %>'
              }, 
              Mensagens: <%= Model.Mensagens %>
          }); 
      }); 
  </script> 
</asp:Content> 
 
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server"> 
  <div id="central"> 
 
    <h1 class="titTela">VRTE</h1><br /> 
 
    <fieldset class="box">
      <div class="block"> 
        <div class="coluna8 append1"> 
          <label for="Vrte_Ano">Ano</label> 
          <%= Html.TextBox("Ano", String.Empty, new { @class = "text txtAno", @maxlength = "4" } )%> 
        </div> 
 
        <div class="coluna15 append2"> 
          <label for="Vrte_VrteEmReais">VRTE</label> 
          <%= Html.TextBox("Vrte",String.Empty, new { @class = "text maskDecimalPonto4 txtVrte", @maxlength = "17"})%> 
        </div> 
 
        <div class="coluna7">
            <button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarVrte" title="Adicionar VRTE">+</button> 
        </div> 
 
        <input type="hidden" class="hdnItemId" value='0' /> 
      </div> 
 
      <div class="DivItens"> 
        <% Html.RenderPartial("FiscalizacaoVrte"); %> 
      </div> 
    </fieldset>

    <div class="block box"> 
      <input id="salvar" type="submit" value="Salvar" class="floatLeft btnSalvar"/> 
    </div> 
  </div> 
</asp:Content> 