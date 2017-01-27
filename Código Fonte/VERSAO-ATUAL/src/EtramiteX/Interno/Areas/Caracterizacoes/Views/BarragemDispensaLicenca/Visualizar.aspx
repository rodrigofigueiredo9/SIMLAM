<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<BarragemDispensaLicencaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Barragem para Dispensa de Licença Ambiental</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/barragemDispensaLicenca.js") %>"></script>
	<script>
		$(function () {
			BarragemDispensaLicenca.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Barragem para Dispensa de Licença Ambiental</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("BarragemDispensaLicencaPartial", Model);%>
		</div>

		<div class="block box">
			<span class="cancelarCaixa"><a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.Caracterizacao.EmpreendimentoID }) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>