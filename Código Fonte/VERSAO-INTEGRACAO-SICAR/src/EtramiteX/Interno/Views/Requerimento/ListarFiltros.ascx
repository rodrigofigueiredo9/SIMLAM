<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMRequerimento" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ListarVM>" %>

<h1 class="titTela">Requerimentos</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="hdnIsAssociar" name="PodeAssociar" value="<%= Model.PodeAssociar %>" />
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("UrlVisualizarPdf", Url.Action("GerarPdf"), new { @class = "urlVisualizarPdf" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		
		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna85">
					<label for="Filtros_Nome">Número</label>
					<%= Html.TextBox("Filtros.Numero", Model.Filtros.Numero > 0 ? Model.Filtros.Numero.ToString() : "", new { @class = "text txtNumeroReq maskNum15 setarFoco" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>
			<div class="block hide">
				<div class="coluna26">
					<label for="Filtros_InteressadoCpfCnpj"><%= Html.RadioButton("RadioCPFCNPJ", 1, true, new { @class = "radio radioInteressadoCpfCnpj radioCPF" })%>CPF</label>
					<label for="Filtros_InteressadoCpfCnpj"><%= Html.RadioButton("RadioCPFCNPJ", 2, false, new { @class = "radio radioInteressadoCpfCnpj" })%>CNPJ do interessado</label>
					<%= Html.TextBox("Filtros.InteressadoCpfCnpj", Model.Filtros.InteressadoCpfCnpj, new { @class = "text txtCpfCnpj maskCpfParcial" })%>
				</div>
				<div class="coluna58">
					<label for="Filtros_InteressadoNomeRazao">Nome/Razão social do interessado</label>
					<%= Html.TextBox("Filtros.InteressadoNomeRazao", Model.Filtros.InteressadoNomeRazao, new { @class = "text " })%>
				</div>
			</div>
			<div class="block hide">
				<div class="coluna26">
					<label for="Filtros_EmpreendimentoCnpj">CNPJ do empreendimento</label>
					<%= Html.TextBox("Filtros.EmpreendimentoCnpj", Model.Filtros.EmpreendimentoCnpj, new { @class = "text maskCnpjParcial" })%>
				</div>
				<div class="coluna58">
					<label for="Filtros-EmpreendimentoDenominador">Razão social/Denominação/Nome do empreendimento</label>
					<%= Html.TextBox("Filtros.EmpreendimentoDenominador", Model.Filtros.EmpreendimentoDenominador, new { @class = "text " })%>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>