<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<TerraplanagemVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Terraplanagem</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/Terraplanagem.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/coordenadaAtividade.js") %>"></script>

	<script>
	    $(function () {
	        Terraplanagem.load($('#central'), {
	            urls: {
	                mergiar: '<%= Url.Action("GeoMergiar", "Terraplanagem") %>'
				}
			});
		    CoordenadaAtividade.settings.mensagens = <%= Model.CoodernadaAtividade.Mensagens%>;
		    CoordenadaAtividade.settings.urls.urlObterDadosCoordenadaAtividade = '<%= Url.Action("ObterDadosCoordenadaAtividade", "Terraplanagem") %>';
		    CoordenadaAtividade.settings.urls.urlObterDadosTipoGeometria = '<%= Url.Action("ObterDadosTipoGeometria", "Terraplanagem") %>';
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Visualizar Terraplanagem</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("Terraplanagem", Model);%>
		</div>

		<div class="block box">
			<span class="cancelarCaixa"><a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.Caracterizacao.EmpreendimentoId}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>