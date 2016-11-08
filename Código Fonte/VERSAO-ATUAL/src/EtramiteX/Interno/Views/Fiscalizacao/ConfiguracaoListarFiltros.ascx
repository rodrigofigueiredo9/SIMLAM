<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ConfiguracaoListarVM>" %>

<h1 class="titTela">Configurações</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">

		<input type="hidden" class="hdnIsAssociar" name="PodeAssociar" value="<%= Model.PodeAssociar %>" />
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("ConfiguracaoFiltrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("ConfiguracaoVisualizar"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("UrlVisualizarPdf", Url.Action("GerarPdf"), new { @class = "urlVisualizarPdf" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		
		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna16 append2">
					<label for="Filtros_NumeroConfiguracao">Nº da configuração</label>
					<%= Html.TextBox("Filtros.NumeroConfiguracao", Model.Filtros.NumeroConfiguracao, new { @class = "text txtNumeroConfig maskNum15 setarFoco" })%>
				</div>

				<div class="coluna29 append2">
					<label for="Filtros_ClassificacaoId">Classificação</label>
					<%= Html.DropDownList("Filtros.ClassificacaoId", Model.Classificacao, new { @class = "text ddlClassificacao" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna76">	
					<label for="Filtros_TipoId">Tipo de infração</label><br />
					<%= Html.DropDownList("Filtros.TipoId", Model.Tipo, new { @class = "text ddlTipos" })%>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna76">	
					<label for="Filtros_ItemId">Item</label><br />
					<%= Html.DropDownList("Filtros.ItemId", Model.Item, new { @class = "text ddlItens" })%>
				</div>
			</div>	
		</div>
	</div>
</div>

<div class="gridContainer"></div>