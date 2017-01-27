<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<DominialidadeVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Caracterização do Empreendimento -  Dominialidade</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/dominialidade.js") %>"></script>

	<script>
		$(function () {
			Dominialidade.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">	
	<div id="central">
		<div class="divDominialidade">
			<%Html.RenderPartial("DominialidadePartial", Model);%>
		</div>

		<div class="block box">
			<span class="cancelarCaixa"><a class="linkCancelar" href="<%= Model.UrlRetorno %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>