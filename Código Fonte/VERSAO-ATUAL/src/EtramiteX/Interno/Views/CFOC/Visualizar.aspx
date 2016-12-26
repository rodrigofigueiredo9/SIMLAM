<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCFOCFOC.CFOC" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<CFOCVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Certificado Fitossanitário de Origem Consolidado</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/CFOC/emitir.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/CFOC/lote.js") %>"></script>

	<script>
		$(function () {
			CFOCEmitir.load($('#central'), {
				urls: {
					loteVisualizar: '<%= Url.Action("LoteVisualizar", "CFOC") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Visualizar Certificado Fitossanitário de Origem Consolidado</h1>
		<br />

		<div class="block">
			<% Html.RenderPartial("CFOCPartial"); %>
		</div>

		<div class="block box">
			<span class="cancelarCaixa"><a class="linkCancelar" href="<%= Url.Action("Index", "CFOC") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>