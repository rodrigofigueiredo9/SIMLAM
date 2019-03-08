<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RelatorioVM>" %>

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<%--<input type="hidden" class="hdnIsAssociar" name="PodeAssociar" value="<%= Model.PodeAssociar %>" />
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlPdf", Url.Action("GerarPdf", "Titulo"), new { @class = "urlPdf" })%>
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>--%>

		<div class="ultima">
			<div class="block fixado">
				<div class="block">
					<div class="coluna71">
						<label for="Filtros_OrigemID">Modelo</label>
						<%= Html.DropDownList("Filtros.OrigemID", Model.LstModelos, new { @class = "text filterModelo" })%>
					</div>
					<div class="coluna10">
						<button class="inlineBotao btnGerar">Buscar</button>
					</div>
				</div>
				<div class="block">
					<div class="coluna23">
						<label for="Filtros_Numero">Início período</label>
						<%= Html.TextBox("Filtros.Numero", null, new { @class = "text maskData filterInicioPeriodo", @maxlength="20" })%>
					</div>
					<div class="coluna23">
						<label for="Filtros_RequerimentoID">Fim período</label>
						<%= Html.TextBox("Filtros.RequerimentoID", null, new { @class = "text maskData filterFImPeriodo", @maxlength="20" })%>
					</div>
					<div class="coluna23">
						<label for="Filtros_EmpreendimentoCodigo">Nome/Razao social do interessado</label>
						<%= Html.TextBox("Filtros.EmpreendimentoCodigo", null, new { @class = "text filterNome", @maxlength="13" })%>
					</div>
				</div>
				<div class="block">
					<div class="coluna27">
						<label for="Filtros_InteressadoCPFCNPJ"><%= Html.RadioButton("RadioCPFCNPJ", 1, true, new { @class = "radio radioCpfCnpj radioCPF" })%>CPF</label>
						<label for="Filtros_InteressadoCPFCNPJ"><%= Html.RadioButton("RadioCPFCNPJ", 2, false, new { @class = "radio radioCpfCnpj" })%>CNPJ do interessado</label>
						<%= Html.TextBox("Filtros.InteressadoCPFCNPJ", null, new { @class = "text maskCpf filterCpfCnpj" })%>
					</div>
					<div class="coluna25">
						<label for="Filtros_OrigemID">Município</label>
						<%= Html.DropDownList("Filtros.OrigemID", Model.LstMunicipio, new { @class = "text filterMunicipio" })%>
					</div>
				</div>
				
			</div>
			
		</div>
	</div>
</div>

<div class="gridContainer"></div>