<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<RelatorioVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Relatório de Títulos Declaratórios</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/TituloDeclaratorio/tituloDeclaratorio.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Requerimento/requerimentoListar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/moment.min.js") %>"></script>

	<script>
		$(function () {
			TituloDeclaratorio.load($('#central'), {
				urls: {
					urlGerar: '<%= Url.Action("GerarRelatorio", "TituloDeclaratorio") %>'
				},
				Mensagens: <%= Model.Mensagens %>
			});

		});
	</script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Relatório Título Declaratório</h1>
		<br />
		<% Html.RenderPartial("RelatorioListarFiltros"); %>
	</div>
</asp:Content>