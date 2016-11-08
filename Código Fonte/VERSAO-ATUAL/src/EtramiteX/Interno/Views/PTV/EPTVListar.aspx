<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<PTVListarVM>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">EPTVs</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/PTV/listarEPTV.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/PTV/comunicadorptv.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			EPTVListar.load($('#central'), {
				urls: {
					urlAnalisar: '<%= Url.Action("EPTVAnalisar", "PTV") %>',
					urlValidarAcessoComunicador: '<%= Url.Action("ValidarAcessoComunicador", "PTV") %>',
					urlComunicadorPTV: '<%= Url.Action("ComunicadorPTV", "PTV") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("EPTVListarFiltros", Model); %>
	</div>
</asp:Content>