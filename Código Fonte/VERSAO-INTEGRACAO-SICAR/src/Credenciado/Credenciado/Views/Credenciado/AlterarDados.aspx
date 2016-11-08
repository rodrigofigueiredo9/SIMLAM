<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCredenciado" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<CredenciadoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Alterar Dados de Credenciado</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/tela.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/pessoa.js") %>" ></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/profissao.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			var pessoaTelaObj = new PessoaTela();

			pessoaTelaObj.load($('#central'), {
				urls: {
					modalAssociarProfissao: '<%= Url.Action("Associar", "Profissao") %>',
					visualizar: '<% = Url.Action("Visualizar", "Pessoa") %>',
					pessoaModal: '<%= Url.Action("PessoaModal", "Pessoa") %>',
					pessoaModalVisualizar: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
					editar: '<% = Url.Action("AlterarDados", "Credenciado") %>'
				},
				msgs: <%= Model.PessoaVM.Mensagens %>,
				editarVisualizar: true
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Alterar Dados de Credenciado</h1>
		<br />
		
		<% Html.RenderPartial("~/Views/Credenciado/CredenciadoPartial.ascx", Model); %>

		<div class="block box divPessoaContainer">
			<input class="btnPessoaSalvar floatLeft" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou </span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("Index", "Home") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>