<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMOrgaosParceirosConveniados" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h1 class="titTela">Órgãos Parceiros/ Conveniados</h1>
<br />

<div class="block filtroExpansivo">
	<span class="titFiltroSimples">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar", "OrgaosParceirosConveniados"))%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>

		<div class="ultima">
			<div class="block fixado">
				<div class="coluna20 append1">
					<label for="Filtros_SolicitacaoNumero">Sigla</label>
					<%= Html.TextBox("Filtros.Sigla", null, new { @class = "text txtSigla setarFoco"})%>
				</div>

				<div class="coluna60 append1">
					<label for="Filtros_NomeOrgao">Nome do órgão</label>
					<%= Html.TextBox("Filtros.Nome", null, new { @class = "text txtNormaoOrgao"})%>
				</div>
                				
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block">
				<div class="coluna20 append1">
					<label for="Filtros_Situacoes">Situação</label>
					<%= Html.DropDownList("Filtros.SituacaoId", Model.ListaSituacoes, new { @class = "text ddlNovaSituacao" })%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>