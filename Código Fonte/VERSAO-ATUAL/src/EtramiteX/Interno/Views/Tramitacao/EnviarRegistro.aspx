<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<EnviarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Enviar Processo/Documento</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
<script src="<%= Url.Content("~/Scripts/Tramitacao/telaRegistro.js") %>"></script>
<script src="<%= Url.Content("~/Scripts/Tramitacao/enviarRegistro.js") %>"></script>

<script>
		$(function () {
			EnviarRegistroTela.load($('#central'), {
				urls: {
					enviarRegistro: '<%= Url.Action("EnviarRegistro", "Tramitacao") %>',
					remetentes:'<%= Url.Action("ObterFuncionariosDoSetor", "Tramitacao") %>',
					funcionariosDestinatario: '<%= Url.Action("ObterFuncionariosDoSetor", "Tramitacao") %>',
					todasTramitacoesRegistro: '<%= Url.Action("ObterTodasTramitacoesRegistro", "Tramitacao") %>',
					carregarPartialTemplateTramitacoes: '<%= Url.Action("ObterTramitacaoTemplate", "Tramitacao") %>',
					addTramitacaoNumeroProcDoc: '<%= Url.Action("AdicionarTramitacaoPorNumeroProtocolo", "Tramitacao") %>',
					visualizarHistorico: '<%= Url.Action("Historico", "Tramitacao") %>',
					abrirPdf: '<%= Url.Action("GerarPdf", "Tramitacao") %>',
					validarTipoSetor: '<%= Url.Action("ValidarTipoSetorRegistro", "Tramitacao") %>'
					},
					msgs: <%= Model.Mensagens %>
			});
		});
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div id="central">
<div class="divEnviarRegistro">

		<%  Html.RenderPartial("EnviarRegistroPartial", Model);
			bool desativarEnviar = Model.Tramitacoes.Count <= 0 && (Model.Enviar.Remetente != null && Model.Enviar.Remetente.Id > 0);%>

		<div class="block box btnEnviarContainer">
			<input class="floatLeft btnEnviar" type="button" value="Enviar" <%= desativarEnviar ? "disabled=\"disabled\"" : ""  %> />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</div>

</asp:Content>