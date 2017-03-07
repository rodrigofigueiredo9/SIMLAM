<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPTVOutro" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<PTVOutroListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">PTVs de Outro Estado</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/PTVOutro/listar.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			PTVListar.load($('#central'), {
				urls: {
					urlVisualizar: '<%= Url.Action("Visualizar", "PTVOutro") %>',
				    urlConfirmCancel: '<%= Url.Action("CancelarConfirm", "PTVOutro") %>',
				    urlEditar: '<%= Url.Action("Editar", "PTVOutro") %>',
					urlCancelar: '<%= Url.Action("PTVCancelar", "PTVOutro") %>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros", Model); %>
	</div>
</asp:Content>