<%@ Import Namespace="Tecnomapas.EtramiteX.Gerencial.Areas.Relatorios.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PersonalizadoListarVM>" %>

<% Html.RenderPartial("Mensagem"); %>
<h1 class="titTela">Relatórios Personalizados</h1>
<br />

<div class="filtroExpansivo">
	<span class="titFiltroSimples">Filtros</span>
	<div class="filtroCorpo filtroSerializarAjax block">
		<input type="hidden" class="ultimaBusca" name="UltimaBusca" value="<%= Model.UltimaBusca %>" />
		<%= Html.Hidden("UrlFiltrar", Url.Action("Filtrar", "Personalizado"), new { @class = "urlFiltrar" })%>

		<div class="coluna98">
			<div class="block fixado">
				<div class="coluna43">
					<label for="Filtros_FonteDados_Id">Tipo do Relatório</label>
					<%= Html.DropDownList("Filtros.FonteDados.Id", Model.FonteDadosLst, new { @class = "text setarFoco" })%>
				</div>
				<div class="coluna43">
					<label for="Filtros_Nome">Título do Relatório</label>
					<%= Html.TextBox("Filtros.Nome", null, new { @class = "text", maxlength = "80" })%>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>
		</div>
	</div>
</div>

<div class="gridContainer"></div>