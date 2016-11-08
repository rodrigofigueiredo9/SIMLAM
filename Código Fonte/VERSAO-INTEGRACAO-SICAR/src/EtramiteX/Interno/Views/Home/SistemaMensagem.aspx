<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div id="central">

	<img src="<%= Url.Content("~/Content/_imgLogo/logo_entrada.png") %>" alt="Logar" style="display: block; margin: 150px auto 0px; width: 776px;" />

</div>

</asp:Content>


