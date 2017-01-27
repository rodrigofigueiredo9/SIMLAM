<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Tecnomapas.EtramiteX.Interno.Areas.Relatorios.ViewModels.IndicadoresModalVM>" %>

<div class="modalIndicadoresArquivo">
<h3 class="inidicadoresModal"><%= Model.Titulo %></h3>

	<% using (Html.BeginForm("InidicadoresArquivo", "Indicadores", FormMethod.Get, new { area="Relatorios", @class = "formIndicadores" }))
	{ %>
	<div class="hide indicadoreBotoes">
		<p class="floatLeft append1"><button class="botaoGerarXLS" title="Gerar Excel">Gerar Excel</button></p>
		<p class="floatLeft"><button class="botaoGerarPDF" title="Gerar PDF">Gerar PDF</button></p>
	</div>

	<%= Html.Hidden("periodoIndice", Model.Periodo, new { @class = "indicadorPeriodo" })%>
	<%= Html.Hidden("tipo", Model.Tipo, new { @class = "indicadorTipo" })%>
	<%= Html.Hidden("extensao", null, new { @class = "indicadorExtensao" })%>

	<%} %>
</div>

<script src="<%= Url.Content("~/Scripts/Areas/Relatorios/IndicadoresModal.js") %>" ></script>