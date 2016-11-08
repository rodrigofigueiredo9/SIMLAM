<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ProducaoCarvaoVegetalVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Produção de Carvão Vegetal</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/producaoCarvaoVegetal.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/materiaPrimaFlorestalConsumida.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/coordenadaAtividade.js") %>"></script>
	
	<script type="text/javascript">
	    $(function () {
	        ProducaoCarvaoVegetal.load($('#central'), {
	            urls: {
				    mergiar: '<%= Url.Action("GeoMergiar", "ProducaoCarvaoVegetal") %>'
				},
			    mensagens: <%= Model.Mensagens %>
			    });
		    CoordenadaAtividade.settings.mensagens = <%= Model.CoodernadaAtividade.Mensagens%>;
		    CoordenadaAtividade.settings.urls.urlObterDadosCoordenadaAtividade = '<%= Url.Action("ObterDadosCoordenadaAtividade", "ProducaoCarvaoVegetal") %>';
		    CoordenadaAtividade.settings.urls.urlObterDadosTipoGeometria = '<%= Url.Action("ObterDadosTipoGeometria", "ProducaoCarvaoVegetal") %>';
		    MateriaPrimaFlorestalConsumida.settings.mensagens = <%= Model.MateriaPrimaFlorestalConsumida.Mensagens%>;
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Visualizar Produção de Carvão Vegetal</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("ProducaoCarvaoVegetal", Model);%>
		</div>

		<div class="block box">
			<span class="cancelarCaixa"><a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.Caracterizacao.EmpreendimentoId}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>