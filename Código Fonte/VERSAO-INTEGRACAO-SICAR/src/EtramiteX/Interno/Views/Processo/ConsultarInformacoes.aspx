<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProcesso" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ConsultarInformacaoVM>" %>

<asp:Content ContentPlaceHolderID="TitleContent" runat="server">Consultar Informações do Processo</asp:Content>

<asp:Content ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Processo/consultarInformacoes.js") %>" ></script>

	<script type="text/javascript">
		$(function () {
			ConsultarInformacoes.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		
		<% Html.RenderPartial("ConsultarInformacoesPartial", Model); %>

		<div class="block box">
			<span class="cancelarCaixa"><a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>