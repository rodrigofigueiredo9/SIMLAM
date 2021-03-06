﻿<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProtocolo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DocumentoVM>" %>

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

<script type="text/javascript" src="<%= Url.Content("~/Scripts/Documento/documento.js") %>"></script>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Requerimento/atividadeSolicitadaAssociar.js") %>"></script>

<script type="text/javascript">
	
	var modalDocVisObj = new Documento();
	
	$(function () {
		AtividadeSolicitadaAssociar.load($('.divDocVisualizar'));
		
		modalDocVisObj.load($('.divDocVisualizar'), {
			urls: {
				editarInteressado: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
				editarEmpreendimento: '<%= Url.Action("EmpreendimentoModalVisualizar", "Empreendimento") %>',
				editarResponsavelModal: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
				visualizarProcesso: '<%= Url.Action("Visualizar", "Processo") %>',
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

<div class="divDocVisualizar documentoPartial">

	<h1 class="titTela">Visualizar Documento</h1>
	<br />
	
	<% Html.RenderPartial("DocumentoVisualizarPartial", Model); %>

</div>