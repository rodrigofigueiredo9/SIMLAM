<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProtocolo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ProcessoVM>" %>

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

<script src="<%= Url.Content("~/Scripts/Requerimento/atividadeSolicitadaAssociar.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Processo/processo.js") %>"></script>

<script>
	$(function () {
		AtividadeSolicitadaAssociar.load($('.processoPartial'));
		Processo.load($('.processoPartial'), {
			urls: {
				editarInteressado: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
				editarEmpreendimento: '<%= Url.Action("EmpreendimentoModalVisualizar", "Empreendimento") %>',
				editarResponsavelModal: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
				visualizarCheckList: '<%= Url.Action("ChecagemRoteiroVisualizar", "ChecagemRoteiro") %>',
				visualizarChecagemPendencia: '<%= Url.Action("Visualizar", "ChecagemPendencia") %>',
				visualizarFiscalizacao: '<%= Url.Action("VisualizarFiscalizacaoModal", "Fiscalizacao") %>',
				pdfRequerimento: '<%= Url.Action("GerarPdf", "Requerimento") %>'
			},
			Mensagens: <%= Model.Mensagens %>,
			modo: 3 //visualizar
		});
	});
</script>

<h1 class="titTela">Visualizar Processo</h1>
<br />

<div class="processoPartial">
	<% Html.RenderPartial("ProcessoVisualizarPartial", Model); %>
</div>