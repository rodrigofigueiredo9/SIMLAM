<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<UnidadeConsolidacaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Unidade de Consolidação</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/unidadeConsolidacao.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/ConfiguracaoVegetal/Cultura/listar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Credenciado/Credenciadolistar.js") %>"></script>

	<script>
		$(function () {
			UnidadeConsolidacao.load($('#central'), {
				urls: {
					listarCulturas: '<%= Url.Action("AssociarCultura", "ConfiguracaoVegetal")%>',
					adicionarCultivar: '<%= Url.Action("AdicionarCultivar", "UnidadeConsolidacao")%>',
					listarCredenciados: '<%= Url.Action("CredenciadoAssociar", "Credenciado")%>',
					adicionarResponsavelTecnico: '<%= Url.Action("AdicionarResponsavelTecnico", "UnidadeConsolidacao")%>',
					obterResponsavelTecnico: '<%= Url.Action("ObterResponsavelTecnico", "UnidadeConsolidacao")%>',
				    salvar: '<%= Url.Action("Criar", "UnidadeConsolidacao")%>',
				    obterLstCultivares: '<%= Url.Action("ObterLstCultivares", "UnidadeConsolidacao")%>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Unidade de Consolidação</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("UnidadeConsolidacaoPartial", Model);%>
		</div>

		<div class="block box">
			<input class="floatLeft btnSalvar" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.UnidadeConsolidacao.Empreendimento.Id}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>