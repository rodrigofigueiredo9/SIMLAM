<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAtividade" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h2 class="titTela atividadeModal">Atividade Principal</h2>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtro</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("FiltrarAtividadeEmp"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>

		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna85">
					<label for="Filtros_AtividadeNome">Atividade</label>	
					<%= Html.TextBox("Filtros.AtividadeNome", null, new { @class = "text txtAtividadeNome setarFoco", @maxlength = "150" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>
			<div class="block coluna10 hide">
				<label for="Filtros_AtividadeNome">CNAE fiscal</label>	
				<%= Html.TextBox("Filtros.CNAE", null, new { @class = "text txtCNAE", @maxlength = "15" })%>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>