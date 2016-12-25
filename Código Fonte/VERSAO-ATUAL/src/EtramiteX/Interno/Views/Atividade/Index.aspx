<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAtividade" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarConfiguracaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Configurações de Atividade Solicitada</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Atividade/atividadeConfiguracaoListar.js") %>" ></script>

	<script>
	    $(function () {
	        AtividadeConfiguracaoListar.urlExcluirConfirm = '<%= Url.Action("ExcluirConfirm", "Atividade") %>';
	        AtividadeConfiguracaoListar.urlExcluir = '<%= Url.Action("Excluir", "Atividade") %>';
	        AtividadeConfiguracaoListar.urlEditar = '<%= Url.Action("ConfiguracaoEditar", "Atividade") %>';

	        AtividadeConfiguracaoListar.load($('#central'));
	    });
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("AtividadeConfiguracaoFiltros"); %>
	</div>
</asp:Content>