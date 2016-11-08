<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloLiberacaoCFOCFOC" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMLiberacaoCFOCFOC" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Liberações de Números de CFO e CFOC</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/LiberacaoCFOCFOC/listar.js") %>" ></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
	<script type="text/javascript">
		$(function () {
			LiberacaoCFOCFOCListar.urlVisualizar = '<%= Url.Action("Visualizar", "LiberacaoCFOCFOC") %>';
			LiberacaoCFOCFOCListar.urlGerarPDF = '<%= Url.Action("GerarPDF", "LiberacaoCFOCFOC") %>';
			LiberacaoCFOCFOCListar.load($('#central'));

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])) { %>
			ContainerAcoes.load($(".containerAcoes"), {
				urls: {
					urlGerarPdf: '<%= Url.Action("GerarPDF", "LiberacaoCFOCFOC", new { id = Request.Params["acaoId"]})%>'
				}
			});
			<% } %>
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros", Model); %>
	</div>
</asp:Content>