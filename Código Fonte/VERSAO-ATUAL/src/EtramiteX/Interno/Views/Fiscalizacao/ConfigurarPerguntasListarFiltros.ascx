<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PerguntaListarVM>" %>

<h1 class="titTela">Perguntas</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">

		<input type="hidden" class="hdnIsAssociar" name="PodeAssociar" value="<%= Model.PodeAssociar %>" />
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("ConfigurarPerguntasFiltrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("UrlVisualizarPdf", Url.Action("GerarPdf"), new { @class = "urlVisualizarPdf" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		
		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna75 append2">
					<label for="Filtros_PerguntaId">Pergunta</label>
					<%= Html.DropDownList("Filtros.PerguntaId", Model.PerguntaLst, new { @class = "text ddlPergunta" })%>
				</div>

				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna57 append2">
					<label for="Filtros_RespostaId">Resposta</label>
					<%= Html.DropDownList("Filtros.RespostaId", Model.RespostaLst, new { @class = "text ddlResposta" })%>
				</div>

				<div class="coluna24">
					<label for="Filtros_CodigoPergunta">Número da pergunta</label>
					<%= Html.TextBox("Filtros.CodigoPergunta", String.Empty, new { @class = "text txtCodigoPergunta maskNum15", @maxlength = "100" })%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>