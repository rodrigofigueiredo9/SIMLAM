<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTVOutro" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PTVOutroListarVM>" %>

<h1 class="titTela">PTVs de Outro Estado</h1>
<br />
<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>

		<div class="ultima">
			<div class="block fixado">
				<div class="coluna40">
					<label for="Filtros_Interessado">Interessado</label>
					<%= Html.TextBox("Filtros.Interessado", string.Empty, new { @class = "text setarFoco", @maxlength="120" })%>
				</div>
				<div class="coluna35 prepend1">
					<label for="Filtros_Destinatario">Destinatário</label>
					<%= Html.TextBox("Filtros.Destinatario", string.Empty, new { @class = "text", @maxlength="100" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>
			<div class="block hide">
				<div class="block">
					<div class="coluna18">
						<label for="Filtros_Numero">Nº PTV outro estado</label>
						<%= Html.TextBox("Filtros.Numero", string.Empty, new { @class = "text maskNumInt", @maxlength="10" })%>
					</div>
					<div class="coluna20 prepend1">
						<label>Cultura/Cultivar</label>
						<%=Html.TextBox("Filtros.CulturaCultivar", string.Empty, new { @class="text ", @maxlength="100"})%>
					</div>
					<div class="coluna35 prepend1">
						<label for="Filtros_ResponsavelTecnico">Responsável técnico</label>
						<%= Html.TextBox("Filtros.ResponsavelTecnico", string.Empty, new { @class = "text", @maxlength="100" })%>
					</div>
					<div class="coluna17 prepend1">
						<label for="Filtros_Situacao">Situação</label>
						<%=Html.DropDownList("Filtros.Situacao", Model.Situacoes, new { @class="text" }) %>
					</div>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>
