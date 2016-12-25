<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<AcompanhamentoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Acompanhamento</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script>
		$(function () {
			$('.fsArquivos', $('#central')).arquivo();
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">

		<h1 class="titTela">Visualizar Acompanhamento</h1><br />

		<% Html.RenderPartial("AcompanhamentoPartial", Model); %>

		<div class="block box">
			<a class="linkCancelar" href="<%= Url.Action("Acompanhamentos", "Fiscalizacao", new { id = Model.Acompanhamento.FiscalizacaoId}) %>">Cancelar</a>
		</div>
	</div>
</asp:Content>
