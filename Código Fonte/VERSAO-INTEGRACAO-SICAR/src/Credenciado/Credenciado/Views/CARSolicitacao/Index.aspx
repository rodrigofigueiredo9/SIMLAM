<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCARSolicitacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<ListarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">CAR</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/CARSolicitacao/listar.js") %>" ></script>
	<script type="text/javascript">
		$(function () {
			CARSolicitacaoListar.urlPDF = '<%= Url.Action("GerarPdf", "CARSolicitacao") %>';
		    CARSolicitacaoListar.urlTituloPDF = '<%= Url.Action("GerarTituloPdf", "CARSolicitacao") %>';
		    CARSolicitacaoListar.urlPDFPendencia = '<%= Url.Action("GerarPdfPendencia", "CARSolicitacao") %>'; 
		    CARSolicitacaoListar.urlEnviarReenviarArquivoSICAR = '<%= Url.Action("EnviarReenviarArquivoSICAR", "CARSolicitacao") %>'; 
			CARSolicitacaoListar.urlMensagemErroEnviarArquivoSICAR = '<%= Url.Action("MensagemErroEnviarArquivoSICAR", "CARSolicitacao") %>'; 		    
			CARSolicitacaoListar.urlGerarPdfComprovanteSICAR= '<%= Url.Action("GerarPdfComprovanteSICAR", "CARSolicitacao") %>';
			CARSolicitacaoListar.urlBaixarAquivoSICAR= '<%= Url.Action("BaixarAquivoSICAR", "CARSolicitacao") %>';
		    CARSolicitacaoListar.idsTela = <%=Model.IdsTela %>;
		    CARSolicitacaoListar.mensagens = <%=Model.Mensagens %>;
			CARSolicitacaoListar.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("ListarFiltros", Model); %>
	</div>
</asp:Content>