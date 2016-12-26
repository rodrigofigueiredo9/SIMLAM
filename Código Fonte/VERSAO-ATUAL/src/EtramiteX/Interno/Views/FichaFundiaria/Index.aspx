<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFichaFundiaria" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Fichas Fundiárias</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/FichaFundiaria/listar.js") %>"></script>
	<script>
		$(function () {
			FichaFundiariaListar.load($('#central'), {
				urls: {
					visualizar: '<%= Url.Action("Visualizar", "FichaFundiaria") %>',
					editar: '<%= Url.Action("Editar", "FichaFundiaria") %>',
					excluir: '<%= Url.Action("Excluir", "FichaFundiaria") %>',
					excluirConfirm: '<%= Url.Action("ExcluirConfirm", "FichaFundiaria") %>'
				}

			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros", Model); %>
	</div>
</asp:Content>