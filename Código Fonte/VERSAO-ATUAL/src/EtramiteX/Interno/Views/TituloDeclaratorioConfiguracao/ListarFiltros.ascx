<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTituloDeclaratorioConfiguracao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h1 class="titTela">Fiscalizações</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">

		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		
		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna16 append2">
					<label for="Filtros_NumeroTitulo">Nº título</label>
					<%= Html.TextBox("Filtros.NumeroTitulo", Model.Filtros.NumeroTitulo, new { @class = "text txtNumeroTitulo maskNum15 setarFoco" })%>
				</div>

				<div class="coluna15 append2">
					<label for="Filtros_Login">Login</label>
					<%= Html.TextBox("Filtros.Login", Model.Filtros.Login, new { @class = "text txtLogin" })%>
				</div>

				<div class="coluna15 append2">
					<label for="Filtros_NomeUsuario">Nome de usuário</label>
					<%= Html.TextBox("Filtros.NomeUsuario", Model.Filtros.NomeUsuario, new { @class = "text txtNomeUsuario maskNum15" })%>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna52 append2">
					<label for="Filtros_NomeInteressado">Nome do interessado</label>
					<%= Html.TextBox("Filtros.NomeInteressado", Model.Filtros.NomeInteressado, new { @class = "text txtNomeInteressado" })%>
				</div>

				<div class="coluna27">
					<label for="Filtros_CpfCnpjInteressado"><%= Html.RadioButton("RadioCPFCNPJ", 1, true, new { @class = "radio radioCpfCnpjInteressado radioCPF" })%>CPF interessado</label>
					<label for="Filtros_AutuadoCpfCnpj"><%= Html.RadioButton("RadioCPFCNPJ", 2, false, new { @class = "radio radioCpfCnpjInteressado" })%>CNPJ interessado</label>
					<%= Html.TextBox("Filtros.CpfCnpjInteressado", Model.Filtros.InteressadoCpfCnpj, new { @class = "text txtCpfCnpj maskCpfParcial" })%>
				</div>

				<div class="coluna20 append2">
					<label for="Filtros_DataSituacao_DataTexto">Data da situação atual</label>
					<%= Html.TextBox("Filtros.DataSituacao.DataTexto", Model.Filtros.DataSituacaoAtual.DataTexto, new { @class = "text txtDataSituacao maskData" })%>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna29 append2">
					<label for="Filtros_SituacaoTipo">Situação</label>
					<%= Html.DropDownList("Filtros.SituacaoTipo", Model.SituacaoTipo, new { @class = "text ddlSituacaoTipo" })%>
				</div>

				<div class="coluna27 append2">
					<label for="Filtros_IP">IP</label>
					<%= Html.TextBox("Filtros.IP", Model.Filtros.IP, new { @class = "text txtIP" })%>
				</div>
			</div>
		</div>
	</div>
</div>



<div class="gridContainer"></div>