<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<CobrancaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Cobrança</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Fiscalizacao/cobranca.js") %>" type="text/javascript"></script>
	<script type="text/javascript">
		$(function () {
			Cobranca.load($('#central'), {
				urls: {
					visualizar: '<%= Url.Action("CobrancaVisualizar", "Fiscalizacao") %>',
					carregar: '<%= Url.Action("Cobranca", "Fiscalizacao") %>',
					cancelar: '<%= Url.Action("NotificacaoVisualizar", "Fiscalizacao") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="central">
        <h1 class="titTela">Visualizar Cobrança</h1>
        <br />

        <% Html.RenderPartial("CobrancaPartial", Model); %>
    </div>
</asp:Content>