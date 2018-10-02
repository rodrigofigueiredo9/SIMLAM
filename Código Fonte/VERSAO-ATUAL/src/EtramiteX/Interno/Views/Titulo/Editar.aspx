<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<SalvarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Editar Título</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">

	<!-- DEPENDENCIAS DE ASSOCIAR -->
	<script src="<%= Url.Content("~/Scripts/Processo/listar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Documento/listar.js") %>"></script>

	<script src="<%= Url.Content("~/Scripts/Empreendimento/listar.js") %>"></script>
	<!-- FIM DEPENDENCIAS DE ASSOCIAR -->

	<script src="<%= Url.Content("~/Scripts/Titulo/titulo.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/Titulo/condicionanteSalvar.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/Titulo/condicionanteDescricaoSalvar.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/Titulo/condicionanteVisualizar.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/Titulo/tituloCondicionante.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/Titulo/atividadeEspecificidade.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/Titulo/tituloLaudoExploracaoFlorestal.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/Titulo/tituloAutorizacaoExploracaoFlorestal.js") %>" ></script>

	<script>
		$(function () {
			Titulo.load($('#central'), {
				urls: {
					modelosCadastrados: '<%= Url.Action("ObterModelosCadastradosSetor", "Titulo") %>',
					associarProcesso: '<%= Url.Action("Associar", "Processo") %>',
					associarDocumento: '<%= Url.Action("Associar", "Documento") %>',
					validarAssociarProcDoc: '<%= Url.Action("ValidarAssociarProtocolo", "Titulo") %>',
					obterProtocolo: '<%= Url.Action("ObterProtocolo", "Titulo") %>',
					associarEmpreendimento: '<%= Url.Action("Associar", "Empreendimento") %>',
					especificidade: '<%= Url.Action("Salvar", "Especificidade", new {area="Especificidades"}) %>',
					tituloProtocolo: '<%= Url.Action("TituloProtocolo", "Titulo") %>',
					tituloCamposModelo: '<%= Url.Action("TituloCamposModelo", "Titulo") %>',
					tituloSalvar: '<%= Url.Action("Salvar", "Titulo") %>',
					condicionanteCriar: '<%= Url.Action("CondicionanteCriar", "Titulo") %>',
					condicionanteEditar: '<%= Url.Action("CondicionanteEditar", "Titulo") %>',
					condicionanteSituacaoAlterar: '<%= Url.Action("CondicionanteSituacaoAlterar", "Titulo") %>',
					salvar: '<%= Url.Action("Salvar", "Titulo") %>',
					redirecionar: '<%= Url.Action("Index", "Titulo") %>',
					obterDestinatarioEmails: '<%= Url.Action("ObterDestinatarioEmails", "Titulo") %>',
					enviarArquivo: '<%= Url.Action("Arquivo", "Arquivo") %>',
					obterAssinanteCargos: '<%= Url.Action("ObterAssinanteCargos", "Titulo") %>',
					obterAssinanteFuncionarios: '<%= Url.Action("ObterAssinanteFuncionarios", "Titulo") %>'
				},
				Mensagens: <%= Model.Mensagens %>,
				obterCondicionantesFunc: TituloCondicionante.obterCondicionantes,
				procDocContemEmp: <%= Model.TemEmpreendimento.ToString().ToLower() %>,
				carregarEspecificidade: <%= Model.Titulo.Modelo.PossuiEspecificidade().ToString().ToLower() %>,
				protocoloSelecionado: '<%= Model.ProtocoloSelecionado %>',
				isEditar: true
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Editar Título</h1>
		<br />
		<% Html.RenderPartial("TituloPartial"); %>
	</div>
</asp:Content>