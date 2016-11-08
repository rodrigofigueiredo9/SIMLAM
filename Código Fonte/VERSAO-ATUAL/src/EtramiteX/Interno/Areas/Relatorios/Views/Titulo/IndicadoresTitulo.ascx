<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Tecnomapas.EtramiteX.Interno.Areas.Relatorios.ViewModels.IndicadoresVM>" %>

<br />
<div class="graficosCaixa">
	<div class="coluna49">
		<div class="titGrafico">
			<span>Vencimento de títulos</span>
		</div>
		<div class="coluna100">
			<div class="caixaIndicadores indicadoresTitulo">
				<% Html.RenderPartial("~/Areas/Relatorios/Views/Shared/IndicadorTituloDefault.ascx", Model.Titulos); %>
			</div>
		</div>
	</div>
	<div class="coluna50 ultima">
		<div class="titGrafico">
			<span>Vencimento de condicionantes de títulos</span>
		</div>
			<div class="caixaIndicadores indicadoresCondicionante">
				<% Html.RenderPartial("~/Areas/Relatorios/Views/Shared/IndicadorTituloDefault.ascx", Model.Condicionantes); %>
			</div>
	</div>
</div>