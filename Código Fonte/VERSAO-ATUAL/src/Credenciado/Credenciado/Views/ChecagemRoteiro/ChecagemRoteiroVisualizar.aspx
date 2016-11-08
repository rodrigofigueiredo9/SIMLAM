<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMChecagemRoteiro" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<SalvarCheckListRoteiroVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Checagem de Itens de Roteiro</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div id="central">
	<% Html.RenderPartial("ChecagemRoteiroVisualizarPartial"); %> 

	<div class="block box">
		<span class="cancelarCaixa"><a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
	</div>
</div>
</asp:Content>