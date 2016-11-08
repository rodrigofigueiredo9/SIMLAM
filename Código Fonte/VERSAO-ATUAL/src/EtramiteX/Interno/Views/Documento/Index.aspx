<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMDocumento" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Documentos</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Documento/listar.js") %>" ></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
	<script type="text/javascript">
		$(function () {
			DocumentoListar.urlExcluir = '<%= Url.Action("Excluir", "Documento") %>';
			DocumentoListar.urlExcluirConfirm = '<%= Url.Action("ExcluirConfirm", "Documento") %>';
			DocumentoListar.urlEditar = '<%= Url.Action("Editar", "Documento") %>';
			DocumentoListar.urlVisualizar = '<%= Url.Action("Visualizar", "Documento") %>';
			DocumentoListar.urlValidarEditar = '<%= Url.Action("ValidarEditar", "Documento") %>';
			DocumentoListar.urlAtividadesSolicitadas = '<%= Url.Action("AtividadesSolicitadas", "Documento") %>';
			DocumentoListar.urlConsultarInformacoes = '<%= Url.Action("ConsultarInformacoes", "Documento") %>';
			DocumentoListar.urlValidarPossuiRequerimentoAtividades = '<%= Url.Action("ValidarPossuiRequerimentoAtividades", "Documento") %>';
			DocumentoListar.load($('#central'));

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])){%>
				ContainerAcoes.load($(".containerAcoes"), {
					urls:{
						urlGerarPdf: '<%= Url.Action("GerarPdfDocRegistroRecebimento", "Processo", new {id = Request.Params["acaoId"].ToString() }) %>'
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