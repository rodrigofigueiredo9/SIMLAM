<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMLiberacaoCFOCFOC" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h1 class="titTela">Liberações de Números de CFO e CFOC</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlPdf", Url.Action("GerarPDF"), new { @class = "urlPdf" })%>
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		
		<div class="ultima">
			<div class="block fixado">
				<div class="coluna80 append1">
					<label for="Filtros_SolicitacaoNumero">Responsável técnico</label>
					<%= Html.TextBox("Filtros.ResponsavelNome", null, new { @class = "text setarFoco", maxlength = "80"})%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block hide">
				<div class="block">
					<div class="coluna25 append1">
						<label for="Filtros_DeclaranteNomeRazao">Tipo do número </label>
						<%= Html.DropDownList("Filtros.TipoNumero", Model.LstTipoNumero, new { @class = "text"})%>
					</div>

					<div class="coluna25 append1">
						<label for="Filtros_DeclaranteNomeRazao">Tipo do documento </label>
						<%= Html.DropDownList("Filtros.TipoDocumento", Model.LstTipoDocumento, new { @class = "text"})%>
					</div>

					<div class="coluna25">
						<label for="Filtros_EmpreendimentoDenominador">Número</label>
						<%= Html.TextBox("Filtros.Numero", null, new { @class = "text", maxlength = "30"})%>
					</div>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>