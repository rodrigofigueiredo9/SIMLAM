<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPTV" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<PTVListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">EPTV</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/PTV/listar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/containerAcoes.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/PTV/comunicadorptv.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			EPTVListar.load($('#central'), {
				urls: {
					urlVisualizar: '<%= Url.Action("Visualizar", "PTV") %>',
					urlEditar: '<%= Url.Action("Editar", "PTV") %>',
					urlExcluirConfirm: '<%= Url.Action("ExcluirConfirm", "PTV") %>',
					urlExcluir: '<%= Url.Action("Excluir", "PTV") %>',
					urlPDF: '<%= Url.Action("GerarPdf", "PTV") %>',
					urlEnviarConfirm: '<%= Url.Action("EnviarConfirm", "PTV") %>',
					urlEnviar: '<%= Url.Action("Enviar", "PTV") %>',
					urlValidarAcessoComunicador: '<%= Url.Action("ValidarAcessoComunicador", "PTV") %>',
					urlValidarAcessoSolicitarDesbloqueio: '<%= Url.Action("ValidarAcessoSolicitarDesbloqueio", "PTV") %>',
					urlComunicadorPTV: '<%= Url.Action("ComunicadorPTV", "PTV") %>',
					urlSolicitarDesbloqueio: '<%= Url.Action("SolicitarDesbloqueio", "PTV") %>',
					urlConfirmarCancelarEnvio: '<%= Url.Action("CancelarEnvioConfirm", "PTV") %>',
					urlCancelarEnvio: '<%= Url.Action("CancelarEnvio", "PTV") %>'
				}
			});

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])) { %>
			ContainerAcoes.load($(".containerAcoes"), {
				botoes: [
					{ label: 'Editar', url: '<%= Url.Action("Editar", "PTV", new { id = Request.Params["acaoId"] })%>' },
						{ label: 'Enviar', url: '<%= Url.Action("EnviarConfirm", "PTV") %>', abrirModal: function () { EPTVListar.enviarItem({ Id: '<%=Request.Params["acaoId"]%>' }); } }
					]
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