<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarCobrancasVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Controle de Cobrança</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
    <script src="<%= Url.Content("~/Scripts/Fiscalizacao/cobrancaListar.js") %>"></script>
    <script>
		$(function () {
			CobrancaListar.load($('#central'), {
				urls: {
					urlEditar: '<%= Url.Action("Cobranca", "FiscalizacaoCobranca") %>',
					urlVisualizar: '<%= Url.Action("CobrancaVisualizar", "FiscalizacaoCobranca") %>'
				}
			});
		});
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="central">
        <%Html.RenderPartial("CobrancaListarFiltros", Model);%>
    </div>
</asp:Content>

