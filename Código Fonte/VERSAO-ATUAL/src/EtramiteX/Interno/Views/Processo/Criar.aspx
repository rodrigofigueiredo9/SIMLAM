<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProcesso" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<SalvarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Cadastrar Processo</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<!-- DEPENDENCIAS DE PESSOA -->
	<script src="<%= Url.Content("~/Scripts/Pessoa/pessoa.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Pessoa/representante.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Pessoa/profissao.js") %>"></script>
	<!-- FIM DEPENDENCIAS DE PESSOA -->

	<!-- DEPENDENCIAS DE EMPREENDIMENTO -->
	<script src="<%= Url.Content("~/Scripts/Empreendimento/empreendimento.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Empreendimento/associar.js") %>"></script>
	<!-- FIM DEPENDENCIAS DE EMPREENDIMENTO -->

	<script src="<%= Url.Content("~/Scripts/ChecagemRoteiro/listarChecagemRoteiro.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Requerimento/requerimentoListar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Fiscalizacao/fiscalizacaoListar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Processo/processo.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Requerimento/atividadeSolicitadaAssociar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>

<script>
	$(function () {
		FinalidadeAssociar.urlObterTituloModelo = '<%= Url.Action("ObterTituloModelo", "Requerimento") %>';
		FinalidadeAssociar.urlObterTituloModeloAnterior = '<%= Url.Action("ObterTituloModeloAnterior", "Requerimento") %>';
		FinalidadeAssociar.urlValidarNumeroModeloAnterior = '<%= Url.Action("ValidarNumeroModeloAnterior", "Requerimento") %>';
		FinalidadeAssociar.Mensagens = <%= Model.Mensagens %>;

		AtividadeSolicitadaAssociar.urlAbriModalAtividade = '<%= Url.Action("ObterFinalidade", "Atividade") %>';
		AtividadeSolicitadaAssociar.Mensagens = <%= Model.Mensagens %>;
		AtividadeSolicitadaAssociar.load($('#central'));

		Processo.load($('#central'), {
			urls: {
				enviarArquivo: '<%= Url.Action("Arquivo", "Arquivo") %>',
				associarCheckList: '<%= Url.Action("AssociarCheckList", "ChecagemRoteiro") %>',
				visualizarCheckList: '<%= Url.Action("ChecagemRoteiroVisualizar", "ChecagemRoteiro") %>',
				associarRequerimento: '<%= Url.Action("Associar", "Requerimento") %>',
				associarRequerimentoValidar: '<%= Url.Action("ValidarRequerimentoAtividades", "Processo") %>',
				obterRequerimento: '<%= Url.Action("ObterRequerimento", "Processo") %>',

				associarFiscalizacao: '<%= Url.Action("Associar", "Fiscalizacao") %>',
				desassociarFiscalizacaoValidar: '<%= Url.Action("ValidarDesassociarFiscalizacao", "Processo") %>',
				obterFiscalizacao: '<%= Url.Action("ObterFiscalizacao", "Processo") %>',
				visualizarFiscalizacao: '<%= Url.Action("VisualizarFiscalizacaoModal", "Fiscalizacao") %>',

				pdfRequerimento: '<%= Url.Action("GerarPdf", "Requerimento") %>',
				associarInteressado: '<%= Url.Action("PessoaModal", "Pessoa") %>',
				editarInteressado: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
				editarEmpreendimento: '<%= Url.Action("EmpreendimentoModalVisualizar", "Empreendimento") %>',
				associarResponsavelModal: '<%= Url.Action("PessoaModal", "Pessoa") %>',
				editarResponsavelModal: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
				validarChecagem: '<%= Url.Action("ValidarAssociacaoChecagem", "Processo") %>',
				validarChecagemTemTituloPendencia: '<%= Url.Action("ValidarChecagemTemTituloPendencia", "Processo") %>',
				salvar: '<%= Url.Action("Criar", "Processo") %>',
				autuar: '<%= Url.Action("Autuar", "Processo") %>'
			},
			Mensagens: <%= Model.Mensagens %>,
			configuracoesProtocoloTipos: <%= Model.ConfiguracoesProtocoloTiposJson %>
		});
		<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])){%>
				ContainerAcoes.load($(".containerAcoes"), {
					urls:{
						urlGerarPdf: '<%= Url.Action("GerarPdfDocRegistroRecebimento", "Processo", new {id = Request.Params["acaoId"].ToString() }) %>',
						urlTramitacao: '<%= Url.Action("Index", "Tramitacao")%>'
					}
				});
		<%}%>
	});
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central" class="processoCriar">

		<h1 class="titTela">Cadastrar Processo</h1>
		<br />

		<div class="processoPartial">
			<% Html.RenderPartial("ProcessoPartial"); %>
		</div>

		<div class="block box btnProcessoContainer">
			<span class="spnProcessoSalvar"><input class="btnProcessoSalvar floatLeft" type="button" value="Salvar" /></span>
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>