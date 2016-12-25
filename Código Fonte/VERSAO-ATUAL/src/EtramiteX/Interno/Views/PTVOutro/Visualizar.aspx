<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTVOutro" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<PTVOutroVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar PTV de Outro Estado</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/PTVOutro/emitirPTVOutro.js") %>"></script>
	<script>
		$(function () {
			PTVOutroEmitir.load($("#central"));
		});
	</script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Visualizar PTV de Outro Estado</h1>
		<br />

		<%Html.RenderPartial("PTVOutroPartial", Model); %>

		<div class="block box">
			<span class="cancelarCaixa"><span class="btnModalOu <%= Model.IsVisualizar ? "hide":"" %>">ou</span> <a class="linkCancelar" href="<%= Url.Action("Index", "PTVOutro") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>