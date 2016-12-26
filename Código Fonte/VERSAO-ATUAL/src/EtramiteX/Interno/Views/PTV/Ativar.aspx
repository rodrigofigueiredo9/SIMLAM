<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPTV" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<PTV>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Ativar PTV</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/PTV/ativar.js") %>"></script>

	<script>
		$(function () {
			PTVAtivar.load($("#central"), {
				urls: {
					urlSalvar: '<%= Url.Action("Ativar", "PTV") %>'
				},
			});
		});
	</script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">

		<%Html.RenderPartial("AtivarPartial", Model); %>

		<div class="block box">
			<button class="btnSalvar floatLeft" type="button" value="Salvar"><span>Salvar</span></button>
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("Index", "PTV") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>