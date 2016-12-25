<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<CadastroAmbientalRuralVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Caracterização do Empreendimento - Cadastro Ambiental Rural</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/GeoProcessamento/navegador.js") %>"></script>
    <script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/cadastroAmbientalRural.js") %>"></script>
    <script src="<%= Url.Content("~/Scripts/Empreendimento/listar.js") %>"></script>

	<script>
		$(function () {
			CadastroAmbientalRural.load($('#central'), {
				urls: {
					processar: '<%= Url.Action("Processar", "CadastroAmbientalRural") %>',
					obterSituacaoProcessamento: '<%= Url.Action("ObterSituacaoProcessamento", "CadastroAmbientalRural") %>',
					obterModuloFiscal: '<%= Url.Action("ObterModuloFiscal", "CadastroAmbientalRural") %>',
					cancelar: '<%= Url.Action("Cancelar", "CadastroAmbientalRural") %>',
					obterArquivosProjeto: '<%= Url.Action("ObterArquivosProcessamento", "CadastroAmbientalRural") %>',
					obterAreasProcessadas: '<%= Url.Action("ObterAreasProcessadas", "CadastroAmbientalRural") %>',
					baixarArquivos: '<%= Url.Action("Baixar", "Arquivo", new {area=""}) %>',
					mergiar: '<%= Url.Action("GeoMergiar", "CadastroAmbientalRural") %>',
				    finalizar: '<%= Url.Action("Finalizar", "CadastroAmbientalRural") %>',
					desenhador:'<%= Url.Action("DesenhadorPartial", "Mapa", new {area="GeoProcessamento"}) %>',
					listarEmpreendimentos:'<%= Url.Action("Associar", "Empreendimento") %>',
					arquivos: <%=Model.UrlsArquivo%>
				},
				isEditar: true,
				idsTelaProjetoGeograficoSituacao: <%=Model.IdsTelaProjetoGeograficoSituacao%>,
				idsTelaAreas:<%=Model.IdsTelaArea%>,
				mensagens: <%=Model.Mensagens%>
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
			<span class="spanBotoes floatLeft spanReprocessar <%:(Model.MostrarReprocessar) ? "" : "hide" %>">
				<input class="floatLeft btnReprocessar" type="button" value="Reprocessar" />
			</span>

			<span class="spanBotoes floatRight spanFinalizar <%:(Model.MostrarFinalizar) ? "" : "hide" %>">
				<input class="floatLeft btnFinalizar" type="button" value="Finalizar" />
			</span>

			<span class="cancelarCaixa"><span class="btnModalOu <%:(Model.MostrarBtnOu) ? "" : "hide" %>">ou </span><a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.Caracterizacao.EmpreendimentoId}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>