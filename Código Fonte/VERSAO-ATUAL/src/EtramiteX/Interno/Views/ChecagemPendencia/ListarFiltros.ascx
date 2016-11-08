<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMChecagemPendencia" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h2 class="titTela">Checagens de Pendência</h2>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="hdnIsAssociar" name="PodeAssociar" value="<%= Model.PodeAssociar %>" />
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar", "ChecagemPendencia"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar", "ChecagemPendencia"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>

		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna35">
					<label for="Filtros_Numero">Número da checagem de pendência</label>
					<%= Html.TextBox("Filtros.Numero", Model.Filtros.Numero > 0 ? Model.Filtros.Numero.ToString() : string.Empty, new { @class = "text txtNumero maskNum15 setarFoco" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block hide">
				<div class="block">
					<div class="coluna30">
						<label for="Filtros_SituacaoPendencia">Situação checagem pendência</label>
						<%= Html.DropDownList("Filtros.SituacaoPendencia", Model.LstSituacoesDeChecagem, new { @class = "text" })%>
					</div>
					<div class="coluna62 prepend2">
						<label for="Filtros_InteressadoNome">Interessado do título</label>
						<%= Html.TextBox("Filtros.InteressadoNome", Model.Filtros.InteressadoNome, new { @class = "text" })%>
					</div>
				</div>
				<div class="block">
					<div class="coluna20">
						<label for="Filtros_TituloNumero">Nº do título</label>
						<%= Html.TextBox("Filtros.TituloNumero", Model.Filtros.TituloNumero, new { @class = "text" })%>
					</div>
					<div class="coluna35 prepend2">
						<label>Nº registro processo/documento do título </label>
						<%= Html.TextBox("Filtros.Protocolo.NumeroTexto", null, new { @class = "text" })%>
					</div>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>