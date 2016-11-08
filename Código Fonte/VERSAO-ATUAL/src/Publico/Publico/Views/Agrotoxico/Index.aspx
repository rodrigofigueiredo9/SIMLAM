<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMAgrotoxico" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Publico.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Consultar Agrotóxicos</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Agrotoxico/listar.js") %>" ></script>
	<script type="text/javascript">
		$(function () {
			AgrotoxicoListar.load($('#central'));
			AgrotoxicoListar.urlPDF = '<%= Url.Action("BaixarPdfAgrotoxico", "Agrotoxico") %>';
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros", Model); %>
	</div>
</asp:Content>