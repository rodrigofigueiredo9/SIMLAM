<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMRoteiro" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Publico.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Roteiros Orientativos</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Roteiro/listar.js") %>"></script>
	<script type="text/javascript">
		$(function () {
			RoteiroListar.urlPdfRoteiro = '<%= Url.Action("RelatorioRoteiro","Roteiro") %>';
			RoteiroListar.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros", Model); %>
	</div>
</asp:Content>