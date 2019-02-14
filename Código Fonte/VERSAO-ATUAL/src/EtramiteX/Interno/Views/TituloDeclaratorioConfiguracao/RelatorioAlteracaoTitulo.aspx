<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTituloDeclaratorioConfiguracao" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Relatório de Alteração de Título Declaratório</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/TituloDeclaratorioConfiguracao/configurar.js") %>"></script>
	<script type="text/javascript">
		$(function () {
			TituloDeclaratorioConfiguracao.load($('#central'), {
				urls: {
					salvar: '<%= Url.Action("Salvar", "TituloDeclaratorioConfiguracao") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<%Html.RenderPartial("ListarFiltros", Model);%>
	</div>
</asp:Content>