<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<PTVListarVM>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">PTV</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/PTV/listar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>

	<script>
		$(function () {
			PTVListar.load($('#central'), {
				urls:{
					urlExcluir: '<%= Url.Action("Excluir", "PTV") %>',
					urlExcluirConfirm: '<%= Url.Action("ExcluirConfirm", "PTV") %>',
					urlEditar: '<%= Url.Action("Editar", "PTV") %>',
					urlVisualizar: '<%= Url.Action("Visualizar", "PTV") %>',
					urlConfirmarAtivar: '<%= Url.Action("AtivarConfirm", "PTV") %>',
					urlConfirmCancel: '<%= Url.Action("CancelarConfirm", "PTV") %>',
					urlAtivar: '<%= Url.Action("Ativar", "PTV") %>',
					urlCancelar: '<%= Url.Action("PTVCancelar", "PTV") %>',
					urlPDF : '<%= Url.Action("GerarPdf", "PTV") %>'
				}
			});



			<% if (!String.IsNullOrEmpty(Request.Params["acaoGerarPdfId"])) { %>
			PTVListar.gerarPDFLoad(<%= Request.Params["acaoGerarPdfId"] %>);
			<% } %>

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])) { %>
				ContainerAcoes.load($(".containerAcoes"), {
					botoes: [
						{ label: 'Gerar PDF', url: '<%= Url.Action("GerarPdf", "PTV", new {id = Request.Params["acaoId"].ToString() }) %>' },
						{ label: 'Ativar', url: '<%= Url.Action("Ativar", "PTV", new { id= Request.Params["acaoId"].ToString() })%>' },
						{ label: 'Novo', url: '<%= Url.Action("Criar","PTV") %>' }
					]
				});
			<% } %>
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

	<div id="central"><% Html.RenderPartial("ListarFiltros", Model); %></div>

</asp:Content>
