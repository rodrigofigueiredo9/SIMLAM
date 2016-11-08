<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMPessoa" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Publico.Master" Inherits="System.Web.Mvc.ViewPage<PessoaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Cadastrar Credenciado</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/tela.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/pessoa.js") %>" ></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/profissao.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			var pessoaTela = new PessoaTela();

			pessoaTela.load($('#central'), {
				urls: {
					modalAssociarProfissao: '<%= Url.Action("Associar", "Profissao") %>',
					modalAssociarRepresentante: '<%= Url.Action("PessoaModal", "Pessoa") %>',
					verificar: '<%= Url.Action("CriarVerificarCpfCnpj", "Credenciado") %>',
					limpar: '<%= Url.Action("Criar", "Credenciado") %>',
					visualizarModal: '<%= Url.Action("PessoaModalVisualizarObjeto", "Pessoa") %>',
					criar: '<%= Url.Action("Criar", "Credenciado") %>',
					obterOrgaoParceiroUnidades: '<%= Url.Action("ObterOrgaoParceiroUnidades", "Pessoa") %>',
					pessoaModalVisualizar: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
					pessoaModalVisualizarConjuge: '<%= Url.Action("PessoaModalVisualizarConjuge", "Pessoa") %>'
				},
				modoCriar: true,
				msgs: <%= Model.Mensagens %>
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("Mensagem"); %>
		<h1 class="titTela">Verificar CPF/CNPJ</h1>
		<br />

		<% Html.RenderPartial("CredenciadoPartial", Model); %>

		<div class="block box divPessoaContainer">
			<input class="btnPessoaSalvar floatLeft <%= (Model.CpfCnpjValido ? "" : "hide") %>" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu <%= (Model.CpfCnpjValido ? "" : "hide") %>">ou</span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("Criar", "Credenciado") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>