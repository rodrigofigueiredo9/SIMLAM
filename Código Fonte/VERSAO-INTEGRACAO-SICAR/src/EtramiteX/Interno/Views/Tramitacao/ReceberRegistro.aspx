<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ReceberRegistroVM>" %>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Receber Processo/Documento por Registro
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Tramitacao/receberRegistro.js") %>"></script>
	<script type="text/javascript">
	$(function() {
		ReceberRegistro.load(
			$('.partialTramitacaoReceber'), 
			{
				urls: {
					receberFiltrar: '<%= Url.Action("ReceberRegistroFiltrar") %>',
					receberSalvar: '<%= Url.Action("ReceberRegistroSalvar", "Tramitacao")%>',
					receberSucesso: '<%= Url.Action("ReceberRegistro")%>',
					visualizarHistorico: '<%= Url.Action("Historico", "Tramitacao") %>',
					
					abrirPdf: '<%= Url.Action("GerarPdf", "Tramitacao") %>'
				},
				msgs: <%= Model.Mensagens %> 
			}
		);
	});
	</script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	
	<div id="central" class="partialTramitacaoReceber">
		<h1 class="titTela">Receber Processo/Documento por Registro</h1>
		<br />

		<% Html.RenderPartial("ReceberRegistroPartial", Model); 
			bool desativarReceber = Model.NumeroTramitacoes <= 0 && Model.SetorDestinatario.Id > 0;%>

		<div class="block box btnSalvarContainer">
			<input class="floatLeft btnReceber" type="button" value="Receber" <%= desativarReceber ? "disabled=\"disabled\"" : "" %> />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>

</asp:Content>