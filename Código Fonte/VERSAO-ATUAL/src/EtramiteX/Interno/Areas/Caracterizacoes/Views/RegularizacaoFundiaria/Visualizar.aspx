<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<RegularizacaoFundiariaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Caracterização do Empreendimento - Regularização Fundiária</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/regularizacaoFundiaria.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/dominio.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/Pessoa/pessoa.js") %>" ></script>

	<script>
		$(function () {
			RegularizacaoFundiaria.load($('#central'), {
				urls: {
					visualizarPosse: '<%= Url.Action("VisualizarPosse", "RegularizacaoFundiaria") %>',
					pessoaVisualizar: '<%= Url.Action("Visualizar","Pessoa", new {area =""}) %>',
					visualizarDominio: '<%= Url.Action("DominioVisualizar", "Dominialidade") %>'
				},
				idsTela: <%= Model.IdsTela %>,
				textoMerge: '<%= Model.TextoMerge %>',
				atualizarDependenciasModalTitulo: '<%= Model.AtualizarDependenciasModalTitulo %>'
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Caracterização do Empreendimento - Regularização Fundiária</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("RegularizacaoFundiariaLista", Model);%>
		</div>

		<div class="block box">
			<span class="cancelarCaixa cancelarCaixaPrincipal"><a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.Caracterizacao.EmpreendimentoId}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>