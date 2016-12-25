<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ConfiguracaoListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Configurações</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Fiscalizacao/fiscalizacaoConfiguracaoListar.js") %>" ></script>
	<script>
		$(function () {
			FiscalizacaoConfiguracaoListar.load($('#central'), {
				urls: {
					urlExcluirConfirm: '<%= Url.Action("ConfiguracaoExcluirConfirm", "Fiscalizacao") %>',
					urlEditar: '<%= Url.Action("ConfiguracaoSalvar", "Fiscalizacao") %>',
					urlVisualizar: '<%= Url.Action("ConfiguracaoVisualizar", "Fiscalizacao") %>',
					urlExcluir: '<%= Url.Action("ConfiguracaoExcluir", "Fiscalizacao") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<%Html.RenderPartial("ConfiguracaoListarFiltros", Model);%>
	</div>
</asp:Content>