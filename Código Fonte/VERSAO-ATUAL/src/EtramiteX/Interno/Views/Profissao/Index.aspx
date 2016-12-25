<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProfissao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ProfissaoListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Profissão</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Profissao/listar.js") %>"></script>

	<script>
		$(function () {
			ProfissaoListar.load($('#central'), {
				urls: {
					criar: '<%= Url.Action("Criar", "Profissao") %>',
					editar: '<%= Url.Action("Editar", "Profissao") %>',
					salvar: '<%= Url.Action("Salvar", "Profissao") %>',
					visualizar: '<%= Url.Action("Visualizar", "Profissao") %>'
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