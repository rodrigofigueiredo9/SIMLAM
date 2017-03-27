<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoDocumentoFitossanitario" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ConfiguracaoNumeracaoListarVM>" %>

<div class="filtroExpansivo">
    <span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
        <%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar", "ConfiguracaoDocumentoFitossanitario"), new { @class = "urlFiltrar" })%>

	    <div class="coluna20">
		    <label>Tipo do Documento*</label>
		    <%= Html.DropDownList("BlocoTipoDocumento", Model.TiposDocumento, new { @class = "text ddlBloco ddlTipoDocumento setarFoco" })%>
	    </div>
	    <div class="coluna20">
            <label>Tipo de Numeração*</label>
		    <%= Html.DropDownList("BlocoTipoNumeracao", Model.TiposNumeracao, new { @class = "text ddlBloco ddlTipoNumeracao"}) %>
	    </div>
	    <div class="coluna20">
            <label>Ano*</label>
		    <%= Html.TextBox("BlocoAno", null, new { @class = "text txtAno txtBloco maskNum4"} ) %>
	    </div>
	    <div class="coluna20">
		    <button class="inlineBotao btnBuscar">Buscar</button>
	    </div>
	</div>
</div>

<div class="gridContainer"></div>