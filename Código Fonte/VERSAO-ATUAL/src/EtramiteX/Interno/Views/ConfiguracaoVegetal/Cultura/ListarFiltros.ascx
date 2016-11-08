<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CulturaListarVM>" %>

<h1 class="titTela"><%= (Model.StraggCultivar ? "Culturas" : "Associar Cultivar") %> </h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<input type="hidden" class="hdnIsAssociar" name="Associar" value="<%= Model.Associar %>" />
		<input type="hidden" class="hdnStraggCultivar" name="StraggCultivar" value="<%= Model.StraggCultivar %>" />
		
		<%= Html.Hidden("UrlFiltrar", Url.Action("FiltrarCultura"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlEditar", Url.Action("EditarCultura"), new { @class = "urlEditar" })%>

		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>

		<div class="ultima">
			<div class="block fixado">
				<div class="coluna78 append1">
					<label for="Filtros_Cultura">Cultura</label>
					<%= Html.TextBox("Filtros.Cultura", null, new { @class = "text setarFoco", @maxlength="100" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>

			</div>
			<div class="block hide">
				<div class="coluna78 append1">
					<label for="Filtros_EmpreendimentoCodigo">Cultivar</label>
					<%= Html.TextBox("Filtros.Cultivar", null, new { @class = "text txtCultivar", @maxlength="100"})%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>
