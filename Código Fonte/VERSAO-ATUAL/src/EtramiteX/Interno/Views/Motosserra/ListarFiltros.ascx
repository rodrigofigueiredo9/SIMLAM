<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMMotosserra" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MotosserraListarVM>" %>

<h1 class="titTela">Motosserras</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="hdnIsAssociar" name="PodeAssociar" value="<%= Model.PodeAssociar %>" />
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar", "Motosserra"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar", "Motosserra"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("UrlVisualizarPessoa", Url.Action("PessoaModalVisualizar", "Pessoa"), new { @class = "urlVisualizarPessoa" })%>
		<%= Html.Hidden("UrlAssociarPessoa", Url.Action("PessoaModal", "Pessoa"), new { @class = "urlAssociarPessoa" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		
		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna25">
					<label for="Filtros_RegistroNumero">Nº Registro</label>
					<%= Html.TextBox("Filtros.RegistroNumero", Model.Filtros.RegistroNumero, new { @class = "text maskNumInt setarFoco", @maxlength = "7" })%>
				</div>

				<div class="coluna60 prepend2">
					<label for="Filtros_SerieNumero">Nº Fabricação/Série</label>
					<%= Html.TextBox("Filtros.SerieNumero", Model.Filtros.SerieNumero, new { @class = "text", @maxlength = "80" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna25 append2">
					<label for="Filtros_NotaFiscalNumero">Nº Nota fiscal</label>
					<%= Html.TextBox("Filtros.NotaFiscalNumero", Model.Filtros.NotaFiscalNumero, new { @class = "text", @maxlength = "80" })%>
				</div>

				<div class="coluna35 append2">
					<label for="Filtros_Modelo">Marca/Modelo</label>
					<%= Html.TextBox("Filtros.Modelo", Model.Filtros.Modelo, new { @class = "text", @maxlength = "12" })%>
				</div>

				<div class="coluna22">
					<label for="Filtros_Situacao">Situação</label>
					<%= Html.DropDownList("Filtros.Situacao", Model.Situacoes, new { @class = "text ddlSituacoes" })%>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna25">
					<label for="Filtros_PessoaCpfCnpj"><%= Html.RadioButton("Filtros.PessoaIsCnpj", true, !Model.Filtros.PessoaIsCnpj, new { @class = "radio radioCpfCnpj radioCPF" })%>CPF</label>
					<label for="Filtros_PessoaCpfCnpj"><%= Html.RadioButton("Filtros.PessoaIsCnpj", false, Model.Filtros.PessoaIsCnpj, new { @class = "radio radioCpfCnpj" })%>CNPJ</label>
					<%= Html.TextBox("Filtros.PessoaCpfCnpj", Model.Filtros.PessoaCpfCnpj, new { @class = "text txtCpfCnpj maskCpfParcial" })%>
				</div>

				<div class="coluna60 prepend2">
					<label for="Filtros_PessoaNomeRazao">Nome/Razão social</label>
					<%= Html.TextBox("Filtros.PessoaNomeRazao", Model.Filtros.PessoaNomeRazao, new { @class = "text" })%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>