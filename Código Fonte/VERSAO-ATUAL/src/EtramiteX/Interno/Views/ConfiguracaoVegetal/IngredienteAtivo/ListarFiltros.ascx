﻿<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IngredienteAtivoListarVM>" %>

<h1 class="titTela">Ingredientes Ativos</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="hdnIsAssociar" name="PodeAssociar" value="<%= Model.PodeAssociar %>" />
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("FiltrarIngredientesAtivos", "ConfiguracaoVegetal"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		
		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna88">
					<label for="Filtros_Texto">Ingrediente Ativo</label>
					<%= Html.TextBox("Filtros.Texto", Model.Filtros.Texto, new { @class = "text setarFoco", @maxlength = "100" })%>
				</div>

				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna35">
					<label for="Filtros_SituacaoId">Situação</label>
					<%= Html.DropDownList("Filtros.SituacaoId", Model.SituacoesLista, new { @class = "text" })%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>