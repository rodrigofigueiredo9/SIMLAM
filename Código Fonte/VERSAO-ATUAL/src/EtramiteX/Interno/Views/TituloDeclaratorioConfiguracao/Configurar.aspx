<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTItuloDeclaratorioConfiguracao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<SalvarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Configurar Caracterização de Barragem Dispensada de Licenciamento Ambiental</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/TituloDeclaratorioConfiguracao/configurar.js") %>"></script>

	<script>
		$(function () {
			TituloDeclaratorioConfiguracao.load($('#central'), {
				urls: {
					modelosCadastrados: '<%= Url.Action("ObterModelosCadastradosSetor", "Titulo") %>',
					associarRequerimento: '<%= Url.Action("Associar", "Requerimento") %>',
					validarAssociarRequerimento: '<%= Url.Action("ValidarAssociarRequerimento", "TituloDeclaratorio") %>',
					tituloCamposModelo: '<%= Url.Action("TituloCamposModelo", "TituloDeclaratorio") %>',
					obterAssinanteCargos: '<%= Url.Action("ObterAssinanteCargos", "TituloDeclaratorio") %>',
					obterAssinanteFuncionarios: '<%= Url.Action("ObterAssinanteFuncionarios", "TituloDeclaratorio") %>',
					especificidade: '<%= Url.Action("Salvar", "Especificidade", new {area="Especificidades"}) %>',

					redirecionar: '<%= Url.Action("Criar", "TituloDeclaratorio") %>',
					salvar: '<%= Url.Action("Salvar", "TituloDeclaratorio") %>'
				},
				Mensagens: <%= Model.Mensagens %>,
				carregarEspecificidade: <%= Model.CarregarEspecificidade.ToString().ToLower() %>
			});
		});
	</script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Configurar Caracterização de Barragem Dispensada de Licenciamento Ambiental</h1>
		<br />
		
	</div>
</asp:Content>