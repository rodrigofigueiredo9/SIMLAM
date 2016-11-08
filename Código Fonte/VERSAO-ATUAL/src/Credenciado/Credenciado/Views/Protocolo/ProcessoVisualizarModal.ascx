<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProtocolo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ProcessoVM>" %>

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
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Processo/processo.js") %>"></script>

<script type="text/javascript">
	$(function () {
		AtividadeSolicitadaAssociar.load($('#central'));
		Processo.load($('#ProcessoModalContainer'), {
			urls: {
				editarInteressado: '<%= Url.Action("PessoaInternoModalVisualizar", "Pessoa") %>',
				editarEmpreendimento: '<%= Url.Action("VisualizarInterno", "Empreendimento") %>',
				editarResponsavelModal: '<%= Url.Action("PessoaInternoModalVisualizar", "Pessoa") %>',
				visualizarCheckList: '<%= Url.Action("ChecagemRoteiroVisualizar", "ChecagemRoteiro") %>',
				visualizarChecagemPendencia: '<%= Url.Action("Visualizar", "ChecagemPendencia") %>',
				pdfRequerimento: '<%= Url.Action("GerarPdfInterno", "Requerimento") %>'
			},
			Mensagens: <%= Model.Mensagens %>,
			modo: 3 //visualizar
		});
	});
</script>

<div id="ProcessoModalContainer">
	<h1 class="titTela">Visualizar Processo</h1>
	<br />

	<% Html.RenderPartial("ProcessoVisualizarPartial"); %>
</div>