<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<PerguntaListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Perguntas de infração</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Fiscalizacao/fiscalizacaoConfiguracaoListarPergunta.js") %>" ></script>
	<script>
		$(function () {
			ConfigurarListarPergunta.load($('#central'), {
				urls: {
					urlEditar: '<%= Url.Action("ConfigurarPergunta", "Fiscalizacao") %>',
					urlVisualizar: '<%= Url.Action("ConfigurarPerguntaVisualizar", "Fiscalizacao") %>',
					urlExcluirConfirm: '<%= Url.Action("ExcluirPerguntaInfracaoConfirm", "Fiscalizacao") %>',
					urlExcluir: '<%= Url.Action("ExcluirPerguntaInfracao", "Fiscalizacao") %>',
					urlAlterarSituacao: '<%= Url.Action("AlterarSituacaoPerguntaInfracao", "Fiscalizacao") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<%Html.RenderPartial("ConfigurarPerguntasListarFiltros", Model);%>
	</div>
</asp:Content>
