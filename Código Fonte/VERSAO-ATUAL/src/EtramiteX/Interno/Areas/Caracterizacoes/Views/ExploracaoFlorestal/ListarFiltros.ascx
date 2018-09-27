<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMExploracaoFlorestal" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h2 class="titTela"><%= Model.IsVisualizar ? "Visualizar ": "Editar " %>Exploração Florestal</h2>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("VisualizarExploracaoFlorestal"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("UrlEditar", Url.Action("EditarExploracaoFlorestal"), new { @class = "urlEditar" })%>
		<%= Html.Hidden("UrlExcluir", Url.Action("Excluir"), new { @class = "urlExcluir" })%>
		<%= Html.Hidden("UrlExcluirConfirm", Url.Action("ExcluirConfirm"), new { @class = "urlExcluirConfirm" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "0", new { @class = "ordenarPor" })%>

		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna15">
					<label for="Filtros_TipoExploracao">Tipo de Exploração</label>
					<%= Html.Hidden("Filtros.EmpreendimentoId", Model.Filtros.EmpreendimentoId, new { @class = "hdnEmpreendimento" })%>
					<%= Html.TextBox("Filtros.TipoExploracao", null, new { @class = "text txtTipoExploracao setarFoco", @maxlength = "3" })%>
				</div>
				<div class="coluna15 prepend2">
					<label for="Filtros_CodigoExploracao">Código Exploração</label>
					<%= Html.TextBox("Filtros.CodigoExploracao", null, new { @class = "text txtCodigoExploracao maskIntegerObrigatorio", @maxlength = "3" })%>
				</div>
					<div class="coluna15 prepend2">
					<label for="Filtros_DataExploracao">Data Exploração</label>
					<%= Html.TextBox("Filtros.DataExploracao", null, new { @class = "text maskData" })%>
				</div>
				<div class="coluna10 prepend2">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>