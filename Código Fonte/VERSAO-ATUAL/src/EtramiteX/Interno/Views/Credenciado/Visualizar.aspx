<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCredenciado" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<CredenciadoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Dados de Credenciado</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Pessoa/representante.js") %>" ></script>
	<script src="<%= Url.Content("~/Scripts/Pessoa/profissao.js") %>"></script>

	<script>
		$(function () {
			CredenciadoTela.load($('#central'), {
				urls: {
					modalAssociarProfissao: '<%= Url.Action("Associar", "Profissao") %>',
					modalAssociarRepresentante: '<%= Url.Action("RepresentanteAssociar", "Pessoa") %>',
					visualizarModal: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
					editar: '<% = Url.Action("AlterarDados", "Credenciado") %>'
				},
				msgs: <%= Model.PessoaVM.Mensagens %>
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Visualizar Credenciado</h1>
		<br />

		<% Html.RenderPartial("CredenciadoPartial", Model); %>

		<div class="block box">
			<span class="cancelarCaixa"><a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>