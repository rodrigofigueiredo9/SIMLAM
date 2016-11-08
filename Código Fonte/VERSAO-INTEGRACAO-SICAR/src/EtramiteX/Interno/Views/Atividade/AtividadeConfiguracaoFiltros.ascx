<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAtividade" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarConfiguracaoVM>" %>

<h2 class="titTela">Configurações de Atividade Solicitada</h2>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar", "Atividade"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("ConfiguracaoVisualizar", "Atividade"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>

		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna85">
					<label for="Filtros_AtividadeSolicitada">Atividade solicitada</label>
					<%= Html.TextBox("Filtros.AtividadeSolicitada", Model.Filtros.AtividadeSolicitada, new { @class = "text txtAtividadeSolicitada", @maxlength = "500" })%>
				</div>
				<div class="coluna10">
					<input class="btnBuscar inlineBotao" type="button" value="Buscar" />
				</div>
			</div>
			<div class="block hide">
				<div class="coluna50">
					<label for="Filtros_NomeGrupo">Nome do grupo</label>
					<%= Html.TextBox("Filtros.NomeGrupo", Model.Filtros.NomeGrupo, new { @class = "text", @maxlength = "100" })%>
				</div>
				<div class="coluna32 prepend2">
					<label for="Filtros_Modelo">Modelo do título</label>
					<%= Html.DropDownList("Filtros.Modelo", Model.Modelos, new { @class = "text" })%>
				</div>
			</div>
			<div class="block hide">
				<div class="coluna50">
					<label for="Filtros_AtividadeNome">Setor da Atividade</label>	
					<%= Html.DropDownList("Filtros.SetorId", Model.Setores, new { @class = "text ddlSetores setarFoco" })%>
				</div>
				<div class="coluna32 prepend2">
					<label for="Filtros_AtividadeNome">Agrupador da Atividade</label>	
					<%= Html.DropDownList("Filtros.AgrupadorId", Model.Agrupadores, new { @class = "text ddlAgrupadores setarFoco" })%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>