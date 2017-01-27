<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMMotosserra" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<MotosserraVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Editar Motosserra</asp:Content>

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
			Motosserra.load($('#central'), {
				urls: {
					associarPessoa: '<%= Url.Action("PessoaModal", "Pessoa") %>',
					visualizarPessoa: '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>',
					salvar: '<%= Url.Action("Editar", "Motosserra") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Editar Motosserra</h1>
		<br />

		<% Html.RenderPartial("VerificarPartial"); %>

		<div class="block box">
			<input class="btnMotosserraSalvar floatLeft" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>