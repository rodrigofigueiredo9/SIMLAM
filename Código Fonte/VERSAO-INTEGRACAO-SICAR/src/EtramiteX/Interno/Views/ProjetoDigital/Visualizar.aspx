<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProjetoDigital" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<ProjetoDigitalVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Visualizar Requerimento Digital</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/ProjetoDigital/projetoDigital.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Requerimento/atividadeSolicitadaAssociar.js") %>"></script>

	<script type="text/javascript">
		ProjetoDigital.urlObterPessoa = '<%= Url.Action("ObterPessoa", "ProjetoDigital") %>';
		ProjetoDigitalObjetivoPedido.urlObterObjetivoPedido = '<%= Url.Action("ObterObjetivoPedido", "ProjetoDigital") %>';
		ProjetoDigitalObjetivoPedido.urlPdfRoteiro = '<%= Url.Action("RelatorioRoteiro", "Roteiro") %>';
		ProjetoDigitalResponsavel.urlObterResponsavel = '<%= Url.Action("ObterResponsavel", "ProjetoDigital") %>';
		ProjetoDigitalEmpreendimento.urlObterEmpreendimento = '<%= Url.Action("ObterEmpreendimento", "ProjetoDigital") %>';
		ProjetoDigitalFinalizar.urlObterFinalizar = '<%= Url.Action("ObterFinalizar", "ProjetoDigital") %>';
		ProjetoDigitalFinalizar.urlFinalizar = '<%= Url.Action("Finalizar", "ProjetoDigital") %>';
		ProjetoDigital.Mensagens = <%= Model.Mensagens %>;

		$(function () {
			ProjetoDigital.load($('.projetoDigitalContainer'));
			ProjetoDigital.requerimento = JSON.parse('<%= Model.ObterJSon() %>');
			ProjetoDigital.isVisualizar = true;

			AtividadeSolicitadaAssociar.load($('.projetoDigitalContainer'));
			ProjetoDigitalObjetivoPedido.atividadeSolicitadaExpansivel();
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<div class="projetoDigitalContainer">
			<h1 class="titTela">Visualizar Requerimento Digital</h1>
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