<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPTV" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<DestinatarioPTVVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Cadastrar Destinatario PTV</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/PTV/destinatario.js") %>"></script>

	<script type="text/javascript">
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
		<h1 class="titTela">Cadastrar Destinatário PTV</h1>
		<br />

		<%Html.RenderPartial("Destinatario/DestinatarioPartial", Model); %>

		<div class="block box">
			<button class="btnSalvar floatLeft hide" type="button" value="Salvar"><span>Salvar</span></button>
			<span class="cancelarCaixa"><span class="btnModalOu hide">ou</span> <a class="linkCancelar" href="<%= Url.Action("DestinatarioIndex", "PTV") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>