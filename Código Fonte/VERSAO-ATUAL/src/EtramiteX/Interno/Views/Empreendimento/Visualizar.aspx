<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMEmpreendimento" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<SalvarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Empreendimento</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("VisualizarPartial"); %>
		
		<div class="block box">
			<div class="block">
				<span class="cancelarCaixa"><a class="linkCancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
			</div>
		</div>
	</div>
</asp:Content>