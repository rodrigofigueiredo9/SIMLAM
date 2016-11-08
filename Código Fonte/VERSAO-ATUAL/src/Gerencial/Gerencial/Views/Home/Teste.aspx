<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Gerencial.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Teste</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Teste</h1>
		<br />

		<input type="button" class="btnAcao" name="name" value="valor" />
	</div>
</asp:Content>