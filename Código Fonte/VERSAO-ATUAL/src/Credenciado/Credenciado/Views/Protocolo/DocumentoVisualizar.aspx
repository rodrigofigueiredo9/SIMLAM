<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProtocolo" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<DocumentoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Documento</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<!-- DEPENDENCIAS DE PESSOA -->
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/pessoa.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/profissao.js") %>"></script>
	<!-- FIM DEPENDENCIAS DE PESSOA -->

	<!-- DEPENDENCIAS DE EMPREENDIMENTO -->
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Empreendimento/empreendimento.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Empreendimento/associar.js") %>"></script>
	<!-- FIM DEPENDENCIAS DE EMPREENDIMENTO -->

	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Requerimento/atividadeSolicitadaAssociar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Documento/documento.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			AtividadeSolicitadaAssociar.load($('#central'));
			Documento.load($('.divModalDocumento'), {
				urls: {
					editarInteressado: '<%= Url.Action("PessoaInternoModalVisualizar", "Pessoa") %>',
					editarEmpreendimento: '<%= Url.Action("VisualizarInterno", "Empreendimento") %>',
					editarResponsavelModal: '<%= Url.Action("PessoaInternoModalVisualizar", "Pessoa") %>',
					visualizarProcesso: '<%= Url.Action("VisualizarProcesso", "Protocolo") %>',
					visualizarCheckList: '<%= Url.Action("ChecagemRoteiroVisualizar", "ChecagemRoteiro") %>',
					visualizarChecagemPendencia: '<%= Url.Action("Visualizar", "ChecagemPendencia") %>',
					pdfRequerimento: '<%= Url.Action("GerarPdfInterno", "Requerimento") %>'
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

		<% Html.RenderPartial("DocumentoVisualizarPartial"); %>

		<div class="block box btnDocumentoContainer">
			<span class="cancelarCaixa"><a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>