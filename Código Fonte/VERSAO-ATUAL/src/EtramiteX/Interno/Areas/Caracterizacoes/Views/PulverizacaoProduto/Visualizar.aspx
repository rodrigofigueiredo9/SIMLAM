<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<PulverizacaoProdutoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Pulverização Aérea de Produtos Agrotóxicos</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/pulverizacaoProduto.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/coordenadaAtividade.js") %>"></script>
	
	<script>
		$(function () {
			PulverizacaoProduto.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("Editar", "PulverizacaoProduto") %>',
					mergiar: '<%= Url.Action("GeoMergiar", "PulverizacaoProduto") %>'
				},
				mensagens: <%= Model.Mensagens %>,
				idsTela: <%= Model.IdsTela %>
				});
			CoordenadaAtividade.settings.mensagens = <%= Model.CoodernadaAtividade.Mensagens%>;
			CoordenadaAtividade.settings.urls.urlObterDadosCoordenadaAtividade = '<%= Url.Action("ObterDadosCoordenadaAtividade", "PulverizacaoProduto") %>';
			CoordenadaAtividade.settings.urls.urlObterDadosTipoGeometria = '<%= Url.Action("ObterDadosTipoGeometria", "PulverizacaoProduto") %>';
		});
	</script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Visualizar Pulverização Aérea de Produtos Agrotóxicos</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("PulverizacaoProduto", Model);%>
		</div>

		<div class="block box">
			<span class="cancelarCaixa"><a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.Caracterizacao.EmpreendimentoId}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>