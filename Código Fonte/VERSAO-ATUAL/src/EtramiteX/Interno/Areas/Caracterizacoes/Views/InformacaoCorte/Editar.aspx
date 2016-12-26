<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<InformacaoCorteVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Editar Informação de Corte</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/informacaoCorte.js") %>"></script>
	
	<script>
		$(function () {
			InformacaoCorte.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("Salvar", "InformacaoCorte") %>',
					mergiar: '<%= Url.Action("GeoMergiar", "InformacaoCorte") %>'
				},
				mensagens: <%= Model.Mensagens %>
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Editar Informação de Corte</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("InformacaoCorte", Model);%>
		</div>

		<div class="block box divLinkVoltar">
			<span><a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.Caracterizacao.EmpreendimentoId}) %>">Voltar</a></span>
		</div>
	</div>
</asp:Content>