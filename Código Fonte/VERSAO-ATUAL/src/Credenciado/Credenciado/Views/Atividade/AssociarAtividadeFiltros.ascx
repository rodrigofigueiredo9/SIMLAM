<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMAtividade" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarAtividadeVM>" %>

<h2 class="titTela">Atividades Solicitadas</h2>
<br />

<script type="text/javascript">
	$(function () {
		AtividadeSolicitadaListar.defaults.atividadesSolicitadas = $.parseJSON('<%= Model.AtividadesSolicitadas %>');
	});
</script>

<div class="filtroExpansivo">
	<span class="titFiltro">Filtro</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("AssociarAtividade", "Atividade"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		
		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna85">
					<label for="Filtros_AtividadeNome">Nome</label>
					<%= Html.TextBox("Filtros.AtividadeNome", null, new { @class = "text txtNome setarFoco", @maxlength = "150" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>
			<div class="block coluna45 hide">
				<label for="Filtros_AtividadeNome">Setor</label>	
				<%= Html.DropDownList("Filtros.SetorId", Model.Setores, new { @class = "text ddlSetores" })%>
			</div>
			<div class="block coluna45 hide">
				<label for="Filtros_AtividadeNome">Agrupador</label>	
				<%= Html.DropDownList("Filtros.AgrupadorId", Model.Agrupadores, new { @class = "text ddlAgrupadores" })%>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>