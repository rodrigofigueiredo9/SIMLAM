<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMHabilitarEmissaoCFOCFOC" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Consultar Habilitação de Praga</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/HabilitarEmissaoCFOCFOC/listar.js") %>" ></script>
	<script type="text/javascript">
		$(function () {
		    HabilitarEmissaoCFOCFOC.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltrosConsultarPraga", Model); %>
	</div>
</asp:Content>