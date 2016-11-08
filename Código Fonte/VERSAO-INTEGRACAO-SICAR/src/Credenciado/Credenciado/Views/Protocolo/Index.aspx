<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProtocolo" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Protocolos</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Protocolo/listar.js") %>" ></script>

	<script type="text/javascript">
		$(function () {
			ProtocoloListar.urlExisteProtocoloAtividade = '<%= Url.Action("ExisteProtocoloAtividade", "Protocolo") %>';
			ProtocoloListar.urlAtividadesSolicitadas = '<%= Url.Action("AtividadesSolicitadas", "Protocolo") %>';
			ProtocoloListar.urlHistoricoTramitacao = '<%= Url.Action("Historico", "Tramitacao") %>';
			ProtocoloListar.urlNotificacaoPendencia = '<%= Url.Action("NotificacaoPendencia", "Protocolo") %>';
			ProtocoloListar.urlGerarPdf = '<%= Url.Action("GerarPdf", "Titulo") %>';
			ProtocoloListar.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros"); %>
	</div>
</asp:Content>