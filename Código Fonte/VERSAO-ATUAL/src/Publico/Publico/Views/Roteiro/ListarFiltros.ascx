<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMRoteiro" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<% Html.RenderPartial("Mensagem"); %>
<h1 class="titTela">Roteiros Orientativos</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro fAberto">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		
		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna85">
					<label for="Filtros_Atividade">Atividade</label>
					<%= Html.TextBox("Filtros.Atividade", null , new { @class = "text setarFoco" })%>
				</div>

				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block">
				<div class="coluna10">
					<label for="Filtros_Numero">Número</label>
					<%= Html.TextBox("Filtros.Numero", null, new { @class = "text", @maxlength = "4" })%>
				</div>

				<div class="coluna72 prepend2">
					<label for="Filtros_Nome">Nome</label>
					<%= Html.TextBox("Filtros.Nome", null, new { @class = "text txtNome", @maxlength = "150" })%>
				</div>
			</div>
			<div class="block">
				<div class="coluna46">
					<label for="Filtros_Setor">Setor</label>
					<%= Html.DropDownList("Filtros.Setor", Model.Setores, new { @class = "text" })%>
				</div>
				<div class="coluna36 prepend2">
					<label for="Filtros_PalavaChave">Palavra-chave</label>
					<%= Html.TextBox("Filtros.PalavaChave", Model.Filtros.PalavaChave, new { @class = "text", @maxlength = "50" })%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>