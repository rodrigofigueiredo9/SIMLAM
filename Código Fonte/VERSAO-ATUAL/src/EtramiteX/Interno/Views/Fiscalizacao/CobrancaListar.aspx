<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarCobrancasVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Controle de Cobrança</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
   <script src="<%= Url.Content("~/Scripts/Fiscalizacao/cobrancaListar.js") %>" type="text/javascript"></script>
	<script type="text/javascript">
		$(function () {
			CobrancaListar.load($('#central'), {
				urls: {
					urlEditar: '<%= Url.Action("Cobranca", "Fiscalizacao") %>',
					urlVisualizar: '<%= Url.Action("CobrancaVisualizar", "Fiscalizacao") %>'
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

