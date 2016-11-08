<%@ Import Namespace="Tecnomapas.EtramiteX.Gerencial.Areas.Relatorios.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Gerencial.Master" Inherits="System.Web.Mvc.ViewPage<PersonalizadoListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Relatórios Personalizados</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Relatorios/personalizadoListar.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			PersonalizadoListar.urlExecutar = '<%= Url.Action("Executar", "Personalizado") %>';
			PersonalizadoListar.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros"); %>
	</div>
</asp:Content>