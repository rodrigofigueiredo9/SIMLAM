<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<PerguntaInfracaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Salvar pergunta</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Fiscalizacao/fiscalizacaoConfiguracao.js") %>" ></script>
	<script>
		$(function () {
			ConfigurarPerguntaInfracao.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("ConfigurarPergunta", "Fiscalizacao") %>'
				},
				mensagens: <%= Model.Mensagens %>
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">


		<h1 class="titTela">Salvar pergunta</h1><br />

		<% Html.RenderPartial("ConfigurarPerguntaPartial", Model); %>
	</div>
</asp:Content>
