﻿<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPessoa" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<SalvarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Pessoa</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/pessoa.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/tela.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/representante.js") %>" ></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Pessoa/profissao.js") %>"></script>
	<script type="text/javascript">
		$(function () {
			
			var pessoaTelaObj = new PessoaTela();

			pessoaTelaObj.load($('#central'), {
				urls: {
					modalAssociarProfissao: '<%= Url.Action("Associar", "Profissao") %>',
					modalAssociarRepresentante: '<%= Url.Action("RepresentanteAssociar", "Pessoa") %>',
					verificar: '<% = Url.Action("CriarVerificarCpfCnpj", "Pessoa") %>',
					limpar: '<%= Url.Action("Criar", "Pessoa") %>',
					visualizar: '<% = Url.Action("Visualizar", "Pessoa") %>',
					visualizarModal: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
					criar: '<% = Url.Action("Criar", "Pessoa") %>',
					editar: '<% = Url.Action("Editar", "Pessoa") %>',
					pessoaModal: '<%= Url.Action("PessoaModal", "Pessoa") %>',
					pessoaModalVisualizar: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
					obterEnderecoPessoa: '<%= Url.Action("ObterEndereco", "Pessoa") %>'
				},
				msgs: <%= Model.Mensagens %>
			});
		}); 
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div id="central">
	<h1 class="titTela">Editar Pessoa</h1>
	<br />

	<% Html.RenderPartial("PessoaPartial", Model); %>

	<div class="block box divPessoaContainer">
		<input class="btnPessoaSalvar floatLeft" type="button" value="Salvar" />
		<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
	</div>
</div>


</asp:Content>
