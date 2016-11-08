<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Arquivos</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Tramitacao/arquivoListar.js") %>" ></script>
	<script type="text/javascript">
		$(function () {
			TramitacaoArquivoListar.urlConfirmarExcluir = '<%= Url.Action("ArquivoConfirmarExcluir", "Tramitacao") %>';
			TramitacaoArquivoListar.urlExcluir = '<%= Url.Action("ArquivoExcluir", "Tramitacao") %>';
			TramitacaoArquivoListar.urlEditar = '<%= Url.Action("ArquivoEditar", "Tramitacao") %>';
			TramitacaoArquivoListar.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ArquivoListarFiltros"); %>
	</div>
</asp:Content>