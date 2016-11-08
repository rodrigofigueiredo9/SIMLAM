<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProfissao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ProfissaoListarVM>" %>

<h1 class="titTela">Profissão</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltroSimples">Filtro</span>
	<div class="filtroCorpo filtroSerializarAjax block">		
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<input type="hidden" class="hdnIsAssociar" name="IsAssociar" value="<%= Model.IsAssociar %>" />

		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar", "Profissao"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar", "Profissao"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		
		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna85">
					<label for="Filtros_Nome">Profissão</label>
					<%= Html.TextBox("Filtros.Texto", Model.Filtros.Texto, new { @class = "text txtTexto setarFoco", maxlength = "250" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>