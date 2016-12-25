<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<PatioLavagemVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Salvar Pátio de Lavagem, Abastecimento e Descontaminação de Aeronave Agrícola</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/patioLavagem.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/coordenadaAtividade.js") %>"></script>
	
	<script>
		$(function () {
			PatioLavagem.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("Criar", "PatioLavagem") %>',
					mergiar: '<%= Url.Action("GeoMergiar", "PatioLavagem") %>'
				},
				mensagens: <%= Model.Mensagens %>
				});
			CoordenadaAtividade.settings.mensagens = <%= Model.CoodernadaAtividade.Mensagens%>;
			CoordenadaAtividade.settings.urls.urlObterDadosCoordenadaAtividade = '<%= Url.Action("ObterDadosCoordenadaAtividade", "PatioLavagem") %>';
			CoordenadaAtividade.settings.urls.urlObterDadosTipoGeometria = '<%= Url.Action("ObterDadosTipoGeometria", "PatioLavagem") %>';
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Salvar Pátio de Lavagem, Abastecimento e Descontaminação de Aeronave Agrícola</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("PatioLavagem", Model);%>
		</div>

		<div class="block box">
			<input class="floatLeft btnSalvar" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.Caracterizacao.EmpreendimentoId}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>