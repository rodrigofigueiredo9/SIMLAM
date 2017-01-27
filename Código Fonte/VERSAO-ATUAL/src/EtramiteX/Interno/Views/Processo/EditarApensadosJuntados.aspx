<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMProcesso" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<EditarApensadosJuntadosVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Requerimento Padrão Adicionado</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Processo/editarApensadosJuntados.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Requerimento/requerimentoListar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Requerimento/atividadeSolicitadaAssociar.js") %>"></script>

	<script>
		$(function () {
			FinalidadeAssociar.urlObterTituloModelo = '<%= Url.Action("ObterTituloModelo", "Requerimento") %>';
			FinalidadeAssociar.urlObterTituloModeloAnterior = '<%= Url.Action("ObterTituloModeloAnterior", "Requerimento") %>';
			FinalidadeAssociar.urlObterNumerosTitulos = '<%= Url.Action("ObterNumerosTitulos", "Requerimento") %>';
			FinalidadeAssociar.urlValidarNumeroModeloAnterior = '<%= Url.Action("ValidarNumeroModeloAnterior", "Requerimento") %>';
			FinalidadeAssociar.Mensagens = <%= Model.Mensagens %>;

			AtividadeSolicitadaAssociar.urlAbriModalAtividade = '<%= Url.Action("ObterFinalidade", "Atividade") %>';
			AtividadeSolicitadaAssociar.Mensagens = <%= Model.Mensagens %>;
			AtividadeSolicitadaAssociar.load($('#central'));

			EditarApensadosJuntados.load($('#central'), {
				urls: {
					pdfRequerimento: '<%= Url.Action("GerarPdf", "Requerimento") %>',
					associarRequerimento: '<%= Url.Action("Associar", "Requerimento") %>',
					validarExcluirAtividade: '<%= Url.Action("ValidarExcluirAtividade", "Processo") %>',
					buscarAtividadesDeRequerimento: '<%= Url.Action("CriarAtividadesSolicitadasDeRequerimento", "Processo") %>',
					salvar: '<%= Url.Action("EditarApensadosJuntados", "Processo") %>'
				},
				Mensagens: <%= Model.Mensagens %>
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Requerimento Padrão Adicionado</h1>
		<br />

		<div class="apensadoJuntadoPartial">
			<% Html.RenderPartial("EditarApensadosJuntadosPartial", Model); %>
		</div>

		<div class="block box">
			<span class="cancelarCaixa"><a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>