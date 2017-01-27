<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV" %>

<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<DestinatarioPTVVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Editar Destinatário PTV</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/PTV/destinatario.js") %>"></script>

	<script>
		$(function () {
			DestinatarioPTV.load($("#central"), {
				urls: {
					verificarCPFCNPJ:'<%= Url.Action("VerificarDestinatarioCPFCNPJ", "PTV") %>',
					salvar: '<%= Url.Action("DestinatarioSalvar", "PTV") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Editar Destinatário PTV</h1>
		<br />
		<%Html.RenderPartial("Destinatario/DestinatarioPartial", Model); %>

		<div class="block box">
			<button class="btnSalvar floatLeft" type="button" value="Salvar"><span>Salvar</span></button>
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("DestinatarioIndex", "PTV") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>