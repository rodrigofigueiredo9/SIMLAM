<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMHabilitarEmissaoCFOCFOC" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Publico.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Responsáveis Técnicos Habilitados para Emissão de CFO e CFOC
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/HabilitarEmissaoCFOCFOC/habilitarEmissaoCFOCFOCListar.js") %>"></script>
	<script type="text/javascript">
		$(function () {
			HabilitarEmissaoCFOCFOCListar.Mensagens = <%= Model.Mensagens %>;
			HabilitarEmissaoCFOCFOCListar.visualizarLink = '<%= Url.Action("VisualizarHabilitarEmissaoCFOCFOC", "HabilitarEmissaoCFOCFOC") %>';
			HabilitarEmissaoCFOCFOCListar.load($('#central'));
		});
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltrosHabilitarEmissaoCFOCFOC", Model); %>
	</div>
</asp:Content>