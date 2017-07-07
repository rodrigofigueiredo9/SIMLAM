<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMAutenticacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Login.Master" Inherits="System.Web.Mvc.ViewPage<LogonVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Login</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

	<div class="logOnTela">
		<% Html.RenderPartial("LogOnPartial", Model); %>
	</div>
</asp:Content>