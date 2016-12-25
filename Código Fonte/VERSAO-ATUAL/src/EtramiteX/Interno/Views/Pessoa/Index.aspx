<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPessoa" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Pessoas</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Pessoa/listar.js") %>"></script>

	<script>
		$(function () {
			PessoaListar.urlExcluirConfirm = '<%= Url.Action("ExcluirConfirm", "Pessoa") %>';
			PessoaListar.urlExcluir = '<%= Url.Action("Excluir", "Pessoa") %>';
			PessoaListar.urlEditar = '<%= Url.Action("Editar", "Pessoa") %>';
			PessoaListar.urlVisualizar = '<%= Url.Action("Visualizar", "Pessoa") %>';
			PessoaListar.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros", Model); %>
	</div>
</asp:Content>