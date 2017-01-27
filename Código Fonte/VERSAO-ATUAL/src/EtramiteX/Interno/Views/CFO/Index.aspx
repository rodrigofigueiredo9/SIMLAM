<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCFOCFOC.CFO" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">CFOs</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/CFO/listar.js") %>"></script>
	<script>
		$(function () {
			CFOListar.urlVisualizar = '<%= Url.Action("Visualizar", "CFO") %>';
			CFOListar.urlPDF = '<%= Url.Action("GerarPdf", "CFO") %>';
			CFOListar.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros", Model); %>
	</div>
</asp:Content>