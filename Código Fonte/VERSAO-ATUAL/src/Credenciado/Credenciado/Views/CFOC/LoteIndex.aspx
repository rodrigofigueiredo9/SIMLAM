<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCFOCFOC.CFOC" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<LoteListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Lotes</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/CFOC/loteListar.js") %>"></script>
	<script type="text/javascript">
		$(function () {
			LoteListar.urlEditar = '<%= Url.Action("LoteEditar", "CFOC") %>';
			LoteListar.urlConfirmarExcluir = '<%= Url.Action("LoteExcluirConfirm", "CFOC") %>';
			LoteListar.urlExcluir = '<%= Url.Action("LoteExcluir", "CFOC") %>';
			LoteListar.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("LoteListarFiltros", Model); %>
	</div>
</asp:Content>
