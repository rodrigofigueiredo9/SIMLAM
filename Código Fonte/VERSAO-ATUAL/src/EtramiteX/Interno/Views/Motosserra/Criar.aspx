<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMMotosserra" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<MotosserraVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Cadastrar Motosserra</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<!-- DEPENDENCIAS DE PESSOA -->
	<script src="<%= Url.Content("~/Scripts/Pessoa/pessoa.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Pessoa/representante.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Pessoa/profissao.js") %>"></script>
	<!-- FIM DEPENDENCIAS DE PESSOA -->

	<script src="<%= Url.Content("~/Scripts/Motosserra/motosserra.js") %>"></script>
	<script>
		$(function () {
			MotosserraVerificar.load($('#central'), {
				urls: {
					associarPessoa: '<%= Url.Action("PessoaModal", "Pessoa") %>',
					visualizarPessoa: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
					salvar: '<%= Url.Action("Criar", "Motosserra") %>',
					verificar: '<%= Url.Action("Verificar", "Motosserra") %>',
					obterPartialCriar: '<%= Url.Action("ObterPartialCriar", "Motosserra") %>',
					visualizarMotosserra: '<%= Url.Action("Visualizar", "Motosserra") %>',
					editarMotosserra: '<%= Url.Action("Editar", "Motosserra") %>',
					validarEditar: '<%= Url.Action("ValidarEditar", "Motosserra") %>',
					associarPessoa: '<%= Url.Action("PessoaModal", "Pessoa") %>',
					visualizarPessoa: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Cadastrar Motosserra</h1>
		<br />

		<% Html.RenderPartial("VerificarPartial"); %>

		<div class="block box">
			<input class="btnMotosserraSalvar hide floatLeft" type="button" value="Salvar" />
			<input class="btnNovo floatLeft hide" type="button" value="Novo" />
			<span class="cancelarCaixa"><span class="btnModalOu hide">ou</span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>