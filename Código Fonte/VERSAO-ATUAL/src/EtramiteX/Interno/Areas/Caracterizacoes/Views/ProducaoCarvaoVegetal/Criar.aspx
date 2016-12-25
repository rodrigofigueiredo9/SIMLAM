<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ProducaoCarvaoVegetalVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Salvar Produção de Carvão Vegetal</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/producaoCarvaoVegetal.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/materiaPrimaFlorestalConsumida.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/coordenadaAtividade.js") %>"></script>
	
	<script>
		$(function () {
			ProducaoCarvaoVegetal.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("Criar", "ProducaoCarvaoVegetal") %>',
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
		<h1 class="titTela">Salvar Produção de Carvão Vegetal</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("ProducaoCarvaoVegetal", Model);%>
		</div>

		<div class="block box">
			<input class="floatLeft btnSalvar" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.Caracterizacao.EmpreendimentoId}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>