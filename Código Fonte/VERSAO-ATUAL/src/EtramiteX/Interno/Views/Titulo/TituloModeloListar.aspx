<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<TituloModeloListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Modelos de Títulos</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Titulo/tituloModeloListar.js") %>" ></script>
	<script>
		$(function () {
			TituloModeloListar.urlEditar = '<%= Url.Action("TituloModeloEditar", "Titulo") %>';
			TituloModeloListar.urlDesativarComfirm = '<%= Url.Action("DesativarConfirm", "Titulo") %>';
			TituloModeloListar.urlDesativar = '<%= Url.Action("Desativar", "Titulo") %>';
			TituloModeloListar.urlAtivar = '<%= Url.Action("Ativar", "Titulo") %>';
			TituloModeloListar.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("TituloModeloListarFiltros"); %>
	</div>
</asp:Content>