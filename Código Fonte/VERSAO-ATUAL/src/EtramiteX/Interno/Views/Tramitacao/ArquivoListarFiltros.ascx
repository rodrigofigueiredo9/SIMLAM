<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ArquivoListarVM>" %>

<h2 class="titTela">Arquivos</h2>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("ArquivoListar", "Tramitacao"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("ArquivoVisualizar", "Tramitacao"), new { @class = "urlVisualizar" })%>
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
				<div class="coluna50">
					<label for="Filtros_Setor">Setor</label>
					<%= Html.DropDownList("Filtros.Setor", Model.ListaSetores, new { @class = "text" })%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>