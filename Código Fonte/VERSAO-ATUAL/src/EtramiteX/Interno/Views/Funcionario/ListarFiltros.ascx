<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFuncionario" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h2 class="titTela">Funcionários</h2>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="hdnIsAssociar" name="PodeAssociar" value="<%= Model.PodeAssociar %>" />
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar", "Funcionario"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		
		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna85">
					<label for="Filtros_Nome">Nome</label>
					<%= Html.TextBox("Filtros.Nome", string.Empty, new { @class = "text txtNome setarFoco", maxlength = "80" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna49">
					<label for="Filtros_Setor">Setor</label>
					<%= Html.DropDownList("Filtros.Setor", Model.Setores, new { @class = "text" })%>
				</div>
				<div class="coluna33 prepend2">
					<label for="Filtros_Login">Login</label>
					<%= Html.TextBox("Filtros.Login", string.Empty, new { @class = "text", maxlength = "30" })%>
				</div>
			</div>
			<div class="block hide">
				<div class="coluna44">
					<label for="Filtros_Cargo">Função</label>
					<%= Html.DropDownList("Filtros.Cargo", Model.Cargos, new { @class = "text" })%>
				</div>
				<div class="coluna15 prepend2">
					<label for="Filtros_Cpf">CPF</label>
					<%= Html.TextBox("Filtros.Cpf", string.Empty, new { @class = "text maskCpfParcial" })%>
				</div>
				<div class="coluna20 prepend2">
					<label for="Filtros_Situacao">Situação</label>
					<%= Html.DropDownList("Filtros.Situacao", Model.Situacoes, new { @class = "text" })%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>