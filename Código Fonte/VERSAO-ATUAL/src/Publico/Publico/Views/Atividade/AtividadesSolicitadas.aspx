<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMAtividade" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Publico.Master" Inherits="System.Web.Mvc.ViewPage<ListarAtividadesSolicitadasVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Atividades Solicitadas</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Atividade/atividadesSolicitadas.js") %>" ></script>
<script type="text/javascript">
	$(function () {
		AtividadesSolicitadas.urlEncerrarAtividade = '<%= Url.Action("EncerrarAtividade", "Atividade") %>';
		AtividadesSolicitadas.urlMotivoEncerrarAtividade = '<%= Url.Action("MotivoEncerrarAtividade", "Atividade") %>';
		AtividadesSolicitadas.urlSalvarMotivoEncerrarAtividade = '<%= Url.Action("SalvarMotivoEncerrarAtividade", "Atividade") %>';
		AtividadesSolicitadas.urlVisualizarEncerrarAtividade = '<%= Url.Action("VisualizarMotivoEncerrarAtividade", "Atividade") %>';
		AtividadesSolicitadas.urlVisualizarPdf = '<%= Url.Action("GerarPdf", "Requerimento") %>';
		AtividadesSolicitadas.load($('#central'));
	});
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

	<div id="central">
		<% Html.RenderPartial("~/Views/Atividade/AtividadesSolicitadasPartial.ascx"); %>

		<div class="block box">
			<span class="cancelarCaixa"><a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>

</asp:Content>