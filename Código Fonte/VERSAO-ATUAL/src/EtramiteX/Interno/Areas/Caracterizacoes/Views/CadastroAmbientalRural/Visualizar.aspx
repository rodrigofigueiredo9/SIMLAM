<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<CadastroAmbientalRuralVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Caracterização do Empreendimento - Cadastro Ambiental Rural</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/GeoProcessamento/navegador.js") %>"></script>
    <script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/cadastroAmbientalRural.js") %>"></script>
	<script>
		$(function () {
			CadastroAmbientalRural.load($('#central'), {
				urls: {
				    baixarArquivos: '<%= Url.Action("Baixar", "Arquivo", new {area=""}) %>',
					desenhador: '<%= Url.Action("DesenhadorPartial", "Mapa", new {area="GeoProcessamento"}) %>',
					arquivos: <%=Model.UrlsArquivo%>
				},
			    idsTelaProjetoGeograficoSituacao: <%=Model.IdsTelaProjetoGeograficoSituacao%>,
				isVisualizar: true
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h2 class="titTela">Caracterização do Empreendimento - Cadastro Ambiental Rural</h2>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("CadastroAmbientalRural", Model);%>
		</div>

		<div class="block box">
			<span class="cancelarCaixa"><a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.Caracterizacao.EmpreendimentoId}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>