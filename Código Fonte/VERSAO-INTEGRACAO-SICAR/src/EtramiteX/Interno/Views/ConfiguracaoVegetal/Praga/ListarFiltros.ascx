<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PragaListarVM>" %>

<h1 class="titTela">Pragas</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<input type="hidden" class="hdnIsAssociar" name="PodeAssociar" value="<%= Model.PodeAssociar %>" />
		<input type="hidden" class="hdnIsAssociarPraga" name="Associar" value="<%= Model.Associar %>" />

		<%= Html.Hidden("UrlFiltrar", Url.Action("FiltrarPraga"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlEditar", Url.Action(""), new { @class = "urlEditar" })%>
		
      <%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		
		<div class="ultima">
			<div class="block fixado">
				<div class="coluna80 append1">
					<label for="Filtros_Cientifico">Nome Científico</label>
					<%= Html.TextBox("Filtros.NomeCientifico", null, new { @class = "text setarFoco", @maxlength="100" })%>
				</div>
                
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>
            <div class="block hide">
				<div class="coluna80 append1">
					<label for="Filtros_EmpreendimentoCodigo">Nome Comum</label>
					<%= Html.TextBox("Filtros.NomeComum", null, new { @class = "text", @maxlength="100"})%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>