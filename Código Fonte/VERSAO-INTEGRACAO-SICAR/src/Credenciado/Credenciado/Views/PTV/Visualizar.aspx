﻿<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPTV" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<PTVVM>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar PTV</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/PTV/emitirPTV.js") %>"></script>
	<script type="text/javascript">
		$(function () {
			PTVEmitir.settings.idsTela = <%= Model.IdsTela %>;
			PTVEmitir.settings.idsOrigem = <%= Model.IdsOrigem %>;
			PTVEmitir.settings.dataAtual = '<%= Model.DataAtual%>';
			PTVEmitir.load($("#central"));
		});
	</script>
		
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Visualizar Permissão de Trânsito de Vegetais</h1>
		<br />

		<%Html.RenderPartial("PTVPartial", Model); %>

		<div class="block box">			
			<span class="cancelarCaixa"><span class="btnModalOu <%= Model.IsVisualizar ? "hide":"" %>">ou</span> <a class="linkCancelar" href="<%= Url.Action("Index", "PTV") %>">Cancelar</a></span>
		</div>	
	</div>
</asp:Content>
