<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Relatorios.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<PersonalizadoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Editar Relatório Personalizado</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/Relatorios/personalizado.js") %>"></script>

    <script>
		$(function () {
			Personalizado.load($('#central'), {
			    mensagens: <%= Model.Mensagens %>,

				stepUrls: ['<%= Url.Action("ObterOpcoes", "Personalizado") %>', '<%= Url.Action("ObterOrdenarColunas", "Personalizado") %>',
				'<%= Url.Action("ObterOrdenarValores", "Personalizado") %>', '<%= Url.Action("ObterFiltros", "Personalizado") %>',
				'<%= Url.Action("ObterSumarizar", "Personalizado") %>', '<%= Url.Action("ObterDimensionar", "Personalizado") %>',
				'<%= Url.Action("ObterAgrupar", "Personalizado") %>', '<%= Url.Action("ObterFinalizar", "Personalizado") %>'],
				urls: {
					obterCamposFiltro: '<%= Url.Action("ObterCamposFiltro", "Personalizado") %>',
					validarFiltros: '<%= Url.Action("ValidarFiltros", "Personalizado") %>',
					finalizar: '<%= Url.Action("Finalizar", "Personalizado") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Editar Relatório Personalizado</h1>
		<br />

		<% Html.RenderPartial("Relatorio"); %>
	</div>
</asp:Content>