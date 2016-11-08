<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCredenciado" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h1 class="titTela">Habilitações para Emissão de CFO e CFOC</h1>
<br />

<div class="filtroExpansivo">
	<span id="titFiltro" class="titFiltro">Filtro</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("FiltrarHabilitarEmissaoCFOCFOC"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("VisualizarHabilitarEmissaoCFOCFOC"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>

		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna88">
					<div class="NomeRazaoContainer">
						<label for="Filtros.NomeRazaoSocial">Responsável técnico</label>
						<%= Html.TextBox("Filtros.NomeRazaoSocial", null, new { @class = "text txtNomeRazaoSocial setarFoco", @maxlength = "80" })%>
					</div>
				</div>
				<div class="coluna5">
					<button type="button" class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>
			<div class="block hide">
				<div class="coluna20">
					<label for="Filtros_CpfCnpj">CPF</label>
					<%= Html.TextBox("Filtros.CpfCnpj", null, new { @class = "text maskCpf maskCpfParcial" })%>
				</div>
				<div class="coluna30 prepend2">
					<label for="Filtros.NumeroHabilitacao">
						Nº da habilitação</label>
					<%= Html.TextBox("Filtros.NumeroHabilitacao", null, new { @class = "text txtNumeroHabilitacao", @maxlength = "30" })%>
				</div>
			</div>
			<div class="block hide">
				<div class="coluna88">
					<label for="Filtros.NomeComumPraga">
						Praga</label>
					<%= Html.TextBox("Filtros.NomeComumPraga", null, new { @class = "text txtPragaNome", @maxlength = "80" })%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>