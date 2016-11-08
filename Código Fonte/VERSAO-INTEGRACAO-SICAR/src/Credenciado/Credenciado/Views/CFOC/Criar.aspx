<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCFOCFOC.CFOC" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<CFOCVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Emitir Certificado Fitossanitário de Origem Consolidado</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/CFOC/loteListar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/CFOC/listar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/CFOC/emitir.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/containerAcoes.js") %>"></script>

	<script type="text/javascript">
		CFOCEmitir.settings.idsTela = <%= Model.IdsTela %>;
		CFOCEmitir.settings.horaServidor = '<%= Model.HoraServidor %>';
		CFOCEmitir.settings.Mensagens = <%= Model.Mensagens %>;

		$(function () {
			CFOCEmitir.load($('#central'), {
				urls:{
					verificarNumero: '<%=Url.Action("VerificarNumero", "CFOC")%>',
					listarLote: '<%=Url.Action("LoteAssociar", "CFOC")%>',
					validarIdentificacaoProduto: '<%=Url.Action("ValidarIdentificacaoProduto", "CFOC")%>',
					obterPragas: '<%=Url.Action("ObterPragas", "CFOC")%>',
					validarPraga: '<%=Url.Action("ValidarPraga", "CFOC")%>',
					validarTratamentoFitossanitario: '<%=Url.Action("ValidarTratamentoFitossanitario", "CFOC")%>',
					verificarCredenciadoHabilitado: '<%=Url.Action("VerificarCredenciadoHabilitado", "CFOC")%>',
					obterDeclaracaoAdicional: '<%=Url.Action("ObterDeclaracaoAdicional", "CFOC")%>',
					salvar: '<%=Url.Action("Criar", "CFOC")%>'
				}
			});

			CFOCListar.urlPDF = '<%= Url.Action("GerarPdf", "CFOC") %>';
			CFOCListar.urlConfirmarAtivar = '<%= Url.Action("AtivarConfirm", "CFOC") %>';
			CFOCListar.urlAtivar = '<%= Url.Action("Ativar", "CFOC") %>';
			CFOCListar.idsTela = <%= Model.IdsTela %>;

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"]))
	  { %>
			ContainerAcoes.load($(".containerAcoes"), {
				botoes: [
					{ label: 'Gerar PDF', url: '<%= Url.Action("GerarPdf", "CFOC", new { id = Request.Params["acaoId"] })%>' },
					{ label: 'Editar', url: '<%= Url.Action("Editar", "CFOC", new { id = Request.Params["acaoId"] })%>' },
					{ label: 'Listar', url: '<%= Url.Action("Index", "CFOC")%>' },
					{ label: 'Ativar', url: '<%= Url.Action("AtivarConfirm", "CFOC")%>', abrirModal: function (){ CFOCListar.ativarItem({ SituacaoId: '<%= (int)eDocumentoFitossanitarioSituacao.EmElaboracao %>', Id: '<%=Request.Params["acaoId"]%>'});}}
				]
			});
			<% } %>
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Emitir Certificado Fitossanitário de Origem Consolidado</h1>
		<br />

		<div class="block">
			<% Html.RenderPartial("CFOCPartial"); %>
		</div>

		<div class="block box">
			<input class="floatLeft btnSalvar" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("Index", "CFOC") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>
