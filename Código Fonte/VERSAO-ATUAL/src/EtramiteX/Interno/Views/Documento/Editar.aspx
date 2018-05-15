<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMDocumento" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<SalvarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Editar Documento</asp:Content>

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
	<script src="<%= Url.Content("~/Scripts/Empreendimento/listar.js") %>"></script>
	<!-- FIM DEPENDENCIAS DE EMPREENDIMENTO -->

	<!-- DEPENDENCIAS DE ASSOCIAR -->
	<script src="<%= Url.Content("~/Scripts/Documento/listar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Processo/listar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/ChecagemPendencia/listar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/ChecagemRoteiro/listarChecagemRoteiro.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Requerimento/requerimentoListar.js") %>"></script>
    <script src="<%= Url.Content("~/Scripts/Fiscalizacao/fiscalizacaoListar.js") %>"></script>
	<!-- FIM DEPENDENCIAS DE ASSOCIAR -->

	<script src="<%= Url.Content("~/Scripts/Documento/documento.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Requerimento/atividadeSolicitadaAssociar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/ChecagemPendencia/salvar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>

<script>
	$(function () {

		AtividadeSolicitadaAssociar.urlAbriModalAtividade = '<%= Url.Action("ObterFinalidade", "Atividade") %>';
		AtividadeSolicitadaAssociar.urlValidarExcluirAtividadeFinalidade = '<%= Url.Action("ValidarExcluirAtividadeFinalidade", "Atividade") %>';
		FinalidadeAssociar.urlObterTituloModelo = '<%= Url.Action("ObterTituloModelo", "Requerimento") %>';
		FinalidadeAssociar.urlObterTituloModeloAnterior = '<%= Url.Action("ObterTituloModeloAnterior", "Requerimento") %>';
		FinalidadeAssociar.urlObterNumerosTitulos = '<%= Url.Action("ObterNumerosTitulos", "Requerimento") %>';

		AtividadeSolicitadaAssociar.Mensagens = <%= Model.Mensagens %>;
		FinalidadeAssociar.Mensagens = <%= Model.Mensagens %>;
		AtividadeSolicitadaAssociar.load($('#central'));

		var DocumentoObj = new Documento();

		DocumentoObj.load($('#central'), {
			urls: {
				enviarArquivo: '<%= Url.Action("Arquivo", "Arquivo") %>',
				associarProcesso: '<%= Url.Action("Associar", "Processo") %>',
				associarDocumento: '<%= Url.Action("Associar", "Documento") %>',
				obterProtocolo: '<%= Url.Action("ObterProtocolo", "Documento") %>',
				associarChecagemPendencia: '<%= Url.Action("Associar", "ChecagemPendencia") %>',
				associarCheckList: '<%= Url.Action("AssociarCheckList", "ChecagemRoteiro") %>',
				associarRequerimento: '<%= Url.Action("Associar", "Requerimento") %>',
			    associarRequerimentoValidar: '<%= Url.Action("ValidarRequerimentoAtividades", "Documento") %>',
			    associarFiscalizacao: '<%= Url.Action("Associar", "Fiscalizacao") %>',
			    desassociarFiscalizacaoValidar: '<%= Url.Action("ValidarDesassociarFiscalizacao", "Documento") %>',
			    obterFiscalizacao: '<%= Url.Action("ObterFiscalizacao", "Documento") %>',
			    visualizarFiscalizacao: '<%= Url.Action("VisualizarFiscalizacaoModal", "Fiscalizacao") %>',
				visualizarCheckList: '<%= Url.Action("ChecagemRoteiroVisualizar", "ChecagemRoteiro") %>',
				visualizarProcesso: '<%= Url.Action("Visualizar", "Processo") %>',
				visualizarDocumento: '<%= Url.Action("Visualizar", "Documento") %>',
				visualizarChecagemPendencia: '<%= Url.Action("Visualizar", "ChecagemPendencia") %>',
				obterRequerimento: '<%= Url.Action("ObterRequerimento", "Documento") %>',
				pdfRequerimento: '<%= Url.Action("GerarPdf", "Requerimento") %>',
				associarInteressado: '<%= Url.Action("PessoaModal", "Pessoa") %>',
				editarInteressado: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
				associarEmpreendimento: '<%= Url.Action("Associar", "Empreendimento") %>',
				editarEmpreendimento: '<%= Url.Action("EmpreendimentoModalVisualizar", "Empreendimento") %>',
				associarResponsavelModal: '<%= Url.Action("PessoaModal", "Pessoa") %>',
				editarResponsavelModal: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
				validarChecagem: '<%= Url.Action("ValidarAssociacaoChecagem", "Documento") %>',
				validarChecagemPendencia: '<%= Url.Action("ValidarAssociacaoChecagemPendencia", "Documento") %>',
				validarChecagemTemTituloPendencia: '<%= Url.Action("ValidarChecagemTemTituloPendencia", "Documento") %>',
				obterProcesso: '<%= Url.Action("ObterProcesso", "Processo") %>',
				funcionariosDestinatario: '<%= Url.Action("ObterFuncionariosDoSetor", "Tramitacao") %>',
				obterAssinanteCargos: '<%= Url.Action("ObterAssinanteCargos", "Documento") %>',
				obterAssinanteFuncionarios: '<%= Url.Action("ObterAssinanteFuncionarios", "Documento") %>',
				salvar: '<%= Url.Action("Editar", "Documento") %>'
			},
			Mensagens: <%= Model.Mensagens %>,
			configuracoesProtocoloTipos: <%= Model.ConfiguracoesProtocoloTiposJson %>,
			modo: 2 //editar
		});
	});
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central" class="documentoEditar">

		<h1 class="titTela">Editar Documento</h1>
		<br />

		<div class="documentoPartial">
			<% Html.RenderPartial("DocumentoPartial"); %>
		</div>

		<div class="block box btnDocumentoContainer">
			<input class="btnDocumentoSalvar floatLeft" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>