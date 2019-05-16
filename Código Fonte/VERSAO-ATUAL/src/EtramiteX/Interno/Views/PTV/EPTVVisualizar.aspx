<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<PTVVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar EPTV</asp:Content>

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
		<h1 class="titTela">Visualizar Permissão de Trânsito de Vegetais</h1>
		<br />

		<%Html.RenderPartial("EPTVPartialCredenciado", Model); %>

		<div class="block box">
			<span class="cancelarCaixa">
                <a class="linkCancelar" href="<%= Url.Action("EPTVIndex", "PTV") %>">Cancelar</a>
			</span>
		</div>
	</div>
</asp:Content>