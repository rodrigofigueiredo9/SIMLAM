<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<UnidadeProducaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Editar Unidade de Produção</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/unidadeProducao.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/unidadeProducaoItem.js") %>"></script>

	<script>
		$(function () {
			UnidadeProducao.load($('#central'), {
				urls: {
					AdicionarUnidadeProducao: '<%=Url.Action("AdicionarUnidadeProducao", "UnidadeProducao")%>',
					salvar: '<%=Url.Action("Editar", "UnidadeProducao")%>'
				},
				mensagens: <%= Model.Mensagens%>
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Editar Unidade de Produção</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("UnidadeProducaoPartial", Model);%>
		</div>

		<div class="block box">
			<input class="floatLeft btnSalvar" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id =Model.UnidadeProducao.Empreendimento.Id}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>