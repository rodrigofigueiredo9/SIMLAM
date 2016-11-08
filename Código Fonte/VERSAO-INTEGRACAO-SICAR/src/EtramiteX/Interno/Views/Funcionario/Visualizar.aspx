<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFuncionario" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<VisualizarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Funcionário</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

	<div id="central">
		<% Html.RenderPartial("VisualizarPartial"); %> 

		<div class="block box">
			<a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a>
		</div>
	</div>
</asp:Content>