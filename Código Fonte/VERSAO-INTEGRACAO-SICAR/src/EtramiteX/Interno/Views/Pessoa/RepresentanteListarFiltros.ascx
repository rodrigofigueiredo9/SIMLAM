<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPessoa" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h1 class="titTela">Representantes</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("RepresentanteFiltrar", "Pessoa"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		
		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna85">
					<label for="Filtros_NomeRazaoSocial">Nome</label>
					<%= Html.TextBox("Filtros.NomeRazaoSocial", null, new { @class = "text txtNome setarFoco" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>
			<div class="block hide">
				<div class="coluna20">
					<label for="Filtros_CpfCnpj">CPF</label>
					<%= Html.Hidden("Filtros.IsCpf", true)%>
					<%= Html.TextBox("Filtros.CpfCnpj", null, new { @class = "text maskCpfParcial" })%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>