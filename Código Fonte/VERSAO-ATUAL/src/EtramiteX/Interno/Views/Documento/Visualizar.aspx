<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMDocumento" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<SalvarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Documento</asp:Content>

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

	<script src="<%= Url.Content("~/Scripts/Documento/documento.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Requerimento/atividadeSolicitadaAssociar.js") %>"></script>

	<script>
		$(function () {
			AtividadeSolicitadaAssociar.load($('#central'));

			var DocumentoVisObj = new Documento();

			DocumentoVisObj.load($('#central'), {
				urls: {
					editarInteressado: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
					editarEmpreendimento: '<%= Url.Action("EmpreendimentoModalVisualizar", "Empreendimento") %>',
					editarResponsavelModal: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
					visualizarProcesso: '<%= Url.Action("Visualizar", "Processo") %>',
					visualizarDocumento: '<%= Url.Action("Visualizar", "Documento") %>',
					visualizarCheckList: '<%= Url.Action("ChecagemRoteiroVisualizar", "ChecagemRoteiro") %>',
				    visualizarChecagemPendencia: '<%= Url.Action("Visualizar", "ChecagemPendencia") %>',
				    visualizarFiscalizacao: '<%= Url.Action("VisualizarFiscalizacaoModal", "Fiscalizacao") %>',
					funcionariosDestinatario: '<%= Url.Action("ObterFuncionariosDoSetor", "Tramitacao") %>',
					obterAssinanteCargos: '<%= Url.Action("ObterAssinanteCargos", "Documento") %>',
					obterAssinanteFuncionarios: '<%= Url.Action("ObterAssinanteFuncionarios", "Documento") %>',
					pdfRequerimento: '<%= Url.Action("GerarPdf", "Requerimento") %>'
				},
				Mensagens: <%= Model.Mensagens %>,
				modo: 3 //visualizar
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Visualizar Documento</h1>
		<br />

		<% Html.RenderPartial("VisualizarPartial"); %>

		<div class="block box btnDocumentoContainer">
			<span class="cancelarCaixa"><a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>