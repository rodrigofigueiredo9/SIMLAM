<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMHabilitarEmissaoCFOCFOC" %>
<h1 class="titTela">Consultar Habilitação de Praga</h1>
<br/>
<div class="filtroExpansivo hide">
	<div class="filtroCorpo filtroSerializarAjax">
	    <input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
        <%= Html.Hidden("UrlFiltrar", Url.Action("FiltrarConsultarPraga"), new { @class = "urlFiltrar" })%>
	    <%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
	    <%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>	
    </div>
</div>
<div class="gridContainer"></div>


