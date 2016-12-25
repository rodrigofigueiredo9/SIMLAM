<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CondicionanteDescricaoListarVM>" %>

<script>
	$(function () {
		CondicionanteDescricaoListar.settings.urls = {
			criar: '<%= Url.Action("CondicionanteDescricaoCriar", "Titulo") %>',
			editar: '<%= Url.Action("CondicionanteDescricaoEditar", "Titulo") %>',
			excluir: '<%= Url.Action("CondicionanteDescricaoExcluir", "Titulo") %>',
			excluirSalvar: '<%= Url.Action("CondicionanteDescricaoExcluirSalvar", "Titulo") %>'
		};
	});
</script>

<h2 class="atividadeModal titTela">Descrições de Condicionantes</h2>

<div class="filtroExpansivo">
	<span class="titFiltroSimples">Filtro</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", null, new { @class = "ordenarPor" })%>
		<%= Html.Hidden("UrlFiltrar", Url.Action("CondicionanteDescricaoFiltrar"), new { @class = "urlFiltrar" })%>

		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna85">
					<label for="Filtro_Descricao">Descrição</label>
					<%= Html.TextBox("Filtros.Descricao", null, new { @class = "text setarFoco" })%>
				</div>
				<div class="coluna10">
					<input class="btnBuscar inlineBotao" type="button" value="Buscar" />
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>