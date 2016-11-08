<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCFOCFOC.CFO" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<CFOVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Editar Certificado Fitossanitário de Origem</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/CFO/emitir.js") %>"></script>

	<script type="text/javascript">
		CFOEmitir.settings.idsTela = <%=Model.IdsTela%>;
		CFOEmitir.settings.horaServidor = '<%=Model.HoraServidor%>';
		CFOEmitir.settings.Mensagens = <%=Model.Mensagens%>;

		$(function () {
			CFOEmitir.load($('#central'), {
				urls:{
					verificarNumeroCFO: '<%=Url.Action("VerificarNumero", "CFO")%>',
					obterEmpreendimentos: '<%=Url.Action("ObterEmpreendimentos", "CFO")%>',
					obterUPs: '<%=Url.Action("ObterUPs", "CFO")%>',
					obterCulturaUP: '<%=Url.Action("ObterCulturaUP", "CFO")%>',
					obterPragas: '<%=Url.Action("ObterPragas", "CFO")%>',
					validarIdentificacaoProduto: '<%=Url.Action("ValidarIdentificacaoProduto", "CFO")%>',
					validarPraga: '<%=Url.Action("ValidarPraga", "CFO")%>',
					validarTratamentoFitossanitario: '<%=Url.Action("ValidarTratamentoFitossanitario", "CFO")%>',
					verificarCredenciadoHabilitado: '<%=Url.Action("VerificarCredenciadoHabilitado", "CFO")%>',
					obterDeclaracaoAdicional: '<%=Url.Action("ObterDeclaracaoAdicional", "CFO")%>',
					salvar: '<%=Url.Action("Editar", "CFO")%>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Editar Certificado Fitossanitário de Origem</h1>
		<br />
		
		<div class="block">
			<% Html.RenderPartial("CFOPartial"); %>
		</div>

		<div class="block box">
			<input class="floatLeft btnSalvar" type="button" value="Salvar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" href="<%= Url.Action("Index", "CFO") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>