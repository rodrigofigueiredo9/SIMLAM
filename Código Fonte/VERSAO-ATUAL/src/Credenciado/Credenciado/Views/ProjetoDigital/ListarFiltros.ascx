<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProjetoDigital" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ProjetoDigitalListarVM>" %>

<h1 class="titTela">Projetos Digitais</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="hdnIsAssociar" name="PodeAssociar" value="<%= Model.PodeAssociar %>" />
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>
		
		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna85">
					<label for="Filtros_Requerimento">Nº do Requerimento Digital/ Projeto Digital</label>
					<%= Html.TextBox("Filtros.Requerimento", null, new { @class = "text maskNumInt setarFoco", @maxlength = "38" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>

			<div class="block hide">
				<div class="block">
					<div class="coluna30">
						<label for="Filtros_IsCpf"><%= Html.RadioButton("Filtros.IsCpf", true, true, new { @class = "radio radioInteressadoCpfCnpj radioCPF" })%>CPF</label>
						<label for="Filtros_IsCpf"><%= Html.RadioButton("Filtros.IsCpf", false, false, new { @class = "radio radioInteressadoCpfCnpj" })%>CNPJ do interessado</label>
						<%= Html.TextBox("Filtros.InteressadoCpfCnpj", null, new { @class = "text txtCpfCnpj maskCpfParcial" })%>
					</div>
					<div class="coluna52 prepend1">
						<label for="Filtros_Responsavel_NomeRazao">Nome/ Razão social do interessado</label>
						<%= Html.TextBox("Filtros.InteressadoNomeRazaoSocial", null, new { @class = "text", @maxlength = "80" })%>
					</div>
				</div>

				<div class="block">
					<div class="coluna30">
						<label for="Filtros_EmpreendimentoCnpj">CNPJ do empreendimento</label>
						<%= Html.TextBox("Filtros.EmpreendimentoCnpj", null, new { @class = "text maskCnpjParcial" })%>
					</div>
					<div class="coluna52 prepend1">
						<label for="Filtros_EmpreendimentoNomeRazaoSocial">Razão social/ Denominação/ Nome do empreendimento</label>
						<%= Html.TextBox("Filtros.EmpreendimentoNomeRazaoSocial", null, new { @class = "text setarFoco", @maxlength = "80" })%>
					</div>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>