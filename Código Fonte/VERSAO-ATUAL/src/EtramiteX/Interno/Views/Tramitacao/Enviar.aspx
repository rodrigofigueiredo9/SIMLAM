<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<EnviarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Enviar Processo/Documento</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
<script src="<%= Url.Content("~/Scripts/Tramitacao/tela.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Tramitacao/enviar.js") %>"></script>

<script>
		$(function () {
			EnviarTela.load($('#central'), {
				urls: {
					enviar: '<%= Url.Action("Enviar", "Tramitacao") %>',
					setoresRemetente:'<%= Url.Action("ObterSetoresDoFuncionario", "Tramitacao") %>',
					funcionariosDestinatario: '<%= Url.Action("ObterFuncionariosDoSetor", "Tramitacao") %>',
					todasTramitacoes: '<%= Url.Action("ObterTodasTramitacoes", "Tramitacao") %>',
					carregarPartialTemplateTramitacoes: '<%= Url.Action("ObterTramitacaoTemplate", "Tramitacao") %>',
					addTramitacaoNumeroProcDoc: '<%= Url.Action("AdicionarTramitacaoPorNumeroProtocolo", "Tramitacao") %>',
					visualizarHistorico: '<%= Url.Action("Historico", "Tramitacao") %>',
					abrirPdf: '<%= Url.Action("GerarPdf", "Tramitacao") %>',
					validarTipoSetor: '<%= Url.Action("ValidarTipoSetorNormal", "Tramitacao") %>',
					visualizarProc: '<%= Url.Action("Visualizar", "Processo") %>',
					visualizarDoc: '<%= Url.Action("Visualizar", "Documento") %>'
				},
				msgs: <%= Model.Mensagens %>
			});
		});
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div id="central">
	<div class="divEnviar">
		<h1 class="titTela">Enviar Processo/Documento</h1>
		<br />

		<%  Html.RenderPartial("EnviarPartial", Model);
			bool desativarEnviar = Model.Tramitacoes.Count <= 0 && Model.Enviar.RemetenteSetor.Id > 0; %>

		<div class="block box btnEnviarContainer">
			<input class="floatLeft btnEnviar" type="button" value="Enviar" <%= desativarEnviar ? "disabled=\"disabled\"" : ""  %> />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</div>
</asp:Content>

