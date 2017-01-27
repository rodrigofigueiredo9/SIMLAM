<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<PTVVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Analisar EPTV</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/PTV/EPTVAnalisar.js") %>"></script>

	<script>
		EPTVAnalisar.settings.idsTela = <%=Model.IdsTelaAnalisar%>;

		$(function () {
			EPTVAnalisar.load($('#central'), {
				urls:{
					salvar: '<%=Url.Action("EPTVAnalisar", "PTV")%>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Analisar Permissão de Trânsito de Vegetais</h1>
		<br />

		<%Html.RenderPartial("EPTVPartial", Model); %>

		<div class="block box">
			<button class="btnSalvar floatLeft" type="button" value="Salvar"><span>Salvar</span></button>
			<span class="cancelarCaixa"><span class="btnModalOu <%= Model.IsVisualizar ? "hide":"" %>">ou</span> <a class="linkCancelar" href="<%= Url.Action("EPTVListar", "PTV") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>