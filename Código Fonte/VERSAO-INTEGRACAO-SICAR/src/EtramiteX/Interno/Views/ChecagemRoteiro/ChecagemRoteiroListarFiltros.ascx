<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMChecagemRoteiro" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarCheckListRoteiroVM>" %>

<h2 class="titTela">Checagens de Itens de Roteiro</h2>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="hdnIsAssociar" name="PodeAssociar" value="<%= Model.PodeAssociar %>" />
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlVisualizar", Url.Action("ChecagemRoteiroVisualizar"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizarPdf", Url.Action("ChecagemRoteiroPDF"), new { @class = "urlVisualizarPdf" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>

		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna85">
					<label for="Filtros_Id">Número</label>
					<%= Html.TextBox("Filtros.Id", Model.Filtros.Id, new { @class = "text txtNumero maskNumInt setarFoco", @maxlength = "37" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>
			<div class="block hide">
				<div class="coluna60">
					<label for="Filtros_NomeRoteiro">Nome do roteiro</label>
					<%= Html.TextBox("Filtros.NomeRoteiro", Model.Filtros.NomeRoteiro, new { @class = "text txtNome", @maxlength = "150" })%>
				</div>
				<div class="coluna21 prepend2">
					<label for="Filtros_SituacaoId">Situação</label>
					<%= Html.DropDownList("Filtros.SituacaoId", Model.ListaSituacao, new { @class = "text" })%>
				</div>
			</div>
			<div class="block hide">
				<div class="coluna85">
					<label for="Filtros_Interessado">Interessado</label>
					<%= Html.TextBox("Filtros.Interessado", Model.Filtros.Interessado, new { @class = "text", @maxlength = "80" })%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>