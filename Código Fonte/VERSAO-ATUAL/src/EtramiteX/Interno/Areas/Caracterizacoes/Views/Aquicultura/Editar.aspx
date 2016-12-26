<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<AquiculturaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Editar Aquicultura</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/Aquicultura.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/aquiculturaAquicult.js") %>"></script>
	<script>
		$(function () {
			Aquicultura.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("Editar", "Aquicultura") %>',
					mergiar: '<%= Url.Action("GeoMergiar", "Aquicultura") %>',
					obterTemplate: '<%= Url.Action("ObterTemplateBeneficiamento", "Aquicultura") %>'
				},
				idsTela: <%=Model.IdsTela %>
			});
			CoordenadaAtividade.settings.mensagens = <%= Model.Mensagens%>;
			CoordenadaAtividade.settings.urls.urlObterDadosCoordenadaAtividade = '<%= Url.Action("ObterDadosCoordenadaAtividade", "Aquicultura") %>';
			CoordenadaAtividade.settings.urls.urlObterDadosTipoGeometria = '<%= Url.Action("ObterDadosTipoGeometria", "Aquicultura") %>';
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Editar Aquicultura</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("Aquicultura", Model);%>
		</div>

		<div class="block box">
			<input class="floatLeft btnSalvar" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.Caracterizacao.EmpreendimentoId}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>