<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMCARSolicitacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Publico.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">CAR</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/CARSolicitacao/listar.js") %>" ></script>
	<script type="text/javascript">
		$(function () {
			CARSolicitacaoListar.load($('#central'));
			CARSolicitacaoListar.urlBaixarDemonstrativoCAR = '<%= Url.Action("BaixarDemonstrativoCar", "CARSolicitacao")%>';
			CARSolicitacaoListar.mensagens = <%=Model.Mensagens %>;
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("Mensagem"); %>
		<% Html.RenderPartial("ListarFiltros", Model); %>
	</div>
</asp:Content>