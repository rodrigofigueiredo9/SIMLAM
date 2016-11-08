<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProcesso" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarVM>" %>

<!-- DEPENDENCIAS DE PESSOA -->
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/pessoa.js") %>"></script>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>"></script>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/representante.js") %>"></script>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/profissao.js") %>"></script>
<!-- FIM DEPENDENCIAS DE PESSOA -->

<!-- DEPENDENCIAS DE EMPREENDIMENTO -->
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Empreendimento/empreendimento.js") %>"></script>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Empreendimento/associar.js") %>"></script>
<!-- FIM DEPENDENCIAS DE EMPREENDIMENTO -->

<script type="text/javascript" src="<%= Url.Content("~/Scripts/Requerimento/atividadeSolicitadaAssociar.js") %>"></script>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Processo/processo.js") %>"></script>

<script type="text/javascript">
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
	<% Html.RenderPartial("VisualizarPartial"); %>
</div>