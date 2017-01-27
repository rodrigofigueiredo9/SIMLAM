<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMRoteiro" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ItemRoteiroVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Cadastrar Item de Roteiro</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Roteiro/salvarItem.js") %>"></script>

	<script>
		$(function () {
			ItemSalvar.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ItemAdicionar", Model); %>

		<div class="block box">
			<button class="bntSalvarItemRoteiro floatLeft" type="button" value="Salvar"><span>Salvar</span></button>
			<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("IndexItem", "Roteiro") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>