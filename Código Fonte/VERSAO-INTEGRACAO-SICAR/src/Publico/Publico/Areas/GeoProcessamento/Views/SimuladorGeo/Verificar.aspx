<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Publico.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Verificar</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/GeoProcessamento/simulador.js") %>"></script>
	<script type="text/javascript">
		$(function () {
			BaseReferencia.settings.urls.gerarArquivoVetorial = '<%= Url.Action("GerarArquivoVetorial", "SimuladorGeo", new {area="GeoProcessamento"}) %>';
			BaseReferencia.settings.urls.obterSituacaoArquivoRef = '<%= Url.Action("ObterSituacao", "SimuladorGeo") %>';
			BaseReferencia.settings.urls.baixarArquivoRef = '<%= Url.Action("BaixarArquivoRef", "SimuladorGeo", new { area = "GeoProcessamento"}) %>';
			BaseReferencia.settings.urls.baixarArquivoVetorial = '<%= Url.Action("Baixar", "SimuladorGeo", new {area="GeoProcessamento"}) %>';
			BaseReferencia.settings.situacoesValidas = <%= Model.SituacoesValidasJson %>;

			EnviarProjeto.mensagens = <%= Model.MensagensImportador %>;
			EnviarProjeto.settings.urls.EnviarArquivo = '<%: Url.Action("arquivo","SimuladorGeo", new { area = "GeoProcessamento"}) %>';
			EnviarProjeto.settings.urls.EnviarProcessar = '<%: Url.Action("EnviarProcessar","SimuladorGeo", new {area="GeoProcessamento"}) %>';
			EnviarProjeto.settings.urls.cancelarProcessamento = '<%= Url.Action("CancelarProcessamentoImportador","SimuladorGeo", new {area="GeoProcessamento"}) %>';
			EnviarProjeto.settings.urls.obterSituacao = '<%= Url.Action("ObterSituacao","SimuladorGeo", new {area="GeoProcessamento"}) %>';
			EnviarProjeto.settings.urls.baixarArquivos = '<%= Url.Action("Baixar", "SimuladorGeo", new {area="GeoProcessamento"}) %>';
			EnviarProjeto.settings.situacoesValidas = <%= Model.SituacoesValidasJson %>;

			Simulador.mensagens = <%= Model.Mensagens %>;
			Simulador.settings.urls.verificarCpf = '<%: Url.Action("VerificarCpf","SimuladorGeo", new {area="GeoProcessamento"}) %>';
			Simulador.load($('#central'), { projetoId: $('.hdnSimuladorGeoId', $('#central')).val() });
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("VerificarPartial"); %>
	</div>
</asp:Content>