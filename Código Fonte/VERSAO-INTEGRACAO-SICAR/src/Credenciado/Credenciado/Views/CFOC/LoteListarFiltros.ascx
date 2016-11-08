<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCFOCFOC.CFOC" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LoteListarVM>" %>

<h1 class="titTela">Lotes</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="hdnIsAssociar" name="PodeAssociar" value="<%= Model.PodeAssociar %>" />
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("LoteFiltrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("LoteVisualizar"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		<%= Html.Hidden("Filtros.EmpreendimentoId", Model.Filtros.EmpreendimentoId)%>

		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna88">
					<label for="Filtros_Numero">Número do lote</label>
					<%= Html.TextBox("Filtros.Numero", null, new { @class = "text maskNumInt setarFoco", @maxlength = "38" }) %>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block hide">
				<div class="block">
					<div class="coluna15">
						<label for="Filtros_ProdutorTexto">Data de criação</label>
						<%= Html.TextBox("Filtros.DataCriacao.DataTexto", null, new { @class = "text maskData" })%>
					</div>
					<div class="coluna50 prepend1">
						<label for="Filtros_Cultivar">Cultivar</label>
						<%= Html.TextBox("Filtros.CulturaCultivar", null, new { @class = "text", @maxlength = "80" })%>
					</div>
					<div class="coluna18 prepend1">
						<label for="Filtros_SituacaoId">Situação</label>
						<%= Html.DropDownList("Filtros.SituacaoId", Model.SituacaoLista, new { @class = "text" })%>
					</div>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>
