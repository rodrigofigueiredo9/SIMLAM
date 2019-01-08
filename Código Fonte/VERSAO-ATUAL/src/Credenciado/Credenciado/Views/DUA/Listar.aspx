<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMDUA" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<DUAVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Dua</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/DUA/dua.js") %>"></script>

	<script>
		$(function () {
			
			Dua.urlGerarPDF = '<%= System.Configuration.ConfigurationManager.AppSettings["gerarPdfDua"].ToString() %>';
			Dua.urlReemitirDUA = '<%= Url.Action("ReemitirDua", "DUA") %>';
			Dua.load($('#central'));
			
		});

	</script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Emissão de DUA</h1>
		<br />
		<% Html.RenderPartial("EmitirDuaListar", Model); %>
	</div>
</asp:Content>