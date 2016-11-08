<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h1 class="titTela">Titulos Declaratórios</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="hdnIsAssociar" name="PodeAssociar" value="<%= Model.PodeAssociar %>" />
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlPdf", Url.Action("GerarPdf", "Titulo"), new { @class = "urlPdf" })%>
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>

		<div class="ultima">
			<div class="block fixado">
				<div class="coluna27">
					<label for="Filtros_Numero">Nº título</label>
					<%= Html.TextBox("Filtros.Numero", null, new { @class = "text", @maxlength="20" })%>
				</div>
				<div class="coluna29 prepend1">
					<label for="Filtros_RequerimentoID">Nº do requerimento / Projeto Digital</label>
					<%= Html.TextBox("Filtros.RequerimentoID", null, new { @class = "text txtEmpreedimento maskNumInt", @maxlength="20" })%>
				</div>
				<div class="coluna28 prepend1">
					<label for="Filtros_EmpreendimentoCodigo">Cód. do empreendimento</label>
					<%= Html.TextBox("Filtros.EmpreendimentoCodigo", null, new { @class = "text maskIntegerObrigatorio txtEmpreendimentoCodigo", @maxlength="13" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>
			<div class="block hide">
				<div class="block">
					<div class="coluna27">
						<label for="Filtros_DataEmisssao">Data de emissão do título</label>
						<%= Html.TextBox("Filtros.DataEmisssao", null, new { @class = "text maskData" })%>
					</div>
					<div class="coluna59 prepend1">
						<label for="Filtros_Modelo">Modelo</label>
						<%= Html.DropDownList("Filtros.Modelo", Model.Modelos, new { @class = "text" })%>
					</div>
				</div>

				<div class="block">
					<div class="coluna27">
						<label for="Filtros_InteressadoCPFCNPJ"><%= Html.RadioButton("RadioCPFCNPJ", 1, true, new { @class = "radio radioCpfCnpj radioCPF" })%>CPF</label>
						<label for="Filtros_InteressadoCPFCNPJ"><%= Html.RadioButton("RadioCPFCNPJ", 2, false, new { @class = "radio radioCpfCnpj" })%>CNPJ do interessado</label>
						<%= Html.TextBox("Filtros.InteressadoCPFCNPJ", Model.Filtros.InteressadoCPFCNPJ, new { @class = "text txtCpfCnpj maskCpfParcial" })%>
					</div>
					<div class="coluna59 prepend1">
						<label for="Filtros_InteressadoNomeRazao"> Nome/Razão social do interessado</label>
						<%= Html.TextBox("Filtros.InteressadoNomeRazao", null, new { @class = "text", @maxlength="80" })%>
					</div>
				</div>

				<div class="block">
					<div class="coluna50">
						<label for="Filtros_Empreendimento">Empreendimento</label>
						<%= Html.TextBox("Filtros.Empreendimento", null, new { @class = "text", @maxlength="80" })%>
					</div>

					<div class="coluna17 prepend1">
						<label for="Filtros_Situacao">Situação</label>
						<%= Html.DropDownList("Filtros.Situacao", Model.Situacoes, new { @class = "text" })%>
					</div>

					<div class="coluna17 prepend1">
						<label for="Filtros_OrigemID">Origem</label>
						<%= Html.DropDownList("Filtros.OrigemID", Model.Origens, new { @class = "text" })%>
					</div>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>