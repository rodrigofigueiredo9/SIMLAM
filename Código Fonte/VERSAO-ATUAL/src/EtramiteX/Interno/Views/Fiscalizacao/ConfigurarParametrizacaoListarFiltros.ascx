<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ParametrizacaoListarVM>" %>

<h1 class="titTela">Parametrizações Financeiras</h1>
<br />

<div class="filtroExpansivo">
    <span class="titFiltro">Filtros</span>
    <div class="filtroCorpo filtroSerializarAjax block">

        <input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
        <%= Html.Hidden("UrlFiltrar", Url.Action("ConfigurarParametrizacaoFiltrar"), new { @class = "urlFiltrar" })%>
        <%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar"), new { @class = "urlVisualizar" })%>
        <%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
        <%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>

        <div class="coluna98">
            <div class="block fixado">
                <div class="coluna45 append2">
                    <label for="Filtros_CodigoReceitaId">Código Receita</label>
                    <%= Html.DropDownList("Filtros.CodigoReceitaId", Model.ParametrizacaoLst, new { @class = "text ddlCodigoReceita" })%>
                </div>

                <div class="coluna10">
                    <button class="inlineBotao btnBuscar">Buscar</button>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="gridContainer"></div>