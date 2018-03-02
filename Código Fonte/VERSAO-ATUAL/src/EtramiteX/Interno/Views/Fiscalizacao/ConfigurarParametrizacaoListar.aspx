<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ParametrizacaoListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Parametrização Financeira</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Fiscalizacao/fiscalizacaoConfiguracaoListarParametrizacao.js") %>" type="text/javascript"></script>
	<script type="text/javascript">
		$(function () {
			ConfigurarListarParametrizacao.load($('#central'), {
				urls: {
					urlEditar: '<%= Url.Action("ConfigurarParametrizacao", "Fiscalizacao") %>',
					urlVisualizar: '<%= Url.Action("ConfigurarParametrizacaoVisualizar", "Fiscalizacao") %>',
					urlExcluirConfirm: '<%= Url.Action("ExcluirParametrizacaoConfirm", "Fiscalizacao") %>',
					urlExcluir: '<%= Url.Action("ExcluirParametrizacao", "Fiscalizacao") %>',
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<%Html.RenderPartial("ConfigurarParametrizacaoListarFiltros", Model);%>
	</div>
</asp:Content>
