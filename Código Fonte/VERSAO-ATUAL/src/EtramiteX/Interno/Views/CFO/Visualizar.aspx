<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCFOCFOC.CFO" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<CFOVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Certificado Fitossanitário de Origem</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/CFO/emitir.js") %>"></script>

	<script>
		$(function () {
			CFOEmitir.load($('#central'), {});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Visualizar Certificado Fitossanitário de Origem</h1>
		<br />

		<div class="block">
			<% Html.RenderPartial("CFOPartial"); %>
		</div>

		<div class="block box">
			<span class="cancelarCaixa"><a class="linkCancelar" href="<%= Url.Action("Index", "CFO") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>