<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPessoa" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<SalvarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Pessoa</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Pessoa/tela.js") %>"></script>
	<script>
		$(function() {

			var pessoaTelaObj = new PessoaTela();

			pessoaTelaObj.Mensagens = <%= Model.Mensagens %>;
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Visualizar Pessoa</h1>
		<br />

		<% Html.RenderPartial("PessoaPartialVisualizar", Model); %>

		<div class="block box">
			<span class="cancelarCaixa"><a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>
