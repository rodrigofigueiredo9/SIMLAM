<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels.VMUnidadeProducao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<UnidadeProducaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Unidade de Produção</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
		<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/unidadeProducao.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/unidadeProducaoItem.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			UnidadeProducao.load($('#central'), {
				urls: {
					AdicionarUnidadeProducao: '<%= Url.Action("AdicionarUnidadeProducao", "UnidadeProducao") %>',
				},
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Unidade de Produção</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("UnidadeProducaoPartial", Model);%>
		</div>

		<div class="block box">
			<span class="cancelarCaixa"><a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.UnidadeProducao.Empreendimento.Id, projetoDigitalId = Request.Params["projetoDigitalId"], visualizar = Model.RetornarVisualizar}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>