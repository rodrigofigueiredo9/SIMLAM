<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProjetoDigital" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ProjetoDigitalVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Importar Requerimento Digital</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/ProjetoDigital/projetoDigital.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Requerimento/atividadeSolicitadaAssociar.js") %>"></script>

	<script>
		ProjetoDigital.urlObterPessoa = '<%= Url.Action("ObterPessoa", "ProjetoDigital") %>';
		ProjetoDigitalObjetivoPedido.urlObterObjetivoPedido = '<%= Url.Action("ObterObjetivoPedido", "ProjetoDigital") %>';
		ProjetoDigitalObjetivoPedido.urlPdfRoteiro = '<%= Url.Action("RelatorioRoteiro", "Roteiro") %>';
		ProjetoDigitalResponsavel.urlObterResponsavel = '<%= Url.Action("ObterResponsavel", "ProjetoDigital") %>';
		ProjetoDigitalEmpreendimento.urlObterEmpreendimento = '<%= Url.Action("ObterEmpreendimento", "ProjetoDigital") %>';
		ProjetoDigitalFinalizar.urlObterFinalizar = '<%= Url.Action("ObterFinalizar", "ProjetoDigital") %>';
		ProjetoDigitalFinalizar.urlFinalizar = '<%= Url.Action("Finalizar", "ProjetoDigital") %>';
		ProjetoDigitalFinalizar.urlValidarObjetivoPedido = '<%= Url.Action("ValidarObjetivoPedido", "ProjetoDigital") %>';
		ProjetoDigitalFinalizar.urlModalNotificacao = '<%= Url.Action("Recusar", "ProjetoDigital") %>';
		ProjetoDigitalFinalizar.urlRecusar = '<%= Url.Action("Recusar", "ProjetoDigital")%>';
		ProjetoDigital.Mensagens = <%= Model.Mensagens %>;

		$(function () {
			ProjetoDigital.load($('.projetoDigitalContainer'));
			ProjetoDigital.requerimento = <%= Model.ObterJSon() %>;

			AtividadeSolicitadaAssociar.load($('.projetoDigitalContainer'));
			ProjetoDigitalObjetivoPedido.atividadeSolicitadaExpansivel();
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<div class="projetoDigitalContainer">
			<h1 class="titTela">Importar Requerimento Digital</h1>
			<br />

			<div class="projetoDigitalPartial">
				<% Html.RenderPartial("ProjetoDigitalPartial"); %>
				<div class="divMensagemTemplate hide">
					<fieldset class="block box">
						<div class="block">
							<div class="coluna100">
								<label class="lblMensagem"></label>
							</div>
						</div>
					</fieldset>
				</div>
			</div>
		</div>
	</div>
</asp:Content>