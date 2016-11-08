<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TituloModeloListarVM>" %>

<h2 class="titTela">Modelos de Títulos</h2>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("TituloModeloFiltrar", "Titulo"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("TituloModeloVisualizar", "Titulo"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>

		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna85">
					<label for="Filtros_Nome">Nome</label>
					<%= Html.TextBox("Filtros.Nome", null, new { @class = "text txtNome setarFoco", @maxlength = "200" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block hide">
				<div class="block">
					<div class="coluna25">
						<label for="Filtros_Tipo">Tipo</label>
						<%= Html.DropDownList("Filtros.Tipo", Model.Tipos, new { @class = "text" })%>
					</div>
					<div class="coluna25 prepend2">
						<label for="Filtros_Situacao">Situação</label>
						<%= Html.DropDownList("Filtros.Situacao", Model.Situacoes, new { @class = "text" })%>
					</div>
				</div>
				<div class="block">
					<div class="coluna53">
						<label for="Filtros_Setor">Setor de cadastro</label>
						<%= Html.DropDownList("Filtros.Setor", Model.Setores, new { @class = "text" })%>
					</div>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>