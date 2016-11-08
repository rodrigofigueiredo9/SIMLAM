<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPessoa" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h1 class="titTela">Pessoas</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar", "Pessoa"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		
		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna85">
					<label for="Filtros_NomeRazaoSocial">Nome/Razão Social</label>
					<%= Html.TextBox("Filtros.NomeRazaoSocial", null, new { @class = "text txtNomeRazaoSocial setarFoco", maxlength = "80" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>
			<div class="block hide">
				<div class="coluna20">
					<label for="Filtros_CpfCnpj"><%= Html.RadioButton("Filtros.IsCpf", true, true, new { @class = "radio radioPessoaCpfCnpj radioCPF" })%>CPF</label>
					<label for="Filtros_CpfCnpj"><%= Html.RadioButton("Filtros.IsCpf", false, false, new { @class = "radio radioPessoaCpfCnpj" })%>CNPJ</label>
					<%= Html.TextBox("Filtros.CpfCnpj", null, new { @class = "text txtCpfCnpj maskCpfParcial" })%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>