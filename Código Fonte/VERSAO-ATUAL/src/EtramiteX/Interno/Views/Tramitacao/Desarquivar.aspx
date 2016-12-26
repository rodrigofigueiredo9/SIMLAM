<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<DesarquivarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Desarquivar Processo/Documento</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/jquery.listar-grid.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Tramitacao/desarquivar.js") %>"></script>

	<script>
			$(function () {
				Desarquivar.load($('#central'), {
					urls: {
						arquivosCadastrados: '<%= Url.Action("ObterArquivosCadastradosSetor", "Tramitacao") %>',
						visualizarHistorico: '<%= Url.Action("Historico", "Tramitacao") %>',
						abrirPdf: '<%= Url.Action("GerarPdfArquivamento", "Tramitacao") %>',
						filtrar: '<%= Url.Action("DesarquivarObterItens", "Tramitacao") %>',
						desarquivar: '<%= Url.Action("Desarquivar", "Tramitacao") %>',
						desarquivarAdicionarItem: '<%= Url.Action("DesarquivarAdicionarItem", "Tramitacao") %>',
						desarquivarBuscarEstantes: '<%= Url.Action("ArquivarBuscarEstantes", "Tramitacao") %>',
						desarquivarBuscarPrateleiras: '<%= Url.Action("ArquivarBuscarPrateleiras", "Tramitacao") %>',
						visualizarProc: '<%= Url.Action("Visualizar", "Processo") %>',
						visualizarDoc: '<%= Url.Action("Visualizar", "Documento") %>'
						},
						Mensagens : <%= Model.Mensagens %>
				});
			});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<% Html.RenderPartial("DesarquivarPartial", Model); %>

		<div class="block box btnEnviarContainer">
			<input class="floatLeft btnDesarquivar" type="button" value="Desarquivar" />
			<span class="cancelarCaixa"><span class="btnModalOu">ou</span> <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>