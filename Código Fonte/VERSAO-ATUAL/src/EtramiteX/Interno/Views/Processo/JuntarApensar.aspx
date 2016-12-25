<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProcesso" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<JuntarApensarVM>" %>

<asp:Content ContentPlaceHolderID="TitleContent" runat="server">Juntar e Apensar em Processo</asp:Content>

<asp:Content ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Processo/juntarapensar.js") %>"></script>
	<script>
		$(function () {
			JuntarApensar.urlSalvar = '<%= Url.Action("JuntarApensarSalvar", "Processo") %>';
			JuntarApensar.urlVerificar = '<%= Url.Action("JuntarApensarVerificar", "Processo") %>';
			JuntarApensar.urlLimpar = '<%= Url.Action("JuntarApensar", "Processo", new {Id = ""}) %>';
			JuntarApensar.urlSalvado = '<%= Url.Action("JuntarApensar", "Processo", new {Id = Model.Processo.Id}) %>';
			JuntarApensar.urlVisualizarProcesso = '<%= Url.Action("Visualizar", "Processo") %>';
			JuntarApensar.urlVisualizarDocumento = '<%= Url.Action("Visualizar", "Documento") %>';
			JuntarApensar.urlJuntarDocumentoVerificar = '<%= Url.Action("JuntarDocumentoVerificar", "Processo") %>';
			JuntarApensar.urlApensarProcessoVerificar = '<%= Url.Action("ApensarProcessoVerificar", "Processo") %>';
			JuntarApensar.mensagens = <%= Model.Mensagens %>;
			JuntarApensar.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Juntar Documento/Apensar Processo</h1>
		<br />

		<div class="juntarApensarPartialContainer">
			<% Html.RenderPartial("JuntarApensarPartial", Model); %>
		</div>

		<div class="block box">
			<input type="button" value="Salvar" class="btnSalvar floatLeft <%= (Model.IsProcessoValido ? "" : "hide") %>" />
			<span class="cancelarCaixa"><span class="ouContainer <%= (Model.IsProcessoValido ? "" : "hide") %>">ou </span><a class="linkCancelar" title="Cancelar" href="<%= Url.Action("", "Tramitacao") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>