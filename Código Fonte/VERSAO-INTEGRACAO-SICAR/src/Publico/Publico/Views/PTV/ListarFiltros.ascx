<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMPTV" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PTVListarVM>" %>

<% Html.RenderPartial("Mensagem"); %>

<h2 class="titTela">PTVs e EPTVs</h2>
<br />

<div class="filtroExpansivo">
	<span class="titFiltro">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlPdf", Url.Action("GerarPdf", "PTV"), new { @class = "urlPdf" })%>
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar"), new { @class = "urlFiltrar" })%>
		<%= Html.Hidden("Paginacao.PaginaAtual", "1", new { @class = "paginaAtual" })%>
		<%= Html.Hidden("Paginacao.OrdenarPor", "1", new { @class = "ordenarPor" })%>

		<div class="ultima">
			<div class="block fixado">
				<div class="coluna60">
					<label for="Filtros_Numero">Número PTV</label>
					<%= Html.TextBox("Filtros.Numero", string.Empty, new { @class = "text setarFoco maskNumInt", @maxlength="10" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>
			<div class="block hide">
				<div class="block">
					<div class="coluna60">
						<label for="Filtros_Nome">Empreendimento</label>
						<%= Html.TextBox("Filtros.Empreendimento", string.Empty, new { @class = "text", @maxlength="100" })%>
					</div>
					<div class="coluna20">
						<label>Cultura/Cultivar</label>
						<%=Html.TextBox("Filtros.CulturaCultivar", string.Empty, new { @class="text ", @maxlength="100"})%>
					</div>
				</div>
				<div class="block">
					<div class="coluna60">
						<label for="Filtros_Destinatario">Destinatário</label>
						<%= Html.TextBox("Filtros.Destinatario", string.Empty, new { @class = "text", @maxlength="100" })%>
					</div>
					<div class="coluna20">
					</div>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>