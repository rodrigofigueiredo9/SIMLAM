<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProtocolo" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Protocolos</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Protocolo/listar.js") %>" ></script>
	<%--<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>--%>
	<script>
		$(function () {
			ProtocoloListar.urlExcluirConfirm = '<%= Url.Action("ExcluirConfirm", "Protocolo") %>';
			ProtocoloListar.urlExcluir = '<%= Url.Action("Excluir", "Protocolo") %>';
			ProtocoloListar.urlConsultarInformacoes = '<%= Url.Action("ConsultarInformacoes", "Protocolo") %>';

			ProtocoloListar.urlEditar = '<%= Url.Action("Editar", "Protocolo") %>';
			ProtocoloListar.urlValidarEditar = '<%= Url.Action("ValidarEditar", "Protocolo") %>';

			ProtocoloListar.urlExisteProtocoloAtividade = '<%= Url.Action("ExisteProtocoloAtividade", "Protocolo") %>';
			ProtocoloListar.urlAtividadesSolicitadas = '<%= Url.Action("AtividadesSolicitadas", "Protocolo") %>';

			ProtocoloListar.urlHistoricoTramitacao = '<%= Url.Action("Historico", "Tramitacao") %>';

			ProtocoloListar.urlEditarApensadosJuntados = '<%= Url.Action("EditarApensadosJuntados") %>';
			ProtocoloListar.urlValidarEditarApensadosJuntados = '<%= Url.Action("ValidarEditarApensadosJuntados", "Protocolo") %>';
			ProtocoloListar.load($('#central'));

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])){%>
				ContainerAcoes.load($(".containerAcoes"), {
					urls:{
						urlGerarPdf: '<%= Url.Action("GerarPdfDocRegistroRecebimento", "Protocolo", new {id = Request.Params["acaoId"].ToString() }) %>'
					}
				});
			<%}%>
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros"); %>
	</div>
</asp:Content>