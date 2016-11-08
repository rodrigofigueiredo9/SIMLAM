<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Gerencial.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Página Principal</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<% Html.RenderPartial("IndexPartial"); %>
</asp:Content>