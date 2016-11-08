<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAdministrador" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h2 class="titTela">Administradores</h2>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar", "Administrador"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>

		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna85">
					<label for="Filtros_Nome">Nome</label>
					<%= Html.TextBox("Filtros.Nome", Model.Filtros.Nome, new { @class = "text txtNome setarFoco" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna17">
					<label for="Filtros_Cpf">CPF</label>
					<%= Html.TextBox("Filtros.Cpf", Model.Filtros.Cpf, new { @class = "text maskCpfParcial" })%>
				</div>
				<div class="coluna27 prepend2">
					<label for="Filtros_Login">Login</label>
					<%= Html.TextBox("Filtros.Login", Model.Filtros.Login, new { @class = "text" })%>
				</div>
				<div class="coluna35 prepend2">
					<label for="Filtros_Situacao">Situação</label>
					<%= Html.DropDownList("Filtros.Situacao", Model.Situacoes, new { @class = "text" })%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>