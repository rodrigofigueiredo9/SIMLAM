<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<TramitacoesVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Tramitações</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Tramitacao/tramitacoes.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/containerAcoes.js") %>" ></script>
	<script type="text/javascript">
		$(function () {
			Tramitacoes.urlObterFuncionarios = '<%= Url.Action("ObterFuncionariosDoSetor", "Tramitacao")%>';
			Tramitacoes.urlObterTramitacaoSetor = '<%= Url.Action("ObterTramitacaoSetor", "Tramitacao")%>';
			Tramitacoes.urlCancelar = '<%= Url.Action("Cancelar", "Tramitacao")%>';
			Tramitacoes.urlVisualizarProc = '<%= Url.Action("Visualizar", "Processo") %>';
			Tramitacoes.urlVisualizarDoc = '<%= Url.Action("Visualizar", "Documento") %>';
			Tramitacoes.urlValidarTramitacaoReceber = '<%= Url.Action("TramitacaoReceber", "Tramitacao") %>';
			Tramitacoes.urlValidarTramitacaoEnviar = '<%= Url.Action("TramitacaoEnviar", "Tramitacao") %>';
			Tramitacoes.urlTramitacaoReceber = '<%= Url.Action("Receber", "Tramitacao") %>';
			Tramitacoes.urlTramitacaoReceberRegistro = '<%= Url.Action("ReceberRegistro", "Tramitacao") %>';
			Tramitacoes.urlTramitacaoEnviar = '<%= Url.Action("Enviar", "Tramitacao") %>';
		    Tramitacoes.urlTramitacaoEnviarRegistro = '<%= Url.Action("EnviarRegistro", "Tramitacao") %>';
		    Tramitacoes.urlTramitacaoGerarPdf = '<%= Url.Action("GerarPdf", "Tramitacao") %>';

			Tramitacoes.visualizarHistorico = '<%= Url.Action("Historico", "Tramitacao") %>';
			Tramitacoes.Mensagens = <%= Model.Mensagens %>;
			Tramitacoes.load($('.visualizarTramitacaoPartial'));

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])){%>
			ContainerAcoes.load($(".containerAcoes"), {
				urls:{
					urlGerarPdf: '<%= Url.Action("GerarPdfZip", "Tramitacao", new {id = Request.Params["acaoId"].ToString() }) %>',
					urlTramitacao: '<%= Url.Action("Index", "Tramitacao")%>'
				}
			});
			<%}%>
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("TramitacoesPartial"); %>
	</div>
</asp:Content>