<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMOrgaosParceirosConveniados" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<OrgaoParceiroConveniadoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Visualizar
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="central">
        <h1 class="titTela">Visualizar Órgão Parceiro/ Conveniado</h1>
        <br />
        <%Html.RenderPartial("OrgaosParceirosConveniados", Model); %>
        
        <div class="block box botoesSalvarCancelar">
	        <div class="block">
		        <span class="cancelarCaixa"><a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		    </div>
	    </div>
    </div>

</asp:Content>
