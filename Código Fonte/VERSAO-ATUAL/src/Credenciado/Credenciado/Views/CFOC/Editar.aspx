<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCFOCFOC.CFOC" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<CFOCVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Editar Certificado Fitossanitário de Origem Consolidado</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/CFOC/loteListar.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/CFOC/emitir.js") %>"></script>

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
					salvar: '<%=Url.Action("Editar", "CFOC")%>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Editar Certificado Fitossanitário de Origem Consolidado</h1>
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
