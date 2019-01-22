<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<BarragemDispensaLicencaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Barragem para Dispensa de Licença Ambiental</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/GeoProcessamento/coordenada.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/barragemDispensaLicenca.js") %>"></script>
	<script type="text/javascript">
		$(function () {
			BarragemDispensaLicenca.load($('#central'), {
				urls: {
					coordenadaGeo: '<%= Url.Action("CoordenadaPartial", "Mapa", new {area="GeoProcessamento" })%>',
					salvar: '<%= Url.Action("Criar", "BarragemDispensaLicenca") %>'
				},
				mensagens: <%= Model.Mensagens %>,
				idsTela: <%= Model.IdsTela %>
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Barragem Dispensada Licença Ambiental</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("BarragemDispensaLicencaPartial", Model);%>
		</div>

		<div class="block box">
			<input class="floatLeft btnSalvar" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.Caracterizacao.EmpreendimentoID, projetoDigitalId = Request.Params["projetoDigitalId"] }) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>