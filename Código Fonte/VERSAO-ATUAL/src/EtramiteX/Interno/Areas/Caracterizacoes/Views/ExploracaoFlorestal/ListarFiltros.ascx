<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMExploracaoFlorestal" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h2 class="titTela">Visualizar Exploração Florestal</h2>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", null, new { @class = "ordenarPor" })%>

		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna28">
					<label for="Filtros_TipoAtividade">Tipo de Atividade</label>
					<%= Html.TextBox("Filtros.TipoAtividade", null, new { @class = "text txtTipoAtividade setarFoco", @maxlength = "3" })%>
				</div>
				<div class="coluna58 prepend1">
					<label for="Filtros_CodigoExploracao">Código Exploração</label>
					<%= Html.TextBox("Filtros.CodigoExploracao", null, new { @class = "text txtCodigoExploracao maskIntegerObrigatorio", @maxlength = "4" })%>
				</div>
					<div class="coluna28">
					<label for="Filtros_DataExploracao">Data Exploração</label>
					<%= Html.TextBox("Filtros.DataExploracao", null, new { @class = "text maskData" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>