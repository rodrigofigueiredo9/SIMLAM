<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMChecagemPendencia" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<SalvarVM>" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" runat="server">Cadastrar Checagem de Pendência</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="JsHeadContent" runat="server">

	<script src="<%= Url.Content("~/Scripts/Titulo/listar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/ChecagemPendencia/salvar.js") %>"></script>

	<script>
		ChecagemPendencia.settings.urls.pdfTitulo = '<%= Url.Action("GerarPdf", "Titulo") %>';
		ChecagemPendencia.settings.urls.pdfPendencia = '<%= Url.Action("ChecagemPendenciaPdfObj", "ChecagemPendencia") %>';
		ChecagemPendencia.settings.urls.salvar = '<%= Url.Action("Salvar", "ChecagemPendencia") %>';
		ChecagemPendencia.settings.urls.associarTituloModal = '<%= Url.Action("Associar", "Titulo", new { modelosCodigos = Model.ModelosListarTitulo }) %>';
		ChecagemPendencia.settings.urls.associarTitulo = '<%= Url.Action("AssociarTitulo", "ChecagemPendencia") %>';
		ChecagemPendencia.settings.mensagens = <%= Model.Mensagens %>;

		$(function() { ChecagemPendencia.load($('#central')); });
	</script>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">

	<div id="central">
		<h1 class="titTela">Cadastrar Checagem de Pendência</h1>
		<br />

		<% Html.RenderPartial("ChecagemPendenciaPartial"); %>

		<div class="block box">
			<button type="button" value="Salvar" class="floatLeft btnSalvar disabled" disabled="disabled">Salvar</button>
			<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
			<button type="button" value="Gerar relatório de pendência" class="floatRight btnGerarPdfPendencia hide">Gerar relatório de pendência</button>
		</div>
	</div>
</asp:Content>