<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMRoteiro" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Roteiros Orientativos</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Roteiro/listar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
	<script>
		$(function () {
			RoteiroListar.urlEditar = '<%= Url.Action("Editar", "Roteiro") %>';
			RoteiroListar.urlDesativarComfirm = '<%= Url.Action("DesativarConfirm", "Roteiro") %>';
			RoteiroListar.urlDesativar = '<%= Url.Action("Desativar", "Roteiro") %>';
			RoteiroListar.urlPdfRoteiro = '<%= Url.Action("RelatorioRoteiro","Roteiro") %>';
			RoteiroListar.urlDesativarRoteiro = '<%= Url.Action("ValidarDesativarRoteiro","Roteiro") %>';
			RoteiroListar.load($('#central'));

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])){%>
				ContainerAcoes.load($(".containerAcoes"), {
					urls:{
						urlGerarPdf: '<%= Url.Action("RelatorioRoteiro", "Roteiro", new {id = Request.Params["acaoId"].ToString() }) %>'
					}
				});
			<%}%>
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros", Model); %>
	</div>
</asp:Content>