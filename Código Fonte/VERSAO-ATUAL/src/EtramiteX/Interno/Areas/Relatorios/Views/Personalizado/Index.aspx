<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Relatorios.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<PersonalizadoListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Relatórios Personalizados</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/Relatorios/personalizadoListar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>

	<script>
		$(function () {
			PersonalizadoListar.urlExecutar = '<%= Url.Action("Executar", "Personalizado") %>';
			PersonalizadoListar.urlEditar = '<%= Url.Action("Editar", "Personalizado") %>';
			PersonalizadoListar.urlExcluirConfirm = '<%= Url.Action("ExcluirConfirm", "Personalizado") %>';
			PersonalizadoListar.urlExcluir = '<%= Url.Action("Excluir", "Personalizado") %>';
			PersonalizadoListar.urlExportar = '<%= Url.Action("Exportar", "Personalizado") %>';
			PersonalizadoListar.urlAtribuirExecutor = '<%= Url.Action("AtribuirExecutor", "Personalizado") %>';
			PersonalizadoListar.load($('#central'));

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])){ %>
				ContainerAcoes.load($(".containerAcoes"), {
					urls: {
						urlAtribuirExecutor: '<%= Url.Action("AtribuirExecutor", "Personalizado", new { id = Request.Params["acaoId"].ToString() }) %>',
						urlGerarRelatorio: '<%= Url.Action("Executar", "Personalizado", new { id = Request.Params["acaoId"].ToString() }) %>',
						urlEditar: '<%= Url.Action("Editar", "Personalizado", new { id = Request.Params["acaoId"].ToString() }) %>'
					}
				});
			<%}%>
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros"); %>
	</div>
</asp:Content>