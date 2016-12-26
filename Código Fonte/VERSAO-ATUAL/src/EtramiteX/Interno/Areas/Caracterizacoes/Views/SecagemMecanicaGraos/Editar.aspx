<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<SecagemMecanicaGraosVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Editar Secagem Mecânica de Grãos</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/secagemMecanicaGraos.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/materiaPrimaFlorestalConsumida.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/coordenadaAtividade.js") %>"></script>
	
	<script>
		$(function () {
			SecagemMecanicaGraos.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("Editar", "SecagemMecanicaGraos") %>',
					mergiar: '<%= Url.Action("GeoMergiar", "SecagemMecanicaGraos") %>'
				},
				mensagens: <%= Model.Mensagens %>
			});
			CoordenadaAtividade.settings.mensagens = <%= Model.CoodernadaAtividade.Mensagens%>;
			CoordenadaAtividade.settings.urls.urlObterDadosCoordenadaAtividade = '<%= Url.Action("ObterDadosCoordenadaAtividade", "SecagemMecanicaGraos") %>';
			CoordenadaAtividade.settings.urls.urlObterDadosTipoGeometria = '<%= Url.Action("ObterDadosTipoGeometria", "SecagemMecanicaGraos") %>';
			MateriaPrimaFlorestalConsumida.settings.mensagens = <%= Model.MateriaPrimaFlorestalConsumida.Mensagens%>;
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Editar Secagem Mecânica de Grãos</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("SecagemMecanicaGraos", Model);%>
		</div>

		<div class="block box">
			<input class="floatLeft btnSalvar" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.Caracterizacao.EmpreendimentoId}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>