﻿<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProtocolo" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ProcessoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Processo</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<!-- DEPENDENCIAS DE PESSOA -->
	<%--<script src="<%= Url.Content("~/Scripts/Pessoa/pessoa.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Pessoa/representante.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Pessoa/profissao.js") %>"></script>-%>
	<!-- FIM DEPENDENCIAS DE PESSOA -->

	<!-- DEPENDENCIAS DE EMPREENDIMENTO -->
	<%--<script src="<%= Url.Content("~/Scripts/Empreendimento/empreendimento.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Empreendimento/associar.js") %>"></script>--%>
	<!-- FIM DEPENDENCIAS DE EMPREENDIMENTO -->

	<%--<script src="<%= Url.Content("~/Scripts/Requerimento/atividadeSolicitadaAssociar.js") %>"></script>--%>
	<script src="<%= Url.Content("~/Scripts/Processo/processo.js") %>"></script>

	<script>
		$(function () {
//			AtividadeSolicitadaAssociar.load($('#central'));
			Processo.load($('#central'), {
				urls: {
					editarInteressado: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
					editarEmpreendimento: '<%= Url.Action("EmpreendimentoModalVisualizar", "Empreendimento") %>',
					editarResponsavelModal: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
					visualizarCheckList: '<%= Url.Action("ChecagemRoteiroVisualizar", "ChecagemRoteiro") %>',
					visualizarChecagemPendencia: '<%= Url.Action("Visualizar", "ChecagemPendencia") %>',
					pdfRequerimento: '<%= Url.Action("GerarPdf", "Requerimento") %>'
				},
				Mensagens: <%= Model.Mensagens %>,
				modo: 3 //visualizar
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("Mensagem"); %>
		<h1 class="titTela">Visualizar Processo</h1>
		<br />

		<% Html.RenderPartial("ProcessoVisualizarPartial"); %>

		<div class="block box btnProcessoContainer">
			<span class="cancelarCaixa"><a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>