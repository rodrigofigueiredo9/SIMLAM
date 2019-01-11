<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMExploracaoFlorestal" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h2 class="titTela"><%= Model.Filtros.IsVisualizar ? "Visualizar ": "Editar " %>Exploração Florestal</h2>
<br />
<fieldset class="block box">
	<legend>Empreendimento</legend>
	<div class="block">
        <div class="coluna20 append1">
			<label>Código</label>
			<%= Html.TextBox("EmpreendimentoId", Model.Caracterizacao.Codigo, new { @class = "text cnpj disabled", @disabled = "disabled" })%>
		</div>
		<div class="coluna77">
			<label><%=Model.Caracterizacao.DenominadorTexto%> *</label>
			<%= Html.TextBox("DenominadorValor", Model.Caracterizacao.DenominadorValor, new { @maxlength = "100", @class = "text denominador disabled", @disabled = "disabled" })%>
		</div>
	</div>

	<div class="block">
		<div class="coluna20 append1">
			<label>Zona de localização *</label>
			<%= Html.DropDownList("Caracterizacao.SelecionarPrimeiroItem", Model.Caracterizacao.ZonaLocalizacao, new { disabled = "disabled", @class = "text disabled" })%>
		</div>
		<div class="coluna7 append1">
			<label>UF</label>
			<%= Html.DropDownList("Caracterizacao.SelecionarPrimeiroItem", Model.Caracterizacao.Uf, new { disabled = "disabled", @class = "text disabled" })%>
		</div>
		<div class="coluna45 append1">
			<label>Município</label>
			<%= Html.DropDownList("Caracterizacao.SelecionarPrimeiroItem", Model.Caracterizacao.Municipio, new { disabled = "disabled", @class = "text disabled" })%>
		</div>
        <div class="coluna20 ">
			<label>CNPJ</label>
			<%= Html.TextBox("CNPJ", Model.Caracterizacao.CNPJ, new { @maxlength = "100", @class = "text cnpj disabled", @disabled = "disabled" })%>
		</div>
	</div>
</fieldset>
<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("VisualizarExploracaoFlorestal"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("UrlEditar", Url.Action("EditarExploracaoFlorestal"), new { @class = "urlEditar" })%>
		<%= Html.Hidden("UrlExcluir", Url.Action("Excluir"), new { @class = "urlExcluir" })%>
		<%= Html.Hidden("UrlExcluirConfirm", Url.Action("ExcluirConfirm"), new { @class = "urlExcluirConfirm" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "0", new { @class = "ordenarPor" })%>

		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna15">
					<label for="Filtros_TipoExploracao">Tipo de Exploração</label>
					<%= Html.Hidden("Filtros.IsVisualizar", Model.Filtros.IsVisualizar)%>
					<%= Html.Hidden("Filtros.EmpreendimentoId", Model.Filtros.EmpreendimentoId, new { @class = "hdnEmpreendimento" })%>
					<%= Html.DropDownList("Filtros.TipoExploracao", Model.TipoExploracaoList, new { @class = "text txtTipoExploracao setarFoco" })%>
				</div>
				<div class="coluna15 prepend2">
					<label for="Filtros_CodigoExploracao">Código Exploração</label>
					<%= Html.TextBox("Filtros.CodigoExploracao", null, new { @class = "text txtCodigoExploracao maskIntegerObrigatorio", @maxlength = "3" })%>
				</div>
					<div class="coluna15 prepend2">
					<label for="Filtros_DataExploracao">Data Exploração</label>
					<%= Html.TextBox("Filtros.DataExploracao", null, new { @class = "text maskData" })%>
				</div>
				<div class="coluna10 prepend2">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>