<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCFOCFOC.CFO" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">CFOs</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/CFO/listar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>

	<script type="text/javascript">
		$(function () {
			CFOListar.urlVisualizar = '<%= Url.Action("Visualizar", "CFO") %>';
			CFOListar.urlEditar = '<%= Url.Action("Editar", "CFO") %>';
			CFOListar.urlConfirmarExcluir = '<%= Url.Action("ExcluirConfirm", "CFO") %>';
			CFOListar.urlExcluir = '<%= Url.Action("Excluir", "CFO") %>';
			CFOListar.urlConfirmarAtivar = '<%= Url.Action("AtivarConfirm", "CFO") %>';
			CFOListar.urlAtivar = '<%= Url.Action("Ativar", "CFO") %>';
			CFOListar.urlPDF = '<%= Url.Action("GerarPdf", "CFO") %>';
			CFOListar.idsTela = <%=Model.IdsTela %>;
			CFOListar.mensagens = <%=Model.Mensagens %>;
			CFOListar.load($('#central'));

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])) { %>
			
			ContainerAcoes.load($(".containerAcoes"), {
				botoes: [
					{ label: 'Gerar PDF', url: '<%= Url.Action("GerarPdf", "CFO", new { id = Request.Params["acaoId"] })%>' },
					{ label: 'Editar', url: '<%= Url.Action("Editar", "CFO", new { id = Request.Params["acaoId"] })%>' },
					{ label: 'Ativar', url: '<%= Url.Action("AtivarConfirm", "CFO")%>', abrirModal: function (){ CFOListar.ativarItem({ SituacaoId: '<%= (int)eDocumentoFitossanitarioSituacao.EmElaboracao %>', Id: '<%=Request.Params["acaoId"]%>'});}}
				]
			});
			<% } %>

			<% if (!String.IsNullOrEmpty(Request.Params["acaoGerarPdfId"])) { %>
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