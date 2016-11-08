<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h1 class="titTela">Titulos</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="hdnIsAssociar" name="PodeAssociar" value="<%= Model.PodeAssociar %>" />
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlPdf", Url.Action("GerarPdf", "Titulo"), new { @class = "urlPdf" })%>
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>

		<div class="ultima">
			<div class="block fixado">
				<div class="coluna61">
					<label for="Filtros_Protocolo_NumeroTexto">Nº de registro</label>
					<%= Html.TextBox("Filtros.Protocolo.NumeroTexto", null, new { @class = "text setarFoco" })%>
				</div>
				<div class="coluna25 prepend1">
					<label for="Filtros_EmpreendimentoCodigo">Cód. do empreendimento</label>
					<%= Html.TextBox("Filtros.EmpreendimentoCodigo", null, new { @class = "text maskIntegerObrigatorio txtEmpreendimentoCodigo", @maxlength="13" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>
			<div class="block hide">
				<div class="block">
					<div class="coluna25">
						<label for="Filtros_Numero">Nº título</label>
						<%= Html.TextBox("Filtros.Numero", null, new { @class = "text txtEmpreedimento" })%>
					</div>
					<div class="coluna61 prepend1">
						<label for="Filtros_Modelo">Modelo</label>
						<%= Html.DropDownList("Filtros.Modelo", Model.Modelos, new { @class = "text" })%>
					</div>
				</div>
				<div class="block">
					<div class="coluna25">
						<label for="Filtros_Situacao">Situação</label>
						<%= Html.DropDownList("Filtros.Situacao", Model.Situacoes, new { @class = "text ddlSituacao" })%>
					</div>
					<div class="coluna61 prepend1">
						<label for="Filtros_Setor">Setor de cadastro</label>
						<%= Html.DropDownList("Filtros.Setor", Model.Setores, new { @class = "text ddlSetor" })%>
					</div>
				</div>
				<div class="block">
					<div class="coluna88">
						<label for="Filtros_Empreendimento">Empreendimento</label>
						<%= Html.TextBox("Filtros.Empreendimento", null, new { @class = "text txtEmpreedimento" })%>
					</div>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>