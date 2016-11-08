<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMSetor" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h1 class="titTela">Setores</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		
		<div class="coluna98">
			<div class="block fixado">
			<div class="coluna30 append2">
					<label for="Filtros_Agrupador">Agrupador *</label>
					<%= Html.DropDownList("Filtros.Agrupador", Model.Agrupadores, new { @class = "ddlAgrupador" })%>
				</div>
				<div class="coluna50">
					<label for="Filtros_Setor">Setor *</label>
					<%= Html.DropDownList("Filtros.Setor", Model.Setores, new { @class = "ddlSetor"})%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna75">
					<label for="Filtros_Municipio">Município</label>
					<%= Html.DropDownList("Filtros.Municipio", Model.ListaMunicipios, new { @class = "ddlMunicipio" })%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>