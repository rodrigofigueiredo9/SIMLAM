<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMDocumento" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<SalvarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Cadastrar Documento</asp:Content>

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
	var DocumentoObj = new Documento();

	$(function () {
		// visualizar de checagem de pendencia
		ChecagemPendencia.settings.urls.pdfTitulo = '<%= Url.Action("GerarPdf", "Titulo") %>';

		AtividadeSolicitadaAssociar.urlAbriModalAtividade = '<%= Url.Action("ObterFinalidade", "Atividade") %>';
		FinalidadeAssociar.urlObterTituloModelo = '<%= Url.Action("ObterTituloModelo", "Requerimento") %>';
		FinalidadeAssociar.urlObterTituloModelo = '<%= Url.Action("ObterTituloModelo", "Requerimento") %>';

		FinalidadeAssociar.urlObterTituloModeloAnterior = '<%= Url.Action("ObterTituloModeloAnterior", "Requerimento") %>';
		AtividadeSolicitadaAssociar.Mensagens = <%= Model.Mensagens %>;
		FinalidadeAssociar.Mensagens = <%= Model.Mensagens %>;
		AtividadeSolicitadaAssociar.load($('#central'));

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

			    visualizarProcesso: '<%= Url.Action("Visualizar", "Processo") %>',
				visualizarDocumento: '<%= Url.Action("Visualizar", "Documento") %>',
				visualizarCheckList: '<%= Url.Action("ChecagemRoteiroVisualizar", "ChecagemRoteiro") %>',
				visualizarChecagemPendencia: '<%= Url.Action("Visualizar", "ChecagemPendencia") %>',
				obterRequerimento: '<%= Url.Action("ObterRequerimento", "Documento") %>',
				pdfRequerimento: '<%= Url.Action("GerarPdf", "Requerimento") %>',
				associarInteressado: '<%= Url.Action("PessoaModal", "Pessoa") %>',
				editarInteressado: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
				editarEmpreendimento: '<%= Url.Action("EmpreendimentoModalVisualizar", "Empreendimento") %>',
				associarResponsavelModal: '<%= Url.Action("PessoaModal", "Pessoa") %>',
				editarResponsavelModal: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
				validarChecagem: '<%= Url.Action("ValidarAssociacaoChecagem", "Documento") %>',
				validarChecagemPendencia: '<%= Url.Action("ValidarAssociacaoChecagemPendencia", "Documento") %>',
				validarChecagemTemTituloPendencia: '<%= Url.Action("ValidarChecagemTemTituloPendencia", "Documento") %>',
				obterProcesso: '<%= Url.Action("ObterProcesso", "Processo") %>',
				salvar: '<%= Url.Action("Criar", "Documento") %>'
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
	<div id="central" class="documentoCriar">

		<h1 class="titTela">Cadastrar Documento</h1>
		<br />

		<div class="documentoPartial">
			<% Html.RenderPartial("DocumentoPartial"); %>
		</div>

		<div class="block box btnDocumentoContainer">
			<span class="spnDocumentoSalvar"><input class="btnDocumentoSalvar floatLeft" type="button" value="Salvar" /></span>
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>