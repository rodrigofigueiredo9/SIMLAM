<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMRequerimento" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<RequerimentoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Visualizar Requerimento Padrão</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
<script src="<%= Url.Content("~/Scripts/jquery.listar-grid.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Atividade/atividadeSolicitadaListar.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/ChecagemRoteiro/salvarChecagemRoteiro.js") %>"></script>
<!-- DEPENDENCIAS DE PESSOA -->
<script src="<%= Url.Content("~/Scripts/Pessoa/pessoa.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Pessoa/inline.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Pessoa/representante.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Pessoa/profissao.js") %>"></script>
<!-- FIM DEPENDENCIAS DE PESSOA -->
<!-- DEPENDENCIAS DE EMPREENDIMENTO -->
<script src="<%= Url.Content("~/Scripts/Empreendimento/empreendimento.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Empreendimento/inline.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Atividade/listar.js") %>"></script>
<!-- FIM DEPENDENCIAS DE EMPREENDIMENTO -->
<script src="<%= Url.Content("~/Scripts/Requerimento/requerimentoVisualizar.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Requerimento/atividadeSolicitadaAssociar.js") %>"></script>

<script>

	RequerimentoVis.urlIndex = '<%= Url.Action("Index", "Requerimento") %>';
	RequerimentoObjetivoPedido.visualizarRoteiroModalLink = '<%= Url.Action("Visualizar", "Roteiro") %>';
	RequerimentoObjetivoPedido.urlObterObjetivoPedido = '<%= Url.Action("ObterObjetivoPedido", "Requerimento") %>';
	RequerimentoObjetivoPedido.urlBaixarPdf = '<%= Url.Action("RelatorioRoteiro", "Roteiro") %>';

	RequerimentoResponsavel.urlObterResponsavel = '<%= Url.Action("ObterResponsavel", "Requerimento") %>';

	RequerimentoInteressado.urlObterInteressado = '<%= Url.Action("PessoaInline", "Pessoa") %>';
	RequerimentoInteressado.urlAssociarInteressado = '<%= Url.Action("AssociarInteressado", "Requerimento") %>';

	RequerimentoEmpreendimento.urlObterEmpreendimento = '<%= Url.Action("EmpreendimentoInline", "Empreendimento") %>';
	RequerimentoEmpreendimento.urlAssociarEmpreendimento = '<%= Url.Action("AssociarEmpreendimento", "Requerimento") %>';

	RequerimentoFinalizar.urlObterFinalizar = '<%= Url.Action("ObterFinalizar", "Requerimento") %>';
	RequerimentoFinalizar.urlFinalizar = '<%= Url.Action("Finalizar", "Requerimento") %>';

	RequerimentoVis.urlObterReqInterEmp = '<%= Url.Action("ObterReqInterEmp", "Requerimento") %>';
	RequerimentoVis.urlPdf = '<%= Url.Action("GerarPdf", "Requerimento") %>';
	RequerimentoVis.Mensagens = <%= Model.Mensagens %>;

	$(function () {
		RequerimentoVis.load($('.RequerimentoCriar'));
		AtividadeSolicitadaAssociar.load($('.RequerimentoCriar'));
		RequerimentoObjetivoPedido.atividadeSolicitadaExpansivel();
		RequerimentoObjetivoPedido.configurarNumeroAnterior();
	});
</script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<div class="RequerimentoCriar">
			<br />
			<div class="requerimentoPartial">
				<% Html.RenderPartial("VisualizarPartial"); %>
				<div class="divMensagemTemplate hide">
				<fieldset class="block box">
					<div class="block">
						<div class="coluna100">
							<label class="lblMensagem"></label>
						</div>
					</div>
				</fieldset>
			</div>
			</div>
		</div>
	</div>
</asp:Content>
