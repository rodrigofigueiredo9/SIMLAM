<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMChecagemRoteiro" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarCheckListRoteiroVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Checagens de Itens de Roteiro</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/ChecagemRoteiro/listarChecagemRoteiro.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
	<script>
		$(function () {
			ChecagemRoteirotListar.urlExcluirConfirm = '<%= Url.Action("ChecagemRoteiroExcluirConfirm", "ChecagemRoteiro") %>';
			ChecagemRoteirotListar.urlExcluir = '<%= Url.Action("ChecagemRoteiroExcluir", "ChecagemRoteiro") %>';
			ChecagemRoteirotListar.urlGerarPdf = '<%= Url.Action("ChecagemRoteiroPDF", "ChecagemRoteiro") %>';

			ChecagemRoteirotListar.load($('#central'));

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])){%>
				ContainerAcoes.load($(".containerAcoes"), {
					urls:{
						urlGerarPdf: '<%= Url.Action("ChecagemRoteiroPDF", "ChecagemRoteiro", new {id = Request.Params["acaoId"].ToString() }) %>'
					}
				});
			<%}%>
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ChecagemRoteiroListarFiltros"); %>
	</div>
</asp:Content>