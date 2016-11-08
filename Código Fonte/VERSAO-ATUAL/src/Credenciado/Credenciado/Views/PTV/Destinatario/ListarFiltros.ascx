<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPTV" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DestinatarioPTVListarVM>" %>

<h1 class="titTela"><%= (Model.Associar ? "Associar Destinatário PTV" : "Destinatários PTV") %> </h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<input type="hidden" class="hdnIsAssociar" name="Associar" value="<%= Model.Associar %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("DestinatarioFiltrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>

		<div class="ultima">
			<div class="block fixado">
				<div class="coluna78 append1">
					<label for="Filtros_Nome">Nome do destinatário</label>
					<%= Html.TextBox("Filtros.Nome", null, new { @class = "text setarFoco", @maxlength="100" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block hide">
				<div class="coluna20">
					<label for="Filtros_CPFCNPJ"><%= Html.RadioButton("Filtros.IsCPF", true, true, new { @class = "radio radioPessoaCpfCnpj radioCPF" })%>CPF</label>
					<label for="Filtros_CPFCNPJ"><%= Html.RadioButton("Filtros.IsCPF", false, false, new { @class = "radio radioPessoaCpfCnpj" })%>CNPJ</label>
					<%= Html.TextBox("Filtros.CPFCNPJ", null, new { @class = "text txtCpfCnpj maskCpfParcial" })%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>