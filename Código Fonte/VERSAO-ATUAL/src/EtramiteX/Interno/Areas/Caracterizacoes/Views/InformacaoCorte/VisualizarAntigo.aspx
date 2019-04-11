<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<InformacaoCorteAntigoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Informação de Corte</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/informacaoCorteAntigo.js") %>"></script>
	
	<script>
		$(function () {
			InformacaoCorte.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Visualizar Informação de Corte</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("InformacaoCorte", Model);%>
		</div>

		<div class="block box divLinkVoltar">
			<span><a class="linkCancelar" href="<%= Url.Action("Listar", "InformacaoCorte", new { id = Model.Caracterizacao.EmpreendimentoId }) %>">Voltar</a></span>
		</div>
	</div>
</asp:Content>