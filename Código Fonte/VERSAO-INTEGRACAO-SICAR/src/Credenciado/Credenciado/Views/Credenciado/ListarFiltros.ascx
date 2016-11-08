<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCredenciado" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h1 class="titTela">Credenciados</h1>
<br />

<div class="filtroExpansivo">
	<span id="titFiltro" class="titFiltro">Filtro</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<input type="hidden" class="hdnIsAssociar" name="IsAssociar" value="<%= Model.IsAssociar%>" />
		
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		
		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna63">
					<label for="Filtros.NomeRazaoSocial">
						Nome/Razão Social</label>
					<%= Html.TextBox("Filtros.NomeRazaoSocial", null, new { @class = "text setarFoco" })%>
				</div>
				<div class="coluna20">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>
			<div class="block hide">
				<div class="coluna20">
					<label for="Filtros_CpfCnpj"><%= Html.RadioButton("Filtros.IsCpf", true, true, new { @class = "radio radioPessoaCpfCnpj radioCPF" })%>CPF</label>
					<label for="Filtros_CpfCnpj"><%= Html.RadioButton("Filtros.IsCpf", false, false, new { @class = "radio radioPessoaCpfCnpj" })%>CNPJ</label>
					<%= Html.TextBox("Filtros.CpfCnpj", null, new { @class = "text txtCpfCnpj maskCpfParcial" })%>
				</div>
				<div class="coluna20 append5">
					<label for="Filtros.CpfCnpj">
						Tipo de Credenciado</label>
					<%= Html.DropDownList("Filtros.Tipo", Model.ListaTipoCred, new { @class = "text" })%>
				</div>
			</div>
			<div class="block hide">
				<div class="coluna20">
					<label for="Filtros.CpfCnpj">
						Situação</label>
					<%= Html.DropDownList("Filtros.Situacao", Model.ListaSituacaoCred, new { @class = "text" })%>
				</div>
				<div class="coluna20 append5">
					<label for="Filtros.CpfCnpj">
						Data Ativação</label>
					<%= Html.TextBox("Filtros.DataAtivacao", null, new { @class = "text maskData" })%>
				</div>
			</div>
		</div>

	</div>
</div>

<div class="gridContainer"></div>