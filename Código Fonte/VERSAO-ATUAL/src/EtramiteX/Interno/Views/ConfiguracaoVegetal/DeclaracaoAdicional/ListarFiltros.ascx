<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DeclaracaoAdicionalListarVM>" %>

<h1 class="titTela">Declarações Adicionais</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		

		<%= Html.Hidden("UrlFiltrar", Url.Action("FiltrarDeclaracao"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlEditar", Url.Action(""), new { @class = "urlEditar" })%>
		
      <%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		
		<div class="ultima">
			<div class="block fixado">
				<div class="coluna80 append1">
					<label for="Filtros_Cientifico">Texto</label>
					<%= Html.TextBox("Filtros.Texto", null, new { @class = "text setarFoco", @maxlength="100" })%>
				</div>
                
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>
            <div class="block hide">
            <label for="OutroEstado">Outro Estado? </label><br />
		    <label>
			    <%=Html.RadioButton("Filtros.OutroEstado", 0, false , new { @class = "rdbOutroEstado radio"})%>
			    Não
		    </label>
		    <label>
			    <%=Html.RadioButton("Filtros.OutroEstado", 1,  false  , new { @class = "rdbOutroEstado radio"})%>
			    Sim
		    </label>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>