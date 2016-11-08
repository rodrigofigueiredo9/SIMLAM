<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMEmpreendimento" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Empreendimentos</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Papel/listar.js") %>"></script>

	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Empreendimento/listar.js") %>" ></script>
	<script type="text/javascript">
		$(function () {
			EmpreendimentoListar.urlEditar = '<%= Url.Action("Editar", "Empreendimento") %>';
			EmpreendimentoListar.urlConfirmarExcluir = '<%= Url.Action("ExcluirConfirm", "Empreendimento") %>';
			EmpreendimentoListar.urlExcluir = '<%= Url.Action("Excluir", "Empreendimento") %>';
			EmpreendimentoListar.urlCaracterizacao = '<%= Url.Action("Caracterizacao", "Empreendimento") %>';
			EmpreendimentoListar.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros"); %>
	</div>
</asp:Content>