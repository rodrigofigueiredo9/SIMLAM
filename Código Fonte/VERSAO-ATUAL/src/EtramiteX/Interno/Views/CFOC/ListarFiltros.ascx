<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCFOCFOC.CFOC" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h1 class="titTela">CFOCs</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		
		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna85">
					<label for="Filtros_Numero">Número CFOC</label>
					<%= Html.TextBox("Filtros.Numero", null, new { @class = "text setarFoco", @maxlength = "12" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block hide">
				<div class="block">
					<div class="coluna52">
						<label for="Filtros_EmpreendimentoTexto">Empreendimento</label>
						<%= Html.TextBox("Filtros.EmpreendimentoTexto", null, new { @class = "text", @maxlength = "80" })%>
					</div>
					<div class="coluna30 prepend1">
						<label for="Filtros_ProdutorTexto">Produtor</label>
						<%= Html.TextBox("Filtros.ProdutorTexto", null, new { @class = "text" })%>
					</div>
				</div>

				<div class="block">
					<div class="coluna52">
						<label for="Filtros_CulturaCultivar">Cultura/Cultivar</label>
						<%= Html.TextBox("Filtros.CulturaCultivar", null, new { @class = "text", @maxlength = "80" })%>
					</div>
					<div class="coluna30 prepend1">
						<label for="Filtros_SituacaoId">Situação</label>
						<%= Html.DropDownList("Filtros.SituacaoId", Model.SituacaoLista, new { @class = "text" })%> 
					</div>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>