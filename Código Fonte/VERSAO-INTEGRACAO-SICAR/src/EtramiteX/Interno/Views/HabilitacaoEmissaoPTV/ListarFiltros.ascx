<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMHabilitacaoEmissaoPTV" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h1 class="titTela">Habilitações de Emissão de PTV</h1>
<br />
<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>	
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>

		<div class="ultima">
			<div class="block fixado">
				<div class="coluna88">
					<label>Funcionário</label>
					<%= Html.TextBox("Filtros.Funcionario", null, new { @class = "text txtFuncionario"})%>
				</div>

				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna15 append1">
					<label for="CPF">CPF</label>
					<%= Html.TextBox("Filtros.CPF", null, new { @class = "text txtCpf maskCpf", @maxlength="11"} )%>
				</div>

				<div class="coluna40">
					<label>Nº da habilitação</label>
					<%= Html.TextBox("Filtros.NumeroHabilitacao", null, new { @class = "text txtNumeroHabilitacao"})%>
				</div>

			</div>

			<div class="block hide">
				<div class="coluna57">
					<label>Setor</label>
					<%= Html.DropDownList("Filtros.SetorId", Model.Setores, new { @class = "text" })%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>