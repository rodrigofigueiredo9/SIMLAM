<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCredenciado" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Credenciados
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<%--<script src="<%= Url.Content("~/Scripts/jquery.listar-grid.js") %>"></script>--%>
    <script src="<%= Url.Content("~/Scripts/Credenciado/CredenciadoListar.js") %>"></script>

	<script>

		$(function () {
			CredenciadoListar.Mensagens = <%= Model.Mensagens %>;
			CredenciadoListar.visualizarLink = '<%= Url.Action("Visualizar", "Credenciado") %>';
			CredenciadoListar.regerarChaveLink = '<%= Url.Action("RegerarChave", "Credenciado") %>';
		    CredenciadoListar.alterarSituacaoLink = '<%= Url.Action("AlterarSituacao", "Credenciado") %>';
		    CredenciadoListar.editarHabilitarLink = '<%= Url.Action("AlterarHabilitarEmissaoCFOCFOC", "Credenciado") %>';
		    CredenciadoListar.obterCredenciadoHabilitar = '<%= Url.Action("ObterCredenciadoHabilitar", "Credenciado") %>';
		    CredenciadoListar.salvarHabilitarEmissaoLink = '<%= Url.Action("SalvarHabilitarEmissao", "Credenciado") %>';
			CredenciadoListar.load($('#central'));
		});
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros", Model); %>
	</div>
</asp:Content>
