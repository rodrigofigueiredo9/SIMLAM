<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPTV" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPTV.PTVVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Emitir EPTV</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/mensagem.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Empreendimento/listar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/ConfiguracaoVegetal/Cultura/listar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/unidadeProducao.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/unidadeProducaoItem.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/PTV/emitirPTV.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/PTV/listar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/containerAcoes.js") %>"></script>
	<script type="text/javascript">
	    $(function () {
	       
			PTVEmitir.settings.idsTela = <%= Model.IdsTela %>;
			PTVEmitir.settings.idsOrigem = <%= Model.IdsOrigem %>;
			PTVEmitir.settings.Mensagens = <%= Model.Mensagens %>;
			PTVEmitir.settings.dataAtual = '<%= Model.DataAtual%>';
			PTVEmitir.load($("#central"), {
				urls: {
				    urlSalvar: '<%= Url.Action("Salvar", "PTV") %>',
				    urlAssociarCultura: '<%= Url.Action("Caracterizacoes", "ConfiguracaoVegetal/AssociarCultura") %>',
					urlVerificarNumeroPTV: '<%= Url.Action("VerificarNumeroPTV", "PTV") %>',
					urlVerificarDua: '<%= Url.Action("VerificarDua", "PTV") %>',
					urlGravarVerificacaoDUA: '<%= Url.Action("GravarVerificacaoDUA", "PTV") %>',
					urlVerificarConsultaDUA: '<%= Url.Action("VerificarConsultaDUA", "PTV") %>',
					urlAssociarEmpreendimento: '<%= Url.Action("Associar","Empreendimento") %>',
					urlAssociarDestinatario: '<%= Url.Action("DestinatarioModal","PTV") %>',
					urlObterResponsaveisEmpreend:'<%= Url.Action("ObterResponsaveisEmpreendimento","PTV") %>',
					urlObterNumeroOrigem: '<%= Url.Action("ObterNumeroOrigem","PTV") %>',
					urlObterCultura: '<%= Url.Action("ObterCultura","PTV") %>',
					urlObterCultivar: '<%= Url.Action("ObterCultivar","PTV") %>',
					urlObterUnidadeMedida: '<%= Url.Action("ObterUnidadeMedida","PTV") %>',
					urlAdicionarProdutos: '<%= Url.Action("ValidarIdentificacaoProduto","PTV") %>',
					urlValidarDocumento: '<%= Url.Action("ValidarDocDestinatario","PTV") %>',
					urlObterDestinatario: '<%= Url.Action("ObterDestinatario","PTV") %>',
					urlObterLaboratorio: '<%= Url.Action("ObterLaboratorio","PTV") %>',
					urlObterTratamentoFisso: '<%= Url.Action("ObterTratamentoFitossanitario","PTV") %>',
					urlObterItinerario: '<%= Url.Action("ObterItinerario","PTV") %>',
					urlVerificarDocumentoOrigem: '<%= Url.Action("VerificarDocumentoOrigem","PTV") %>',
				    urlObterDiasHorasVistoria: '<%= Url.Action("ObterDiasHorasVistoria","PTV") %>',
				    urlObterDeclaracaoAdicional: '<%=Url.Action("ObterDeclaracaoAdicional", "PTV")%>'
				},
			});

			EPTVListar.load($('#central'), {
				urls: {
					urlHistorico: '<%= Url.Action("Historico", "PTV") %>',
					urlVisualizar: '<%= Url.Action("Visualizar", "PTV") %>',
					urlEditar: '<%= Url.Action("Editar", "PTV") %>',
					urlExcluirConfirm: '<%= Url.Action("ExcluirConfirm", "PTV") %>',
					urlExcluir: '<%= Url.Action("Excluir", "PTV") %>',
					urlPDF: '<%= Url.Action("GerarPdf", "PTV") %>',
					urlEnviarConfirm: '<%= Url.Action("EnviarConfirm", "PTV") %>',
					urlEnviar: '<%= Url.Action("Enviar", "PTV") %>',
					urlSolicitarDesbloqueio: '<%= Url.Action("ComunicadorPTV", "PTV") %>'
				}
			});

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"]))
	  { %>
			ContainerAcoes.load($(".containerAcoes"), {
				botoes: [
					{ label: 'Editar', url: '<%= Url.Action("Editar", "PTV", new { id = Request.Params["acaoId"] })%>' },
					{ label: 'Listar', url: '<%= Url.Action("Index", "PTV")%>' },
					{ label: 'Enviar', url: '<%= Url.Action("EnviarConfirm", "PTV") %>', abrirModal: function () { EPTVListar.enviarItem({ Id: '<%= Request.Params["acaoId"] %>' }); } }
				]
			});
			<% } %>
		});

	</script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Emitir Permissão de Trânsito de Vegetais</h1>
		<br />

		<%Html.RenderPartial("PTVPartial", Model); %>

		<div class="block box">
			<button class="btnSalvar floatLeft <%= Model.IsVisualizar ? "hide":"" %>" type="button" value="Salvar"><span>Salvar</span></button>
			<span class="cancelarCaixa"><span class="btnModalOu <%= Model.IsVisualizar ? "hide":"" %>">ou</span> <a class="linkCancelar" href="<%= Url.Action("Index", "PTV") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>
