<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMRoteiro" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarItemVM>" %>

<h1 class="titTela">Itens de Roteiro</h1>
<br />

<script type="text/javascript">
	$(function () {
		ItemListar.urlCriarItem = '<%= Url.Action("ItemModal", "Roteiro") %>';
		ItemListar.urlEditar = '<%= Url.Action("EditarItem", "Roteiro") %>';
		ItemListar.urlComfirmExcluir = '<%= Url.Action("ItemComfirmExcluir", "Roteiro") %>';
		ItemListar.urlExcluir = '<%= Url.Action("ExcluirItem", "Roteiro") %>';
		ItemListar.urlValidarAbrirEditar = '<%= Url.Action("ValidarItemAbrirEditar", "Roteiro") %>';
		ItemListar.mensagens = <%= Model.Mensagens %>;
	});
</script>

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="hdnCriarItem" value="<%= Model.PodeCadastrar%>" />
		<input type="hidden" class="hdnIsAssociar" name="PodeAssociar" value="<%= Model.PodeAssociar %>" />
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlVisualizar", Url.Action("ItemVisualizar", "Roteiro"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("UrlFiltrar", Url.Action("FiltrarItem", "Roteiro"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>

		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna85">
					<label for="Filtros_Nome">Nome</label>
					<%= Html.TextBox("Filtros.Nome", null, new { @class = "text txtNome setarFoco" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>
			<div class="block hide">
				<div class="coluna30">
					<label for="Filtros_TipoId">Tipo</label>
					<%= Html.DropDownList("Filtros.TipoId", Model.ListaTipos, new { @class = "text" })%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>