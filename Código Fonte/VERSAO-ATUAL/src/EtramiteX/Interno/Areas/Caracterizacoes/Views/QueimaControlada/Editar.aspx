<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<QueimaControladaVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Editar Queima Controlada</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/queimaControlada.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Areas/Caracterizacoes/queimaControladaQueima.js") %>"></script>
	
	<script>
		$(function () {
			QueimaControlada.load($('#central'), { urls: { salvar: '<%= Url.Action("Editar", "QueimaControlada") %>'} });
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Editar Queima Controlada</h1>
		<br />

		<div class="divCaracterizacao">
			<%Html.RenderPartial("QueimaControlada", Model);%>
		</div>

		<div class="block box">
			<input class="floatLeft btnSalvar" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("", "Caracterizacao", new { id = Model.Caracterizacao.EmpreendimentoId}) %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>