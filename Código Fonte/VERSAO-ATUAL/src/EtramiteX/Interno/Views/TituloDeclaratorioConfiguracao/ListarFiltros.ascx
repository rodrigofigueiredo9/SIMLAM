<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTituloDeclaratorioConfiguracao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h1 class="titTela">Relatório de Alteração de Título Declaratório</h1>
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
				<div class="coluna18 append2">
					<label for="Filtros_NumeroTitulo">Nº título</label>
					<%= Html.TextBox("Filtros.NumeroTitulo", Model.Filtros.NumeroTitulo, new { @class = "text txtNumeroTitulo setarFoco" })%>
				</div>

				<div class="coluna18 append2">
					<label for="Filtros_Login">Login</label>
					<%= Html.TextBox("Filtros.Login", Model.Filtros.Login, new { @class = "text txtLogin" })%>
				</div>

				<div class="coluna30 append2">
					<label for="Filtros_NomeUsuario">Nome de usuário</label>
					<%= Html.TextBox("Filtros.NomeUsuario", Model.Filtros.NomeUsuario, new { @class = "text txtNomeUsuario" })%>
				</div>

				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna30 append2">
					<label for="Filtros_NomeInteressado">Nome do interessado</label>
					<%= Html.TextBox("Filtros.NomeInteressado", Model.Filtros.NomeInteressado, new { @class = "text txtNomeInteressado" })%>
				</div>

				<div class="coluna20 append2">
					<label for="Filtros_CpfCnpjInteressado"><%= Html.RadioButton("RadioCPFCNPJ", 1, true, new { @class = "radio radioCpfCnpjInteressado radioCPF" })%>CPF</label>
					<label for="Filtros_CpfCnpjInteressado"><%= Html.RadioButton("RadioCPFCNPJ", 2, false, new { @class = "radio radioCpfCnpjInteressado" })%>CNPJ do interessado</label>
					<%= Html.TextBox("Filtros.InteressadoCpfCnpj", Model.Filtros.InteressadoCpfCnpj, new { @class = "text txtCpfCnpj maskCpfParcial" })%>
				</div>

				<div class="coluna16">
					<label for="Filtros_DataSituacao_DataTexto">Data da situação atual</label>
					<%= Html.TextBox("Filtros.DataSituacaoAtual.DataTexto", Model.Filtros.DataSituacaoAtual.DataTexto, new { @class = "text txtDataSituacao maskData" })%>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna21 append2">
					<label for="Filtros_SituacaoTipo">Situação</label>
					<%= Html.DropDownList("Filtros.Situacao", Model.SituacaoTipo, new { @class = "text ddlSituacaoTipo" })%>
				</div>

				<div class="coluna23 append2">
					<label for="Filtros_IP">IP</label>
					<%= Html.TextBox("Filtros.IP", Model.Filtros.IP, new { @class = "text txtIP" })%>
				</div>
			</div>
		</div>
	</div>
</div>



<div class="gridContainer"></div>