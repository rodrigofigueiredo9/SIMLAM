<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoDocumentoFitossanitario" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ConfiguracaoNumeracaoListarVM>" %>

<div class="filtroExpansivo">
    <span class="titFiltroSimples">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
        <input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<input type="hidden" class="hdnIsAssociar" name="IsAssociar" value="<%= Model.IsAssociar %>" />

        <%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar", "ConfiguracaoDocumentoFitossanitario"), new { @class = "urlFiltrar" })%>
        <%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>

        <div class="coluna98">
            <div class="block fixado">
                <%= Html.Hidden("Filtros.EhConsolidado", Model.Filtros.EhConsolidado, new { @class = "text txtTipoBuscaConsolidado"} ) %>
	            <div class="coluna20">
                    <label>Ano*</label>
		            <%= Html.TextBox("Filtros.AnoConsolidado", Model.Filtros.AnoConsolidado, new { @class = "text txtAnoConsolidado txtBloco maskNum4"} ) %>
	            </div>
	            <div class="coluna20">
		            <button class="inlineBotao btnBuscar btnBuscarConsolidado">Buscar</button>
	            </div>
            </div>
        </div>
	</div>
</div>

<div class="gridContainerConsolidado"></div>