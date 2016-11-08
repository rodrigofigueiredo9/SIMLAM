<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAgrotoxico" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<AgrotoxicoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Agrotóxico</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Agrotoxico/agrotoxico.js") %>" ></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Agrotoxico/agrotoxicoCultura.js") %>" ></script>

	<script type="text/javascript">
		$(function () {
			Agrotoxico.load($("#central"), {
				urls: {
					agrotoxicoCulturaVisualizar: '<%=Url.Action("VisualizarCultura", "Agrotoxico")%>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Visualizar Agrotóxico</h1>
		<br />

		<%Html.RenderPartial("AgrotoxicoPartial", Model); %>

		<div class="block box">
			<span class="cancelarCaixa"><a class="linkCancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>