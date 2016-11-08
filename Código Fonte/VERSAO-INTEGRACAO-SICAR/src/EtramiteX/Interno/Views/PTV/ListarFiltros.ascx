<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PTVListarVM>" %>

<h1 class="titTela">PTV</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<input type="hidden" class="hdnIsAssociar" name="Associar" value="<%= Model.Associar %>" />
		<%= Html.Hidden("UrlPdf", Url.Action("GerarPdf", "PTV"), new { @class = "urlPdf" })%>
		<%= Html.Hidden("UrlVisualizar", Url.Action("Visualizar"), new { @class = "urlVisualizar" })%>
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>

		<div class="ultima">
			<div class="block fixado">
				<div class="coluna61">
					<label for="Filtros_Numero">Número PTV</label>
					<%= Html.TextBox("Filtros.Numero", string.Empty, new { @class = "text setarFoco maskNumInt", @maxlength="10" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>
			<div class="block hide">
				<div class="block">
					<div class="coluna40">
						<label for="Filtros_Nome">Empreendimento</label>
						<%= Html.TextBox("Filtros.Empreendimento", string.Empty, new { @class = "text", @maxlength="100" })%>
					</div>
					<div class="coluna20">
						<label>Situação</label>
						<%=Html.DropDownList("Filtros.Situacao", Model.Situacoes, new { @class="text" }) %>
					</div>
				</div>
				<div class="block">
					<div class="coluna40">
						<label for="Filtros_Destinatario">Destinatário</label>
						<%= Html.TextBox("Filtros.Destinatario", string.Empty, new { @class = "text", @maxlength="100" })%>
					</div>
					<div class="coluna20">
						<label>Cultura/Cultivar</label>
						<%=Html.TextBox("Filtros.CulturaCultivar", string.Empty, new { @class="text ", @maxlength="100"})%>
					</div>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>
