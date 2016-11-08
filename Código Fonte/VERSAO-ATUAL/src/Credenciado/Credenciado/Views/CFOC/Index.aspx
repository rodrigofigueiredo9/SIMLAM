<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCFOCFOC.CFOC" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">CFOCs</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/CFOC/listar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/containerAcoes.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			CFOCListar.urlVisualizar = '<%= Url.Action("Visualizar", "CFOC") %>';
			CFOCListar.urlEditar = '<%= Url.Action("Editar", "CFOC") %>';
			CFOCListar.urlConfirmarExcluir = '<%= Url.Action("ExcluirConfirm", "CFOC") %>';
			CFOCListar.urlExcluir = '<%= Url.Action("Excluir", "CFOC") %>';
			CFOCListar.urlConfirmarAtivar = '<%= Url.Action("AtivarConfirm", "CFOC") %>';
			CFOCListar.urlAtivar = '<%= Url.Action("Ativar", "CFOC") %>';
			CFOCListar.urlPDF = '<%= Url.Action("GerarPdf", "CFOC") %>';
			CFOCListar.idsTela = <%=Model.IdsTela %>;
			CFOCListar.mensagens = <%=Model.Mensagens %>;
			CFOCListar.load($('#central'));

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"]))
	  { %>
			
			ContainerAcoes.load($(".containerAcoes"), {
				botoes: [
					{ label: 'Gerar PDF', url: '<%= Url.Action("GerarPdf", "CFOC", new { id = Request.Params["acaoId"] })%>' },
					{ label: 'Editar', url: '<%= Url.Action("Editar", "CFOC", new { id = Request.Params["acaoId"] })%>' },
					{ label: 'Ativar', url: '<%= Url.Action("AtivarConfirm", "CFOC")%>', abrirModal: function (){ CFOCListar.ativarItem({ SituacaoId: '<%= (int)eDocumentoFitossanitarioSituacao.EmElaboracao %>', Id: '<%=Request.Params["acaoId"]%>'});}}
				]
			});
			<% } %>

			<% if (!String.IsNullOrEmpty(Request.Params["acaoGerarPdfId"]))
	  { %>
			CFOListar.gerarPDFLoad(<%= Request.Params["acaoGerarPdfId"] %>);
			<% } %>
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros", Model); %>
	</div>
</asp:Content>
