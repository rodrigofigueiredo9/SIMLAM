<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProjetoDigital" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ProjetoDigitalListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Requerimentos Digitais</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/ProjetoDigital/listar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>

	<script type="text/javascript">
		$(function () {
			ProjetoDigitalListar.urlImportar = '<%= Url.Action("Importar", "ProjetoDigital") %>';
			ProjetoDigitalListar.urlPdfRequerimento = '<%= Url.Action("GerarPdfRequerimento", "ProjetoDigital") %>';
			ProjetoDigitalListar.load($('#central'));

		<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])){%>
				ContainerAcoes.load($(".containerAcoes"), {
					urls:{
						urlGerarPdf: '<%= Url.Action("GerarPdf", "Requerimento", new {id = Request.Params["acaoId"].ToString() }) %>',
						urlProcesso: '<%= Url.Action("Criar", "Processo") %>',
						urlDocumento: '<%= Url.Action("Criar", "Documento") %>'
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